using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : チュートリアル最冒頭の同意画面.
/// </summary>
public class View_Popup_Terms : ViewBase
{

    /// <summary>
    /// 生成.同意するまでは王様ループ.
    /// </summary>
	public static void Create(Action didAgree)
	{
		var go = GameObjectEx.LoadAndCreateObject("Tutorial/View_Popup_Terms");
		var c = go.GetOrAddComponent<View_Popup_Terms>();
		c.InitInternal(didAgree);
	}
    private void InitInternal(Action didAgree)
	{
		m_didAgree = didAgree;

		this.SetCanvasCustomButtonMsg("Yes/bt_Common", DidTapAgree);
		this.SetCanvasCustomButtonMsg("No/bt_Common", DidTapNotAgree);
		this.SetCanvasCustomButtonMsg("Terms/bt_Common", DidTapTerms);
		this.SetCanvasCustomButtonMsg("Policy/bt_Common", DidTapPrivacypolicy);
	}

	#region ButtonDelegate.

	// ボタン : 同意する.
    void DidTapAgree()
	{
		this.StartCoroutine(this.CoPlayClose(m_didAgree));
	}
	// ボタン : 同意しない.
    void DidTapNotAgree()
	{
		PopupManager.OpenPopupOK("『利用規約』と\n『プライバシーポリシー』に\n同意いただかないと、\n「プレカトゥスの天秤」は\nプレイできません。");
	}
    
	// ボタン : 利用規約
    void DidTapTerms()
    {
		var menu = MasterDataTable.option_menu.DataList.Find(d => d.Enum == OptionMenuEnum.TermsOfService);
		var info = MasterDataTable.help_notice.DataList.Find(d => d.subject == menu.Enum);
		PopupManager.OpenPopupSystemOkWithScroll(info.text, null, menu.name);
    }
	// ボタン : プライバシーポリシー
    void DidTapPrivacypolicy()
	{
		var menu = MasterDataTable.option_menu.DataList.Find(d => d.Enum == OptionMenuEnum.PrivacyPolicy);
        var info = MasterDataTable.help_notice.DataList.Find(d => d.subject == menu.Enum);
		PopupManager.OpenPopupSystemOkWithScroll(info.text, null, menu.name);
	}   

    #endregion

	IEnumerator CoPlayClose(Action didClose = null)
    {
		this.IsEnableButton = false;    // ボタン表示用
        LockInputManager.SharedInstance.IsLock = true;  // Fadeを貫通して押せるのでその制御.
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didClose != null) {
            didClose();
        }
        this.Dispose();
		this.IsEnableButton = true;    // ボタン表示用
        LockInputManager.SharedInstance.IsLock = false;  // Fadeを貫通して押せるのでその制御.
    }

	private void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	private Action m_didAgree;
}
