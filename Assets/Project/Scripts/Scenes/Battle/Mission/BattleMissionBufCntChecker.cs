using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// BattleMissionChecker : バフ、デバフの回数チェック.
/// </summary>
public class BattleMissionBufCntChecker : IBattleMissionChecker
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

        m_bAchived = data.SuccessDebufCount >= m_needCnt;
    }

    /// <summary>初期化.使用回数を指定.</summary>
    public BattleMissionBufCntChecker(BattleMissionConditionDefine defineInfo, int needCnt, bool bAlreadyAchived = false)
    {
        m_explanatoryText = string.Format(defineInfo.text_detail, needCnt);
        m_needCnt = needCnt;
        m_bAchived = false;
        m_bAlreadyAchived = bAlreadyAchived;
    }

    private int m_needCnt;
}
