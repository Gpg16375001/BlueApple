/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardLive2DAnchorSettingTable : ScriptableObject
{
    [SerializeField]
    private List<CardLive2DAnchorSetting> _dataList;

    public List<CardLive2DAnchorSetting> DataList {
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

    private Dictionary<int, CardLive2DAnchorSetting> _dataDict = null;
    public CardLive2DAnchorSetting this[int key]
    {
        get {
            CardLive2DAnchorSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardLive2DAnchorSetting
{
    // カードID
    [SerializeField]
    public int id;

    // x座標
    [SerializeField]
    public int x;

    // y座標
    [SerializeField]
    public int y;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
