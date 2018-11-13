using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_GemLimitBreakOKPop : PopupViewBase {
    public static View_GemLimitBreakOKPop Create(CardData card, MaterialGrowthBoardItemCombination itemCombination, System.Action tapOK)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_GemLimitBreakOKPop");
        var c = go.GetOrAddComponent<View_GemLimitBreakOKPop>();
        c.InitInternal(card, itemCombination, tapOK);
        return c;
    }

    private void InitInternal(CardData card, MaterialGrowthBoardItemCombination itemCombination, System.Action tapOK)
    {
        m_TapOK = tapOK;

        SetCanvasCustomButtonMsg ("OK/bt_CommonS02", DidTapOK);
        SetCanvasCustomButtonMsg ("Cancel/bt_CommonS01", DidTapCancel);
        SetCanvasCustomButtonMsg ("bt_Close", DidTapCancel);

        var gem = itemCombination.gem_quantity;
        GetScript<TextMeshProUGUI> ("txtp_UseGem").SetText (gem);
        GetScript<TextMeshProUGUI> ("txtp_TotalGem").SetText (AwsModule.UserData.UserData.GemCount - gem);
        GetScript<CustomButton> ("OK/bt_CommonS02").interactable = AwsModule.UserData.UserData.GemCount >= gem;

        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapCancel();
    }

    void DidTapOK()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            if(m_TapOK != null) {
                m_TapOK();
            }
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
