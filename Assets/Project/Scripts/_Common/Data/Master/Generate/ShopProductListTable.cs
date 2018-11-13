/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ShopProductListTable : ScriptableObject
{
    [SerializeField]
    private List<ShopProductList> _dataList;

    public List<ShopProductList> DataList {
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

    private Dictionary<int, ShopProductList> _dataDict = null;
    public ShopProductList this[int key]
    {
        get {
            ShopProductList ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ShopProductList
{
    // 商品セットID
    [SerializeField]
    public int id;

    // 商品セットの名前.ショップ内で並ぶ際は別途ショップテーブルで定義した商品名を使用する.商品セット識別用の名前.
    [SerializeField]
    public string name_set;

    // 販売アイテム1のカテゴリー名
    [SerializeField]
    public string category_name1;

    // 販売アイテム1のID
    [SerializeField]
    public int item_id_1;

    // 販売アイテム1の販売個数
    [SerializeField]
    public int count_1;

    // 販売アイテム2のカテゴリー名
    [SerializeField]
    public string category_name2;

    // 販売アイテム2のID
    [SerializeField]
    public int item_id_2;

    // 販売アイテム2の販売個数
    [SerializeField]
    public int count_2;

    // 販売アイテム3のカテゴリー名
    [SerializeField]
    public string category_name3;

    // 販売アイテム3のID
    [SerializeField]
    public int item_id_3;

    // 販売アイテム3の販売個数
    [SerializeField]
    public int count_3;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
