using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;


/// <summary>
/// 武器一覧リスト表示するコンポーネント.
/// </summary>
public class WeaponListView : ViewBase 
{
	// Sort/bt_TypeBのドロップダウンのアイテム設定と同じ順番になっていないといけない。
    public enum SortType
    {
        Get = 0,
        Rarity,
        Level,
        ATK,
        DEF,
        HP,
        SPD,
        LimitBreak,
        Price
    }

    // ドロップダウン.
	[SerializeField]
	private TMP_Dropdown sortDropDown;
	[SerializeField]
	private CustomButton filterButton;

	[SerializeField]
	private InfiniteGridLayoutGroup gridLayoutGroup;

    public SortType sortType {
        get { return m_sortType; }
        set { m_sortType = value; }
    }
    public bool isDescending {
        get { return m_isDescending; }
        set { m_isDescending = value; }
    }

    public event Action<WeaponData> DidTapIconEvent;
	public event Action<WeaponData> DidLongTapIconEvent;
	public event Action<int/*index*/, GameObject/*updateObj*/> DidCallbackUpateListItem;
	public event Action DidUpdateList;

    /// <summary>生成しているListItem_WeaponIconのリスト.</summary>
	public List<ListItem_WeaponIcon> WeaponIconList { get { return gridLayoutGroup.GetComponentsInChildren<ListItem_WeaponIcon>().ToList(); } }


    /// <summary>
    /// 初期化.表示したくないWaponがあれば可変長でWeaponData単位で指定する.
    /// </summary>
    public void Init(WeaponData removeTarget = null, SortType afterSortType = SortType.Get, params WeaponData[] invisibleWeapons)
	{
		m_removeWeaponTarget = removeTarget;
		m_invisibleWeaponList = new List<WeaponData>(invisibleWeapons);
        m_filterData = new WeaponFilterSetting.Data();

        var weaponSortData = AwsModule.LocalData.WeaponSortData;
        var sortIndex = (int)m_sortType;
        m_sortType = (SortType)weaponSortData.SortType;
        m_isDescending = weaponSortData.IsDescending;
        if (m_sortType != afterSortType) {
            m_sortType = afterSortType;
            weaponSortData.SortType = sortIndex;
            AwsModule.LocalData.WeaponSortData = weaponSortData;
        }

        // 初回リスト生成.
        m_WeaponIconPrefab = Resources.Load("_Common/View/ListItem_WeaponIcon") as GameObject;
        this.CreateList();

		// ドロップダウンの初期設定
		if(sortDropDown != null){
			sortDropDown.onValueChanged.AddListener(SortDropdownValueChange);
            sortDropDown.value = sortIndex;
            sortDropDown.captionText.text = sortDropDown.options[sortIndex].text;
        }

        // ボタン.
        if (filterButton != null){
			filterButton.onClick.AddListener(DidTapFilter);
		}
	}

    private void CreateList()
    {
        var weaponSortData = AwsModule.LocalData.WeaponSortData;
        if (weaponSortData.SortType != (int)m_sortType || weaponSortData.IsDescending != m_isDescending) {
            weaponSortData.SortType = (int)m_sortType;
            weaponSortData.IsDescending = m_isDescending;
            AwsModule.LocalData.WeaponSortData = weaponSortData;
        }

        m_SortFilteredList = WeaponData.CacheGetAll();
        if(m_invisibleWeaponList.Count > 0){
            m_SortFilteredList.RemoveAll(w => m_invisibleWeaponList.Exists(i => i.BagId == w.BagId));
        }
        this.SortAndFilter();
        gridLayoutGroup.ResetScrollPosition();
        gridLayoutGroup.UpdateList (m_WeaponIconPrefab, m_SortFilteredList.Count);
        gridLayoutGroup.OnUpdateItemEvent.AddListener(CallbackUpdateListItem);
        gridLayoutGroup.Initialize(m_WeaponIconPrefab, 30, m_SortFilteredList.Count, false);
        if(DidUpdateList != null){
            DidUpdateList();
        }
    }

