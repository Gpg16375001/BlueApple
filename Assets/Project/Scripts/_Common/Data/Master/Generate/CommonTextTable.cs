/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CommonTextTable : ScriptableObject
{
    [SerializeField]
    private List<CommonText> _dataList;

    public List<CommonText> DataList {
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

    private Dictionary<string, CommonText> _dataDict = null;
    public CommonText this[string key]
    {
        get {
            CommonText ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CommonText
{
    // 参照用キーワード
    [SerializeField]
    public string Keyword;

    // 出力文言
    [SerializeField]
    public string Text;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
