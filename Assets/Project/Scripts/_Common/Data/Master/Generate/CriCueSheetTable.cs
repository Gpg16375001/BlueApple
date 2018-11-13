/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CriCueSheetTable : ScriptableObject
{
    [SerializeField]
    private List<CriCueSheet> _dataList;

    public List<CriCueSheet> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.CueSheetName);
        InitExtension();
    }

    private Dictionary<string, CriCueSheet> _dataDict = null;
    public CriCueSheet this[string key]
    {
        get {
            CriCueSheet ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CriCueSheet
{
    // キューシート名定義
    [SerializeField]
    public string CueSheetName;

    // ACBファイル名指定
    [SerializeField]
    public string ACBFileName;

    // AWBファイル名指定
    [SerializeField]
    public string AWBFileName;

    // ストリーミングアセットかどうか
    [SerializeField]
    public bool isStreamingAssets;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