    /// <summary>
    /// リスト更新.キャッシュの取り直しから行う.false指定で差分更新だけ行う.
    /// </summary>
	public void UpdateList()
    {
        var weaponSortData = AwsModule.LocalData.WeaponSortData;
        if (weaponSortData.SortType != (int)m_sortType || weaponSortData.IsDescending != m_isDescending) {
            weaponSortData.SortType = (int)m_sortType;
            weaponSortData.IsDescending = m_isDescending;
            AwsModule.LocalData.WeaponSortData = weaponSortData;
        }

        m_SortFilteredList = WeaponData.CacheGetAll();
		if(m_invisibleWeaponList.Count > 0){
            m_SortFilteredList.RemoveAll(w => m_invisibleWeaponList.Exists(i => i.BagId == w.BagId));
		}
        this.SortAndFilter();
        gridLayoutGroup.UpdateList (m_WeaponIconPrefab, m_SortFilteredList.Count);
		if(DidUpdateList != null){
			DidUpdateList();
		}
    }

	// ソートとフィルタ
    private void SortAndFilter()
    {
        if (!m_SortFilteredList.Any()) {
            return;
        }
		
		// フィルタ
		Filter();

        // ソート
        m_SortFilteredList.Sort(SortCore);
  
		// はずす表示に関して
		if(m_removeWeaponTarget != null){
			int index = m_SortFilteredList.FindIndex(x => x.BagId == m_removeWeaponTarget.BagId);
			if (index >= 0) {
				var removeCard = m_SortFilteredList[index];
				m_SortFilteredList.RemoveAt(index);
				m_SortFilteredList.Insert(0, removeCard);
			}
		}
    }
	void Filter()
	{
		if(m_filterData == null) {
			return;
        }

        List<WeaponData> viewWeapons = new List<WeaponData>();
        if (m_filterData.RarityList != null && m_filterData.RarityList.Any()) {
            foreach (var x in m_filterData.RarityList) {
                viewWeapons.AddRange(m_SortFilteredList.Where(r => r.Rarity == x));
            }
		} else {
            viewWeapons = m_SortFilteredList;
        }

        if (m_filterData.WeaponTypeIndexList != null && m_filterData.WeaponTypeIndexList.Any()) { 
            List<WeaponData> tempFilter = new List<WeaponData>();
            foreach (var x in m_filterData.WeaponTypeIndexList) {
                tempFilter.AddRange(viewWeapons.Where(r => r.Weapon.type == x));
            }
            viewWeapons = tempFilter;
        }

        if (m_filterData.IsVisibleLock){
            viewWeapons.RemoveAll(r => !r.IsLocked);
		}else if(m_filterData.IsVisibleWithoutLock){
            viewWeapons.RemoveAll(r => r.IsLocked);
        }

        if (m_filterData.IsVisibleMaterial) {
            viewWeapons.RemoveAll(r => r.Weapon.type != 101);
            //m_SortFilteredList.RemoveAll(w => w.Weapon.type != 101);   // TODO :
        }
        m_SortFilteredList.RemoveAll(w => !viewWeapons.Exists(r => r.BagId == w.BagId));
    }
    int SortCore(WeaponData a, WeaponData b)
    {
        int sub = 0;
        switch (m_sortType) {
            case SortType.Get:
                sub = (int)(a.CreationDateTime - b.CreationDateTime).TotalSeconds;
                break;
            case SortType.Rarity:
                sub = a.Rarity - b.Rarity;
                break;
            case SortType.Level:
                sub = a.Level - b.Level;
                break;
            case SortType.ATK:
                sub = a.Parameter.Attack - b.Parameter.Attack;
                break;
            case SortType.DEF:
                sub = a.Parameter.Defense - b.Parameter.Defense;
                break;
            case SortType.HP:
                sub = a.Parameter.Hp - b.Parameter.Hp;
                break;
            case SortType.SPD:
                sub = a.Parameter.Agility - b.Parameter.Agility;
                break;
            case SortType.LimitBreak:
                sub = a.LimitBreakGrade - b.LimitBreakGrade;
                break;
            case SortType.Price:
                sub = a.Price - b.Price;
                break;
        }

        if (sub == 0) {
            return a.WeaponId - b.WeaponId;
        }
        return m_isDescending ? sub * -1 : sub;
    }

