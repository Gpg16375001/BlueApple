using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net;

public static class PurchaseEventListener {

    public static IEnumerator Validate(string transactionID, string receipt, string signature, SkuItem item, 
        Action<PurchaseManager.ValidateError> validateError)
    {
        //Debug.Log (string.Format("OnValidate1-1 item:{0} transactionID:{1} signature:{2} receipt:{3}", item.title, transactionID, signature, receipt));

        bool isApiWait = true;
        SendAPI.PaymentsSubmitReceipt (receipt, signature, (result, response) => {
            isApiWait = false;
            if(!result) {
                validateError(PurchaseManager.ValidateError.DUPLICATION_ERROR);
                return;
            }
            if(response.ResultCode == 0) {
                // ResultCodeが0の時は課金終了とする。
                AwsModule.UserData.UserData = response.UserData;
            } else {
                validateError(PurchaseManager.ValidateError.UNKNOWN_ERROR);
                return;
            }
        });

        yield return new WaitUntil (() => !isApiWait);

        //Debug.Log ("OnValidate1-2");
    }

    public static void OnSucceed(SkuItem item)
    {
        LockInputManager.SharedInstance.IsLock = false;
        PopupManager.OpenPopupSystemOK (string.Format("{0}の購入処理が完了しました。", item.title));
    }

    public static void OnError(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item)
    {
        // キャンセルの時は何もしない。
        if (errorCode == PurchaseManager.PurchaseError.CANCELED_ERROR) {
            return;
        }

        LockInputManager.SharedInstance.IsLock = false;
        Debug.LogError(string.Format("code:{0} error:{1}", errorCode, error));
        if (string.IsNullOrEmpty (error)) {
            PopupManager.OpenPopupSystemOK ("不明なエラーが発生しました。");
        } else {
            if (!item.HasValue) {
                PopupManager.OpenPopupSystemOK (string.Format ("課金システムにてエラーが発生しました。\n{0}", error));
            } else {
                PopupManager.OpenPopupSystemOK (string.Format ("購入処理中にエラーが発生しました。\n{0}", error));
            }
        }
    }
}
