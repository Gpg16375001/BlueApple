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
            LockInputManager.SharedInstance.IsLock = true;
            if (details != null) {
                TransitionEvent(MasterDataTable.event_info[(int)details]);
            } else {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    LockInputManager.SharedInstance.IsLock = false;
                    ScreenChanger.SharedInstance.GoToEvent();
                });
            }
            break;
        }
    }

    static void TransitionEvent(EventInfo eventInfo)
    {
        // nullの時はとりあえずTopに遷移させる
        if (eventInfo == null) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToEvent();
            });
        }
        switch (eventInfo.event_type) {
        case EventTypeEnum.Enhance:
            // 強化イベントへ
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToDailyQuest(1);
            });
            break;
        case EventTypeEnum.Evolution:
            // 進化イベントへ
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToDailyQuest(2);
            });
            break;
        case EventTypeEnum.Main:
            // メインイベントへ
            // 進化イベントへ
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToEventQuest(eventInfo.event_arg.Value);
            });
            break;
        case EventTypeEnum.PvP:
            // PvPへ
            if(AwsModule.PartyData.PvPTeam.IsEmpty) {
                // PVPチーム編成に飛ばす
                LockInputManager.SharedInstance.IsLock = false;
                PopupManager.OpenPopupOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                    LockInputManager.SharedInstance.IsLock = true;
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                        LockInputManager.SharedInstance.IsLock = false;
                        ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                    });
                });
            } else {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    LockInputManager.SharedInstance.IsLock = false;
                    ScreenChanger.SharedInstance.GoToPVP();
                });
            }
            break;
        }
    }

}
