/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EnemyUnitSizeDefineTable : ScriptableObject
{
    [SerializeField]
    private List<EnemyUnitSizeDefine> _dataList;

    public List<EnemyUnitSizeDefine> DataList {
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

    private Dictionary<int, EnemyUnitSizeDefine> _dataDict = null;
    public EnemyUnitSizeDefine this[int key]
    {
        get {
            EnemyUnitSizeDefine ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EnemyUnitSizeDefine
{
    // ID
    [SerializeField]
    public int id;

    // None
    [SerializeField]
    public EnemyUnitSizePositionEnum ColumnPosition;

    // None
    [SerializeField]
    public EnemyUnitSizePositionEnum RowPosition;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
