/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ItemTypeTable : ScriptableObject
{
    [SerializeField]
    private List<ItemType> _dataList;

    public List<ItemType> DataList {
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

    private Dictionary<int, ItemType> _dataDict = null;
    public ItemType this[int key]
    {
        get {
            ItemType ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ItemType
{
    // ID
    [SerializeField]
    public int index;

    // 名前
    [SerializeField]
    public string name;

    // 表示名
    [SerializeField]
    public string display_name;

    // None
    [SerializeField]
    public ItemTypeEnum Enum;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
