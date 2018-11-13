/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardGrowthBoardSettingTable : ScriptableObject
{
    [SerializeField]
    private List<CardGrowthBoardSetting> _dataList;

    public List<CardGrowthBoardSetting> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.card_id);
        InitExtension();
    }

    private Dictionary<int, CardGrowthBoardSetting> _dataDict = null;
    public CardGrowthBoardSetting this[int key]
    {
        get {
            CardGrowthBoardSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardGrowthBoardSetting
{
    // カードID
    [SerializeField]
    public int card_id;

    // 育成ボードパターン
    [SerializeField]
    public int growth_board_pattern_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
