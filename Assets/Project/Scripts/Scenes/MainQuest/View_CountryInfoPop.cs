using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// View : 国詳細ビュー.
/// </summary>
public class View_CountryInfoPop : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
    public static View_CountryInfoPop Create(Belonging belonging, Action didIn = null, Action didOut = null)
    {
        var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_CountryInfoPop");
        var c = go.GetOrAddComponent<View_CountryInfoPop>();
        c.InitInternal(belonging, didIn, didOut);
        return c;
    }
    private void InitInternal(Belonging belonging, Action didIn, Action didOut)
    {
        m_didOut = didOut;

        this.GetScript<TextMeshProUGUI>("txtp_PickupCountryName").text = belonging.name;
        // ボタン.
        this.SetCanvasButtonMsg("OK/bt_Common", DidTapClose);
        SetBackButton ();

        // 開くのを待つ.
        PlayOpenCloseAnimation(true, didIn);
    }

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

    // ボタン : 閉じる.
    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation(false, () => {
            if(m_didOut != null){
                m_didOut();
            }
            this.Dispose();
        });
    }

    private Action m_didOut;
}
