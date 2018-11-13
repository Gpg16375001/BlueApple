using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public static class ServerResultCodeProcEnumExtension {
    public static void Execute(this ServerResultCodeProcEnum proc, ServerResultCodeEnum resultCode, string errorMessage = null)
    {
        if (proc == ServerResultCodeProcEnum.None) {
            return;
        }

        LockInputManager.SharedInstance.IsLock = false;
        switch (proc) {
        case ServerResultCodeProcEnum.Reboot:
            // PopupOKでOK押したらreboot
            PopupManager.OpenPopupSystemOK (TextData.GetText (resultCode.ToString ()),
                () => {
                    ScreenChanger.SharedInstance.Reboot();
                }
            );
            break;

        case ServerResultCodeProcEnum.Maintenance:
			// メンテナンス画面を開く
			View_Popup_Maintenance.CreateMaintenance(errorMessage);
            break;

        case ServerResultCodeProcEnum.MoveStore:
            // PopupOKでOK押したらStoreに移動
			View_Popup_Maintenance.CreateGoToStore();
            break;

        case ServerResultCodeProcEnum.HealAP:
            // AP回復を開く?
            PopupManager.OpenPopupSystemYN (TextData.GetText (resultCode.ToString ()),
                () => {
                    // AP回復を開く
                    View_APRecovery.Create();
                },
                () => {
                }
            );
            break;

        case ServerResultCodeProcEnum.HealBP:
            // BP回復を開く?
            PopupManager.OpenPopupSystemYN (TextData.GetText (resultCode.ToString ()),
                () => {
                    // BP回復を開く
                    //View_BPRecovery.Create();
                },
                () => {
                }
            );
            break;

        case ServerResultCodeProcEnum.GemShop:
            // ジェムショップを開く?
            PopupManager.OpenPopupSystemYN (TextData.GetText (resultCode.ToString ()),
                () => {
                    // ジェムショップを開く
                    View_GemShop.Create();
                },
                () => {
                }
            );
            break;
        }
    }
}