    // ソートドロップダウンの値変更.
    void SortDropdownValueChange(int index)
    {
        var afterSortType = (SortType)index;

        if (m_sortType != afterSortType) {
            var weaponSortData = AwsModule.LocalData.WeaponSortData;
            weaponSortData.SortType = index;
            AwsModule.LocalData.WeaponSortData = weaponSortData;
            m_sortType = afterSortType;
            UpdateList();
        }
    }
	// ボタン : フィルターボタンタップ.
	void DidTapFilter()
    {
		this.IsEnableButton = false;
		if(m_viewFilterPop == null){
			m_viewFilterPop = WeaponSContoller.CreateFilterPop(data => {
                this.IsEnableButton = true;
                m_filterData = data;
                UpdateList();
            });
		}else{
			m_viewFilterPop.IsEnable = true;
		}
    }
	public void DisposeFilterPop()
    {
		if(m_viewFilterPop != null){
            m_viewFilterPop.Dispose();
		}
    }

    // コールバック : InfiniteGridLayoutGroupによるListItem更新処理時.
    void CallbackUpdateListItem(int index, GameObject go)
    {
        var weapon = m_SortFilteredList[index];
		go.GetOrAddComponent<ListItem_WeaponIcon>().Init(weapon, 
		                                                 GetDispStatusType(), 
		                                                 m_removeWeaponTarget != null && m_removeWeaponTarget.BagId == weapon.BagId, 
                                                         true,
                                                         false,
		                                                 DidTapIconEvent, 
		                                                 DidLongTapIconEvent);
		if(DidCallbackUpateListItem != null){
			DidCallbackUpateListItem(index, go);
		}
    }
    private ListItem_WeaponIcon.DispStatusType GetDispStatusType()
    {
        switch (m_sortType) {
            case SortType.Level:
            case SortType.Rarity:
                return ListItem_WeaponIcon.DispStatusType.Default;
            case SortType.ATK:
                return ListItem_WeaponIcon.DispStatusType.ATK;
            case SortType.DEF:
                return ListItem_WeaponIcon.DispStatusType.DEF;
            case SortType.HP:
                return ListItem_WeaponIcon.DispStatusType.HP;
            case SortType.SPD:
                return ListItem_WeaponIcon.DispStatusType.SPD;
			case SortType.LimitBreak:
				return ListItem_WeaponIcon.DispStatusType.LimitBreak;
			case SortType.Price:
				return ListItem_WeaponIcon.DispStatusType.Price;

        }
        return ListItem_WeaponIcon.DispStatusType.Default;
    }
    public void UpdateSortDropDownCaption()
    {
        var sortIndex = (int)m_sortType;
        sortDropDown.value = sortIndex;
        sortDropDown.captionText.text = sortDropDown.options[sortIndex].text;
    }
    public void UpdateWeaponSortData()
    {
        var weaponSortData = AwsModule.LocalData.WeaponSortData;
        sortDropDown.value = weaponSortData.SortType;
        sortDropDown.captionText.text = sortDropDown.options[weaponSortData.SortType].text;
        m_isDescending = weaponSortData.IsDescending;
    }
    public void UpdateFilterData()
    {
        var filter = new WeaponFilterSetting();
        m_filterData.Apply(filter);
    }
    public override void Dispose()
    {
        AwsModule.LocalData.Sync((bSuccess, sender, eArgs) => {
            base.Dispose();
        });
    }

    private WeaponData m_removeWeaponTarget;
    private List<WeaponData> m_SortFilteredList;
	private View_WeaponFilterPop m_viewFilterPop;
	private WeaponFilterSetting.Data m_filterData;
	private GameObject m_WeaponIconPrefab;

    private static SortType m_sortType = SortType.Get;
    private static bool m_isDescending = false;

    private List<WeaponData> m_invisibleWeaponList; // 表示しない例外リスト.
}
