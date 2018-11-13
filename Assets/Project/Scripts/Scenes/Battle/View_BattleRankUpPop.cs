using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using TMPro;


/// <summary>
/// View : バトル結果画面でランクアップ.
/// </summary>
public class View_BattleRankUpPop : PopupViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
    public static View_BattleRankUpPop Create(int startLevel, int nextLevel, Action didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleRankUpPop");
        var c = go.GetOrAddComponent<View_BattleRankUpPop>();
        c.InitInternal(startLevel, nextLevel, didClose);
		return c; 
	}

	private void InitInternal(int startLevel, int nextLevel, Action didClose)
	{
		m_didClose = didClose;

		// ラベル.
        var current = MasterDataTable.user_level[startLevel];
		var next = MasterDataTable.user_level[nextLevel];
        this.GetScript<TextMeshProUGUI>("txtp_PlayerRankNumBefore").SetText(current.level);
        this.GetScript<TextMeshProUGUI>("txtp_PlayeAPTitleNumBefore").SetText(current.ap_max);
        this.GetScript<TextMeshProUGUI>("txtp_FollowNumBefore").SetText(current.follow_max);

        this.GetScript<TextMeshProUGUI>("txtp_PlayerRankNumAfter").SetText(next.level);
        this.GetScript<TextMeshProUGUI>("txtp_PlayeAPTitleNumAfter").SetText(next.ap_max);
        this.GetScript<TextMeshProUGUI>("txtp_FollowNumAfter").SetText(next.follow_max);
 
        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_Next", DidTapOk);
		this.SetCanvasCustomButtonMsg("bt_Common", DidTapOk);

        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapOk();
    }

	#region ButtonDelegate.
	// ボタン : ok
    void DidTapOk()
	{
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            if (m_didClose != null) {
                m_didClose();
            }
            this.Dispose();
        });
	}

    #endregion

	private Action m_didClose;
}