using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_MagiList : ViewBase {
    enum SortType {
        CreateDate, 
        Rarity,
        Equiped,
    }

    public static View_MagiList Create(CardData cardData, long? equipedBagID, Action<MagikiteData> selectIcon)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_MagiList");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_MagiList>();
        c.InitInternal(cardData, equipedBagID, selectIcon);
        return c;
    }

    public override void Dispose ()
    {
        if (m_MagikiteEquipPopup != null) {
            m_MagikiteEquipPopup.Close ();
        }
        base.Dispose ();
    }

    public bool TapBackButton ()
    {
        if (m_MagikiteSaleList != null) {
            m_MagikiteSaleList.Dispose ();
            m_MagikiteSaleList = null;
            return false;
        }
        if (m_MagikiteEquipPopup != null) {
            m_MagikiteEquipPopup.Close ();
            m_MagikiteEquipPopup = null;
            return false;
        }
        if (m_MagikiteDetailsPopup != null) {
            m_MagikiteDetailsPopup.Close ();
            m_MagikiteDetailsPopup = null;
            return false;
        }
        Dispose ();
        return true;
    }

    private void InitInternal(CardData cardData, long? equipedBagID, Action<MagikiteData> selectIcon)
    {
        m_CardData = cardData;
        m_EquipedBagID = equipedBagID;
        m_SelectCallback = selectIcon;
        m_MagikiteList = MagikiteData.CacheGetAll ();

        // 所持数の表示
        GetScript<TextMeshProUGUI> ("txtp_Limit").SetTextFormat ("/ {0}", AwsModule.UserData.UserData.MagikiteBagCapacity);
        GetScript<TextMeshProUGUI> ("txtp_Total").SetText (m_MagikiteList.Count);

        if (m_MagikiteList.Count == 0) {
            GetScript<RectTransform> ("NoItem").gameObject.SetActive (true);
        } else {
            GetScript<RectTransform> ("NoItem").gameObject.SetActive (false);

            m_Prototype = Resources.Load ("UnitDetails/ListItem_MagiIcon") as GameObject;
            m_InfiniteGridLayoutGroup = GetScript<InfiniteGridLayoutGroup> ("MagiGrid");
            m_InfiniteGridLayoutGroup.OnUpdateItemEvent.AddListener (UpdateIcon);

            SortProc ();
        }

        GetScript<RectTransform> ("bt_Ascentd").gameObject.SetActive (m_IsAscenrd);
        GetScript<RectTransform> ("bt_Descend").gameObject.SetActive (!m_IsAscenrd);
        // コールバックの設定
        SetCanvasCustomButtonMsg ("bt_Ascentd", DidTapAscentdOrDescend);
        SetCanvasCustomButtonMsg ("bt_Descend", DidTapAscentdOrDescend);

        SetCanvasCustomButtonMsg ("Sale/bt_Common", DidTapSale);

        GetScript<TMP_Dropdown> ("bt_TypeB").onValueChanged.AddListener (SortTypeChanged);
    }


    void DidTapAscentdOrDescend()
    {
        m_IsAscenrd = !m_IsAscenrd;
        GetScript<RectTransform> ("bt_Ascentd").gameObject.SetActive (m_IsAscenrd);
        GetScript<RectTransform> ("bt_Descend").gameObject.SetActive (!m_IsAscenrd);

        SortProc ();
    }

    void DidTapSale()
    {
        m_MagikiteSaleList = View_MagiSaleList.Create (() => {
            gameObject.SetActive (true);
            ResetList();
        });
        gameObject.SetActive (false);
    }

    void SortTypeChanged(int index)
    {
        var sortType = (SortType)index;
        if (sortType != m_SortType) {
            m_SortType = sortType;
            SortProc ();
        }
    }

    void SortItem()
    {
        switch(m_SortType) {
        case SortType.CreateDate:
            if (m_IsAscenrd) {
                m_SortList = m_MagikiteList.OrderBy (x => DateTime.Parse (x.CreationDate)).ToList();
            } else {
                m_SortList = m_MagikiteList.OrderByDescending (x => DateTime.Parse (x.CreationDate)).ToList();
            }
            break;
        case SortType.Equiped:
            if (m_IsAscenrd) {
                m_SortList = m_MagikiteList.OrderBy (x => x.IsEquipped ? 0 : 1).ToList();
            } else {
                m_SortList = m_MagikiteList.OrderBy (x => x.IsEquipped ? 1 : 0).ToList();
            }
            break;
        case SortType.Rarity:
            if (m_IsAscenrd) {
                m_SortList = m_MagikiteList.OrderBy (x => x.Magikite.rarity).ToList();
            } else {
                m_SortList = m_MagikiteList.OrderByDescending (x => x.Magikite.rarity).ToList();
            }
            break;
        }

        if (m_EquipedBagID.HasValue) {
            m_SortList.RemoveAll (x => x.BagId == m_EquipedBagID.Value);
            m_SortList.Insert(0, m_MagikiteList.FirstOrDefault(x => x.BagId == m_EquipedBagID.Value));
        }
    }


    void SortProc()
    {
        if (m_MagikiteList.Count == 0) {
            return;
        }
        SortItem ();
        m_InfiniteGridLayoutGroup.Initialize(m_Prototype, 25, m_SortList.Count, false);
    }
       

    void UpdateIcon(int index, GameObject icon)
    {
        var behaviour = icon.GetOrAddComponent<ListItem_MagiIcon> ();
        var magikite = m_SortList [index];
        behaviour.UpdateItem (magikite, DidTapIcon, DidLongTapIcon,
            m_EquipedBagID.HasValue ? magikite.BagId == m_EquipedBagID.Value : false,
            magikite.IsLocked,
            magikite.IsEquipped
        );
    }

    void DidTapIcon(ListItem_MagiIcon icon, MagikiteData magikite)
    {
        m_MagikiteEquipPopup = View_MagiEquip.Create (m_CardData, m_EquipedBagID, magikite, m_SelectCallback);
    }

    void DidLongTapIcon(MagikiteData magikite)
    {
        m_MagikiteDetailsPopup = View_MagiDetailsPop.Create (magikite, null, SaleMagikite, UpdateItem);
    }

    void EquipMagikite(MagikiteData magikite)
    {
        m_MagikiteEquipPopup = View_MagiEquip.Create (m_CardData, m_EquipedBagID, magikite, m_SelectCallback);
    }

    void SaleMagikite(string name, int price)
    {
        View_SaleOKPop.Create (name, price, null);
        ResetList ();
    }

    void ResetList()
    {
        // リストの作り直し
        m_MagikiteList = MagikiteData.CacheGetAll ();

        // 所持数の表示
        GetScript<TextMeshProUGUI> ("txtp_Limit").SetTextFormat ("/ {0}", AwsModule.UserData.UserData.MagikiteBagCapacity);
        GetScript<TextMeshProUGUI> ("txtp_Total").SetText (m_MagikiteList.Count);

        if (m_MagikiteList.Count == 0) {
            if (m_InfiniteGridLayoutGroup != null) { 
                m_InfiniteGridLayoutGroup.gameObject.DestroyChildren ();
            }
            GetScript<RectTransform> ("NoItem").gameObject.SetActive (true);
            return;
        }

        SortProc ();
        m_InfiniteGridLayoutGroup.ResetScrollPosition ();
    }

    void UpdateItem(MagikiteData magikite)
    {
        var index = m_SortList.FindIndex (x => x.BagId == magikite.BagId);
        if (index >= 0) {
            m_SortList [index] = magikite;
            var itemGo = m_InfiniteGridLayoutGroup.GetItem (index);
            if (itemGo != null) {
                var behaviour = itemGo.GetOrAddComponent<ListItem_MagiIcon> ();
                behaviour.UpdateItem (magikite, DidTapIcon, DidLongTapIcon,
                    m_EquipedBagID.HasValue ? magikite.BagId == m_EquipedBagID.Value : false,
                    magikite.IsLocked,
                    magikite.IsEquipped
                );
            }
        }
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    GameObject m_Prototype;
    CardData m_CardData;
    SortType m_SortType = SortType.CreateDate;
    bool m_IsAscenrd = true;
    List<MagikiteData> m_MagikiteList;
    List<MagikiteData> m_SortList;
    Action<MagikiteData> m_SelectCallback;

    InfiniteGridLayoutGroup m_InfiniteGridLayoutGroup;

    View_MagiEquip m_MagikiteEquipPopup;
    View_MagiSaleList m_MagikiteSaleList;
    View_MagiDetailsPop m_MagikiteDetailsPopup;
    long? m_EquipedBagID;
}
