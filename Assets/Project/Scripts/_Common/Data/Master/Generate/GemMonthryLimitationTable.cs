/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GemMonthryLimitationTable : ScriptableObject
{
    [SerializeField]
    private List<GemMonthryLimitation> _dataList;

    public List<GemMonthryLimitation> DataList {
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

    private Dictionary<int, GemMonthryLimitation> _dataDict = null;
    public GemMonthryLimitation this[int key]
    {
        get {
            GemMonthryLimitation ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class GemMonthryLimitation
{
    // ユニークID
    [SerializeField]
    public int id;

    // 指定年齢未満に適用
    [SerializeField]
    public int age;

    // 制限課金額
    [SerializeField]
    public int limitation;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
