using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;
using System.Linq;
using TMPro;
using SmileLab.Net.API;
using System;
using SmileLab.UI;

public enum ItemListTabType {
    None = 0,       // Null
    All,            // ALL
    Reinforcement,  // 強化
    Evolution,      // 進化
    ReleaseKey,     // 解放キー
    Soulpiece,      // ソウルピース
    Magikite,       // マギカイト
    Other,          // その他
}

public enum ItemListViewSortType {
    None = 0,   // なし
    Elenemt,    // 属性
    Rarity,     // レアリティ
    Type,       // 種類
    Obtaining,  // 入手順
    Equipped,   // 装備中
}

public class ItemListCurrencyData 
{
    /// <summary> アイテムタイプ </summary>
    public ItemTypeEnum itemTypeEnum;
    /// <summary> アイテムデータ </summary>
    public Currency itemData;
    /// <summary> アイテム数 </summary>
    public int itemCount;
}

public class ItemListTabSetting
{
    /// <summary> タブ名 </summary>
    public string tabName;
    /// <summary> ソートオプション </summary>
    public string[] sortOptions;
    /// <summary> タブのオブジェクト </summary>
    public ListItem_HorizontalTextTab tabObject = null;
}

public class Screen_ItemList : ViewBase 
{
    /// <summary> タブ毎の設定 </summary>
    Dictionary<ItemListTabType, ItemListTabSetting> tabSettings;
    /// <summary> 現在のタブタイプ </summary>
    ItemListTabType currentTabType;
    /// <summary> 現在のソートタイプ </summary>
    ItemListViewSortType currentSortType;
    /// <summary> 現在のタブタイプ </summary>
    TMP_Dropdown sortDropDown;
    /// <summary> 所持しているマテリアルのリスト </summary>
    List<MaterialData> myMaterialDataList;
    /// <summary> 所持しているマギカイトのリスト </summary>
    List<MagikiteData> myMagikiteDataList;
    /// <summary> 所持しているその他のリスト </summary>
    List<ConsumerData> myConsumerDataList;
    /// <summary> 所持している通貨のリスト </summary>
    List<ItemListCurrencyData> myCurrencyDataList;
    /// <summary> 現在表示しているアイテムのリスト </summary>
    List<object> currentContentItemList;
    /// <summary> アイテムアイコンのプレハブ </summary>
    GameObject m_ItemIconPrefab;
    /// <summary> スクロールレイアウトグループ </summary>
    InfiniteGridLayoutGroup layoutGroup;
    /// <summary> 降順フラグ </summary>
    private bool IsDescending;
    /// <summary> 昇順ボタン </summary>
    private GameObject btAscentd;
    /// <summary> 降順ボタン </summary>
    private GameObject btDescend;

