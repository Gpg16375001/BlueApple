using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using TMPro;
using SmileLab.UI;
using SmileLab.Net.API;


/// <summary>
/// View : アイテム購入確認ポップ.
/// </summary>
public class View_ShopItemPurchasePop : PopupViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_ShopItemPurchasePop Create(ShopList shopItem, ShopProductData serverData, Action<ShopProductData> didBuy)
	{
		var go = GameObjectEx.LoadAndCreateObject("Shop/View_ShopItemPurchasePop");
		var c = go.GetOrAddComponent<View_ShopItemPurchasePop>();
		c.InitInternal(shopItem, serverData, didBuy);
		return c;
	}
	private void InitInternal(ShopList shopItem, ShopProductData serverData, Action<ShopProductData> didBuy)
	{
		m_shopItem = shopItem;
		m_serverData = serverData;
		m_didBuy = didBuy;

		// ラベル
		this.GetScript<TextMeshProUGUI>("txtp_ShopItemName").text = shopItem.product_name;
		this.GetScript<TextMeshProUGUI>("txtp_SelectShopItemText").text = shopItem.explanatory_text;
		this.GetScript<TextMeshProUGUI>("txtp_Price").text = shopItem.use_item_count.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_TotalPrice").text = (m_currentSelectNum * m_shopItem.use_item_count).ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_TotalNumLimit").text = NumCanBuyMax.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_SelectTotalNum").text = m_currentSelectNum.ToString();
		this.GetScript<RectTransform>("StockNum").gameObject.SetActive(serverData.StockItemDataList.Length == 1);
		if(serverData.StockItemDataList.Length == 1){
			this.GetScript<TextMeshProUGUI>("txtp_StockNum").text = serverData.StockItemDataList[0].Quantity.ToString();
		}

		var purchaseLimitation = (PurchaseLimitationEnum)m_serverData.PurchaseLimitation;
		var bNumLimitaion = purchaseLimitation != PurchaseLimitationEnum.unlimited && purchaseLimitation != PurchaseLimitationEnum.timelimit;
		this.GetScript<RectTransform>("NumberLimit").gameObject.SetActive(bNumLimitaion);
		if (bNumLimitaion) {
			var limitaion = new PurchaseLimitation() { index=m_shopItem.limitaion.index, Enum=(PurchaseLimitationEnum)m_serverData.PurchaseLimitation };
			this.GetScript<TextMeshProUGUI>("txtp_NumberLimit").text = limitaion.Denominator().ToString();
			this.GetScript<TextMeshProUGUI>("txtp_Number").text = Mathf.Clamp( limitaion.Denominator() - m_serverData.MaxPurchaseQuantity, 0, m_serverData.MaxPurchaseQuantity ).ToString();
		}
		var bTimeLimitaion = purchaseLimitation == PurchaseLimitationEnum.timelimit;
		this.GetScript<RectTransform>("TimeLimit").gameObject.SetActive(bTimeLimitaion);
		if (bTimeLimitaion) {
			this.GetScript<TextMeshProUGUI>("txtp_TimeLimitDate").text = m_shopItem.end_date.ToString( "MM/dd" );
			this.GetScript<TextMeshProUGUI>("txtp_TimeLimitTime").text = m_shopItem.end_date.ToString( "HH:mm" );
		}

	    m_shopItem.product.LoadProductIcon(spt => this.GetScript<Image>("ItemIcon").sprite = spt);

		var useItemType = m_shopItem.shop_category.use_item_type;
		var iconName = useItemType.Enum.GetCurrencyIconName(useItemType.index);
		this.GetScript<uGUISprite>("txtp_TotalPrice/CurrencyIcon").ChangeSprite(iconName);
		this.GetScript<uGUISprite>("Price/IconGem").ChangeSprite(iconName);

		// ボタン
		this.SetCanvasCustomButtonMsg("Buy/bt_Common", DidTapBuy);
		this.SetCanvasCustomButtonMsg("Cancel/bt_Common", DidTapCancel);
		this.SetCanvasCustomButtonMsg("bt_Plus", DidTapPlus, null, null, DidRepeatPlus);
		this.SetCanvasCustomButtonMsg("bt_Minus", DidTapSub, null, null, DidRepeatSub);
		this.GetScript<CustomButton>("bt_Minus").interactable = false;
		this.GetScript<CustomButton>("bt_Plus").interactable = m_currentSelectNum <= NumCanBuyMax;
		this.GetScript<CustomButton>("Buy/bt_Common").interactable = m_currentSelectNum <= NumCanBuyMax;
        SetBackButton ();
	}

	void UpdateInfo()
	{
		this.GetScript<TextMeshProUGUI>("txtp_SelectTotalNum").text = m_currentSelectNum.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_TotalPrice").text = (m_currentSelectNum * m_shopItem.use_item_count).ToString("#,0");
		this.GetScript<CustomButton>("bt_Minus").interactable = m_currentSelectNum > 1;      
		if(!this.GetScript<CustomButton>("bt_Plus").interactable){
			this.GetScript<CustomButton>("bt_Plus").interactable = m_currentSelectNum <= NumCanBuyMax;
		}
		this.GetScript<CustomButton>("Buy/bt_Common").interactable = m_currentSelectNum <= NumCanBuyMax;
	}

    protected override void DidBackButton ()
    {
        DidTapCancel();
    }
	#region ButtonDelegate.

	// ボタン: 購入確定.
	void DidTapBuy()
	{
        if (IsClosed) {
            return;
        }

		if(m_currentSelectNum <= 0){
			PopupManager.OpenPopupOK("購入数を選択してください。");
			return;
		}
		if(m_currentSelectNum > NumCanBuyMax){
			return;
		}
		View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;
		SendAPI.ShopPurchaseProduct(m_shopItem.id, m_currentSelectNum, (bSuccess, res) => {
			if (!bSuccess || res == null){
				//Debug.LogError("View_ShopItemPurchasePop DidTapBuy Error!! : request error.");
				PlayOpenCloseAnimation (false, () => {
					Dispose();
					PopupManager.OpenPopupOK( "購入できませんでした。" );
				});
				View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
				return;
			}
			this.UpdateCache(res.ShopProductData.StockItemDataList);
			AwsModule.UserData.UserData = res.UserData;
			View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
			if (res.ShopProductData.StockItemDataList.Length == 1) {
				this.GetScript<TextMeshProUGUI>("txtp_StockNum").text = res.ShopProductData.StockItemDataList[0].Quantity.ToString();
            }
            PlayOpenCloseAnimation (false, () => {
                Dispose();
			    View_ShopItemOKPop.Create(m_shopItem, m_currentSelectNum, () => {
    				if (m_didBuy != null) {
    					m_didBuy(res.ShopProductData);
                    }
                });
            });
			View_FadePanel.SharedInstance.IsLightLoading = false;
            LockInputManager.SharedInstance.IsLock = false;
		});
	}

	// ボタン: キャンセル.
	void DidTapCancel()
	{
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
	}

	void DidRepeatSub(int repeatCnt)
	{
		if (IsClosed) {
			return;
		}

		if(m_currentSelectNum <= 1){
			return;
		}

		if (repeatCnt < 20) {
			m_currentSelectNum--;
		} else if (repeatCnt < 40) {
			m_currentSelectNum -= 10;
		} else {
			m_currentSelectNum -= 100;
		}

		m_currentSelectNum = Mathf.Max (m_currentSelectNum, 1);
		UpdateInfo ();
	}
	// ボタン: 購入数の増減.
	void DidTapSub()
	{
        if (IsClosed) {
            return;
        }

		if(m_currentSelectNum <= 1){
			return;
		}
		--m_currentSelectNum;
		UpdateInfo ();
	}

	void DidRepeatPlus(int repeatCnt)
	{
		if (IsClosed) {
			return;
		}

		if (m_currentSelectNum >= NumCanBuyMax) {
			return;
		}

		if (repeatCnt < 20) {
			m_currentSelectNum++;
		} else if (repeatCnt < 40) {
			m_currentSelectNum += 10;
		} else {
			m_currentSelectNum += 100;
		}

		m_currentSelectNum = Mathf.Min (m_currentSelectNum, NumCanBuyMax);
		UpdateInfo ();
	}
	void DidTapPlus()
	{
        if (IsClosed) {
            return;
        }

		if (m_currentSelectNum >= NumCanBuyMax) {
            return;
        }
        ++m_currentSelectNum;
		UpdateInfo ();
	}

	#endregion
 
	private void UpdateCache(StockItemData[] itemList)
    {
		if(itemList == null  || itemList.Length <= 0){
			return;
		}
		foreach(var item in itemList){
			if(item.MagikiteData != null){
				item.MagikiteData.CacheSet();
			}
			if (item.CardData != null) {
				item.CardData.CacheSet();
            }
			if (item.WeaponData != null) {
				item.WeaponData.CacheSet();
            }
			switch ((ItemTypeEnum)item.ItemType) {
				case ItemTypeEnum.material:
					(new MaterialData(item.ItemId, item.Quantity)).CacheSet();
					break;
				case ItemTypeEnum.consumer:
					(new ConsumerData(item.ItemId, item.Quantity)).CacheSet();
                    break;
			}
		}
    }

	private int HeveItemCount(ItemTypeEnum itemType)
	{
		switch(itemType){
			case ItemTypeEnum.card:
				return CardData.CacheGet(m_shopItem.use_item_id) != null ? 1 : 0;
			case ItemTypeEnum.free_gem:
                return AwsModule.UserData.UserData.GemCount;
            case ItemTypeEnum.paid_gem:
                return AwsModule.UserData.UserData.PaidGemCount;
            case ItemTypeEnum.money:
                return AwsModule.UserData.UserData.GoldCount;
            case ItemTypeEnum.gacha_coin:
                return 0;
            case ItemTypeEnum.pvp_medal:
                return AwsModule.UserData.UserData.PvpMedalCount;
        }
        return 0;
	}

	private Action<ShopProductData> m_didBuy; 
	private ShopList m_shopItem;
	private ShopProductData m_serverData;
	private int m_currentSelectNum = 1;


    // 購入可能数.
    private int NumCanBuyMax
	{
		get {
			var limitaionList = new List<int>();
			limitaionList.Add(NumCanBuyHaveLimit);
			limitaionList.Add(NumCanBuyPrice);
			if(m_serverData != null){
				limitaionList.Add(m_serverData.MaxPurchaseQuantity);
			}
			return limitaionList.Min();
		}
	}
	// 所持数的な購入上限数.
    private int NumCanBuyHaveLimit
	{
		get {
			var capaList = new List<int>();
			foreach(var c in m_shopItem.product.category_list){
				switch(c.Enum){
					case ItemTypeEnum.weapon:
						capaList.Add(AwsModule.UserData.UserData.WeaponBagCapacity - WeaponData.CacheGetAll().Count);
						break;
					case ItemTypeEnum.magikite:
						capaList.Add(AwsModule.UserData.UserData.MagikiteBagCapacity - MagikiteData.CacheGetAll().Count);
						break;
					case ItemTypeEnum.consumer:
						{
							var itemList = new ConsumerData[] { ConsumerData.CacheGet(m_shopItem.product.item_id_1),
                                                                ConsumerData.CacheGet(m_shopItem.product.item_id_2),
                                                                ConsumerData.CacheGet(m_shopItem.product.item_id_3) };
                            capaList.Add(MasterDataTable.CommonDefine["CONSUMER_CAPACITY"].define_value - itemList.Max(i => i != null ? i.Count: 0));                  
						}                  
						break;
					case ItemTypeEnum.material:
						{
							var itemList = new MaterialData[] { MaterialData.CacheGet(m_shopItem.product.item_id_1),
                                                                MaterialData.CacheGet(m_shopItem.product.item_id_2),
                                                                MaterialData.CacheGet(m_shopItem.product.item_id_3) };
							capaList.Add(MasterDataTable.CommonDefine["MATERIAL_CAPACITY"].define_value - itemList.Max(i => i != null ? i.Count: 0));                  
						}                  
						break;
					default:
						capaList.Add(MasterDataTable.CommonDefine["BUY_LIMIT"].define_value);
						break;
				}
			}
			return capaList.Min();
		}
	}
    // 金銭的な購入上限数.
    private int NumCanBuyPrice
	{
		get {
			return Mathf.FloorToInt((float)(HeveItemCount(m_shopItem.shop_category.use_item_type.Enum)) / (float)m_shopItem.use_item_count);
		}
	}
}
