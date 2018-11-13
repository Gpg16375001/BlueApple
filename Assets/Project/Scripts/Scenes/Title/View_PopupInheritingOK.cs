using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class View_PopupInheritingOK : PopupViewBase {

    private static View_PopupInheritingOK instance;
    public static View_PopupInheritingOK Create(string name, string level, string customerId, Action tapOk, Action tapCancle)
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Title/View_PopupInheritingOK");
        instance = go.GetOrAddComponent<View_PopupInheritingOK>();
        instance.InitInternal(name, level, customerId, tapOk, tapCancle);
        return instance;
    }


    private void InitInternal(string name, string level, string customerId, Action tapOk, Action tapCancle)
    {
        m_TapOk = tapOk;
        m_TapCancel = tapCancle;

        GetScript<TextMeshProUGUI> ("txtp_PlayerName").SetText (name);
        GetScript<TextMeshProUGUI> ("txtp_PlayerID").SetText (customerId);
        GetScript<TextMeshProUGUI> ("txtp_Rank").SetText (level);
        SetCanvasCustomButtonMsg ("Ok/bt_Common", DidTapOk);
        SetCanvasCustomButtonMsg ("Cancel/bt_Common", DidTapCancel);

        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

    void DidTapOk()
    {
        if (IsClosed) {
            return;
        }
        if (m_TapOk != null) {
            m_TapOk ();
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapCancel()
    {
        if (IsClosed) {
            return;
        }
        if (m_TapCancel != null) {
            m_TapCancel ();
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    Action m_TapOk;
    Action m_TapCancel;
}
