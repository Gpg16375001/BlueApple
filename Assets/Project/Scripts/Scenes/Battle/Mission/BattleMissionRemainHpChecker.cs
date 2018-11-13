using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BattleMissionChecker : 残りHPチェッカー.
/// </summary>
public class BattleMissionRemainHpChecker : IBattleMissionChecker
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
        if(data.RemainHpRatio >= 1f){
            return;
        }

        var nPer = Mathf.FloorToInt(data.RemainHpRatio * 100f);
        m_bAchived = nPer >= m_limitVal;
    }

    /// <summary>初期化.指定回数がある場合.</summary>
    public BattleMissionRemainHpChecker(BattleMissionConditionDefine defineInfo, int limit, bool bAlreadyAchived = false)
    {
        m_explanatoryText = string.Format(defineInfo.text_detail, limit);
        m_limitVal = limit;
        m_bAchived = true;
        m_bAlreadyAchived = bAlreadyAchived;
    }

    private int m_limitVal;
}
