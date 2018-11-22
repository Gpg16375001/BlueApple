using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// MiniView : バトル結果の報酬獲得画面.
/// </summary>
public class MiniView_BattleRewardResult : ViewBase, IBattleResultPage
{
	/// <summary>
    /// リザルト内のページインデックス.
    /// </summary>
    public int Index { get { return m_index; } }
    private int m_index = 0;

	/// <summary>
    /// 演出中？
    /// </summary>
	public bool IsEffecting { get { return m_bEffecting; } }
	private bool m_bEffecting = false;

    public ResultTitle GetResultTitle()
    {
        return ResultTitle.Clear;
    }

	/// <summary>
	/// 初期化.
	/// </summary>
	public void Init(int index, ReceiveQuestsCloseQuest questResult)
	{
		m_index = index;
		m_result = questResult;

		// ミッション.
        if (AwsModule.BattleData.MissionProgress != null) {
            var missionList = AwsModule.BattleData.MissionProgress.MissionList;
            for (var i = 1; i <= 3; ++i) {
                var idx = i - 1;
                var text = missionList.Count > idx ? missionList[idx].ExplanatoryText : "";
                GetScript<TextMeshProUGUI>("Mission" + i + "/txtp_MissionText").text = text;
                GetScript<RectTransform>("Mission" + i).gameObject.SetActive(!string.IsNullOrEmpty(text));
                if (missionList.Count <= idx) {
                    continue;
                }
                var bAlreadyAchive = missionList[idx] != null && missionList[idx].IsAlreadyAchived;
                GetScript<RectTransform>("Mission" + i + "/Normal").gameObject.SetActive(!bAlreadyAchive);
                GetScript<RectTransform>("Mission" + i + "/Clear").gameObject.SetActive(bAlreadyAchive);
            }
        } else {
            for (var i = 1; i <= 3; ++i) {
                GetScript<RectTransform>("Mission" + i).gameObject.SetActive(false);
            }
        }
            

        if (AwsModule.BattleData.MissionProgress != null) {
            var questData = AwsModule.ProgressData.CurrentQuest;
            var currentReleaseQuest = new BattleMissionLocalSaveData.QuestMissionInfo (questData.QuestType, questData.ID,
                AwsModule.ProgressData.CurrentQuestAchievedMissionIdList.Length, AwsModule.ProgressData.CurrentQuestAchievedMissionIdList.ToList());
            if (currentReleaseQuest.MissionSetting != null && currentReleaseQuest.MissionSetting.item_type_3.HasValue) {
                //Debug.Log ("既に貰っている報酬の数 : " + currentReleaseQuest.RewardCount + "/" + currentReleaseQuest.MaxRewardCount);
                var bAlreadyGet = currentReleaseQuest.IsAchivedReward;
                var bExistReward = m_result.MissionRewardItemList != null && m_result.MissionRewardItemList.Length > 0;
                // 報酬受け取り時はアニメーションがあるのでLockを表示したままにしておく
                GetScript<RectTransform> ("RewardLock").gameObject.SetActive (!bAlreadyGet || bExistReward);
                GetScript<RectTransform>("MissionReward").gameObject.SetActive(true);

                var rType = currentReleaseQuest.MissionSetting.item_type_3.Value;
                var rID = currentReleaseQuest.MissionSetting.item_id_3;
                var iconInfo =rType.GetIconInfo (rID,
                    rType != ItemTypeEnum.card && rType != ItemTypeEnum.weapon);
                if (iconInfo.IsEnableSprite) {
                    GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                    GetScript<RectTransform> ("Item").gameObject.SetActive (true);
                    GetScript<RectTransform> ("ItemMask").gameObject.SetActive (true);
                    GetScript<RectTransform> ("WeaponMask").gameObject.SetActive (false);
                    GetScript<RectTransform> ("UnitMask").gameObject.SetActive (false);

                    var uGuiSprite = GetScript<uGUISprite> ("ItemIcon");
                    uGuiSprite.gameObject.SetActive (true);
                    uGuiSprite.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
                } else if (iconInfo.IconObject != null) {
                    Transform root;
                    if (rType != ItemTypeEnum.card && rType != ItemTypeEnum.weapon) {
                        GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                        GetScript<RectTransform> ("Item").gameObject.SetActive (true);
                        GetScript<RectTransform> ("ItemMask").gameObject.SetActive (true);
                        GetScript<RectTransform> ("WeaponMask").gameObject.SetActive (false);
                        GetScript<RectTransform> ("UnitMask").gameObject.SetActive (false);

                        var image = GetScript<Image> ("ItemIcon");
                        image.enabled = false;
                        image.gameObject.SetActive (true);
                        root = image.transform;
                    } else {
                        GetScript<RectTransform> ("Item").gameObject.SetActive (false);
                        GetScript<RectTransform> ("ItemMask").gameObject.SetActive (false);
                        GetScript<RectTransform> ("WeaponMask").gameObject.SetActive (rType == ItemTypeEnum.weapon);
                        GetScript<RectTransform> ("UnitMask").gameObject.SetActive (rType == ItemTypeEnum.card);
                        root = GetScript<RectTransform> ("UnitWeaponRoot");

                        root.gameObject.SetActive (true);
                    }

                    iconInfo.IconObject.transform.SetParent (root);
                    iconInfo.IconObject.transform.localPosition = Vector3.zero;
                    iconInfo.IconObject.transform.localScale = Vector3.one;
                    iconInfo.IconObject.transform.localRotation = Quaternion.identity;
                }
            } else {
                GetScript<RectTransform>("MissionReward").gameObject.SetActive(false);
            }
        }

		var battleResult = m_result.BattleEntryData;
		if (battleResult != null) {
            // スクロールアイテム設定
            var dropItemScroll = GetScript<ScrollRect>("DropItemScrollView");
            foreach (var dropItemID in battleResult.DropItemIdList) {
                var dropItem = MasterDataTable.battle_drop_item [dropItemID];
                if (dropItem == null || dropItem.reward_type == ItemTypeEnum.event_point) {
                    continue;
                }
                var item = GameObjectEx.LoadAndCreateObject("Battle/ListItem_DropItem", dropItemScroll.content.gameObject);
                item.GetOrAddComponent<ListItem_DropItem>().Init(dropItem);
            }         
        }
	}

