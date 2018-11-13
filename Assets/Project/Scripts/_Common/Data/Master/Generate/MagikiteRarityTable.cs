/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MagikiteRarityTable : ScriptableObject
{
    [SerializeField]
    private List<MagikiteRarity> _dataList;

    public List<MagikiteRarity> DataList {
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

    private Dictionary<int, MagikiteRarity> _dataDict = null;
    public MagikiteRarity this[int key]
    {
        get {
            MagikiteRarity ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MagikiteRarity
{
    // レアリティ
    [SerializeField]
    public int rarity;

    // 売却価格
    [SerializeField]
    public int price;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
