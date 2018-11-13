/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleAIDefineTable : ScriptableObject
{
    [SerializeField]
    private List<BattleAIDefine> _dataList;

    public List<BattleAIDefine> DataList {
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

    private Dictionary<int, BattleAIDefine> _dataDict = null;
    public BattleAIDefine this[int key]
    {
        get {
            BattleAIDefine ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleAIDefine
{
    // ID
    [SerializeField]
    public int id;

    // ai識別名
    [SerializeField]
    public string name;

    // ターゲット逆転
    [SerializeField]
    public bool target_reversal;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
