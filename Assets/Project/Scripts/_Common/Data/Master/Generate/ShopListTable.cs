/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ShopListTable : ScriptableObject
{
    [SerializeField]
    private List<ShopList> _dataList;

    public List<ShopList> DataList {
        get {
            return _dataList;
        }
    }

    void OnEnable()
    {
        Init ();
    }

    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    void Init()
    {
        for (int i = 0; i < _dataList.Count; ++i) {
            _dataList [i].Init ();
        }
        _dataDict = _dataList.ToDictionary (x => x.id);
        InitExtension();
    }

    private Dictionary<int, ShopList> _dataDict = null;
    public ShopList this[int key]
    {
        get {
            ShopList ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ShopList
{
    // ショップアイテムID
    [SerializeField]
    public int id;

    [SerializeField]
    private int _product;
    // 商品セット情報
    public ShopProductList product;

    // 商品名称
    [SerializeField]
    public string product_name;

    // 商品を説明する文
    [SerializeField]
    public string explanatory_text;

    [SerializeField]
    private int _shop_category;
    // ジェムで販売しているショップなどのカテゴリー
    public ShopCategory shop_category;

    // 購入に使用する通貨アイテムID.ジェムなどカテゴリーだけで判断できる場合は使用しない.
    [SerializeField]
    public int use_item_id;

    // 購入に使用する通貨アイテムの個数.
    [SerializeField]
    public int use_item_count;

    [SerializeField]
    private int _limitaion;
    // 1日1回のみなどの購入制限情報
    public PurchaseLimitation limitaion;

	[SerializeField]
    private string start_date_value;
    // 開始日時
    public DateTime start_date;

	[SerializeField]
    private string end_date_value;
    // 終了日時
    public DateTime end_date;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        product = MasterDataTable.shop_product_list.DataList.First (x => x.id == _product);
        shop_category = MasterDataTable.shop_category.DataList.First (x => x.index == _shop_category);
        limitaion = MasterDataTable.purchase_limitation.DataList.First (x => x.index == _limitaion);
       start_date = DateTime.Parse(start_date_value);
       end_date = DateTime.Parse(end_date_value);
        InitExtension();
    }
}
