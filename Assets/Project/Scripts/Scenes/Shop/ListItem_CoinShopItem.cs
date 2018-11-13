using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// ListItem : コインショップ.
/// </summary>
public class ListItem_CoinShopItem : ViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(ShopList shopList, ShopProductData serverData, Action didBuy)
	{
		m_shopItem = shopList;
		m_data = serverData;
		m_didBuy = didBuy;

		this.GetScript<TextMeshProUGUI>("txtp_Price").text = m_shopItem.use_item_count.ToString("#,0");
        this.GetScript<TextMeshProUGUI>("txtp_Coin").text = m_shopItem.product.count_1.ToString("#,0");    // クレドは単体販売想定
		shopList.product.LoadProductIcon(spt => this.GetScript<Image>("WhitePanel").sprite = spt);

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_Common02", DidTap);
	}

	// ボタン: 購入.
    void DidTap()
	{
		var limit = MasterDataTable.CommonDefine["CURRENCY_CAPACITY"].define_value;
		var totalValue = AwsModule.UserData.UserData.GoldCount + m_shopItem.product.count_1;      
		if(totalValue > limit){
			PopupManager.OpenPopupOK(string.Format("{0}が所持上限を超過します。\n購入できません。", m_shopItem.product.category_name1));
			return;
		}
		View_CoinShopOk.Create(m_shopItem, m_didBuy);
	}

	private ShopList m_shopItem;
	private ShopProductData m_data;
	private Action m_didBuy;
}
