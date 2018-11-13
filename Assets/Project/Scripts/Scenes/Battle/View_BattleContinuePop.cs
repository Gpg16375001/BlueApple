using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_BattleContinuePop : PopupViewBase {

    public static View_BattleContinuePop Create(Action didContinue, Action didCancel)
    {
        // 
        var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleContinuePop");
        var c = go.GetOrAddComponent<View_BattleContinuePop>();
        c.InitInternal(didContinue, didCancel);
        return c;
    }

    public override void Dispose ()
    {
        AwsModule.UserData.UpdateUserData -= UpdateUserData;
        base.Dispose ();
    }

    private void InitInternal(Action didContinue, Action didCancel)
    {
        AwsModule.UserData.UpdateUserData += UpdateUserData;
        m_DidContinue = didContinue;
        m_DidCancle = didCancel;

        // ここはBackButton効いていいのかな?
        SetBackButton();
        SetCanvasCustomButtonMsg ("Yes/bt_Common", DidTapContinue);
        SetCanvasCustomButtonMsg ("Shop/bt_Common", DidTapBuyGem);
        SetCanvasCustomButtonMsg ("No/bt_Common", DidTapCancel);
        SetCanvasCustomButtonMsg ("bt_Close", DidTapCancel);

        UpdateInfo ();
    }

    private void UpdateInfo()
    {
        GetScript<TextMeshProUGUI> ("txtp_TollGemNum").SetText (AwsModule.UserData.UserData.PaidGemCount);
        GetScript<TextMeshProUGUI> ("txtp_FreeGemNum").SetText (AwsModule.UserData.UserData.FreeGemCount);
        if (AwsModule.UserData.UserData.GemCount >= 10) {
            GetScript<RectTransform> ("Yes").gameObject.SetActive (true);
            GetScript<RectTransform> ("Shop").gameObject.SetActive (false);
            GetScript<RectTransform> ("Warning").gameObject.SetActive (false);
        } else {
            GetScript<RectTransform> ("Shop").gameObject.SetActive (true);
            GetScript<RectTransform> ("Yes").gameObject.SetActive (false);
            GetScript<RectTransform> ("Warning").gameObject.SetActive (true);
        }
    }

    private void UpdateUserData(UserData userData)
    {
        UpdateInfo ();
    }

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

    private void DidTapContinue()
    {
        if (IsClosed) {
            return;
        }

        if (m_DidContinue != null) {
            m_DidContinue ();
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    private void DidTapBuyGem()
    {
        View_GemShop.OpenGemShop ();
    }

    private void DidTapCancel()
    {
        if (IsClosed) {
            return;
        }

        if (m_DidCancle != null) {
            m_DidCancle ();
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    Action m_DidCancle;
    Action m_DidContinue;
}
