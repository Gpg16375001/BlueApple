using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

/// <summary>
/// お問い合わせView
/// </summary>
public class MiniView_OptionContact : ViewBase {

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {
        if (!isInit) {
            SetCanvasCustomButtonMsg ("Contact/bt_Common", DidTapContact);
            isInit = true;
        }
    }

    void DidTapContact()
    {
#if UNITY_IOS && !UNITY_EDITOR
        SafariView.Init (gameObject);
        SafariView.LaunchURL (ContactUs.CreateUrl());
#else
        Application.OpenURL(ContactUs.CreateUrl());
#endif
    }

    private bool isInit = false;
}
