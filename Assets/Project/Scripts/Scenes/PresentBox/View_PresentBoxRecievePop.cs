using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_PresentBoxRecievePop : PopupViewBase
{
    public static View_PresentBoxRecievePop Create(PresentData[] recieveData, Action didClose)
    {
        var go = GameObjectEx.LoadAndCreateObject("PresentBox/View_PresentBoxRecievePop");
        var c = go.GetOrAddComponent<View_PresentBoxRecievePop>();
        c.InitInternal(recieveData, didClose);
        return c;
    }

    private void InitInternal(PresentData[] recieveData, Action didClose)
    {
        m_DidClose = didClose;

        System.Text.StringBuilder builder = new System.Text.StringBuilder ();
        foreach (var present in recieveData) {
            ItemTypeEnum type = (ItemTypeEnum)present.ItemType;
            builder.Append (type.GetNameAndQuantity(present.ItemId, present.Quantity));
            builder.AppendLine ();
        }
        var txtpItemList = GetScript<TextMeshProUGUI> ("txtp_ItemList");
        txtpItemList.autoSizeTextContainer = true;
        txtpItemList.SetText(builder.ToString());


		SetCanvasCustomButtonMsg("OK/bt_Common", DidTapClose);
		SetCanvasCustomButtonMsg("bt_Close", DidTapClose);

        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    // ボタン : 閉じる.
    void DidTapClose()
    {
        PlayOpenCloseAnimation (false, () => {
            this.Dispose ();
            if (m_DidClose != null) {
                m_DidClose ();
            }
        });
    }

    private Action m_DidClose;
}
