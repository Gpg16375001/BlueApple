using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;


/// <summary>
/// AWS SDK "Cognito"を利用したローカルに保存したいデータ.
/// </summary>
public class AwsLocalSaveData : AwsCognitoDatasetBase
{
    private WeaponSortData weaponSortData;
    private UnitListSortData unitListSortData;
    private FriendSelectSortData friendSelectSortData;
    private OptionSupportSortData optionSupportSortData;
    private ItemListSortData itemListSortData;

	/// <summary>AP回復時の通知が有効かどうか.</summary>
    public bool IsNotificateAP
    {
		get { return Get<bool>("IsNotificateAP"); }
	    set { Put("IsNotificateAP", value); }
    }
	/// <summary>BP回復時の通知が有効かどうか.</summary>
    public bool IsNotificateBP
    {
        get { return Get<bool>("IsNotificateBP"); }
        set { Put("IsNotificateBP", value); }
    }

    /// <summary>BGM音量.</summary>
    public float Volume_BGM
    {
        get { return Get<float>("Volume_BGM"); }
        set { Put("Volume_BGM", value); }
    }
    /// <summary>SE音量.</summary>
    public float Volume_SE
    {
        get { return Get<float>("Volume_SE"); }
        set { Put("Volume_SE", value); }
    }
    /// <summary>ボイス音量.</summary>
    public float Volume_Voice
    {
        get { return Get<float>("Volume_Voice"); }
        set { Put("Volume_Voice", value); }
    }
	/// <summary>シナリオオート</summary>
	public bool Scenario_Auto
	{
		get { return Get<bool> ("Scenario_Auto"); }
		set { Put ("Scenario_Auto", value); }
	}
    /// <summary>武器ソート・フィルターデータ.</summary>
    public WeaponSortData WeaponSortData {
        get { return Get<WeaponSortData>("WeaponSortData"); }
        set { Put("WeaponSortData", value); }
    }
    /// <summary>ユニットソートデータ.</summary>
    public UnitListSortData UnitListSortData {
        get { return Get<UnitListSortData>("UnitListSortData"); }
        set { Put("UnitListSortData", value); }
    }
    /// <summary>フレンドセレクトソート・フィルターデータ.</summary>
    public FriendSelectSortData FriendSelectSortData {
        get { return Get<FriendSelectSortData>("FriendSelectSortData"); }
        set { Put("FriendSelectSortData", value); }
    }
    /// <summary>オプションサポートソートデータ.</summary>
    public OptionSupportSortData OptionSupportSortData {
        get { return Get<OptionSupportSortData>("OptionSupportSortData"); }
        set { Put("OptionSupportSortData", value); }
    }
    /// <summary>オプションサポートソートデータ.</summary>
    public ItemListSortData ItemListSortData {
        get { return Get<ItemListSortData>("ItemListSortData"); }
        set { Put("ItemListSortData", value); }
    }

    /// <summary>ジェムショップ開いたフラグ</summary>
    public bool IsOpenedGemshop {
        get { return Get<bool>("OpenedGemshop"); }
        set { Put("OpenedGemshop", value); }
    }

    public bool IsModify {
        get {
            return weaponSortData.IsModify;
        }
    }

    // コンストラクタ.
    public AwsLocalSaveData(CognitoSyncManager mng) : base(mng, "PlayerData")
    {
        LoadWeaponSortData();
        LoadUnitListSortData();
        LoadFriendSelectSortData();
        LoadOptionSupportSortData();
        LoadItemListSortData();
    }

    // 全値のリセット.
    protected override void ClearValues()
    {
		this.IsNotificateAP = true;
		this.IsNotificateBP = true;
        this.Volume_BGM = 1f;
        this.Volume_SE = 1f;
        this.Volume_Voice = 1f;
		this.Scenario_Auto = false;
        this.IsOpenedGemshop = false;
    }

