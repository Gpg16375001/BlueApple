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
/// View : プレイヤー情報切り替え.
/// </summary>
public class View_PlayerInfoPop : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
	public static View_PlayerInfoPop Create(Action<bool> didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("MyPage/View_PlayerInfoPop");
		var c = go.GetOrAddComponent<View_PlayerInfoPop>();
		c.InitInternal(didClose);
		return c;
	}
    private void InitInternal(Action<bool> didClose)
	{
        m_didClose = didClose;

		// メニュー押せないように.
		View_GlobalMenu.IsEnableButtons = false;      
		View_PlayerMenu.IsEnableButtons = false;      

		// ラベル設定.
		var userData = AwsModule.UserData;      
        this.GetScript<InputField>("PlayerName/bt_Rewrite").text = AwsModule.UserData.EditUserName ?? userData.UserName;
        this.GetScript<InputField> ("PlayerName/bt_Rewrite").onValueChanged.AddListener (UserNameChanged);
		this.GetScript<TextMeshProUGUI>("txtp_Rank").text = userData.UserData.Level.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_PlayerID").text = userData.CustomerID.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_TotalGem").text = userData.UserData.GemCount.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_TotalCoin").text = userData.UserData.GoldCount.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_FollowNum").text = userData.UserData.FollowCount.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_FollowerNum").text = userData.UserData.FollowerCount.ToString();
        this.GetScript<InputField>("Message/bt_Rewrite").text = AwsModule.UserData.EditProfileMessage ?? userData.ProfileMessage;
        this.GetScript<InputField> ("Message/bt_Rewrite").onValueChanged.AddListener (MessageChanged);

		// サポート編成
		var root = this.GetScript<GridLayoutGroup>("SupportUnitGrid");
        for(ElementEnum element=ElementEnum.fire; element <= ElementEnum.dark; ++element){
            var card = userData.GetSupportCardList (element);
			var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", root.gameObject);
			var c = go.GetOrAddComponent<ListItem_UnitIcon>();
			c.Init(card);
		}

		// ボタン
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("HomeCharacterChange/bt_Common", DidTapCharacterChange);
		this.SetCanvasCustomButtonMsg("SupportChange/bt_Common", DidTapSupportChange);
        SetBackButton ();
	}

    IEnumerator ClosePopup()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        bool isChange = false;
        // 空文字などのチェック
        var userName = AwsModule.UserData.EditUserName ?? AwsModule.UserData.UserName;
        if (string.IsNullOrEmpty (userName) || string.IsNullOrEmpty (userName.Trim ())) {
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
            PopupManager.OpenPopupOK ("ユーザー名を設定してください。");
            yield break;
        }
        var profileMessage = AwsModule.UserData.EditProfileMessage ?? AwsModule.UserData.ProfileMessage;
        if (string.IsNullOrEmpty (profileMessage) || string.IsNullOrEmpty (profileMessage.Trim ())) {
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
            PopupManager.OpenPopupOK ("一言メッセージは空欄にできません。");
            yield break;
        }

        if (AwsModule.UserData.EditUserName != null && AwsModule.UserData.UserName != AwsModule.UserData.EditUserName) {
            bool isWait = true;
            bool isAccept = false;
            // NGワードチェック
            SendAPI.TextInspect (userName,
                (success, response) => {
                    isWait = false;
                    if(success && response.IsAccept) {
                        isAccept = true;
                        AwsModule.UserData.UserName = AwsModule.UserData.EditUserName;
                    } else {
                        PopupManager.OpenPopupOK("名前に不適切な表現が含まれています。");
                        this.GetScript<InputField>("PlayerName/bt_Rewrite").text = AwsModule.UserData.UserName;
                    }
                    AwsModule.UserData.EditUserName = null;
                }
            );

            yield return new WaitUntil (() => !isWait);

            if (!isAccept) {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                yield break;
            }

            isChange = true;
        }

        if (AwsModule.UserData.EditProfileMessage != null && AwsModule.UserData.ProfileMessage != AwsModule.UserData.EditProfileMessage) {
            bool isWait = true;
            bool isAccept = false;
            // NGワードチェック
            SendAPI.TextInspect (profileMessage,
                (success, response) => {
                    isWait = false;
                    if(success && response.IsAccept) {
                        isAccept = true;
                        AwsModule.UserData.ProfileMessage = AwsModule.UserData.EditProfileMessage;
                    } else {
                        PopupManager.OpenPopupOK("一言メッセージに不適切な表現が含まれています。");
                        this.GetScript<InputField>("Message/bt_Rewrite").text = AwsModule.UserData.ProfileMessage;
                    }
                    AwsModule.UserData.EditProfileMessage = null;
                }
            );

            yield return new WaitUntil (() => !isWait);

            if (!isAccept) {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                yield break;
            }

            isChange = true;
        }

        if (isChange) {
            bool isWait = true;
            AwsModule.UserData.Sync (
                (success, sender, e) => {
                    isWait = false;
                }
            );

            yield return new WaitUntil (() => !isWait);
        }

        LockInputManager.SharedInstance.IsLock = false;
        View_FadePanel.SharedInstance.IsLightLoading = false;

        this.PlayOpenCloseAnimation(false,
            () => {
                if (m_didClose != null) {
                    m_didClose(true);
                }
                this.Dispose();
                View_GlobalMenu.IsEnableButtons = true;
                View_PlayerMenu.IsEnableButtons = true;
            }
        );
    }

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

	#region ButtonDelegate.
    void DidTapBack()
	{
        if (IsClosed) {
            return;
        }
        this.StartCoroutine(this.ClosePopup());
	}   

	// ボタン : 閉じる押下.
	void DidTapClose()
	{
        if (IsClosed) {
            return;
        }
        this.StartCoroutine(this.ClosePopup());
	}
	// ボタン : ホームキャラクター変更.
    void DidTapCharacterChange()
	{
        if (IsClosed) {
            return;
        }

#if false
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                Dispose();
                ScreenChanger.SharedInstance.GoToSelectUnit(
                    false,
                    false,
                    false,
                    false,
                    0, 0, 
                    null,
                    false,
                    (card) => {
                        AwsModule.UserData.MainCardID = card.CardId;
                        if(m_didClose != null) {
                            m_didClose(false, card);
                        }
                    },
                    () => {
                        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
                            () => {
                                ScreenChanger.SharedInstance.GoToMyPage(
                                    () => {
                                        // PlayerInfoPopupを再度開く
                                        View_PlayerMenu.CreateIfMissing().OpenPlayerInfoPop();
                                    }
                                );
                            }
                        );
                    }
                );
            }
        );
