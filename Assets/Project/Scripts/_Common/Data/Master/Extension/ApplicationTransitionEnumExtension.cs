using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public static class ApplicationTransitionEnumExtension {

    public static void Transition(this ApplicationTransitionEnum self, int? details)
    {
        switch (self) {
        case ApplicationTransitionEnum.Story:
            LockInputManager.SharedInstance.IsLock = true;
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToMainQuestSelect ();
            });
            break;
        case ApplicationTransitionEnum.Shop:
            LockInputManager.SharedInstance.IsLock = true;

            // TODO: shopタイプ設定出来るようにする。
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToShop ();
            });
            break;
        case ApplicationTransitionEnum.GemShop:
            View_GemShop.OpenGemShop();
            break;
        case ApplicationTransitionEnum.Gacha:
            LockInputManager.SharedInstance.IsLock = true;
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToGacha ();
            });
            break;
        case ApplicationTransitionEnum.PVP:
            if (AwsModule.PartyData.PvPTeam.IsEmpty) {
                // PVPチーム編成に飛ばす
                PopupManager.OpenPopupOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                    });
                });
            } else {
                LockInputManager.SharedInstance.IsLock = true;
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => {
                    LockInputManager.SharedInstance.IsLock = false;
                    ScreenChanger.SharedInstance.GoToPVP ();
                });
            }
            break;
        case ApplicationTransitionEnum.Event:
            // TODO: イベントクエストへの遷移
            PopupManager.OpenPopupOK ("未実装");
            break;
        }
    }
}
