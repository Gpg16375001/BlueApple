using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// バトルミッション進捗管理クラス.
/// </summary>
public class BattleMissionProgressManager
{
    /// <summary>
    /// ミッションリスト.
    /// </summary>
    public List<IBattleMissionChecker> MissionList { get; private set; }

    /// <summary>
    /// 初期化.
    /// </summary>
    public BattleMissionProgressManager(int stageId)
    {
        MissionList = new List<IBattleMissionChecker>();

        if(!MasterDataTable.battle_mission_setting.DataList.Exists(m => m.stage_id == stageId)){
            return;
        }

        var info = MasterDataTable.battle_mission_setting.DataList.Find(m => m.stage_id == stageId);
        if(info.condition_1 != null){
            MissionList.Add(GetMissionChecker(1, info.condition_1.Value, info.variable_1));
            Debug.Log("SetBattleMission "+info.condition_1.Value+" : "+MissionList[0].ExplanatoryText);
        }
        if(info.condition_2 != null){
            MissionList.Add(GetMissionChecker(2, info.condition_2.Value, info.variable_2));
            Debug.Log("SetBattleMission " + info.condition_2.Value + " : " + MissionList[1].ExplanatoryText);
        }
        if(info.condition_3 != null){
            MissionList.Add(GetMissionChecker(3, info.condition_3.Value, info.variable_3));
            Debug.Log("SetBattleMission " + info.condition_3.Value + " : " + MissionList[2].ExplanatoryText);
        }
    }
    // 対応するミッションチェッカーを取得.
    private IBattleMissionChecker GetMissionChecker(int index, int id, int variable)
    {
        var data = MasterDataTable.battle_mission_condition_define[id];
        var info = MasterDataTable.battle_mission.DataList.Find(m => m.name == data.name);
        var bAlreadyAchived = false;
        if (AwsModule.ProgressData.CurrentQuestAchievedMissionIdList != null) {
            bAlreadyAchived = AwsModule.ProgressData.CurrentQuestAchievedMissionIdList.Contains (index);
        }
        switch (info.Enum) {
            case BattleMissionEnum.NoNobadytInactive:       // 戦闘不能無し
                return new BattleMissionInactiveMemberChecker(data, bAlreadyAchived);
            case BattleMissionEnum.CountInactiveMember:     // 戦闘不能人数
                return new BattleMissionInactiveMemberChecker(data, variable, bAlreadyAchived);
            case BattleMissionEnum.NoContinue:              // コンティニュー無し
                return new BattleMissionContinueChecker(data, bAlreadyAchived);
            case BattleMissionEnum.CountContinue:           // コンティニュー回数
                return new BattleMissionContinueChecker(data, variable, bAlreadyAchived);
            case BattleMissionEnum.CountAction:             // 手数
                return new BattleMissionActionCntChecker(data, variable, bAlreadyAchived);
            case BattleMissionEnum.RemainHP:                // 残HP割合
                return new BattleMissionRemainHpChecker(data, variable, bAlreadyAchived);
            case BattleMissionEnum.SpecificCardElement:     // カード属性限定
            case BattleMissionEnum.SpecificCharacter:       // 特定キャラクター使用
            case BattleMissionEnum.SpecificCard:            // 特定カード使用
                return new BattleMissionCardChecker(data, variable, bAlreadyAchived);
            case BattleMissionEnum.CountSpecial:            // 必殺技回数
            case BattleMissionEnum.CountCharaSkill:         // キャラスキル回数
            case BattleMissionEnum.CountWeaponSkill:        // 武器スキル回数
                return new BattleMissionSkillChecker(data, variable, bAlreadyAchived);
            case BattleMissionEnum.CountDebuf:              // デバフ付与回数
                return new BattleMissionBufCntChecker(data, variable, bAlreadyAchived);
        }
        return null;
    }

    /// <summary>
    /// 更新処理.
    /// </summary>
    public void Update(BattleMissionProgressData progress)
    {
        foreach(var c in MissionList){
            c.UpdateProgress(progress);
        }
    }

    /// <summary>
    /// 全ミッションの説明文リストを返す.
    /// </summary>
    public List<string> GetMissionTextList()
    {
        return MissionList.Select(m => m.ExplanatoryText).ToList();
    }

    /// <summary>
    /// 達成しているミッションについて1から始まるindexのリスト形式で返す.
    /// </summary>
    public List<int> GetAchivedMissionIndexList()
    {
        var rtn = new List<int>();
        var idx = 1;
        foreach(var m in MissionList){
            if(m.IsAchived){
                rtn.Add(idx);
            }
            ++idx;
        }
        return rtn;
    }
}

/// <summary>
/// バトルミッション進捗共有データ.
/// </summary>
[System.Serializable]
public class BattleMissionProgressData
{
    /// <summary>コンティニューカウント.</summary>
    public int ContinueCount;
    /// <summary>キャラ死亡回数.</summary>
    public int CharaDeadCount;
    /// <summary>行動回数.</summary>
    public int ActionCount;
    /// <summary>残りHP割合.</summary>
    public float RemainHpRatio { get { return (float)CurrentMembersHp / (float)m_firstHP; } }
    /// <summary>現在のメンバー合計HP.</summary>
    public int CurrentMembersHp;
    /// <summary>使用カードのIDリスト</summary>
    public int[] UseCardIdList  { get; private set; }
    /// <summary>必殺技使用回数.</summary>
    public int UseSpecialCount;
    /// <summary>キャラスキル使用回数.</summary>
    public int UseCharaSkillCount;
    /// <summary>武器スキル使用回数.</summary>
    public int UseWeaponSkillCount;
    /// <summary>デバフ付与回数.</summary>
    public int SuccessDebufCount;

    public BattleMissionProgressData(int[] useCardIdList, int allHp)
    {
        UseCardIdList = new int[useCardIdList.Length];
        System.Array.Copy(useCardIdList, UseCardIdList, useCardIdList.Length);
        m_firstHP = CurrentMembersHp = allHp;
    }

    public void Reversion(int[] useCardIdList)
    {
        UseCardIdList = new int[useCardIdList.Length];
        System.Array.Copy(useCardIdList, UseCardIdList, useCardIdList.Length);
    }

    [SerializeField]
    private int m_firstHP;
}
