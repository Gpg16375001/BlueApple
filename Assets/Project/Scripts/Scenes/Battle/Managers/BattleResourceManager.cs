using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using UnityEngine;

using SmileLab;
using BattleLogic;

using UniRx;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

public class BattleResourceManager {
    /// <summary>共通インスタンス.</summary>
    public static BattleResourceManager Shared
    {
        get {
            if(m_instance == null) {
                m_instance = new BattleResourceManager();
            }
            return m_instance;
        }
    }
    private static BattleResourceManager m_instance;

    private Dictionary<UniRx.Tuple<bool, int>, UnitResourceLoader> m_UnitResourceCache;
    private Dictionary<int, WeaponResourceLoader> m_WeaponResourceCache;
    private Dictionary<string, GameObject> m_BattleSkillEffectCache;

    private int m_LoadBattleSkillEffectCount = 0;
    private GameObject m_BattleBG;

    private bool m_IsBgmLoaded;

    private Action m_DidLoaded;

    public void BattleLoadResource(System.Action didLoaded)
    {
        isLoaded = false;
        DLCManager.StartProgress ();

        m_DidLoaded = didLoaded;

        m_UnitResourceCache = new Dictionary<UniRx.Tuple<bool, int>, UnitResourceLoader> ();
        m_WeaponResourceCache = new Dictionary<int, WeaponResourceLoader> ();
        m_BattleSkillEffectCache = new Dictionary<string, GameObject> ();

        List<string> battleSkillEffectPrefabNames = new List<string> ();
        var battleData = AwsModule.BattleData;
        // BGMのダウンロード
        m_IsBgmLoaded = false;

        // waveごとに違うことがあるので全てのBGM用のファイルをロードする。
        List<string> bgmClipNames = new List<string>();
        bgmClipNames.Add (battleData.Stage.bgm_clip_name);
        bgmClipNames.AddRange(battleData.StageWaveSettings.Where (x => !string.IsNullOrEmpty (x.wave_bgm)).Select(x => x.wave_bgm));
        SoundManager.SharedInstance.DownloadResource (bgmClipNames.ToArray(),
            () => {
                m_IsBgmLoaded = true;
                CheckEndLoadResource();
            }
        );
        m_BattleBG = null;
        DLCManager.AssetBundleFromFileOrDownload<GameObject>(DLCManager.DLC_FOLDER.BattleBG,
            string.Format("battlebg_{0}", battleData.Stage.background_id),
            "BattleBG",
            (bgObject) => {
                m_BattleBG = bgObject;
                CheckEndLoadResource();
            }
        );

        // 出現敵ユニットのリソースをダウンロードする。
        if (battleData.BattleEntryData == null) {
            var stageEnemys = battleData.StageEnemy;
            foreach (var enemy in stageEnemys.Distinct(new StageEnemyResourceEqualityComparer())) {
                var unitResource = new UnitResourceLoader (enemy);
                if (enemy.card != null) {
                    m_UnitResourceCache.Add (new UniRx.Tuple<bool, int> (false, enemy.card.id), unitResource);
                } else if (enemy.monster.resource_id.HasValue) {
                    m_UnitResourceCache.Add (new UniRx.Tuple<bool, int> (false, enemy.monster.resource_id.Value), unitResource);
                } else {
                    m_UnitResourceCache.Add (new UniRx.Tuple<bool, int> (false, enemy.monster.id), unitResource);
                }
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetStageEnemyBattleSkillEffectPrefabNames(enemy));
            }

            // 敵武器ダウンロード
            foreach (var weaponID in stageEnemys.Where(x => x.equip_weapon.HasValue).Select(x => x.equip_weapon.Value)) {
                if(!m_WeaponResourceCache.ContainsKey(weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource ());
                }
            }
        } else {
            var sallyEnemyIDs = AwsModule.BattleData.BattleEntryData.StageEnemyList.Select(x => x.EnemyId);
            var stageEnemys = AwsModule.BattleData.StageEnemy.Where (x => sallyEnemyIDs.Contains (x.id));
            foreach (var enemy in stageEnemys.Distinct(new StageEnemyResourceEqualityComparer())) {
                var unitResource = new UnitResourceLoader (enemy);
                if (enemy.card != null) {
                    m_UnitResourceCache.Add (new UniRx.Tuple<bool, int> (false, enemy.card.id), unitResource);
                } else if (enemy.monster.resource_id.HasValue) {
                    m_UnitResourceCache.Add (new UniRx.Tuple<bool, int> (false, enemy.monster.resource_id.Value), unitResource);
                } else {
                    m_UnitResourceCache.Add (new UniRx.Tuple<bool, int> (false, enemy.monster.id), unitResource);
                }
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetStageEnemyBattleSkillEffectPrefabNames(enemy));
            }

