/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleMissionConditionDefineTable : ScriptableObject
{
    [SerializeField]
    private List<BattleMissionConditionDefine> _dataList;

    public List<BattleMissionConditionDefine> DataList {
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

    private Dictionary<int, BattleMissionConditionDefine> _dataDict = null;
    public BattleMissionConditionDefine this[int key]
    {
        get {
            BattleMissionConditionDefine ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleMissionConditionDefine
{
    // 条件ID
    [SerializeField]
    public int id;

    // 条件名
    [SerializeField]
    public string name;

    // 説明文テンプレート.数値を含んで条件説明する.
    [SerializeField]
    public string text_detail;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