    public void Init()
    {
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        // タブとソートの設定情報を初期化
        tabSettings = new Dictionary<ItemListTabType, ItemListTabSetting> () 
        { 
            { ItemListTabType.All, new ItemListTabSetting () { tabName = "ALL", sortOptions = new string[] { "なし" } } }, 
            { ItemListTabType.Reinforcement, new ItemListTabSetting () { tabName = "強化", sortOptions = new string[] { "属性", "レアリティ" } } }, 
            { ItemListTabType.Evolution, new ItemListTabSetting () { tabName = "進化", sortOptions = new string[] { "種類", "レアリティ" } } }, 
            { ItemListTabType.ReleaseKey, new ItemListTabSetting () { tabName = "解放キー", sortOptions = new string[] { "属性", "レアリティ" } } }, 
            { ItemListTabType.Soulpiece, new ItemListTabSetting () { tabName = "ソウル", sortOptions = new string[] { "なし" } } }, 
            { ItemListTabType.Magikite, new ItemListTabSetting () { tabName = "マギカイト", sortOptions = new string[] { "入手順", "レアリティ", "装備中" } } }, 
            { ItemListTabType.Other, new ItemListTabSetting () { tabName = "その他", sortOptions = new string[] { "なし" } } },
        };

        // 現在のタブ状態を初期化
        currentTabType = ItemListTabType.All;
        // タブの生成と設定
        var TabMenuRoot = this.GetScript<Transform> ("TabMenu").gameObject;
        foreach (var tabSetting in tabSettings) {
            var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_HorizontalTextTab", TabMenuRoot);
            var c = go.GetOrAddComponent<ListItem_HorizontalTextTab> ();
            c.Init (tabSetting.Value.tabName, DidTapTab);
            tabSetting.Value.tabObject = c;
        }

        // 現在の順序状態を初期化
        IsDescending = AwsModule.LocalData.ItemListSortData.IsDescending;
        // 昇順・降順ボタンの設定
        btAscentd = this.GetScript<CustomButton> ("bt_Ascentd").gameObject;
        btDescend = this.GetScript<CustomButton> ("bt_Descend").gameObject;
        btAscentd.SetActive (!IsDescending);
        btDescend.SetActive (IsDescending);
        this.SetCanvasCustomButtonMsg("Order/bt_Ascentd", DidTapOrder);
        this.SetCanvasCustomButtonMsg("Order/bt_Descend", DidTapOrder);

        // 現在のソート状態を初期化
        currentSortType = ItemListViewSortType.None;
        // ソートのドロップダウン
        sortDropDown = this.GetScript<TMP_Dropdown> ("bt_TypeB");
        sortDropDown.onValueChanged.AddListener (
            (index) => {
                OnSortValueChanged(sortDropDown.options[index].text);
            }
        );

        // 各種データの取得
        myMaterialDataList = MaterialData.CacheGetAll ().FindAll(x => x.CharaMaterialInfo != null && x.Count > 0);   // マテリアルデータ
        myMagikiteDataList = MagikiteData.CacheGetAll ();   // マギカイトデータ
        myConsumerDataList = ConsumerData.CacheGetAll ();   // その他データ
        try {
            myCurrencyDataList = new List<ItemListCurrencyData>(); // 通貨データ
            if(AwsModule.UserData.UserData.FriendPointCount > 0)
            {
                myCurrencyDataList.Add(new ItemListCurrencyData () { itemTypeEnum = ItemTypeEnum.friend_point, itemData = MasterDataTable.currency.Get(ItemTypeEnum.friend_point), itemCount = AwsModule.UserData.UserData.FriendPointCount });
            }
        }
        catch {
            myCurrencyDataList = new List<ItemListCurrencyData> (); // 通貨データ
        }

        // TODO: イベントアイテムデータ

        // 変数の初期化
        currentContentItemList = new List<object> ();
        // アイコンプレハブのロード
        m_ItemIconPrefab = Resources.Load("ItemList/ListItem_ItemIcon") as GameObject;
        // スクロールのレイアウトグループ
        layoutGroup = this.GetScript<InfiniteGridLayoutGroup>("UnitIcon");
        layoutGroup.OnUpdateItemEvent.AddListener (UpdateListItem);

        // 初期選択状態の設定
        ReLoadContent ();
    }

    void OnSortValueChanged(string type, bool forcedUpdate = false)
    {
        ItemListViewSortType _sortType = ItemListViewSortType.None;

        switch (type) {
        case "なし":
            _sortType = ItemListViewSortType.None;
            break;
        case "属性":
            _sortType = ItemListViewSortType.Elenemt;
            break;
        case "レアリティ":
            _sortType = ItemListViewSortType.Rarity;
            break;
        case "種類":
            _sortType = ItemListViewSortType.Type;
            break;
        case "入手順":
            _sortType = ItemListViewSortType.Obtaining;
            break;
        case "装備中":
            _sortType = ItemListViewSortType.Equipped;
            break;
        }

        if (currentSortType != _sortType || forcedUpdate) {
            currentSortType = _sortType;
            ReLoadContent ();
        }
    }

