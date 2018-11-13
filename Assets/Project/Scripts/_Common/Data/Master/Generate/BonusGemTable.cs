/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BonusGemTable : ScriptableObject
{
    [SerializeField]
    private List<BonusGem> _dataList;

    public List<BonusGem> DataList {
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

    private Dictionary<int, BonusGem> _dataDict = null;
    public BonusGem this[int key]
    {
        get {
            BonusGem ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BonusGem
{
    // ユニークID
    [SerializeField]
    public int id;

    // 商品ID
    [SerializeField]
    public int product_id;

    // 配布ボーナス石
    [SerializeField]
    public int gem_count;

    // キャッチテキスト
    [SerializeField]
    public string catch_copy;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
