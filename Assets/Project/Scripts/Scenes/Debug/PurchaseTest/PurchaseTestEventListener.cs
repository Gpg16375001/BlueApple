using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SmileLab;

public static class PurchaseTestEventListener {

    public static IEnumerator Before1 (SkuItem item, Action<string> error)
    {
        Debug.Log ("Before1-1 " + item.title);
        yield return null;

        Debug.Log ("Before1-2 " + item.title);
    }

    public static IEnumerator Before2 (SkuItem item, Action<string> error)
    {
        Debug.Log ("Before2-1 " + item.title);
        yield return null;

        Debug.Log ("Before2-2 " + item.title);
    }

    public static void Succeed(SkuItem item)
    {
        Debug.Log ("Succeed " + item.title);
    }

    public static IEnumerator Validate(string transactionID, string receipt, string signature, SkuItem item, 
        Action<PurchaseManager.ValidateError> validateError)
    {
        Debug.Log (string.Format("OnValidate1-1 item:{0} transactionID:{1} signature:{2} receipt:{3}", item.title, transactionID, signature, receipt));
        yield return null;

        Debug.Log ("OnValidate1-2");
    }

    public static void Error(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item)
    {
        Debug.LogError (errorCode.ToString () + " " + error);
    }
}
