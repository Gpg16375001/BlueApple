using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_EventShopItemPurchasePop : PopupViewBase {
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_EventShopItemPurchasePop Create(int EventPoint, EventQuestExchangeSetting setting, EventShopProductData productData, Action<EventQuestExchangeSetting, EventShopProductData, int> didBuy)
    {
        var go = GameObjectEx.LoadAndCreateObject("EventQuest/View_EventShopItemPurchasePop");
        var c = go.GetOrAddComponent<View_EventShopItemPurchasePop>();
        c.InitInternal(EventPoint, setting, productData, didBuy);
        return c;
    }
    private void InitInternal(int EventPoint, EventQuestExchangeSetting setting, EventShopProductData productData, Action<EventQuestExchangeSetting, EventShopProductData, int> didBuy)
    {
        productDataSetting = productData;
        exchangeSetting = setting;
        m_didBuy = didBuy;
        m_currentSelectNum = 1;
        m_usePoint = productData.ExchangeQuantity;

        int pointLimit =  EventPoint / m_usePoint;
        int haveLimit = HaveLimitCount () / exchangeSetting.quantity;
        int exchangeLimit = productData.MaxPurchaseQuantity;
        CanBuyMaxNumber = pointLimit < haveLimit ? 
            ((pointLimit < exchangeLimit) ? pointLimit : exchangeLimit) :
            ((haveLimit < exchangeLimit) ? haveLimit : exchangeLimit);
        SetIcon (setting);

        // ラベル
        this.GetScript<TextMeshProUGUI>("txtp_ShopItemName").SetText(setting.item_type.GetNameAndQuantity(setting.item_id, setting.quantity));
        this.GetScript<TextMeshProUGUI>("txtp_SelectShopItemText").SetText(setting.details);
        this.GetScript<TextMeshProUGUI>("txtp_Price").SetText(setting.use_point.ToString("#,0"));
        this.GetScript<TextMeshProUGUI>("txtp_TotalPrice").SetText(m_currentSelectNum * m_usePoint);
        this.GetScript<TextMeshProUGUI>("txtp_TotalNumLimit").SetText(CanBuyMaxNumber);
        this.GetScript<TextMeshProUGUI>("txtp_SelectTotalNum").SetText(m_currentSelectNum);
        this.GetScript<RectTransform>("StockNum").gameObject.SetActive(productDataSetting.StockItemDataList.Length == 1);
        if(productDataSetting.StockItemDataList.Length == 1){
            this.GetScript<TextMeshProUGUI>("txtp_StockNum").SetText(productDataSetting.StockItemDataList[0].Quantity);
        }
        this.GetScript<RectTransform>("TimeLimit").gameObject.SetActive(false); // TODO : アイテム販売期間が無制限なら表示しない.
        this.GetScript<RectTransform>("NumberLimit").gameObject.SetActive(productData.UpperLimit > 0); // TODO : アイテム購入制限がなければ表示しない.
        if (productData.UpperLimit > 0) {
            GetScript<TextMeshProUGUI> ("txtp_Number").SetText (productData.UpperLimit - productData.MaxPurchaseQuantity);
            GetScript<TextMeshProUGUI> ("txtp_NumberLimit").SetText (productData.UpperLimit);
        }

        var iconName = ItemTypeEnum.event_point.GetCurrencyIconName();
        this.GetScript<uGUISprite>("txtp_TotalPrice/CurrencyIcon").ChangeSprite(iconName);
        this.GetScript<uGUISprite>("Price/IconGem").ChangeSprite(iconName);

        // ボタン
        this.SetCanvasCustomButtonMsg("Buy/bt_Common", DidTapBuy, m_currentSelectNum <= CanBuyMaxNumber);
        this.SetCanvasCustomButtonMsg("Cancel/bt_Common", DidTapCancel);
        this.SetCanvasCustomButtonMsg("bt_Plus", DidTapPlus, m_currentSelectNum <= CanBuyMaxNumber);
        this.SetCanvasCustomButtonMsg("bt_Minus", DidTapSub, m_currentSelectNum > 1);
        SetBackButton ();
    }

    int HaveLimitCount()
    {
        int capNum = 0;
        switch (exchangeSetting.item_type) {
        case ItemTypeEnum.weapon:
            capNum = (AwsModule.UserData.UserData.WeaponBagCapacity - WeaponData.CacheGetAll ().Count);
            break;
        case ItemTypeEnum.magikite:
            capNum = (AwsModule.UserData.UserData.MagikiteBagCapacity - MagikiteData.CacheGetAll ().Count);
            break;
        case ItemTypeEnum.consumer:
            {
                var item = ConsumerData.CacheGet (exchangeSetting.item_id);
                capNum = (MasterDataTable.CommonDefine ["CONSUMER_CAPACITY"].define_value -  (item != null ? item.Count : 0));                  
            }                  
            break;
        case ItemTypeEnum.material:
            {
                var item = MaterialData.CacheGet (exchangeSetting.item_id);
                capNum = (MasterDataTable.CommonDefine ["MATERIAL_CAPACITY"].define_value - (item != null ? item.Count : 0));                  
            }                  
            break;
        case ItemTypeEnum.money:
            capNum = (MasterDataTable.CommonDefine ["CURRENCY_CAPACITY"].define_value) - AwsModule.UserData.UserData.GoldCount;
            break;
        case ItemTypeEnum.pvp_medal:
            capNum = (MasterDataTable.CommonDefine ["CURRENCY_CAPACITY"].define_value) - AwsModule.UserData.UserData.PvpMedalCount;
            break;
        default:
            capNum = (MasterDataTable.CommonDefine ["BUY_LIMIT"].define_value);
            break;
        }
        return capNum;
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

    public void Close()
    {
        DidTapCancel ();
    }

    protected override void DidBackButton ()
    {
        DidTapCancel();
    }

    void DidTapBuy()
    {
        if (IsClosed) {
            return;
        }
        if (m_didBuy != null) {
            m_didBuy (exchangeSetting, productDataSetting, m_currentSelectNum);
        }
    }

    // ボタン: キャンセル.
    void DidTapCancel()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapPlus()
    {
        if (IsClosed) {
            return;
        }

        if (m_currentSelectNum >= CanBuyMaxNumber) {
            return;
        }
        m_currentSelectNum++;

        UpdateInfo ();
    }

    void DidTapSub()
    {
        if (IsClosed) {
            return;
        }

        if (m_currentSelectNum <= 1) {
            UpdateInfo ();
            return;
        }
        m_currentSelectNum--;

        UpdateInfo ();
    }

    void UpdateInfo()
    {
        this.GetScript<TextMeshProUGUI>("txtp_TotalPrice").SetText(m_currentSelectNum * m_usePoint);
        this.GetScript<TextMeshProUGUI>("txtp_SelectTotalNum").SetText(m_currentSelectNum);
        this.GetScript<CustomButton>("bt_Plus").interactable = m_currentSelectNum < CanBuyMaxNumber;
        this.GetScript<CustomButton>("bt_Minus").interactable = m_currentSelectNum > 1;
    }


    int m_currentSelectNum;
    int m_usePoint;
    int CanBuyMaxNumber = 0;

    EventQuestExchangeSetting exchangeSetting;
    EventShopProductData productDataSetting;
    Action<EventQuestExchangeSetting, EventShopProductData, int> m_didBuy;
}
