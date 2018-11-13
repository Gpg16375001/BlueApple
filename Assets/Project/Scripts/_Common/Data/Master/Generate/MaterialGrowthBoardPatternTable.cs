/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MaterialGrowthBoardPatternTable : ScriptableObject
{
    [SerializeField]
    private List<MaterialGrowthBoardPattern> _dataList;

    public List<MaterialGrowthBoardPattern> DataList {
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

    private Dictionary<int, MaterialGrowthBoardPattern> _dataDict = null;
    public MaterialGrowthBoardPattern this[int key]
    {
        get {
            MaterialGrowthBoardPattern ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MaterialGrowthBoardPattern
{
    // 育成ボードパターンID
    [SerializeField]
    public int id;

    // ボード総数
    [SerializeField]
    public int total_board_number;

    // 初期ボード数
    [SerializeField]
    public int initial_board_number;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
