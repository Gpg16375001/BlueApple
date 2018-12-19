using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class Screen_PlayerSupportEdit : ViewBase {

    public void Init(bool dispGlobalMenu, string prevSceneName)
    {
        m_DispGlobalMenu = dispGlobalMenu;
        m_PrevSceneName = prevSceneName;
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackButon;
        View_GlobalMenu.DidTapButton += DidTapGlobalMenu;
        AwsLocalUserData userData = AwsModule.UserData;
        SetSupportCardList(userData);

        SetCanvasCustomButtonMsg ("bt_PartySave", DidTapSave);
        SetCanvasCustomButtonMsg("AutoSelect/bt_CommonS01", DidTapAutoOrganization);

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    private void DidTapGlobalMenu(System.Action exec)
    {
        if (AwsModule.UserData.IsSupportCardListModfiy) {
            PopupManager.OpenPopupYN ("サポートユニットが変更されていますが保存されていません。保存しなくてよろしいでしょうか？",
                () => {
                    AwsModule.UserData.SupportUnitReset();
                    exec();
                },
                () => {
                }
            );
            return;
        }
        exec();
    }

    private void DidTapSave()
    {
        if (AwsModule.UserData.IsSupportCardListModfiy) {
            LockInputManager.SharedInstance.IsLock = true;
            View_FadePanel.SharedInstance.IsLightLoading = true;
            AwsModule.UserData.Sync (
                (success, sender, e) => {
                    LockInputManager.SharedInstance.IsLock = false;
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                }
            );
        }
    }

	void DidTapAutoOrganization()
	{
		PopupManager.OpenPopupYN ("各属性を編成します\nよろしいですか？",
			() => {
				foreach( var pair in View_PartyEditAutoSelect.GetElementTypeDict() ) {
					if( pair.Value.Count > 0 ) {
						AwsModule.UserData.SetSupportCardList( pair.Key, pair.Value[0], true );
					}
				}
				SetSupportCardList( AwsModule.UserData, true );
			},
			() => {}
		);
	}

    private void DidTapBackButon()
    {
        if (AwsModule.UserData.IsSupportCardListModfiy) {
            PopupManager.OpenPopupYN ("サポートユニットが変更されていますが保存されていません。保存せずに戻りますか？",
                () => {
                    AwsModule.UserData.SupportUnitReset();
                    BackScene();
                },
                () => {
                }
            );
            return;
        }
        BackScene ();
    }

    private void BackScene()
    {
        if (m_PrevSceneName == "MyPage") {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
                () => {
                    ScreenChanger.SharedInstance.GoToMyPage(
                        () => {
                            View_PlayerMenu.CreateIfMissing().OpenPlayerInfoPop();
                        }
                    );
                }
            );
        } else {
            View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black,
                () => {
                    ScreenChanger.SharedInstance.GoToPartyEditTop();
                }
            );
        }
    }

    private void SetSupportCardList(AwsLocalUserData userData, bool isUpdate=false)
    {
        for (ElementEnum element = ElementEnum.fire; element <= ElementEnum.dark; ++element) {
            var card = userData.GetSupportCardList (element);
            var script = GetScript<ListItem_PartyEditUnit> (string.Format ("ListItem_PartyEditUnit{0}", (int)element));
			if( !isUpdate ) {
				script.Init (
					card,
					null,
					(int)element,
					DidTapEditUnit,
					DidLongTapEditUnit
				);
			}else{
				script.UpdateUnit( card, null, (int)element );
			}
            // 長押し有効か
            script.EnableLongPress = true;
        }
    }

    private bool DidTapEditUnit(int position, int cardId)
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToSelectUnit (
                    false, false, false, true, cardId, 0, (ElementEnum)position, false,
                    (card) => {
                        SelectUnit(position, card);
                    },
                    NextScreenBackScene
                );
            }
        );
        return false;
    }

    private void DidLongTapEditUnit(CardData card, bool supportCard)
    {
        if (card != null) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
                () => {
                    ScreenChanger.SharedInstance.GoToUnitDetails (
                        card,
                        NextScreenBackScene,
                        false,
                        false
                    );
                }
            );
        }
    }

    private void SelectUnit(int position, CardData card)
    {
        AwsModule.UserData.SetSupportCardList ((ElementEnum)position, card);
    }

    private void NextScreenBackScene()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToPlayerSupportEdit(m_DispGlobalMenu, m_PrevSceneName);
            }
        );
    }



    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    bool m_DispGlobalMenu;
    string m_PrevSceneName;
}
