/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MaterialGrowthBoardItemCombinationTable : ScriptableObject
{
    [SerializeField]
    private List<MaterialGrowthBoardItemCombination> _dataList;

    public List<MaterialGrowthBoardItemCombination> DataList {
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

    private Dictionary<int, MaterialGrowthBoardItemCombination> _dataDict = null;
    public MaterialGrowthBoardItemCombination this[int key]
    {
        get {
            MaterialGrowthBoardItemCombination ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MaterialGrowthBoardItemCombination
{
    // 組合せID
    [SerializeField]
    public int id;

    // ジェムのみで開放する場合の費用
    [SerializeField]
    public int gem_quantity;

    // 素材で開放する場合の費用（クレド）
    [SerializeField]
    public int cost;

    [SerializeField]
    private bool material_type_1_has_value;
    [SerializeField]
    private CharaMaterialTypeEnum material_type_1_value;
    // None
    public CharaMaterialTypeEnum? material_type_1;

    // 素材レアリティ1
    [SerializeField]
    public int rarity_1;

    // 素材個数1
    [SerializeField]
    public int count_1;

    [SerializeField]
    private bool material_type_2_has_value;
    [SerializeField]
    private CharaMaterialTypeEnum material_type_2_value;
    // None
    public CharaMaterialTypeEnum? material_type_2;

    // 素材レアリティ2
    [SerializeField]
    public int rarity_2;

    // 素材個数2
    [SerializeField]
    public int count_2;

    [SerializeField]
    private bool material_type_3_has_value;
    [SerializeField]
    private CharaMaterialTypeEnum material_type_3_value;
    // None
    public CharaMaterialTypeEnum? material_type_3;

    // 素材レアリティ3
    [SerializeField]
    public int rarity_3;

    // 素材個数3
    [SerializeField]
    public int count_3;

    [SerializeField]
    private bool material_type_4_has_value;
    [SerializeField]
    private CharaMaterialTypeEnum material_type_4_value;
    // None
    public CharaMaterialTypeEnum? material_type_4;

    // 素材レアリティ4
    [SerializeField]
    public int rarity_4;

    // 素材個数4
    [SerializeField]
    public int count_4;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        material_type_1 = null;
        if(material_type_1_has_value) {
            material_type_1 = material_type_1_value;
        }
        material_type_2 = null;
        if(material_type_2_has_value) {
            material_type_2 = material_type_2_value;
        }
        material_type_3 = null;
        if(material_type_3_has_value) {
            material_type_3 = material_type_3_value;
        }
        material_type_4 = null;
        if(material_type_4_has_value) {
            material_type_4 = material_type_4_value;
        }
        InitExtension();
    }
}
