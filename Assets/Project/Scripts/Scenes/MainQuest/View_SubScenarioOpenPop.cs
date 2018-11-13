using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;

/// <summary>
/// View : サブシナリオ解放時のポップ.
/// </summary>
public class View_SubScenarioOpenPop : PopupViewBase
{

    /// <summary>
    /// 生成メソッド.
    /// </summary>
    public static View_SubScenarioOpenPop Create(List<SubQuest> questList, Action<SubQuest> jumpViewProc)
    {
        var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_SubScenarioOpenPop");
        var c = go.GetOrAddComponent<View_SubScenarioOpenPop>();
        c.InitInternal(questList, jumpViewProc);
        return c;
    }
    private void InitInternal(List<SubQuest> questList, Action<SubQuest> jumpViewProc)
    {
        m_questList = questList;
        m_jumpViewProc = jumpViewProc;
        m_decideQuest = m_questList[0];
        m_questList.RemoveAt(0);

        // ボタン登録.
        this.SetCanvasButtonMsg("Yes/bt_Common", DidTapYes);
        this.SetCanvasButtonMsg("No/bt_Common", DidTapNo);
        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapNo ();
    }
    #region ButtonDelegate.

    // ボタン : すぐにサブシナリオを見に行きますか? Yes選択.
    void DidTapYes()
    {
        if (IsClosed) {
            return;
        }
        AwsModule.ProgressData.CurrentQuest = m_decideQuest;
        this.PlayOpenCloseAnimation(false, () => {
            m_jumpViewProc(m_decideQuest);
            Dispose();
        });
    }

    // ボタン : すぐにサブシナリオを見に行きますか? No選択. まだクエストリストが残っていれば連続してポップを開く.
    void DidTapNo()
    {
        if (IsClosed) {
            return;
        }
        this.PlayOpenCloseAnimation(false, () => {
            if(m_questList.Count > 0){
                Create(m_questList, m_jumpViewProc);
            }
            Dispose();
        });
    }

    #endregion


    private Action<SubQuest> m_jumpViewProc;
    private List<SubQuest> m_questList = new List<SubQuest>();
    private SubQuest m_decideQuest;
}