    void DidTapTab(ListItem_HorizontalTextTab tabItem)
    {
        var setting = tabSettings.SingleOrDefault (data => data.Value.tabName == tabItem.CategoryName);

        // タブと同じカテゴリ名をもつEnumタイプを取得
        ItemListTabType isSelect = setting.Key;

        // 現在のタブと違う場合コンテンツ内の変更を行う
        if (currentTabType != isSelect) {
            currentTabType = isSelect;
        }

        // 並び順を設定
        OnSortValueChanged (setting.Value.sortOptions [0], true);
    }

    /// <summary>
    /// コンテンツを再読み込みする
    /// </summary>
    void ReLoadContent()
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;

        foreach (var tabs in tabSettings) {
            tabs.Value.tabObject.IsSelected = tabs.Key == currentTabType;

            if (tabs.Value.tabObject.IsSelected) {
                if (currentSortType == ItemListViewSortType.None) {
                    sortDropDown.gameObject.SetActive (false);
                } else {
                    // ソートオプションの初期化
                    sortDropDown.ClearOptions ();
                    sortDropDown.AddOptions (tabs.Value.sortOptions.ToList ());
                    sortDropDown.gameObject.SetActive (true);
                }
            }
        }

        currentContentItemList.Clear ();

        switch (currentTabType) {
        case ItemListTabType.All:           // 全て
            // 各種リストを突っ込む
            foreach (var item in myMaterialDataList) {
                currentContentItemList.Add (item);
            }
            foreach (var item in myMagikiteDataList) {
                currentContentItemList.Add (item);
            }
            foreach (var item in myConsumerDataList) {
                currentContentItemList.Add (item);
            }
            foreach (var item in myCurrencyDataList) {
                currentContentItemList.Add (item);
            }
            // ソート
            currentContentItemList.Sort (ListSort);
            break;
        case ItemListTabType.Reinforcement: // 強化
            // 強化素材のみをID順にソートしてリストに突っ込む
            var _myMaterialDataList = myMaterialDataList.FindAll(x => x.CharaMaterialInfo.IsCharaEnhanceMaterial);
            _myMaterialDataList.Sort(ListSort);
            foreach(MaterialData item in _myMaterialDataList) {
                currentContentItemList.Add (item);
            }
            break;
        case ItemListTabType.Evolution:     // 進化
            // 進化素材のみをID順にソートしてリストに突っ込む
            _myMaterialDataList = myMaterialDataList.FindAll (x => x.CharaMaterialInfo.IsCharaEvolutionMaterial);
            _myMaterialDataList.Sort (ListSort);
            foreach (MaterialData item in _myMaterialDataList) {
                currentContentItemList.Add (item);
            }
            break;
        case ItemListTabType.ReleaseKey:    // 解放キー
            // 属性固有育成
            _myMaterialDataList = myMaterialDataList.FindAll(x => x.CharaMaterialInfo.IsSelectTypeMaterial(CharaMaterialTypeEnum.element_based_growth_material));
            _myMaterialDataList.Sort(ListSort);
            foreach (MaterialData item in _myMaterialDataList) {
                currentContentItemList.Add (item);
            }
            break;
        case ItemListTabType.Soulpiece:     // ソウルピース
            // カード固有原素材、固有素材
            _myMaterialDataList = myMaterialDataList.FindAll (x => x.CharaMaterialInfo.IsSelectTypeMaterial (CharaMaterialTypeEnum.card_based_raw_material, CharaMaterialTypeEnum.card_based_material));
            _myMaterialDataList.Sort (ListSort);
            foreach (MaterialData item in _myMaterialDataList) {
                currentContentItemList.Add (item);
            }
            break;
        case ItemListTabType.Magikite:      // マギカイト
            myMagikiteDataList.Sort(ListSort);
            foreach (MagikiteData item in myMagikiteDataList) {
                currentContentItemList.Add (item);
            }
            break;
        case ItemListTabType.Other:         // 他
            foreach (ConsumerData item in myConsumerDataList) {
                currentContentItemList.Add (item);
            }
            foreach (var item in myCurrencyDataList) {
                currentContentItemList.Add (item);
            }
            // ソート
            currentContentItemList.Sort (ListSort);
            break;
        }

