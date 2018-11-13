/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ElementAffinityTable : ScriptableObject
{
    [SerializeField]
    private List<ElementAffinity> _dataList;

    public List<ElementAffinity> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.Enum);
        InitExtension();
    }

    private Dictionary<ElementAffinityEnum, ElementAffinity> _dataDict = null;
    public ElementAffinity this[ElementAffinityEnum key]
    {
        get {
            ElementAffinity ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ElementAffinity
{
    // None
    [SerializeField]
    public ElementAffinityEnum Enum;

    // 名前
    [SerializeField]
    public string name;

    // 補正値
    [SerializeField]
    public int correction_value;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
