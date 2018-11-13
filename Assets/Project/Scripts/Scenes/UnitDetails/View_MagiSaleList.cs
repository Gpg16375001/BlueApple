using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;
using SmileLab;
using SmileLab.Net.API;

public class View_MagiSaleList : ViewBase {
    enum SortType {
        CreateDate, 
        Rarity,
        Price,
    }

    public static View_MagiSaleList Create(Action didClose)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_MagiSaleList");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_MagiSaleList>();
        c.InitInternal(didClose);
        return c;
    }

    public override void Dispose ()
    {
        if (m_DidClose != null) {
            m_DidClose ();
        }
        base.Dispose ();
    }

    private void InitInternal(Action didClose)
    {
        m_DidClose = didClose;
        m_MagikiteList = MagikiteData.CacheGetAll ().Where(x => !x.IsEquipped && !x.IsLocked).ToList();
        m_Selected = new List<MagikiteData> ();

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

        SetSaleInfo ();
        GetScript<RectTransform> ("bt_Ascentd").gameObject.SetActive (m_IsAscenrd);
        GetScript<RectTransform> ("bt_Descend").gameObject.SetActive (!m_IsAscenrd);
        // コールバックの設定
        SetCanvasCustomButtonMsg ("bt_Ascentd", DidTapAscentdOrDescend);
        SetCanvasCustomButtonMsg ("bt_Descend", DidTapAscentdOrDescend);

        SetCanvasCustomButtonMsg ("Sale/bt_Common", DidTapSale);
        SetCanvasCustomButtonMsg ("Deselect/bt_TopLineGray", DidTapDeselect);

        GetScript<TMP_Dropdown> ("bt_TypeB").onValueChanged.AddListener (SortTypeChanged);
    }

    void SetSaleInfo()
    {
        GetScript<TextMeshProUGUI> ("txtp_SelectNum").SetText (m_Selected.Count);
        var getPrice = m_Selected.Sum (x => x.Price);
        GetScript<TextMeshProUGUI> ("txtp_GetCoin").SetText (getPrice);
        var nowGold = AwsModule.UserData.UserData.GoldCount;
        GetScript<TextMeshProUGUI> ("txtp_Coin").SetText (nowGold + getPrice);
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
        if (m_Selected.Count == 0) {
            PopupManager.OpenPopupSystemOK ("マギカイトが選択されていません。");
            return;
        }
        View_SaleBundlePop.Create (m_Selected, SaleMagikite);
    }

    void DidTapDeselect()
    {
        var oldSelected = m_Selected.ToArray ();
        m_Selected.Clear ();
        foreach (var magi in oldSelected) {
            var index = m_SortList.FindIndex (x => x == magi);
            var go = m_InfiniteGridLayoutGroup.GetItem (index);
            if (go != null) {
                UpdateIcon (index, go);
            }
        }
        SetSaleInfo ();
    }

    void SaleMagikite(string name, int price)
    {
        View_SaleOKPop.Create (name, price, null);

        m_Selected.Clear();
        SetSaleInfo ();

        ResetList ();
    }

    void ResetList()
    {
        if (m_MagikiteList.Count == 0) {
            return;
        }
        // リストの作り直し
        m_MagikiteList = MagikiteData.CacheGetAll ().Where(x => !x.IsEquipped && !x.IsLocked).ToList();

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
        case SortType.Price:
            if (m_IsAscenrd) {
                m_SortList = m_MagikiteList.OrderBy (x => x.Price).ToList();
            } else {
                m_SortList = m_MagikiteList.OrderByDescending (x => x.Price).ToList();
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
        behaviour.UpdateItem (magikite, DidTapIcon, null,
            false,
            magikite.IsLocked,
            magikite.IsEquipped,
            false,
            m_Selected.Contains(magikite)
        );
    }

    void DidTapIcon(ListItem_MagiIcon icon, MagikiteData magikite)
    {
        if (!m_Selected.Contains (magikite)) {
            if (m_Selected.Count < 10) {
                icon.Selected = true;
                m_Selected.Add (magikite);
            }
        } else {
            icon.Selected = false;
            m_Selected.Remove (magikite);
        }

        SetSaleInfo ();
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    GameObject m_Prototype;
    SortType m_SortType = SortType.CreateDate;
    bool m_IsAscenrd = true;
    List<MagikiteData> m_MagikiteList;
    List<MagikiteData> m_SortList;

    List<MagikiteData> m_Selected;

    Action m_DidClose;
    InfiniteGridLayoutGroup m_InfiniteGridLayoutGroup;
}
