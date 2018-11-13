using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class View_DLSelectPop : ViewBase {
    private static View_DLSelectPop instance;
    public static View_DLSelectPop Create(Action<bool> didDLStart)
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Download/View_DLSelectPop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        instance = go.GetOrAddComponent<View_DLSelectPop>();
        instance.InitInternal(didDLStart);
        return instance;
    }

    private Action<bool> m_dlStartAction;
    private void InitInternal(Action<bool> didDLStart)
    {
        m_dlStartAction = didDLStart;
        // 年齢確認前の情報表示
        SetCanvasCustomButtonMsg ("LargeDL/bt_Common02", DidTapAll);
        SetCanvasCustomButtonMsg ("SmallDL/bt_Common02", DidTapMiniman);

        var smallDlSize = DLCManager.DownloadMinimumContentsFilesSize();
        double smallDlSizeMB = (double)smallDlSize / 1024D / 1024D;

        var allDlSize = DLCManager.DownloadAllFilesSize();
        double allDlSizeMB = (double)allDlSize / 1024D / 1024D;

        GetScript<TextMeshProUGUI> ("txtp_SmallDLSize").SetTextFormat ("{0:F2}", smallDlSizeMB);
        GetScript<TextMeshProUGUI> ("txtp_LargeDLSize").SetTextFormat ("{0:F2}", allDlSizeMB);

        PlayOpenCloseAnimation (true);
    }

    void DidTapMiniman()
    {
        if (m_Closed) {
            return;
        }

        m_dlStartAction (false);

        // 閉じる
        PlayOpenCloseAnimation (false, () => {
            Dispose();
        });
    }

    void DidTapAll()
    {
        if (m_Closed) {
            return;
        }

        m_dlStartAction (true);

        // 閉じる
        PlayOpenCloseAnimation (false, () => {
            Dispose();
        });
    }

    // 開閉アニメーション処理.
    private void PlayOpenCloseAnimation(bool bOpen, System.Action didEnd = null)
    {
        m_Closed = !bOpen;
        this.StartCoroutine(CoPlayOpenClose(bOpen, didEnd));
    }
    IEnumerator CoPlayOpenClose(bool bOpen, System.Action didEnd)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play(bOpen ? "CommonPopOpen" : "CommonPopClose");
        do{
            yield return null;
        }while(anim.isPlaying);
        if(didEnd != null){
            didEnd();
        }
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.FadeCamera;
        }
    }

    private bool m_Closed = false;
}