	/// <summary>
	/// 開く.
	/// </summary>
	public void Open()
	{
		m_bEffecting = true;
        if (!this.gameObject.activeSelf) {
            this.gameObject.SetActive(true);
        }
		this.StartCoroutine(this.PlayOpenClose(true, () => {
			// ミッション.
            this.StartCoroutine(this.CoPlayDelayReleaseMissionAnimation(() => {
                if(m_result.QuestAchievement == null) {
                    m_bEffecting = false;
                    return;
                }

                var currentReleaseQuest = new BattleMissionLocalSaveData.QuestMissionInfo(m_result.QuestAchievement);
                // ミッション報酬数.
                if (currentReleaseQuest.MaxRewardCount <= 0) {
                    m_bEffecting = false;
                    return;
                }

                if (m_result.MissionRewardItemList == null || m_result.MissionRewardItemList.Length <= 0) {
					m_bEffecting = false;
					return;               
                }
				this.StartCoroutine(this.CoPlayMissionRewardAnimation(() => m_bEffecting = false));
            }));
		}));      
	}
	private IEnumerator CoPlayDelayReleaseMissionAnimation(Action didEnd)
	{
		if (AwsModule.BattleData.MissionProgress == null) {
			yield break;
		}
		var missionList = AwsModule.BattleData.MissionProgress.MissionList;
        for (var i = 1; i <= 3; ++i) {
            var idx = i - 1;
            if (missionList.Count <= idx) {
                break;
            }
            if (missionList[idx] != null && !missionList[idx].IsAlreadyAchived && missionList[idx].IsAchived) {
                GetScript<Animation>("Mission" + i).Play();
				yield return new WaitForSeconds(0.4f);  // TODO : 目分量.
            }
        }
		if(didEnd != null){
			didEnd();
		}
	}
	private IEnumerator CoPlayMissionRewardAnimation(Action didEnd)
	{
		var anim = GetScript<Animation>("MissionReward");
		anim.Play();
		do {
            yield return null;
        } while (anim.isPlaying);
		if (didEnd != null) {
            didEnd();
        }      
	}
    
	/// <summary>
    /// 閉じる.
    /// </summary>
    public void Close(Action didEnd)
    {
		StartCoroutine(PlayOpenClose(false, didEnd));
    }

	private IEnumerator PlayOpenClose(bool bOpen, Action didEnd = null)
    {
        var anim = GetScript<Animation>("AnimParts");
        anim.Play(bOpen ? "CommonPopOpen" : "CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didEnd != null) {
            didEnd();
        }
    }

	/// <summary>
	/// アニメーションを強制的に即時終了させる.
	/// </summary>
	public void ForceImmediateEndAnimation()
	{ 
		// TODO : 特に強制終了処理はない.
	}

	private ReceiveQuestsCloseQuest m_result;
}