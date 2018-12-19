using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ListItem : ショップアイテム.
/// </summary>
public class ListItem_ShopItem : ViewBase
{
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(Action<ShopProductData> didBuy)
	{
		m_didBuy = didBuy;
		this.SetCanvasCustomButtonMsg("bt_ListItemBase", DidTapBuy);
	}

    /// <summary>
    /// 情報設定.
    /// </summary>
	public void SetInfo(ShopList shopItem, ShopProductData res)
	{
		m_data = shopItem;
		m_res = res;

		this.GetScript<TextMeshProUGUI>("txtp_ShopItemName").text = m_data.product_name;
		this.GetScript<TextMeshProUGUI>("txtp_ShopItemText").text = m_data.explanatory_text;
		this.GetScript<TextMeshProUGUI>("txtp_Price").text = m_data.use_item_count.ToString("#,0");

		var iconName = m_data.shop_category.use_item_type.Enum.GetCurrencyIconName();
		if(!string.IsNullOrEmpty(iconName)){
			this.GetScript<uGUISprite>("CurrencyIcon").ChangeSprite(iconName);
		}
		shopItem.product.LoadProductIcon(spt => this.GetScript<Image>("Icon").sprite = spt);

#if false //1.0.4 初期化通らない場合がある
		var purchaseLimitation = (PurchaseLimitationEnum)m_res.PurchaseLimitation;
		var bNumLimitaion = purchaseLimitation != PurchaseLimitationEnum.unlimited && purchaseLimitation != PurchaseLimitationEnum.timelimit;
		this.GetScript<RectTransform>("NumberLimit").gameObject.SetActive(bNumLimitaion);
		if (bNumLimitaion) {
			var limitaion = new PurchaseLimitation() { index=m_data.limitaion.index, Enum=(PurchaseLimitationEnum)m_res.PurchaseLimitation };
			this.GetScript<TextMeshProUGUI>("txtp_NumberLimit").text = limitaion.Denominator().ToString();
			this.GetScript<SmileLab.UI.CustomButton>("bt_ListItemBase").interactable = m_res.IsPurchasable;	//タップ禁止
			if( m_res.IsPurchasable ) {
				this.GetScript<TextMeshProUGUI>("txtp_Number").text = Mathf.Clamp( limitaion.Denominator() - m_res.MaxPurchaseQuantity, 0, Int32.MaxValue ).ToString();
			}else{
				this.GetScript<TextMeshProUGUI>("txtp_Number").text = limitaion.Denominator().ToString();
			}
			this.GetScript<Transform>("Disable").gameObject.SetActive( !m_res.IsPurchasable );	//購入できません表示
        }
#else //1.0.5以降
		this.GetScript<SmileLab.UI.CustomButton>("bt_ListItemBase").interactable = m_res.IsPurchasable;	//タップ禁止
		this.GetScript<Transform>("Disable").gameObject.SetActive( !m_res.IsPurchasable );	//購入できません表示

		var purchaseLimitation = (PurchaseLimitationEnum)m_res.PurchaseLimitation;
		var bNumLimitaion = purchaseLimitation != PurchaseLimitationEnum.unlimited && purchaseLimitation != PurchaseLimitationEnum.timelimit;
		this.GetScript<RectTransform>("NumberLimit").gameObject.SetActive(bNumLimitaion);
		if (bNumLimitaion) {	//個数限定
			var limitaion = new PurchaseLimitation() { index=m_data.limitaion.index, Enum=(PurchaseLimitationEnum)m_res.PurchaseLimitation };
			this.GetScript<TextMeshProUGUI>("txtp_NumberLimit").text = limitaion.Denominator().ToString();
			if( m_res.IsPurchasable ) {
				this.GetScript<TextMeshProUGUI>("txtp_Number").text = Mathf.Clamp( limitaion.Denominator() - m_res.MaxPurchaseQuantity, 0, Int32.MaxValue ).ToString();
			}else{
				this.GetScript<TextMeshProUGUI>("txtp_Number").text = limitaion.Denominator().ToString();
			}
        }
#endif
		var bTimeLimitaion = purchaseLimitation == PurchaseLimitationEnum.timelimit;
		this.GetScript<RectTransform>("TimeLimit").gameObject.SetActive(bTimeLimitaion);
		if (bTimeLimitaion) {	//期限限定
			this.GetScript<TextMeshProUGUI>("txtp_TimeLimitDate").text = m_data.end_date.ToString( "MM/dd" );
			this.GetScript<TextMeshProUGUI>("txtp_TimeLimitTime").text = m_data.end_date.ToString( "HH:mm" );
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate( (RectTransform)this.GetScript<Transform>("SaleInfoGrid") );	//レイアウトアップデート
	}

	// ボタン: 購入.
    void DidTapBuy()
	{
		View_ShopItemPurchasePop.Create(m_data, m_res, m_didBuy);
	}

	private Action<ShopProductData> m_didBuy;
	private ShopList m_data;
	public ShopList Data { get { return m_data; } }
	private ShopProductData m_res;
}
