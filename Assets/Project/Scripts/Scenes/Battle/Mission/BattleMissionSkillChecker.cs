using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// BattleMissionChecker : スキルや必殺技の仕様チェック.
/// </summary>
public class BattleMissionSkillChecker : IBattleMissionChecker
{
    /// <summary>
    /// 開始以前に前回プレイ時などで既に達成済みか.
    /// </summary>
    public bool IsAlreadyAchived { get { return m_bAlreadyAchived; } }
    private bool m_bAlreadyAchived;

    /// <summary>
    /// 達成しているかどうか.
    /// </summary>
    public bool IsAchived { get { return IsAlreadyAchived || m_bAchived; } }
    private bool m_bAchived = true;

    /// <summary>
    /// 説明文.
    /// </summary>
    public string ExplanatoryText { get { return m_explanatoryText; } }
    private string m_explanatoryText;


    /// <summary>
    /// 進捗更新.
    /// </summary>
    public void UpdateProgress(BattleMissionProgressData data)
    {
        if(IsAchived){
            return;
        }

        // 必殺技、キャラと武器スキルの回数.
        if(m_type == BattleMissionEnum.CountSpecial){
            m_bAchived = data.UseSpecialCount >= m_needCnt;
        } 
        else if(m_type == BattleMissionEnum.CountCharaSkill){
            m_bAchived = data.UseCharaSkillCount >= m_needCnt;
        }
        else if (m_type == BattleMissionEnum.CountWeaponSkill) {
            m_bAchived = data.UseWeaponSkillCount >= m_needCnt;
        }
    }

    /// <summary>初期化.使用回数を指定.</summary>
    public BattleMissionSkillChecker(BattleMissionConditionDefine defineInfo, int needCnt, bool bAlreadyAchived = false)
    {
        m_explanatoryText = string.Format(defineInfo.text_detail, needCnt);
        m_type = MasterDataTable.battle_mission.DataList.Find(m => m.name == defineInfo.name).Enum;
        m_needCnt = needCnt;
        m_bAchived = false;
        m_bAlreadyAchived = bAlreadyAchived;
    }

    private BattleMissionEnum m_type;
    private int m_needCnt;
}