    private void LoadWeaponSortData()
    {
        weaponSortData = new WeaponSortData(0, false);
        string key = "WeaponSortData";
        if (ExistKey (key)) {
            weaponSortData = Get<WeaponSortData> (key);
        } else {
            Put("WeaponSortData", weaponSortData);
        }
        weaponSortData.Commit ();
    }

    private void LoadUnitListSortData()
    {
        unitListSortData = new UnitListSortData(0, false);
        string key = "UnitListSortData";
        if (ExistKey (key)) {
            unitListSortData = Get<UnitListSortData> (key);
        } else {
            Put("UnitListSortData", unitListSortData);
        }
        unitListSortData.Commit ();
    }

    private void LoadFriendSelectSortData()
    {
        friendSelectSortData = new FriendSelectSortData(0, false, "火");
        string key = "FriendSelectSortData";
        if (ExistKey (key)) {
            friendSelectSortData = Get<FriendSelectSortData> (key);
        } else {
            Put("FriendSelectSortData", friendSelectSortData);
        }
        friendSelectSortData.Commit ();
    }

    private void LoadOptionSupportSortData()
    {
        optionSupportSortData = new OptionSupportSortData(0, false);
        string key = "OptionSupportSortData";
        if (ExistKey (key)) {
            optionSupportSortData = Get<OptionSupportSortData> (key);
        } else {
            Put("OptionSupportSortData", optionSupportSortData);
        }
        optionSupportSortData.Commit ();
    }

    private void LoadItemListSortData()
    {
        itemListSortData = new ItemListSortData(false);
        string key = "ItemListSortData";
        if (ExistKey (key)) {
            itemListSortData = Get<ItemListSortData> (key);
        } else {
            Put("ItemListSortData", itemListSortData);
        }
        itemListSortData.Commit ();
    }
}

[Serializable]
public class WeaponSortData
{
    /// <summary>
    /// ソートの種類
    /// </summary>
    public int SortType;
    /// <summary>
    /// 降順かどうか
    /// </summary>
    public bool IsDescending;

    /// <summary>
    /// 内容を持っているか？
    /// </summary>
    public bool IsHaveContents;
    /// <summary>
    /// レアリティ設定.フィルター設定しているもののリスト.
    /// </summary>
    public int[] RarityList;
    /// <summary>
    /// 武器種index設定.フィルター設定しているもののリスト.
    /// </summary>
    public int[] WeaponTypeIndexList;
    /// <summary>
    /// ロック中を表示するか.
    /// </summary>
    public bool IsVisibleLock;
    /// <summary>
    /// ロック中以外を表示するか.
    /// </summary>
    public bool IsVisibleWithoutLock;
    /// <summary>
    /// 素材武器を表示するか.
    /// </summary>
    public bool IsVisibleMaterial;


    private int backupSortType;
    private bool backupIsDescending;

    private bool backupIsHaveContents;
    private int[] backupRarityList;
    private int[] backupWeaponTypeIndexList;
    private bool backupIsVisibleLock;
    private bool backupIsVisibleWithoutLock;
    private bool backupIsVisibleMaterial;


    public void ApplyFilterData(WeaponFilterSetting.Data data)
    {
        RarityList = data.RarityList;
        WeaponTypeIndexList = data.WeaponTypeIndexList;
        IsVisibleLock = data.IsVisibleLock;
        IsVisibleWithoutLock = data.IsVisibleWithoutLock;
        IsVisibleMaterial = data.IsVisibleMaterial;
        IsHaveContents = data.IsHaveContents;

        Commit();
    }

    public void ClearFilterData()
    {
        RarityList = null;
        WeaponTypeIndexList = null;
        IsVisibleLock = false;
        IsVisibleWithoutLock = false;
        IsVisibleMaterial = false;
        IsHaveContents = false;

        Commit();
    }

    public WeaponSortData(int sortType, bool isDescending)
    {
        SortType = sortType;
        IsDescending = isDescending;

        RarityList = null;
        WeaponTypeIndexList = null;
        IsVisibleLock = false;
        IsVisibleWithoutLock = false;
        IsVisibleMaterial = false;
        IsHaveContents = false;
    }