            // 敵武器ダウンロード
            foreach (var weaponID in stageEnemys.Where(x => x.equip_weapon.HasValue).Select(x => x.equip_weapon.Value)) {
                if(!m_WeaponResourceCache.ContainsKey(weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource ());
                }
            }
        }

        // 出撃味方ユニットのリソースをダウンロードする。
        foreach (var ally in AwsModule.BattleData.AllyParameterList) {
            var key = new UniRx.Tuple<bool, int> (true, ally.ID);
            if (!m_UnitResourceCache.ContainsKey (key)) {
                var unitResource = new UnitResourceLoader (ally);
                m_UnitResourceCache.Add (key, unitResource);
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetParamaterBattleSkillEffectPrefabNames(ally));
            }
            if (ally.Weapon != null) {
                int weaponID = ally.Weapon.weapon.id;
                if (!m_WeaponResourceCache.ContainsKey (weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource());
                }
            }
        }

        // スキル用エフェクトの取得
        DownloadBattleSkillEffects(battleSkillEffectPrefabNames);


        View_FadePanel.SharedInstance.SetProgress(0);
    }

    public void PVPBattleLoadResource(System.Action didLoaded)
    {
        isLoaded = false;
        DLCManager.StartProgress ();

        m_DidLoaded = didLoaded;

        m_UnitResourceCache = new Dictionary<UniRx.Tuple<bool, int>, UnitResourceLoader> ();
        m_WeaponResourceCache = new Dictionary<int, WeaponResourceLoader> ();
        m_BattleSkillEffectCache = new Dictionary<string, GameObject> ();

        List<string> battleSkillEffectPrefabNames = new List<string> ();

        var battleData = AwsModule.BattleData;
        // BGMのダウンロード
        m_IsBgmLoaded = false;
        SoundManager.SharedInstance.DownloadResource (battleData.Stage.bgm_clip_name,
            () => {
                m_IsBgmLoaded = true;
                CheckEndLoadResource();
            }
        );
        m_BattleBG = null;
        DLCManager.AssetBundleFromFileOrDownload<GameObject>(DLCManager.DLC_FOLDER.BattleBG,
            string.Format("battlebg_{0}", battleData.Stage.background_id),
            "BattleBG",
            (bgObject) => {
                m_BattleBG = bgObject;
                CheckEndLoadResource();
            }
        );

        // 出撃味方ユニットのリソースをダウンロードする。
        foreach (var ally in AwsModule.BattleData.AllyParameterList) {
            var key = new UniRx.Tuple<bool, int> (true, ally.ID);
            if (!m_UnitResourceCache.ContainsKey (key)) {
                var unitResource = new UnitResourceLoader (ally);
                m_UnitResourceCache.Add (key, unitResource);
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetParamaterBattleSkillEffectPrefabNames(ally));
            }
            if (ally.Weapon != null) {
                int weaponID = ally.Weapon.weapon.id;
                if (!m_WeaponResourceCache.ContainsKey (weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource());
                }
            }
        }

        // 出撃敵ユニットのリソースをダウンロードする。
        foreach (var enemy in AwsModule.BattleData.EnemyParameterList) {
            var key = new UniRx.Tuple<bool, int> (true, enemy.ID);
            if (!m_UnitResourceCache.ContainsKey (key)) {
                var unitResource = new UnitResourceLoader (enemy);
                m_UnitResourceCache.Add (key, unitResource);
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetParamaterBattleSkillEffectPrefabNames(enemy));
            }
            if (enemy.Weapon != null) {
                int weaponID = enemy.Weapon.weapon.id;
                if (!m_WeaponResourceCache.ContainsKey (weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource());
                }
            }
        }

        DownloadBattleSkillEffects (battleSkillEffectPrefabNames);

        View_FadePanel.SharedInstance.SetProgress(0);
    }

    public void TutorialBattleLoadResource(List<Parameter> allyList, List<Parameter> enemyList, int background_id, System.Action didLoaded)
    {
        isLoaded = false;
        DLCManager.StartProgress ();

        m_DidLoaded = didLoaded;

        m_UnitResourceCache = new Dictionary<UniRx.Tuple<bool, int>, UnitResourceLoader> ();
        m_WeaponResourceCache = new Dictionary<int, WeaponResourceLoader> ();
        m_BattleSkillEffectCache = new Dictionary<string, GameObject> ();

        List<string> battleSkillEffectPrefabNames = new List<string> ();

        // BGMのダウンロード
        m_IsBgmLoaded = true;
        m_BattleBG = null;
        DLCManager.AssetBundleFromFileOrDownload<GameObject>(DLCManager.DLC_FOLDER.BattleBG,
            string.Format("battlebg_{0}", background_id),
            "BattleBG",
            (bgObject) => {
                m_BattleBG = bgObject;
                CheckEndLoadResource();
            }
        );

        // 出撃味方ユニットのリソースをダウンロードする。
        foreach (var ally in allyList) {
            var key = new UniRx.Tuple<bool, int> (ally.IsCard, ally.ID);
            if (!m_UnitResourceCache.ContainsKey (key)) {
                var unitResource = new UnitResourceLoader (ally);
                m_UnitResourceCache.Add (key, unitResource);
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetParamaterBattleSkillEffectPrefabNames(ally));
            }
            if (ally.Weapon != null) {
                int weaponID = ally.Weapon.weapon.id;
                if (!m_WeaponResourceCache.ContainsKey (weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource());
                }
            }
        }

        // 出撃敵ユニットのリソースをダウンロードする。
        foreach (var enemy in enemyList) {
            var key = new UniRx.Tuple<bool, int> (enemy.IsCard, enemy.ID);
            if (!m_UnitResourceCache.ContainsKey (key)) {
                var unitResource = new UnitResourceLoader (enemy);
                m_UnitResourceCache.Add (key, unitResource);
                unitResource.LoadResource ((res) => CheckEndLoadResource ());

                battleSkillEffectPrefabNames.AddRange(GetParamaterBattleSkillEffectPrefabNames(enemy));
            }
            if (enemy.Weapon != null) {
                int weaponID = enemy.Weapon.weapon.id;
                if (!m_WeaponResourceCache.ContainsKey (weaponID)) {
                    var weaponRes = new WeaponResourceLoader (weaponID);
                    m_WeaponResourceCache.Add (weaponID, weaponRes);
                    weaponRes.LoadResource ((res) => CheckEndLoadResource());
                }
            }
        }

        DownloadBattleSkillEffects (battleSkillEffectPrefabNames);

        View_FadePanel.SharedInstance.SetProgress(0);
    }

    private IEnumerable<string> GetStageEnemyBattleSkillEffectPrefabNames(BattleStageEnemy enemy)
    {
        return enemy.ToUseSkills.SelectMany (skill => GetBattleSkillEffectPrefabs (skill));
    }

    private IEnumerable<string> GetParamaterBattleSkillEffectPrefabNames(Parameter unit)
    {
        List<string> ret = new List<string> ();
        ret.AddRange(unit.ActionSkillList.SelectMany(skill => GetBattleSkillEffectPrefabs (skill)));
        ret.AddRange(unit.PassiveSkillList.SelectMany(skill => GetBattleSkillEffectPrefabs (skill)));
        if (unit.SpecialSkill != null) {
            ret.AddRange(GetBattleSkillEffectPrefabs (unit.SpecialSkill));
        }
        return ret;
    }

    private void  DownloadBattleSkillEffects(List<string> battleSkillEffectPrefabNames)
    {
        m_LoadBattleSkillEffectCount = 0;
        var DownloadBattleEffectPrefabNames = new HashSet<string>(battleSkillEffectPrefabNames);
        foreach (var prefabname in DownloadBattleEffectPrefabNames) {
            if (!string.IsNullOrEmpty (prefabname)) {
                LoadBattleSkillEffect (prefabname);
            }
        }
    }

    private void LoadBattleSkillEffect(string prefabname)
    {
        var fileName = System.IO.Path.GetFileName (prefabname);
        DLCManager.AssetBundleFromFileOrDownload (DLCManager.DLC_FOLDER.BattleEffect,
            fileName.ToLower(),
            (assetbundleRef) => {
                ObservableUnity.FromCoroutine<GameObject>(
                    (observer, cancellatio) => CoLoadBattleSkillEffect(assetbundleRef, fileName, observer, cancellatio)
                ).Subscribe((go) => {
                    m_BattleSkillEffectCache.Add (prefabname, go);
                    CheckEndLoadResource ();
                }, ex => { Debug.LogException(ex); });
            },
            (ex) => {
                Debug.LogException(ex);
                // エラー出ても先に
                m_LoadBattleSkillEffectCount++;
            }
        );
    }

    private IEnumerator CoLoadBattleSkillEffect(AssetBundleRef ABRef, string prefabname, IObserver<GameObject> observer, CancellationToken cancel)
    {
        var loadAsync = ABRef.assetbundle.LoadAssetAsync(prefabname);
        yield return loadAsync;

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        ABRef.Unload (false);
        observer.OnNext(loadAsync.asset as GameObject);
        observer.OnCompleted();
    }

    private void CheckEndLoadResource()
    {
        if (!isLoaded) {
            View_FadePanel.SharedInstance.SetProgress (DLCManager.AllProgress ());
            if (m_IsBgmLoaded && m_BattleBG != null &&
            m_UnitResourceCache.Values.All (x => x.IsLoaded) &&
            m_WeaponResourceCache.Values.All (x => x.IsLoaded) &&
            m_LoadBattleSkillEffectCount <= m_BattleSkillEffectCache.Count) {
                DLCManager.EndProgress ();
                m_DidLoaded ();
                View_FadePanel.SharedInstance.DeativeProgress ();
                isLoaded = true;
            }
        }
    }

    public UnitResourceLoader GetResource(Parameter parameter)
    {
        if (parameter == null || m_UnitResourceCache == null) {
            return null;
        }
        UnitResourceLoader ret;
        m_UnitResourceCache.TryGetValue (new UniRx.Tuple<bool, int> (parameter.IsCard, parameter.ResourceID), out ret);
        return ret;
    }

    public WeaponResourceLoader GetResource(WeaponParameter parameter)
    {
        if (parameter == null || m_WeaponResourceCache == null) {
            return null;
        }

        WeaponResourceLoader ret;
        m_WeaponResourceCache.TryGetValue(parameter.weapon.id, out ret);
        return ret;
    }

    public GameObject GetBattleBG()
    {
        return m_BattleBG;
    }

    public GameObject GetBattleSkillEffectPrefab(string prefabName)
    {
        if (m_BattleSkillEffectCache == null) {
            return null;
        }

        GameObject prefab = null;
        if (!m_BattleSkillEffectCache.TryGetValue (prefabName, out prefab)) {
            return null;
        }
        return prefab;
    }

    private List<string> GetBattleSkillEffectPrefabs(SkillParameter skill)
    {
        return GetBattleSkillEffectPrefabs (skill.Skill);
    }

    /// <summary>
    /// スキルの情報から必要なPrefab名を全て取得する。
    /// </summary>
    /// <returns>The battle skill effect prefabs.</returns>
    /// <param name="skill">Skill.</param>
    private List<string> GetBattleSkillEffectPrefabs(Skill skill)
    {
        List<string> BattleSkillEffectPrefabs = new List<string> ();
        // 状態異常付与のスキルを持っているか確認
        var grantingConditions = skill.SkillEffects.Where (x => x.skill_effect.IsConditionGrantingLogic() && x.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Condition))
            .Select (x => x.skill_effect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition));
        foreach (var condition in grantingConditions) {
            if (condition.timing_skill_id.HasValue) {
                var timingSkill = MasterDataTable.skill [condition.timing_skill_id.Value];
                if (timingSkill != null) {
                    BattleSkillEffectPrefabs.AddRange(timingSkill.GetBattleSkillEffectName ());
                }
            }
            if (condition.within_skill_id.HasValue) {
                var withinSkill = MasterDataTable.skill [condition.within_skill_id.Value];
                if (withinSkill != null) {
                    BattleSkillEffectPrefabs.AddRange(withinSkill.GetBattleSkillEffectName ());
                }
            }
        }
        var skillInSkillList = skill.SkillEffects.Where (x => x.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Skill))
            .Select (x => x.skill_effect.GetValue<Skill> (SkillEffectLogicArgEnum.Skill));
        foreach (var inSkill in skillInSkillList) {
            BattleSkillEffectPrefabs.AddRange(GetBattleSkillEffectPrefabs (inSkill));
        }
        BattleSkillEffectPrefabs.AddRange(skill.GetBattleSkillEffectName ());

        return BattleSkillEffectPrefabs;
    }

    public void Dispose()
    {
        foreach (var cache in m_UnitResourceCache.Values) {
            cache.Dispose ();
        }
        m_UnitResourceCache.Clear ();
        m_UnitResourceCache = null;
        foreach (var cache in m_WeaponResourceCache.Values) {
            cache.Dispose ();
        }
        m_WeaponResourceCache.Clear ();
        m_WeaponResourceCache = null;

        m_BattleSkillEffectCache.Clear ();
        m_BattleSkillEffectCache = null;

        m_BattleBG = null;
        m_IsBgmLoaded = false;

        m_DidLoaded = null;

        m_instance = null;

        isLoaded = false;
    }

    bool isLoaded;
}
