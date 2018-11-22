using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class MiniView_BattleEventResult : ViewBase, IBattleResultPage
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
        return ResultTitle.Event;
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(int index, ReceiveQuestsCloseQuest questResult)
    {
        m_index = index;

        GetScript<TextMeshProUGUI> ("txtp_EventRewardNum").SetText (questResult.GainEventPoint - questResult.GainBonusEventPoint);
        GetScript<TextMeshProUGUI> ("txtp_EventBounsNum").SetText (questResult.GainBonusEventPoint);
        GetScript<TextMeshProUGUI> ("txtp_EventTotalNum").SetText (questResult.GainEventPoint);
    }

    /// <summary>開く.</summary>
    public void Open()
    {
        if (!this.gameObject.activeSelf) {
            this.gameObject.SetActive(true);
        }

        m_bEffecting = true;
        StartCoroutine(PlayOpenClose (true,
            () => {

                m_bEffecting = false;
            }
        ));
    }

    /// <summary>閉じる.</summary>
    public void Close(Action didEnd)
    {
        StartCoroutine(PlayOpenClose (false, didEnd));
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
    /// <summary>アニメーションを強制的に即時終了させる.</summary>
    public void ForceImmediateEndAnimation()
    {
        //m_bEffecting = false;
    }
}
