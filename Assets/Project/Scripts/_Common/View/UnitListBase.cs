using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

// Sort/bt_TypeBのドロップダウンのアイテム設定と同じ順番になっていないといけない。
public enum UnitListSortType {
    Create = 0,
    Level,
    Rarity,
    HP,
    ATK,
    DEF,
    SPD
}

public class UnitListBase : ViewBase
{
    public void Init(int focusCardID, bool isTapIcon = false, bool isLongTapIcon = false, ElementEnum? onlyElement=null)
    {
        // nullはALLとして扱う
        m_ElementFilter = null;
        m_DispOnlyElement = onlyElement;

        string onlyElementName = null;
        if (onlyElement.HasValue) {
            onlyElementName = MasterDataTable.element [onlyElement.Value].name;
        }

        // タブの作成
        var TabMenuGo = this.GetScript<Transform>("TabMenu").gameObject;
        List<string> ElementStringList = MasterDataTable.element.GetCategoryNameList();
        foreach (var elementString in ElementStringList) {
            var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_HorizontalTextTab", TabMenuGo);
            var c = go.GetOrAddComponent<ListItem_HorizontalElementTab> ();
            c.Init (elementString, DidTapTab);
            if ((string.IsNullOrEmpty(onlyElementName) && elementString == "ALL") ||
                (!string.IsNullOrEmpty(onlyElementName) && onlyElementName == elementString))
            {
                c.IsSelected = true;
            }

            if (!string.IsNullOrEmpty(onlyElementName) && onlyElementName != elementString) {
                c.SetInteractable (false);
            }
            m_OptionTabs.Add (c);
        }

        // カード用のリスト取得
        m_CardDataList = GetCardList();
        SortAndFilter ();

        // スクロールの初期設定
        m_UnitIconPrefab = Resources.Load ("_Common/View/ListItem_UnitIcon") as GameObject;
        var layoutGroup = this.GetScript<InfiniteGridLayoutGroup> ("UnitIcon");
		var digit = (m_SortFilterCardDataList.Count <= 0) ? 1 : ((int)Mathf.Log10(m_SortFilterCardDataList.Count) + 1);
		var limit = digit <= 1 ? 10 : Mathf.CeilToInt((float)m_SortFilterCardDataList.Count / (float)Math.Pow(10, digit - 1)) * 10;
        layoutGroup.OnUpdateItemEvent.AddListener (new UnityEngine.Events.UnityAction<int, GameObject>(UpdateListItem));
        layoutGroup.Initialize (m_UnitIconPrefab,
            limit, m_SortFilterCardDataList.Count, false);
        /*
        if (focusCardID != 0) {
            var focusIndex = m_SortFilterCardDataList.FindIndex (x => x.CardId == focusCardID);
            if (focusIndex >= 0) {
            }
        }
        */
        // ドロップダウンの初期設定
        var dropdown = GetScript<TMP_Dropdown> ("Sort/bt_TypeB");
        dropdown.onValueChanged.AddListener (new UnityEngine.Events.UnityAction<int> (SortDropdownValueChange));
		dropdown.captionText.text = dropdown.options[(int)sortType].text;

        GetScript<CustomButton>("bt_Ascentd").gameObject.SetActive(!isDescending);
        GetScript<CustomButton>("bt_Descend").gameObject.SetActive(isDescending);
        SetCanvasCustomButtonMsg("Order/bt_Ascentd", DidTapOrder);
		SetCanvasCustomButtonMsg("Order/bt_Descend", DidTapOrder);
    }   

    protected virtual List<CardData> GetCardList()
    {
        // カード用のリスト取得
        var list = CardData.CacheGetAll();
        // ベースリストを属性で制限する。
        if (m_DispOnlyElement.HasValue) {
            list = list.Where (x => x.Card.element.Enum == m_DispOnlyElement.Value).ToList ();
        }
        return list;
    }

    protected virtual ListItem_UnitIcon.DispStatusType GetDispStatusType()
    {
        switch (sortType) {
        case UnitListSortType.Level:
			return ListItem_UnitIcon.DispStatusType.Level;
        case UnitListSortType.Create:
        case UnitListSortType.Rarity:
            return ListItem_UnitIcon.DispStatusType.Default;
        case UnitListSortType.ATK:
            return ListItem_UnitIcon.DispStatusType.ATK;
        case UnitListSortType.DEF:
            return ListItem_UnitIcon.DispStatusType.DEF;
        case UnitListSortType.HP:
            return ListItem_UnitIcon.DispStatusType.HP;
        case UnitListSortType.SPD:
            return ListItem_UnitIcon.DispStatusType.SPD;
        }
        return ListItem_UnitIcon.DispStatusType.Default;
    }

    protected virtual bool GetDispRemove(CardData card)
    {
        return false;
    }

    protected virtual bool GetDispOrganizing(CardData card)
    {
        return false;
    }

