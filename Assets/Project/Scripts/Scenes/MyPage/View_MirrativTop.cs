using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : Mirrativトップ.
/// </summary>
public class View_MirrativTop : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
    public static View_MirrativTop Create(Action didClose=null)
	{
		var go = GameObjectEx.LoadAndCreateObject("MyPage/View_MirrativTop");
		var c = go.GetOrAddComponent<View_MirrativTop>();
        c.InitInternal(didClose);
        return c;
	}
    private void InitInternal(Action didClose)
	{
        m_DidClose = didClose;
		// TODO : 文言設定.

		// ボタン
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("Broadcast/bt_Common", DidTapBroadcast);
		this.SetCanvasCustomButtonMsg("Watch/bt_Common", DidTapWatch);

        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

	#region ButtonDelegate.

	// ボタン : 閉じる.
    void DidTapClose()
	{
        this.PlayOpenCloseAnimation(false, () => {
            if (m_DidClose != null){
                m_DidClose();
            }
            base.Dispose();
        });
	}

	// ボタン : 動画を配信する.
    void DidTapBroadcast()
	{
		Application.OpenURL("https://ws9f.adj.st//broadcast/live?adjust_t=34dm61q_67in87g&adjust_deeplink=mirr%3A%2F%2F%2Fbroadcast%2Flive%3Fapp_id%3Djp.fg.precatus%26title%3D%2523%25E3%2583%2597%25E3%2583%25AC%25E3%2582%25AB%25E3%2583%2588%25E3%2582%25A5%25E3%2582%25B9&app_id=jp.fg.precatus&title=%23%E3%83%97%E3%83%AC%E3%82%AB%E3%83%88%E3%82%A5%E3%82%B9");
	}

	// ボタン : 動画を観る.
    void DidTapWatch()
	{
		Application.OpenURL("https://ws9f.adj.st//app/jp.fg.precatus?adjust_t=34dm61q_67in87g&adjust_deeplink=mirr:///app/jp.fg.precatus");
	}

    #endregion

    Action m_DidClose = null;
}
