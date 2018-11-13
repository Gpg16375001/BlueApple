using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class Screen_PartyEditTop : ViewBase {

    public void Init()
    {
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        // 遷移のみ設定
        SetCanvasCustomButtonMsg("bt_EditQuestParty", EditQuestParty);
        SetCanvasCustomButtonMsg("bt_EditPvpParty", EditPvpParty);
        SetCanvasCustomButtonMsg("bt_EditSupportParty", EditSupportParty);

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    void EditQuestParty()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToPartyEdit();
            }
        );
    }

    void EditPvpParty()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToPVPPartyEdit();
            }
        );
    }

    void EditSupportParty()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToPlayerSupportEdit(true);
            }
        );
    }

    void DidTapBack()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToMyPage();
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
}
