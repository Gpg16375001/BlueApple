using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// BattleMissionChecker : コンテニュー回数
/// </summary>
public class BattleMissionContinueChecker : IBattleMissionChecker
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
        if (IsAlreadyAchived) {
            return;
        }
        if(data.ContinueCount <= 0){
            return;
        }

        if(m_limitCount <= 0){
            m_bAchived = false;
        }else{
            m_bAchived = data.ContinueCount <= m_limitCount;
        }
    }

    /// <summary>初期化.</summary>
    public BattleMissionContinueChecker(BattleMissionConditionDefine defineInfo, bool bAlreadyAchived = false)
    {
        m_explanatoryText = defineInfo.text_detail;
        m_bAchived = true;
        m_bAlreadyAchived = bAlreadyAchived;
    }
    /// <summary>初期化.指定回数がある場合.</summary>
    public BattleMissionContinueChecker(BattleMissionConditionDefine defineInfo, int limit, bool bAlreadyAchived = false)
    {
        m_explanatoryText = string.Format(defineInfo.text_detail, limit);
        m_limitCount = limit;
        m_bAchived = true;
        m_bAlreadyAchived = bAlreadyAchived;
    }

    private int m_limitCount;
}
