using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using SmileLab;

public class Screen_PurchaseTest : ViewBase
{

    public void Init ()
    {
#if UNITY_ANDROID
        PurchaseManager.SharedInstance.Initialize<Prime31GoogleIABControll> (new string[1] { "android.test.purchased" });
#elif UNITY_IOS || UNITY_TVOS
        PurchaseManager.SharedInstance.Initialize<Prime31StoreKitControll> (new string[1] {"android.test.purchased"});
#else
#endif
        PurchaseManager.SharedInstance.BeforeEvent += PurchaseTestEventListener.Before1;
        PurchaseManager.SharedInstance.BeforeEvent += PurchaseTestEventListener.Before2;
        PurchaseManager.SharedInstance.SucceedEvent += PurchaseTestEventListener.Succeed;
        PurchaseManager.SharedInstance.ValidateEvent += PurchaseTestEventListener.Validate;
        PurchaseManager.SharedInstance.ErrorEvent += PurchaseTestEventListener.Error;

        PurchaseManager.SharedInstance.BeforeEvent += this.Before;
        PurchaseManager.SharedInstance.ValidateEvent += this.Validate;
        PurchaseManager.SharedInstance.SucceedEvent += this.Succeed;
        PurchaseManager.SharedInstance.ErrorEvent += this.Error;

        this.SetCanvasButtonMsg ("Btn_Buy", DidTapBuy);
        this.SetCanvasButtonMsg ("Btn_Check", DidTapExistNonValidateTransaction);


        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
    }

    // ボタン：ストーリー.
    void DidTapBuy ()
    {
#if UNITY_ANDROID
        var testSku = PurchaseManager.SharedInstance.GetSkuItem ("android.test.purchased");
        if (testSku.HasValue) {
            this.IsEnableButton = false;
            PurchaseManager.SharedInstance.BuyItem (testSku.Value);
        }
#elif UNITY_IOS || UNITY_TVOS
#else
#endif
    }

    void DidTapExistNonValidateTransaction ()
    {
        if (PurchaseManager.SharedInstance.ExistNonValidateTransaction ()) {
            Debug.Log ("ExistNonValidateTransaction");
        } else {
            Debug.Log ("NotExistNonValidateTransaction");
        }
    }
       

    IEnumerator Before (SkuItem item, Action<string> error)
    {
        Debug.Log ("Screen_PurchaseTest.Before");
        yield break;
    }

    IEnumerator Validate(string transactionID, string receipt, string signature, SkuItem item, 
        Action<PurchaseManager.ValidateError> validateError)
    {
        Debug.Log ("Screen_PurchaseTest.Validate");
        yield return null;
    }

    void Succeed (SkuItem item)
    {
        Debug.Log ("Screen_PurchaseTest.Succeed");
        this.IsEnableButton = true;
    }

    void Error (PurchaseManager.PurchaseError errorCode, string error, SkuItem? item)
    {
        Debug.Log ("Screen_PurchaseTest.Error");
        this.IsEnableButton = true;
    }
}
