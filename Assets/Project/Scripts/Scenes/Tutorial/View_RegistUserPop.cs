using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// View : プロローグユーザー名入力View.
/// </summary>
public class View_RegistUserPop : ViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_RegistUserPop Create(Action didEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("Tutorial/View_RegistUserPop");
		var c = go.GetOrAddComponent<View_RegistUserPop>();
		c.InitInternal(didEnd);
		return c;
	}
	private void InitInternal(Action didEnd)
	{
		m_didEnd = didEnd;

        // 禁止ワード対応に備え、一旦デフォルト設定している文言で設定をかける.
		AwsModule.UserData.UserName = this.GetScript<Text>("NameInput/txt_Input").text ?? NAME_BASE;   

		// ボタン.
		this.SetCanvasCustomButtonMsg("Decision/bt_Common", DidTapDecide);
	}

	// ボタン : 名前確定.
    void DidTapDecide()
	{
		// 名前を設定.
		var name = this.GetScript<Text>("NameInput/txt_Input").text;
		if(string.IsNullOrEmpty(name)){
			PopupManager.OpenPopupSystemOK("名前を入力してください。", () => this.GetScript<InputField>("bt_InputAreaParchment").text = NAME_BASE);
			return;
		}
  
		this.IsEnableButton = false;    // ボタン表示用
		LockInputManager.SharedInstance.IsLock = true;  // Fadeを貫通して押せるのでその制御.
		View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.TextInspect(name, (bSuccess, res) => {
			if(!bSuccess || res == null){
				View_FadePanel.SharedInstance.IsLightLoading = false;
				this.IsEnableButton = true;
				LockInputManager.SharedInstance.IsLock = false;
				return;
			}
			if(!res.IsAccept){
				PopupManager.OpenPopupSystemOK("名前に不適切な表現が含まれています。", () => this.GetScript<InputField>("bt_InputAreaParchment").text = NAME_BASE);
				View_FadePanel.SharedInstance.IsLightLoading = false;
				this.IsEnableButton = true;
				LockInputManager.SharedInstance.IsLock = false;
				return;
			}

            AwsModule.UserData.UserName = name;
			AwsModule.UserData.Sync((bool success, object sender, EventArgs e) => {
				UtageModule.SharedInstance.SetAdvParam("user_name", name);
				this.Dispose();
				LockInputManager.SharedInstance.IsLock = false;
				if (m_didEnd != null) {
					m_didEnd();
				}
				View_FadePanel.SharedInstance.IsLightLoading = false;
			});
        });
	}

	void Awake()
    {
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.FadeCamera);
    }

	private Action m_didEnd;
	const string NAME_BASE = "観測者";   
}
