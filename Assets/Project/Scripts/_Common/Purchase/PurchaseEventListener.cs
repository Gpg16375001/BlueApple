using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net;
using SmileLab.Net.API;

public static class PurchaseEventListener {

	private static void PaymentsSubmitReceipt(string Receipt, string Signature, Action<bool, ReceivePaymentsSubmitReceipt> didLoad)
	{
		SendPaymentsSubmitReceipt request = new SendPaymentsSubmitReceipt ();
		request.Receipt = Receipt;
		request.Signature = Signature;
		AwsModule.Request.Exec<ReceivePaymentsSubmitReceipt> (request, (response) => {
			AwsModule.Request.CheckResultCodeRetry<ReceivePaymentsSubmitReceipt>(response, didLoad, false, false,
                () => {
				    PaymentsSubmitReceipt(Receipt, Signature, didLoad);
                }, 
                () => {
                    // リタイヤ時はエラーだけ出して終わらせる。
                    didLoad(false, null);
                }
            );
		});
	}

    public static IEnumerator Validate(string transactionID, string receipt, string signature, SkuItem item, 
        Action<PurchaseManager.ValidateError> validateError)
    {
        //Debug.Log (string.Format("OnValidate1-1 item:{0} transactionID:{1} signature:{2} receipt:{3}", item.title, transactionID, signature, receipt));

        bool isApiWait = true;
        PaymentsSubmitReceipt (receipt, signature, (result, response) => {
            isApiWait = false;
            if(!result) {
                if(response == null) {
                    validateError(PurchaseManager.ValidateError.CONNECTION_ERROR);
                    return;
                }

                if(response.ResultCode == (int)ServerResultCodeEnum.INVALID_RECEIPT || 
                    response.ResultCode == (int)ServerResultCodeEnum.INVALID_RECEIPT_SIGNATURE)
                {
                    validateError(PurchaseManager.ValidateError.VERIFY_ERROR);
                } else {
                    validateError(PurchaseManager.ValidateError.UNKNOWN_ERROR);
                }
                return;
            }
            AwsModule.UserData.UserData = response.UserData;
        });

        yield return new WaitUntil (() => !isApiWait);
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
        View_FadePanel.SharedInstance.IsLightLoading = false;
        Debug.LogError(string.Format("code:{0} error:{1}", errorCode, error));
        if (errorCode == PurchaseManager.PurchaseError.CONNECTION_ERROR) {
            PopupManager.OpenPopupSystemOK ("通信エラーが発生しました。\n通信環境の良いところでアプリを再起動してください。");
        } else if (string.IsNullOrEmpty (error)) {
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
