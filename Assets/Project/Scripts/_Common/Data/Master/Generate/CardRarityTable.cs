/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardRarityTable : ScriptableObject
{
    [SerializeField]
    private List<CardRarity> _dataList;

    public List<CardRarity> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.rarity);
        InitExtension();
    }

    private Dictionary<int, CardRarity> _dataDict = null;
    public CardRarity this[int key]
    {
        get {
            CardRarity ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardRarity
{
    // レアリティ
    [SerializeField]
    public int rarity;

    // このレアリティでの最大レベル
    [SerializeField]
    public int max_level;

    // このレアリティのパラメータ計算時の係数
    [SerializeField]
    public int coefficient;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
