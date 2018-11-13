/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GachaProductTable : ScriptableObject
{
    [SerializeField]
    private List<GachaProduct> _dataList;

    public List<GachaProduct> DataList {
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

    private Dictionary<int, GachaProduct> _dataDict = null;
    public GachaProduct this[int key]
    {
        get {
            GachaProduct ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class GachaProduct
{
    // ユニークID
    [SerializeField]
    public int id;

    // 商品名
    [SerializeField]
    public string product_name;

    // ガチャID
    [SerializeField]
    public int gacha_id;

    // 詳細説明が載ってるurl
    [SerializeField]
    public string url_description;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