    public bool IsModify {
        get;
        private set;
    }

    public void Reset()
    {
        SortType = backupSortType;
        IsDescending = backupIsDescending;

        RarityList = backupRarityList;
        WeaponTypeIndexList = backupWeaponTypeIndexList;
        IsVisibleLock = backupIsVisibleLock;
        IsVisibleWithoutLock = backupIsVisibleWithoutLock;
        IsVisibleMaterial = backupIsVisibleMaterial;
        IsHaveContents = backupIsHaveContents;

        IsModify = false;
    }

    public void Commit()
    {
        IsModify = false;

        backupSortType = SortType;
        backupIsDescending = IsDescending;

        backupRarityList = RarityList;
        backupWeaponTypeIndexList = WeaponTypeIndexList;
        backupIsVisibleLock = IsVisibleLock;
        backupIsVisibleWithoutLock = IsVisibleWithoutLock;
        backupIsVisibleMaterial = IsVisibleMaterial;
        backupIsHaveContents = IsHaveContents;
    }
}

[Serializable]
public class UnitListSortData
{
    /// <summary>
    /// ソートの種類
    /// </summary>
    public int SortType;
    /// <summary>
    /// 降順かどうか
    /// </summary>
    public bool IsDescending;


    private int backupSortType;
    private bool backupIsDescending;


    public UnitListSortData(int sortType, bool isDescending)
    {
        SortType = sortType;
        IsDescending = isDescending;
    }

    public bool IsModify {
        get;
        private set;
    }

    public void Reset()
    {
        SortType = backupSortType;
        IsDescending = backupIsDescending;

        IsModify = false;
    }

    public void Commit()
    {
        IsModify = false;

        backupSortType = SortType;
        backupIsDescending = IsDescending;
    }
}

[Serializable]
public class FriendSelectSortData
{
    /// <summary>
    /// ソートの種類
    /// </summary>
    public int SortType;
    /// <summary>
    /// 降順かどうか
    /// </summary>
    public bool IsDescending;
    /// <summary>
    /// 属性
    /// </summary>
    public string CategoryName;


    private int backupSortType;
    private bool backupIsDescending;
    private string backupCategoryName;


    public FriendSelectSortData(int sortType, bool isDescending, string categoryName)
    {
        SortType = sortType;
        IsDescending = isDescending;
        CategoryName = categoryName;
    }

    public bool IsModify {
        get;
        private set;
    }

    public void Reset()
    {
        SortType = backupSortType;
        IsDescending = backupIsDescending;
        CategoryName = backupCategoryName;

        IsModify = false;
    }

    public void Commit()
    {
        IsModify = false;

        backupSortType = SortType;
        backupIsDescending = IsDescending;
        backupCategoryName = CategoryName;
    }
}

[Serializable]
public class OptionSupportSortData
{
    /// <summary>
    /// ソートの種類
    /// </summary>
    public int SortType;
    /// <summary>
    /// 降順かどうか
    /// </summary>
    public bool IsDescending;


    private int backupSortType;
    private bool backupIsDescending;


    public OptionSupportSortData(int sortType, bool isDescending)
    {
        SortType = sortType;
        IsDescending = isDescending;
    }

    public bool IsModify {
        get;
        private set;
    }

    public void Reset()
    {
        SortType = backupSortType;
        IsDescending = backupIsDescending;

        IsModify = false;
    }

    public void Commit()
    {
        IsModify = false;

        backupSortType = SortType;
        backupIsDescending = IsDescending;
    }
}

[Serializable]
public class ItemListSortData
{
    /// <summary> 降順フラグ </summary>
    public bool IsDescending;

    private bool backupIsDescending;

    public ItemListSortData(bool isDescending)
    {
        IsDescending = isDescending;
    }

    public bool IsModify {
        get;
        private set;
    }

    public void Reset()
    {
        IsDescending = backupIsDescending;

        IsModify = false;
    }

    public void Commit()
    {
        IsModify = false;

        backupIsDescending = IsDescending;
    }
}