/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GachaTable : ScriptableObject
{
    [SerializeField]
    private List<Gacha> _dataList;

    public List<Gacha> DataList {
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

    private Dictionary<int, Gacha> _dataDict = null;
    public Gacha this[int key]
    {
        get {
            Gacha ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Gacha
{
    // ガチャID
    [SerializeField]
    public int index;

    // ガチャ名
    [SerializeField]
    public string name;

    [SerializeField]
    private string _type;
    // ガチャカテゴリータイプ
    public GachaType type;

    // 表示優先度
    [SerializeField]
    public int priority_view;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        type = MasterDataTable.gacha_type.DataList.First (x => x.name == _type);
        InitExtension();
    }
}
