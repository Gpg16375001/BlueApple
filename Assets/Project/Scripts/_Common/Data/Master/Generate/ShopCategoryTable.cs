/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ShopCategoryTable : ScriptableObject
{
    [SerializeField]
    private List<ShopCategory> _dataList;

    public List<ShopCategory> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.index);
        InitExtension();
    }

    private Dictionary<int, ShopCategory> _dataDict = null;
    public ShopCategory this[int key]
    {
        get {
            ShopCategory ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ShopCategory
{
    // ID
    [SerializeField]
    public int index;

    // 名前
    [SerializeField]
    public string name;

    [SerializeField]
    private int _use_item_type;
    // None
    public ItemType use_item_type;

    // ショップ説明文言
    [SerializeField]
    public string notes;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        use_item_type = MasterDataTable.item_type.DataList.First (x => x.index == _use_item_type);
        InitExtension();
    }
}
