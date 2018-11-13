using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;

using BattleLogic;

/// <summary>
/// Spineのモデル関連の設定を行うクラス
/// </summary>
[RequireComponent(typeof(SkeletonRenderer))]
public class SpineModelController : MonoBehaviour {

    public Dictionary<string, float> MoveTimes = new Dictionary<string, float>();
    void Awake()
    {
        m_SkeletonRenderer = GetComponent<SkeletonRenderer> ();
    }

    /// <summary>
    /// Spineモデル初期化
    /// </summary>
    /// <param name="param">Parameter.</param>
    /// <param name="UnitResource">Unit resource.</param>
    /// <param name="weaponAtlasAsset">Weapon atlas asset.</param>
    public void Init (int? motionType, UnitResourceLoader UnitResource, AtlasAsset weaponAtlasAsset, bool isPlayer=true) {
        if (!isPlayer) {
            m_SkeletonRenderer.initialFlipX = true;
        }
        m_SkeletonRenderer.Initialize (true);

        if (motionType.HasValue) {
            // weaponTypeからモデル設定情報を取得
            int weaponMotionType = motionType.Value;
            var unitSetting = MasterDataTable.battle_unit_model_setting [weaponMotionType];

            // アニメーションの入れ替え
            if (UnitResource.AnimationClips != null && UnitResource.AnimationClips.Length > 0) {
                var animator = GetComponent<Animator> ();
                var animatorCtl = Resources.Load<AnimatorOverrideController> (string.Format ("Animator/{0}", unitSetting.animator_controller_name));

                // 複製して設定する。
                animatorCtl = AnimatorOverrideController.Instantiate (animatorCtl);

                // 上書き設定
                List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>> (animatorCtl.overridesCount);
                List<KeyValuePair<AnimationClip, AnimationClip>> newOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>> (animatorCtl.overridesCount);

                animatorCtl.GetOverrides (overrides);
                string motionTypeString = string.Format("{0:D2}", weaponMotionType);

                foreach (var pair in overrides) {
                    var clip = Array.Find (UnitResource.AnimationClips, (x) => (x.name.StartsWith(motionTypeString) || x.name.StartsWith("a") || x.name.StartsWith("b"))
                        && x.name.ToLower().Contains (pair.Key.name));
                    newOverrides.Add (new KeyValuePair<AnimationClip, AnimationClip> (pair.Key, clip));

                    if (clip != null) {
                        var stepSt = clip.events.FirstOrDefault (x => x.functionName == "StepSt");
                        var stepEn = clip.events.FirstOrDefault (x => x.functionName == "StepEn");
                        if (stepSt != null && stepEn != null) {
                            MoveTimes.Add (pair.Key.name, stepEn.time - stepSt.time);
                        }
                    }
                }
                animatorCtl.ApplyOverrides (newOverrides);

                // ランタイムデータを変更
                animator.runtimeAnimatorController = animatorCtl;
            }

            // 武器持ち替え設定からSkinを設定
            var weaponExchangeSettings = MasterDataTable.battle_weapon_exchange_setting.GetSettings(weaponMotionType);
            SetSkin(unitSetting.skin_name, weaponAtlasAsset, weaponExchangeSettings);
        }
	}

    void OnDestory()
    {
        FollowerCache.Clear ();
    }