#else
		CharacterChange(
			() => {
				if(m_didClose != null)
					m_didClose(false);
			},
			() => {
				ScreenChanger.SharedInstance.GoToMyPage( () => View_PlayerMenu.CreateIfMissing().OpenPlayerInfoPop() );
			}
		);
#endif
	}

	public static void CharacterChange(Action didChangeCard=null, Action didEndFade=null)
	{
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToSelectUnit(
                    false,
                    false,
                    false,
                    false,
                    0, 0, 
                    null,
                    false,
                    (card) => {
						AwsModule.UserData.MainCardID = card.CardId;
						if( didChangeCard != null )
							didChangeCard();
					},
                    () => {
                        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
                            () => {
								if( didEndFade != null )
									didEndFade();
                            }
                        );
                    }
                );
            }
        );
	}

	// ボタン : サポート編成.
    void DidTapSupportChange()
	{
        if (IsClosed) {
            return;
        }

        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                Dispose();
                ScreenChanger.SharedInstance.GoToPlayerSupportEdit ();
            }
        );
	}

    void UserNameChanged(string value)
    {
        AwsModule.UserData.EditUserName = value;
    }

    void MessageChanged(string value)
    {
        AwsModule.UserData.EditProfileMessage = value;
    }
    #endregion

	private Action<bool> m_didClose;
}
