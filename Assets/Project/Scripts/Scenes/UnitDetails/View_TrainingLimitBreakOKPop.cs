using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_TrainingLimitBreakOKPop : PopupViewBase {
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_TrainingLimitBreakOKPop Create(System.Action tapOK)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_TrainingLimitBreakOKPop");
        var c = go.GetOrAddComponent<View_TrainingLimitBreakOKPop>();
        c.InitInternal(tapOK);
        return c;
    }

    private void InitInternal(System.Action tapOK)
    {
        m_TapOK = tapOK;
        SetCanvasCustomButtonMsg ("OK/bt_CommonS02", DidTapOK);
        SetCanvasCustomButtonMsg ("Cancel/bt_CommonS01", DidTapCancel);
        SetCanvasCustomButtonMsg ("bt_Close", DidTapCancel);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

    void DidTapOK()
    {
        if (IsClosed) {
            return;
        }
        if(m_TapOK != null) {
            m_TapOK();
        }
        PlayOpenCloseAnimation (false, () => {
            Dispose ();
        });
    }

    void DidTapCancel()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => Dispose ());
    }

    System.Action m_TapOK;
}