    private Dictionary<UniRx.Tuple<string, bool, bool, bool, bool>, GameObject> FollowerCache = new Dictionary<UniRx.Tuple<string, bool, bool, bool, bool>, GameObject>();
    // 自身のボーンを追従するゲームオブジェクトを生成する。
    public GameObject CreateBoneFollower(string bone, bool followBoneRotation, bool followLocalScale, bool followSkeletonFlip, bool followZPosition, GameObject parent)
    {
        UniRx.Tuple<string, bool, bool, bool, bool> key = new UniRx.Tuple<string, bool, bool, bool, bool> (bone, followBoneRotation, followLocalScale, followSkeletonFlip, followZPosition);
        GameObject boneFollowerGo = null;
        // 同じ設定のものがあればそれを返す。
        if (FollowerCache.TryGetValue (key, out boneFollowerGo)) {
            if (boneFollowerGo != null) {
                return boneFollowerGo;
            } else {
                FollowerCache.Remove (key);
            }
        }

        if (m_SkeletonRenderer.Skeleton.FindBoneIndex (bone) < 0) {
            return null;
        }
        boneFollowerGo = new GameObject (bone);
        if (parent != null) {
            boneFollowerGo.transform.SetParent (parent.transform, true);
        }


        var boneFollower = boneFollowerGo.AddComponent<BoneFollower> ();
        boneFollower.SkeletonRenderer = m_SkeletonRenderer;
        boneFollower.boneName = bone;
        boneFollower.followBoneRotation = followBoneRotation;
        boneFollower.followLocalScale = followLocalScale;
        boneFollower.followSkeletonFlip = followSkeletonFlip;
        boneFollower.followZPosition = followZPosition;
        boneFollower.Initialize ();
        // アップデートを無理やり呼び出す
        boneFollower.LateUpdate ();
        FollowerCache.Add (key, boneFollowerGo);
        return boneFollowerGo;
    }

    // スキン変更ロジック
    void SetSkin(string skinName, AtlasAsset weaponAtlasAsset,  BattleWeaponExchangeSetting[] weaponExchangeSettings)
    {
        var skeleton = m_SkeletonRenderer.Skeleton;

        var templateSkin = skeleton.Data.FindSkin(skinName);
        if (templateSkin == null) {
            Debug.LogError (string.Format("Not found skin name {0}.", skinName));
            return;
        }

        if (weaponAtlasAsset == null) {
            skeleton.SetSkin(templateSkin);
            skeleton.SetSlotsToSetupPose();
            return;
        }

        var atlas = weaponAtlasAsset.GetAtlas();
        if (atlas == null) {
            skeleton.SetSkin(templateSkin);
            skeleton.SetSlotsToSetupPose();
            return;
        }

        var customSkin = new Skin("custom skin");
        float scale = m_SkeletonRenderer.skeletonDataAsset.scale;

        foreach (var setting in weaponExchangeSettings) {
            int slotIndex = skeleton.FindSlotIndex (setting.slot_name);
            AtlasRegion region = atlas.FindRegion(setting.atlas_region_name);
			// TODO : 新しい武器(ID名義のアトラスのもの)はアトラス領域名が[武器ID]_[領域名]になっているようなのでとりあえずこうしておく.
			if(region == null){
				var atlasName = weaponAtlasAsset.name.Split('_')[0];
				var regionName = setting.atlas_region_name.Split('_')[1];
				region = atlas.FindRegion(string.Format("{0}_{1}", atlasName, regionName));
			}
			// TODO : さらにアトラス領域名が武器名になっているものもあったのでこうする.というかこれなら設定されている領域名なんでもいける気がする.
			if(region == null){
				region = atlas.FindRegion(weaponAtlasAsset.GetAtlas().First().name);
			}

            if (region != null) {
                Attachment newAttachment = null;
                Attachment templateAttachment = templateSkin.GetAttachment (slotIndex, setting.attachment_name);
                if (templateAttachment != null) {
                    newAttachment = templateAttachment.GetRemappedClone (region, true, true, scale);
                } else {
                    newAttachment = region.ToRegionAttachment (region.name, scale);
                    /*
                    var regionAttachment = newAttachment as RegionAttachment;
                    regionAttachment.SetPositionOffset (setting.position * scale);
                    regionAttachment.SetRotation (setting.rotation);
                    regionAttachment.SetScale (setting.scale);
                    regionAttachment.UpdateOffset ();
                    */
                }
                customSkin.SetAttachment(slotIndex, setting.attachment_name, newAttachment);
            }
        }
        skeleton.SetSkin(customSkin);
        skeleton.SetSlotsToSetupPose();
    }

    public Bounds GetBounds()
    {
        return gameObject.GetComponent<MeshRenderer> ().bounds;
    }

    private SkeletonRenderer m_SkeletonRenderer;
}
