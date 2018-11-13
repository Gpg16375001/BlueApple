/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MaterialGrowthBoardParameterTypeTable : ScriptableObject
{
    [SerializeField]
    private List<MaterialGrowthBoardParameterType> _dataList;

    public List<MaterialGrowthBoardParameterType> DataList {
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

    private Dictionary<MaterialGrowthBoardParameterTypeEnum, MaterialGrowthBoardParameterType> _dataDict = null;
    public MaterialGrowthBoardParameterType this[MaterialGrowthBoardParameterTypeEnum key]
    {
        get {
            MaterialGrowthBoardParameterType ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MaterialGrowthBoardParameterType
{
    // None
    [SerializeField]
    public MaterialGrowthBoardParameterTypeEnum Enum;

    // スロット表示設定
    [SerializeField]
    public string slot_display;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
