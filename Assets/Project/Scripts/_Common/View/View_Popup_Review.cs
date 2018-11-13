using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class View_Popup_Review : PopupViewBase {

    public static View_Popup_Review Create(Action didClose)
    {
        LockInputManager.SharedInstance.IsLock = true;
        if(ReviewController.OpenReviewiOS ()) {
            if (didClose != null) {
                didClose ();
            }
            LockInputManager.SharedInstance.IsLock = false;
            return null;
        }
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_Popup_Review");
        var c = go.GetOrAddComponent<View_Popup_Review>();
        c.InitInternal(didClose);
        return c;
    }

    private void InitInternal(Action didClose)
    {
        SetCanvasCustomButtonMsg("Cancel/bt_Common", DidTapCancel);
        SetCanvasCustomButtonMsg("Review/bt_Common", DidTapReview);

        m_DidClose = didClose;
        SetBackButton ();
        PlayOpenCloseAnimation (true, () => {
            LockInputManager.SharedInstance.IsLock = false;
        });
    }

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

    void DidTapReview()
    {
        if (IsClosed) {
            return;
        }

        if (m_DidClose != null) {
            m_DidClose ();
        }
        string openId = "";
#if UNITY_IPHONE
        openId = "1410091440";
#elif UNITY_ANDROID
        openId = "jp.fg.precatus";
#endif
        ReviewController.OpenReview (openId);
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapCancel()
    {
        if (IsClosed) {
            return;
        }

        if (m_DidClose != null) {
            m_DidClose ();
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    Action m_DidClose;
}
