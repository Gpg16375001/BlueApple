using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using TMPro;


/// <summary>
/// View : ショップアイテム購入後ポップ.
/// </summary>
public class View_ShopItemOKPop : PopupViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_ShopItemOKPop Create(ShopList shopItem, int num, Action didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("Shop/View_ShopItemOKPop");
		var c = go.GetOrAddComponent<View_ShopItemOKPop>();
		c.InitInternal(shopItem, num, didClose);
		return c;
	}
	private void InitInternal(ShopList shopItem, int num, Action didClose)
	{
		m_didClose = didClose;
		
		shopItem.product.LoadProductIcon(spt => this.GetScript<Image>("ItemIcon").sprite = spt);
		this.GetScript<TextMeshProUGUI>("txtp_ItemIconNum").text = num.ToString();

        // ボタン.
        this.SetCanvasCustomButtonMsg("OK/bt_Common", DidTapOK);
        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapOK ();
    }
        
    // OKボタン
    void DidTapOK()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            if (m_didClose != null) {
                m_didClose();
            }
            this.Dispose();
        });
    }

	private Action m_didClose;
}
