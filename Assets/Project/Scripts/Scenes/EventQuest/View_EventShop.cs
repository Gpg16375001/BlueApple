using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_EventShop : ViewBase {

    public void Init(int EventId, int eventPoint, EventShopProductData[] productDatas, Screen_EventQuest root)
    {
        m_Root = root;
        EventID = EventId;
        EventPoint = eventPoint;
        ProductDatas = productDatas;

        EventQuest questData = MasterDataTable.event_quest [EventId];

        GetScript<TextMeshProUGUI> ("txtp_LimitDay").SetTextFormat ("{0}月{1}日 {2}:{3}まで",
            questData.exchange_time_limit.Month, questData.exchange_time_limit.Day,
            questData.exchange_time_limit.Hour, questData.exchange_time_limit.Minute);
        GetScript<TextMeshProUGUI> ("txtp_Wallet").SetText (eventPoint);

        ExchangeList = MasterDataTable.event_quest_exchange_setting [EventId];
        ExchangeList = ExchangeList.OrderBy(x => {
            var productData = System.Array.Find (ProductDatas, p => p.ShopProductId == x.id);
            if(productData != null) {
                if(productData.MaxPurchaseQuantity <= 0) {
                    return 1;
                }
            }
            return 0;
        }).ToArray();
        ScrollRect sr = GetScript<ScrollRect> ("ScrollAreaEventShopItem");
        var contentGo = sr.content.gameObject;


        layoutGroup = contentGo.GetComponent<InfiniteGridLayoutGroup> ();
        layoutGroup.OnUpdateItemEvent.AddListener (UpdateItem);
        layoutGroup.OnInitItemEvent.AddListener (InitItem);

        var o = Resources.Load("EventQuest/ListItem_EventShopItem") as GameObject;
        layoutGroup.Initialize (o, 7, ExchangeList.Length, false);
    }

    public void  SetEventPoint(int eventPoint)
    {
        EventPoint = eventPoint;
        GetScript<TextMeshProUGUI> ("txtp_Wallet").SetText (EventPoint);
    }

    public void Show()
    {
    }

    public void Hide()
    {
        if (layoutGroup != null) {
            layoutGroup.ResetScrollPosition (true);
        }
    }

    public override void Dispose ()
    {
        base.Dispose ();
    }

    public bool BackProc()
    {
        if (m_PruchasePop != null) {
            m_PruchasePop.Close ();
            m_PruchasePop = null;
            return false;
        }
        if (m_PruchaseOKPop != null) {
            m_PruchaseOKPop.Close ();
            m_PruchaseOKPop = null;
            return false;
        }
        return true;
    }

    private void InitItem(GameObject go)
    {
        go.GetOrAddComponent<ListItem_EventShopItem> ().InitItem(OpenPruchasePop);
    }

    private void UpdateItem(int index, GameObject go)
    {
        var data = ExchangeList [index];
        var productData = System.Array.Find (ProductDatas, x => x.ShopProductId == data.id);
        EventShopProductData releaseProductData = null;
        if (data.release_condition.HasValue) {
            releaseProductData = System.Array.Find (ProductDatas, x => x.ShopProductId == data.release_condition.Value);
        }
        go.GetComponent<ListItem_EventShopItem> ().UpdateItem(ExchangeList[index], productData, releaseProductData);
    }

    private void UpdateList ()
    {
        ExchangeList = ExchangeList.OrderBy(x => {
            var productData = System.Array.Find (ProductDatas, p => p.ShopProductId == x.id);
            if(productData != null) {
                if(productData.MaxPurchaseQuantity <= 0) {
                    return 1;
                }
            }
            return 0;
        }).ToArray();
        var o = Resources.Load("EventQuest/ListItem_EventShopItem") as GameObject;
        layoutGroup.UpdateList(o, ExchangeList.Length);
    }

    private void DidBuy(EventQuestExchangeSetting exchange, EventShopProductData product, int count)
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;
        SendAPI.EventPurchaseProduct (EventID, exchange.id, count,
            (s, res) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(s) {
                    AwsModule.UserData.UserData = res.UserData;
                    m_Root.SetEventPoint(res.EventPoint);

                    int index = System.Array.FindIndex(ProductDatas, x => x.ShopProductId == res.EventShopProductData.ShopProductId);
                    ProductDatas[index] = res.EventShopProductData;

                    UpdateCache(res.EventShopProductData.StockItemDataList);
                    if(res.EventShopProductData.MaxPurchaseQuantity <= 0) {
                        // 売り切れの時はリスト全体の並び替え
                        UpdateList();
                    } else {
                        var updateIndex = System.Array.FindIndex(ExchangeList, x => x.id == res.EventShopProductData.ShopProductId);
                        var go = layoutGroup.GetItem(updateIndex);
                        if(go != null) {
                            UpdateItem(updateIndex, go);
                        }
                    }

                    if(m_PruchasePop != null) {
                        m_PruchasePop.Close();
                        m_PruchasePop = null;
                    }

                    m_PruchaseOKPop = View_EventShopItemOKPop.Create(exchange, count, null);
                }
            }
        );  
    }

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

    private void OpenPruchasePop(EventQuestExchangeSetting exchange, EventShopProductData product)
    {
        m_PruchasePop = View_EventShopItemPurchasePop.Create (EventPoint, exchange, product, DidBuy);
    }

    int EventID;
    int EventPoint;
    EventQuestExchangeSetting[] ExchangeList;
    EventShopProductData[] ProductDatas;
    InfiniteGridLayoutGroup layoutGroup;

    View_EventShopItemPurchasePop m_PruchasePop;
    View_EventShopItemOKPop m_PruchaseOKPop;
    Screen_EventQuest m_Root;
}
