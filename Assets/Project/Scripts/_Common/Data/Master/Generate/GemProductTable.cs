/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GemProductTable : ScriptableObject
{
    [SerializeField]
    private List<GemProduct> _dataList;

    public List<GemProduct> DataList {
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

    private Dictionary<int, GemProduct> _dataDict = null;
    public GemProduct this[int key]
    {
        get {
            GemProduct ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class GemProduct
{
    // 商品ID
    [SerializeField]
    public int id;

    // 配布有償石
    [SerializeField]
    public int gem_count;

    // 価格
    [SerializeField]
    public int price;

    // 商品名
    [SerializeField]
    public string description;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