        // リストの初期化 もしくは 更新
        if (!layoutGroup.IsInit) {
            layoutGroup.Initialize (m_ItemIconPrefab, 36, currentContentItemList.Count, false);
        } else {
            layoutGroup.UpdateList (m_ItemIconPrefab, currentContentItemList.Count);
        }

        View_FadePanel.SharedInstance.IsLightLoading = false;
    }

    /// <summary>
    /// (リロード時)リストのソート
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    int ListSort(object x, object y)
    {
        int res = 0;
        int xId = 0;
        int yId = 0;
        switch (currentTabType) {
        case ItemListTabType.All:           // すべて
        case ItemListTabType.Other:         // その他
            res = GetDataTypePriority (x) - GetDataTypePriority (y);
            if (res == 0) {
                if (x.GetType () == typeof(MaterialData)) {
                    return MaterialDataSort ((MaterialData)x, (MaterialData)y);
                }
                if (x.GetType () == typeof(MagikiteData)) {
                    return MagikiteDataSort ((MagikiteData)x, (MagikiteData)y);
                }
                if (x.GetType () == typeof(ConsumerData)) {
                    return ConsumerDataSort ((ConsumerData)x, (ConsumerData)y);
                }
                if (x.GetType () == typeof(ItemListCurrencyData)) {
                    return CurrencyDataSort ((ItemListCurrencyData)x, (ItemListCurrencyData)y);
                }
            }
            break;
        case ItemListTabType.Reinforcement: // 強化
        case ItemListTabType.ReleaseKey:    // 解放キー
            return MaterialDataSort ((MaterialData)x, (MaterialData)y);
        case ItemListTabType.Evolution:     // 進化
            return MaterialDataSort ((MaterialData)x, (MaterialData)y);
        case ItemListTabType.Soulpiece:     // ソウルピース
            return MaterialDataSort ((MaterialData)x, (MaterialData)y);
        case ItemListTabType.Magikite:      // マギカイト
            return MagikiteDataSort ((MagikiteData)x, (MagikiteData)y);
        }

        if (res == 0) {
            return xId - yId;
        }

        return IsDescending ? res * -1 : res;
    }

    int GetDataTypePriority(object obj) {
        if (obj.GetType () == typeof(MaterialData)) {
            return 1;
        } else if (obj.GetType () == typeof(MagikiteData)) {
            return 2;
        } else if (obj.GetType () == typeof(ConsumerData)) {
            return 3;
        } else if (obj.GetType () == typeof(ItemListCurrencyData)) {
            return 99;
        }

        return 0;
    }

    #region 素材データソート

    /// <summary>
    /// 素材データのソート
    /// </summary>
    /// <param name="x">A</param>
    /// <param name="y">B</param>
    int MaterialDataSort(MaterialData x, MaterialData y)
    {
        int res = 0;
        switch (currentSortType) {
        case ItemListViewSortType.Elenemt:
            // 属性値
            res = (int)x.Element.Enum - (int)y.Element.Enum;
            break;
        case ItemListViewSortType.Rarity:
            // レアリティ
            res = x.CharaMaterialInfo.rarity - y.CharaMaterialInfo.rarity;
            break;
        case ItemListViewSortType.Type:
            int xres = SetMaterialTypeEnumPriority (x.CharaMaterialInfo.type);
            int yres = SetMaterialTypeEnumPriority (y.CharaMaterialInfo.type);
            res = xres - yres;
            break;
        case ItemListViewSortType.None:
            break;
        }

        if (res == 0) {
            res = x.MaterialId - y.MaterialId;
        }

        return IsDescending ? res * -1 : res;
    }

    /// <summary>
    /// タイプ毎のIndexを取得 (今のところ進化素材のみ)
    /// </summary>
    /// <param name="type">タイプ</param>
    int SetMaterialTypeEnumPriority(string type)
    {
        if (type == MasterDataTable.chara_material_type.DataList.Find (m => m.Enum == CharaMaterialTypeEnum.element_based_evolution_material).name) {
            return MasterDataTable.chara_material_type.DataList.Find (m => m.Enum == CharaMaterialTypeEnum.element_based_evolution_material).index;
        }
        else if(type == MasterDataTable.chara_material_type.DataList.Find (m => m.Enum == CharaMaterialTypeEnum.enemy_based_material).name) {
            return MasterDataTable.chara_material_type.DataList.Find (m => m.Enum == CharaMaterialTypeEnum.enemy_based_material).index;
        }
        else if(type == MasterDataTable.chara_material_type.DataList.Find (m => m.Enum == CharaMaterialTypeEnum.role_based_material).name) {
            return MasterDataTable.chara_material_type.DataList.Find (m => m.Enum == CharaMaterialTypeEnum.role_based_material).index;
        }
            
        return 0;
    }

    #endregion

    #region マギカイトソート

    /// <summary>
    /// マギカイトデータのソート (入手順、レアリティ、装備中のみ)
    /// </summary>
    /// <param name="x">A</param>
    /// <param name="y">B</param>
    /// <param name="sortType">ソートタイプ</param>
    int MagikiteDataSort(MagikiteData x, MagikiteData y)
    {
        int res = 0;

        switch (currentSortType) {
        case ItemListViewSortType.Obtaining:
            res = DateTime.Parse(x.CreationDate).CompareTo(DateTime.Parse(y.CreationDate));
            break;
        case ItemListViewSortType.Rarity:
            res = x.Rarity - y.Rarity;
            break;
        case ItemListViewSortType.Equipped:
            int xres = x.IsEquipped ? 1 : 99;
            int yres = y.IsEquipped ? 1 : 99;
            res = xres - yres;
            break;
        }

        if (res == 0) {
            res = x.MagikiteId - y.MagikiteId;
        }

        return IsDescending ? res * -1 : res;
    }

    #endregion

    #region その他

    /// <summary>
    /// その他データのソート
    /// </summary>
    /// <param name="x">A</param>
    /// <param name="y">B</param>
    /// <param name="sortType">ソートタイプ</param>
    int ConsumerDataSort(ConsumerData x, ConsumerData y)
    {
        int res = x.ConsumerId - y.ConsumerId;

        return IsDescending ? res * -1 : res;
    }

    #endregion

    #region 通貨

    int CurrencyDataSort(ItemListCurrencyData x, ItemListCurrencyData y)
    {
        int res = x.itemData.id - y.itemData.id;

        return IsDescending ? res * -1 : res;
    }

    #endregion

    void DidTapOrder()
    {
        IsDescending = !IsDescending;
        AwsModule.LocalData.ItemListSortData.IsDescending = IsDescending;
        btAscentd.SetActive (!IsDescending);
        btDescend.SetActive (IsDescending);
        ReLoadContent ();
    }

    void DidTapBack()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToMyPage ();
            }
        );
    }

    void UpdateListItem(int index, GameObject go)
    {
        ListItem_ItemIcon itemIcon = go.GetComponent<ListItem_ItemIcon> ();

        if (itemIcon == null) {
            itemIcon = go.AddComponent<ListItem_ItemIcon> ();
        }

        itemIcon.Init (currentContentItemList [index], OnClickIcon);
    }

    void OnClickIcon(ListItem_ItemIcon itemIcon)
    {
        View_ItemListDetailsPop.Create (itemIcon.ItemData);
    }
}
