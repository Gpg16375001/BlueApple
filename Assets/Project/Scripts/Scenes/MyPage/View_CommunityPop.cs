using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using TMPro;
using SmileLab.Net.API;


/// <summary>
/// View : コミュニティメニュー
/// </summary>
public class View_CommunityPop : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
	public static View_CommunityPop Create()
	{
		var go = GameObjectEx.LoadAndCreateObject("MyPage/View_CommunityPop");
		var c = go.GetOrAddComponent<View_CommunityPop>();
		c.InitInternal();
		return c;
	}

    private void InitInternal()
	{
		// ボタン
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("Twitter_1/bt_Link", DidTapTwitter);
		this.SetCanvasCustomButtonMsg("Twitter_2/bt_Link", DidTapTwitterSupport);
		this.SetCanvasCustomButtonMsg("Other_1/bt_Link", DidTapWiki);
		this.SetCanvasCustomButtonMsg("Other_2/bt_Link", DidTapMirrative);
	}

	// ボタン : 閉じる押下.
	void DidTapClose()
	{
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            this.Dispose ();
        } );
	}

	void DidTapTwitter()
	{
		openUrl( "TWITTER_URL", "https://twitter.com/precatus_pr" );
	}

	void DidTapTwitterSupport()
	{
		openUrl( "TWITTER_SUPPORT_URL", "https://twitter.com/precatus_sp" );
	}

	void DidTapWiki()
	{
		openUrl( "WIKI_URL", "https://precatus.com/index.html" );
	}

	void DidTapMirrative()
	{
		if( !Screen_MyPage.Instance ) return;

		//破棄するのでScreen_MyPageに投げる
		Screen_MyPage.Instance.OpenMirrativ();
		DidTapClose();
	}

	void openUrl( string key, string defaultUrl=null )
	{
		var url = TextData.GetText( key, defaultUrl );
		if( !string.IsNullOrEmpty( url ) ) {
			var uri = new Uri( url );
			Application.OpenURL( uri.AbsoluteUri );
		}
	}
}
