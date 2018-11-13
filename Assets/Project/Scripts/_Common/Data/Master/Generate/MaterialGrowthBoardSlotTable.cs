/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MaterialGrowthBoardSlotTable : ScriptableObject
{
    [SerializeField]
    private List<MaterialGrowthBoardSlot> _dataList;

    public List<MaterialGrowthBoardSlot> DataList {
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
        _dataDict = _dataList.ToDictionary (x => new UniRx.Tuple<int, int, int>(x.pattern_id, x.board_index, x.slot_index));
        InitExtension();
    }

    private Dictionary<UniRx.Tuple<int, int, int>, MaterialGrowthBoardSlot> _dataDict = null;
    public MaterialGrowthBoardSlot this[int pattern_id, int board_index, int slot_index]
    {
        get {
            MaterialGrowthBoardSlot ret;
            var key = new UniRx.Tuple<int, int, int>(pattern_id, board_index, slot_index);
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MaterialGrowthBoardSlot
{
    // 育成ボードパターンID
    [SerializeField]
    public int pattern_id;

    // ボード番号
    [SerializeField]
    public int board_index;

    // スロット番号
    [SerializeField]
    public int slot_index;

    [SerializeField]
    private int _item_combination;
    // None
    public MaterialGrowthBoardItemCombination item_combination;

    // None
    [SerializeField]
    public MaterialGrowthBoardParameterTypeEnum parameter_type;

    // 強化値
    [SerializeField]
    public int parameter_value;

    [SerializeField]
    private bool release_conditoin1_has_value;
    [SerializeField]
    private int release_conditoin1_value;
    // 解放条件1
    public int? release_conditoin1;

    [SerializeField]
    private bool release_conditoin2_has_value;
    [SerializeField]
    private int release_conditoin2_value;
    // 解放条件2
    public int? release_conditoin2;

    [SerializeField]
    private bool release_conditoin3_has_value;
    [SerializeField]
    private int release_conditoin3_value;
    // 解放条件3
    public int? release_conditoin3;

    [SerializeField]
    private bool release_conditoin4_has_value;
    [SerializeField]
    private int release_conditoin4_value;
    // 解放条件4
    public int? release_conditoin4;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        item_combination = MasterDataTable.material_growth_board_item_combination.DataList.First (x => x.id == _item_combination);
        release_conditoin1 = null;
        if(release_conditoin1_has_value) {
            release_conditoin1 = release_conditoin1_value;
        }
        release_conditoin2 = null;
        if(release_conditoin2_has_value) {
            release_conditoin2 = release_conditoin2_value;
        }
        release_conditoin3 = null;
        if(release_conditoin3_has_value) {
            release_conditoin3 = release_conditoin3_value;
        }
        release_conditoin4 = null;
        if(release_conditoin4_has_value) {
            release_conditoin4 = release_conditoin4_value;
        }
        InitExtension();
    }
}
