using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class GrowthPushModule : ViewBase
{
    public string APPLICATION_ID;
    public string SDK_CREDENTIAL_ID;
    public string SENDER_ID;

    /// <summary>
    /// 共通インスタンス
    /// </summary>
    public static GrowthPushModule SharedInstance { get; private set; }
    public bool IsReady { get; private set; }

    void Awake()
    {
        IsReady = false;
        if(SharedInstance != null) {
            SharedInstance.Dispose();
        }
        SharedInstance = this;
    }

    public override void Dispose ()
    {
        IsReady = false;
        base.Dispose ();
    }

    public void Init()
    {
        // identifierが違う場合は初期化しない
        if (Application.identifier != "jp.fg.precatus") {
            IsReady = true;
            return;
        }
        GrowthPush.GetInstance().Initialize(APPLICATION_ID, SDK_CREDENTIAL_ID, Debug.isDebugBuild ? GrowthPush.Environment.Development : GrowthPush.Environment.Production);
        // Android のデバイストークン取得（必ず initialize 後に呼び出してください）
        GrowthPush.GetInstance ().RequestDeviceToken (SENDER_ID);

#if !UNITY_EDITOR && UNITY_IPHONE
        StartCoroutine (ReceiveToken());
#endif
        IsReady = true;
    }


#if !UNITY_EDITOR && UNITY_IPHONE
    IEnumerator ReceiveToken ()
    {
        yield return new WaitUntil (() => UnityEngine.iOS.NotificationServices.deviceToken != null);
        byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;
        GrowthPush.GetInstance ().SetDeviceToken(System.BitConverter.ToString(token).Replace("-", "").ToLower());
        IsReady = true;
    }
#endif
}