    protected virtual Action<CardData> GetDidTapIcon()
    {
        return DidTapIcon;
    }

    protected virtual Action<CardData> GetDidLondTapIcon()
    {
        return DidLongTapIcon;
    }

    protected virtual void DidTapOrder()
    {
        isDescending = !isDescending;
		GetScript<CustomButton>("bt_Ascentd").gameObject.SetActive(!isDescending);
		GetScript<CustomButton>("bt_Descend").gameObject.SetActive(isDescending);
        SortAndFilter ();
        UpdateList ();
    }

    protected virtual void SortDropdownValueChange(int index)
    {
        // 並び替え
		var type = (UnitListSortType)index;

		if (sortType != type) {
			sortType = type;
            SortAndFilter ();
            UpdateList ();
        }
    }

    protected virtual void UpdateList()
    {
		View_FadePanel.SharedInstance.IsLightLoading = true;

        var layoutGroup = this.GetScript<InfiniteGridLayoutGroup>("UnitIcon");
        var digit = (m_SortFilterCardDataList.Count <= 0) ? 1 : ((int)Mathf.Log10(m_SortFilterCardDataList.Count) + 1);
        var limit = digit <= 1 ? 10 : Mathf.CeilToInt((float)m_SortFilterCardDataList.Count / (float)Math.Pow(10, digit - 1)) * 10;
        layoutGroup.ResetScrollPosition();
        layoutGroup.OnUpdateItemEvent.AddListener(new UnityEngine.Events.UnityAction<int, GameObject>(UpdateListItem));
		layoutGroup.InitializeAsync(m_UnitIconPrefab, limit, m_SortFilterCardDataList.Count, 3, () => {
            View_FadePanel.SharedInstance.IsLightLoading = false;
        }, false);
    }   

	protected virtual void UpdateListItem(int index, GameObject go)
    {
        var card = m_SortFilterCardDataList[index];
        go.GetComponent<ListItem_UnitIcon>().Init(card, GetDispStatusType(),
            GetDispRemove(card), GetDispOrganizing(card), false, GetDidTapIcon(), GetDidLondTapIcon());
    }

    protected virtual void SortAndFilter()
    {
        // 属性でフィルタ
        if (m_ElementFilter.HasValue) {
            m_SortFilterCardDataList = m_CardDataList.Where (x => x.Card.element.Enum == m_ElementFilter.Value).ToList ();
        } else {
            // ALL
            m_SortFilterCardDataList = new List<CardData>(m_CardDataList);
        }
        // ソート
        m_SortFilterCardDataList.Sort (SortCore);
    }

    protected virtual int SortCore(CardData a, CardData b)
    {
        int sub = 0;
        switch (sortType) {
        case UnitListSortType.Create:
            sub = a.CreateAt.CompareTo (b.CreateAt);
            break;
        case UnitListSortType.Level:
            sub = a.Level - b.Level;
            break;
        case UnitListSortType.Rarity:
            sub = a.Card.rarity - b.Card.rarity;
            break;
        case UnitListSortType.HP:
            sub = a.Parameter.MaxHp - b.Parameter.MaxHp;
            break;
        case UnitListSortType.ATK:
            sub = a.Parameter.Attack - b.Parameter.Attack;
            break;
        case UnitListSortType.DEF:
            sub = a.Parameter.Defense - b.Parameter.Defense;
            break;
        case UnitListSortType.SPD:
            sub = a.Parameter.Agility - b.Parameter.Agility;
            break;
        }

        if(sub == 0) {
            return a.CardId - b.CardId;
        }
        return isDescending ? sub * -1 : sub;
    }
        
    protected virtual void DidTapIcon (CardData card)
    {
    }

    protected virtual void DidLongTapIcon (CardData card)
    {
    }

    // 属性Tabタップ時
    protected virtual void DidTapTab(ListItem_HorizontalElementTab tabItem)
    {
        m_OptionTabs.ForEach (x => x.IsSelected = false); 
        tabItem.IsSelected = true;

        var filter = tabItem.ElementType;
        if (m_ElementFilter != filter) {
            m_ElementFilter = filter;
            SortAndFilter ();
            UpdateList ();
        }
    }

    protected GameObject m_UnitIconPrefab;

    protected List<CardData> m_CardDataList;

    // 属性フィルター設定
    protected ElementEnum? m_ElementFilter;

    // ソートオーダー
    protected static UnitListSortType sortType = UnitListSortType.Create;
    // 降順か？
	protected static bool isDescending = false;

    // ソートフィルタ済みリスト
    protected List<CardData> m_SortFilterCardDataList;

    // タブ管理
    protected List<ListItem_HorizontalElementTab> m_OptionTabs = new List<ListItem_HorizontalElementTab>();

    // 特定属性のみ表示させたいとき用
    protected ElementEnum? m_DispOnlyElement;
}
