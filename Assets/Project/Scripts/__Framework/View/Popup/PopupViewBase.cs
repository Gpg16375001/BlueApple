using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using SmileLab;
public class PopupViewBase : ViewBase {
    protected string PopOpenAnimeName = "CommonPopOpen";
    protected string PopCloseAnimeName = "CommonPopClose";
    protected bool IsClosed = false;

    protected virtual void Awake()
    {
        gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
    }

    protected virtual void SetBackButton()
    {
        if (string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButtonInfoId = AndroidBackButton.SetEventInThisScene (DidBackButton);
        }
    }

    protected virtual void DidBackButton()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    public override void Dispose ()
    {
        if (!string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButton.DeleteEvent (AndroidBackButtonInfoId);
        }
        base.Dispose ();
    }

    // 開閉アニメーション処理.
    protected void PlayOpenCloseAnimation(bool bOpen, System.Action didEnd = null)
    {
        IsClosed = !bOpen;
        this.StartCoroutine(CoPlayOpenClose(bOpen, didEnd));
    }
    IEnumerator CoPlayOpenClose(bool bOpen, System.Action didEnd)
    {
        var obj = GetScript<RectTransform> ("AnimParts");
        obj.gameObject.SetActive (true);
        var ianim = obj.GetComponent<IgnoreTimeScaleAnimation> ();
        if (ianim != null) {
            ianim.Play (bOpen ? PopOpenAnimeName : PopCloseAnimeName);

            yield return new WaitUntil (() => !ianim.isPlaying);
        } else {
            var anim = obj.GetComponent<Animation> ();
            if (anim != null) {
                anim.Play (bOpen ? PopOpenAnimeName : PopCloseAnimeName);

                yield return new WaitUntil (() => !anim.isPlaying);
            }
        }
        if (didEnd != null) {
            didEnd ();
        }
    }

    private string AndroidBackButtonInfoId;
}
