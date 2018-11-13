/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SubQuestNameTable : ScriptableObject
{
    [SerializeField]
    private List<SubQuestName> _dataList;

    public List<SubQuestName> DataList {
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

    private Dictionary<int, SubQuestName> _dataDict = null;
    public SubQuestName this[int key]
    {
        get {
            SubQuestName ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SubQuestName
{
    // サブクエスト識別用連番Index
    [SerializeField]
    public int index;

    // サブクエストの名前
    [SerializeField]
    public string name;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
