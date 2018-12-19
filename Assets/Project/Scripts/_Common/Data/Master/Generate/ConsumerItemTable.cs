/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ConsumerItemTable : ScriptableObject
{
    [SerializeField]
    private List<ConsumerItem> _dataList;

    public List<ConsumerItem> DataList {
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

    private Dictionary<int, ConsumerItem> _dataDict = null;
    public ConsumerItem this[int key]
    {
        get {
            ConsumerItem ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ConsumerItem
{
    // ID
    [SerializeField]
    public int index;

    // 名前
    [SerializeField]
    public string name;

    // サブタイプ
    [SerializeField]
    public string sub_type;

    // 説明テキスト
    [SerializeField]
    public string flavor_text;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
