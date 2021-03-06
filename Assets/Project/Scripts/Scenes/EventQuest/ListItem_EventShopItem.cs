﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
public class ListItem_EventShopItem : ViewBase {

    public void InitItem(Action<EventQuestExchangeSetting, EventShopProductData> didTap)
    {
        m_DidTap = didTap;
        SetCanvasCustomButtonMsg ("bt_ListItemBase", DidTap);
    }

    public void UpdateItem(EventQuestExchangeSetting setting, EventShopProductData productData, EventShopProductData releaseProductData)
    {
        productDataSetting = productData;
        exchangeSetting = setting;
        // アイコン設定
        SetIcon (setting);

        // 
		GetScript<Image>("IconEventPoint").overrideSprite = null; 
		IconLoader.LoadEventPoint (setting.event_quest_id, DidLoadEventPointIcon);
        GetScript<TextMeshProUGUI> ("txtp_ShopItemName").SetText (setting.item_type.GetNameAndQuantity(setting.item_id, setting.quantity));
        GetScript<TextMeshProUGUI> ("txtp_Price").SetText (setting.use_point);
        GetScript<TextMeshProUGUI> ("txtp_ShopItemText").SetText (setting.details);

        var saleInfoGrid = GetScript<HorizontalLayoutGroup> ("SaleInfoGrid");
        saleInfoGrid.gameObject.SetActive (false);
        // 購入可否
        bool isLimit = false;
        bool isNeedItemLimit = releaseProductData != null && releaseProductData.MaxPurchaseQuantity > 0;
        var NumberLimitGo = GetScript<RectTransform> ("NumberLimit").gameObject;
        if (productData != null && productData.UpperLimit > 0) {
            NumberLimitGo.SetActive (true);
            GetScript<TextMeshProUGUI> ("txtp_NumberLimit").SetText (productData.UpperLimit);
            GetScript<TextMeshProUGUI> ("txtp_Number").SetText (productData.UpperLimit - productData.MaxPurchaseQuantity);
            isLimit = productData.MaxPurchaseQuantity == 0;
        } else {
            NumberLimitGo.SetActive (false);
        }

		var eventQuest = MasterDataTable.event_quest [setting.event_quest_id];
		var TimerLimitGo = GetScript<RectTransform> ("TimeLimit").gameObject;
		if (productData != null && setting.end_date.HasValue && eventQuest.exchange_time_limit != setting.end_date.Value) {
			TimerLimitGo.SetActive (true);
			GetScript<TextMeshProUGUI> ("txtp_TimeLimitDate").SetTextFormat ("{0}/{1}", setting.end_date.Value.Month, setting.end_date.Value.Day);
			GetScript<TextMeshProUGUI> ("txtp_TimeLimitTime").SetTextFormat ("{0}:{1}", setting.end_date.Value.Hour, setting.end_date.Value.Minute);
		} else {
			TimerLimitGo.SetActive (false);
		}
        saleInfoGrid.gameObject.SetActive (true);

        var DisableSet = GetScript<RectTransform> ("DisableSet").gameObject;
		if (isLimit || isNeedItemLimit || !productData.IsPurchasable) {
            DisableSet.SetActive (true);
            if(isLimit) {
                GetScript<TextMeshProUGUI> ("txtp_DisableNotes").SetText ("売り切れ");
            } else if(isNeedItemLimit) {
                var releaseItem = MasterDataTable.event_quest_exchange_setting.DataList.Find(x => x.id == releaseProductData.ShopProductId);
                if (releaseItem != null) {
                    GetScript<TextMeshProUGUI> ("txtp_DisableNotes").SetTextFormat ("{0}の売り切れ後に購入可能になります", releaseItem.item_type.GetNameAndQuantity (releaseItem.item_id, releaseItem.quantity));
                } else {
                    GetScript<TextMeshProUGUI> ("txtp_DisableNotes").SetText("購入不可");
                }
			} else {
				GetScript<TextMeshProUGUI> ("txtp_DisableNotes").SetText("購入不可");
			}
        } else {
            DisableSet.SetActive (false);
        }
        GetScript<CustomButton>("bt_ListItemBase").interactable = !isLimit && !isNeedItemLimit;
    }

	void DidLoadEventPointIcon(IconLoadSetting data, Sprite icon)
	{
		if (data.type == ItemTypeEnum.event_point && data.id == exchangeSetting.event_quest_id) {
			GetScript<Image> ("IconEventPoint").overrideSprite = icon;
		}
	}
        
    void SetIcon(EventQuestExchangeSetting setting)
    {
        bool weaponOrUnit = setting.item_type == ItemTypeEnum.card || setting.item_type == ItemTypeEnum.weapon;
        var iconInfo = setting.item_type.GetIconInfo (setting.item_id, true);

        var otherIconRoot = GetScript<RectTransform> ("OtherIconRoot");
        var unitWeaponIconRoot = GetScript<RectTransform> ("UnitWeaponRoot");
        var ItemIcon = GetScript<uGUISprite> ("ItemIcon");

        otherIconRoot.gameObject.DestroyChildren ();
        unitWeaponIconRoot.gameObject.DestroyChildren ();

        if (iconInfo.IsEnableSprite) {
            // Spriteの画像のみ
            ItemIcon.gameObject.SetActive (true);
            otherIconRoot.gameObject.SetActive (false);
            unitWeaponIconRoot.gameObject.SetActive (false);

            ItemIcon.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (weaponOrUnit) {
            // ユニットや武器のアイコン
            ItemIcon.gameObject.SetActive (false);
            otherIconRoot.gameObject.SetActive (false);
            unitWeaponIconRoot.gameObject.SetActive (true);

            iconInfo.IconObject.transform.SetParent (unitWeaponIconRoot);
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        } else {
            // それ以外のアイコン
            ItemIcon.gameObject.SetActive (false);
            otherIconRoot.gameObject.SetActive (true);
            unitWeaponIconRoot.gameObject.SetActive (false);

            iconInfo.IconObject.transform.SetParent (otherIconRoot);
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        }
    }

    void DidTap()
    {
        if (m_DidTap != null) {
            m_DidTap (exchangeSetting, productDataSetting);
        }
    }


    Action<EventQuestExchangeSetting, EventShopProductData> m_DidTap;
    EventQuestExchangeSetting exchangeSetting;
    EventShopProductData productDataSetting;
}
