/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CommonDefineTable : ScriptableObject
{
    [SerializeField]
    private List<CommonDefine> _dataList;

    public List<CommonDefine> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.Keyword);
        InitExtension();
    }

    private Dictionary<string, CommonDefine> _dataDict = null;
    public CommonDefine this[string key]
    {
        get {
            CommonDefine ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CommonDefine
{
    // 参照用キーワード
    [SerializeField]
    public string Keyword;

    // 定数値
    [SerializeField]
    public int define_value;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
