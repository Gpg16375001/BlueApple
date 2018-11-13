/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EnemyUnitSizeTable : ScriptableObject
{
    [SerializeField]
    private List<EnemyUnitSize> _dataList;

    public List<EnemyUnitSize> DataList {
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
        _dataDict_size_id_array = _dataList.GroupBy (x => x.size_id).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<int, EnemyUnitSize[]> _dataDict_size_id_array = null;
    public EnemyUnitSize[] this[int key]
    {
        get {
            EnemyUnitSize[] ret;
            _dataDict_size_id_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EnemyUnitSize
{
    // ID
    [SerializeField]
    public int size_id;

    // Row
    [SerializeField]
    public int row;

    // Column
    [SerializeField]
    public int column;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
