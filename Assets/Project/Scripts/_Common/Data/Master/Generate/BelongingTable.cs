/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BelongingTable : ScriptableObject
{
    [SerializeField]
    private List<Belonging> _dataList;

    public List<Belonging> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.Enum);
        InitExtension();
    }

    private Dictionary<BelongingEnum, Belonging> _dataDict = null;
    public Belonging this[BelongingEnum key]
    {
        get {
            Belonging ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Belonging
{
    // 名前
    [SerializeField]
    public string name;

    // None
    [SerializeField]
    public BelongingEnum Enum;

    // 表示順
    [SerializeField]
    public int priority_view;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
