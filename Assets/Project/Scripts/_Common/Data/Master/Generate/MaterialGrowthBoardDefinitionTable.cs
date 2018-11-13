/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MaterialGrowthBoardDefinitionTable : ScriptableObject
{
    [SerializeField]
    private List<MaterialGrowthBoardDefinition> _dataList;

    public List<MaterialGrowthBoardDefinition> DataList {
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
        _dataDict = _dataList.ToDictionary (x => new UniRx.Tuple<int, int>(x.pattern_id, x.board_index));
        InitExtension();
    }

    private Dictionary<UniRx.Tuple<int, int>, MaterialGrowthBoardDefinition> _dataDict = null;
    public MaterialGrowthBoardDefinition this[int pattern_id, int board_index]
    {
        get {
            MaterialGrowthBoardDefinition ret;
            var key = new UniRx.Tuple<int, int>(pattern_id, board_index);
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MaterialGrowthBoardDefinition
{
    // 育成ボードパターンID
    [SerializeField]
    public int pattern_id;

    // ボード番号
    [SerializeField]
    public int board_index;

    // スロット数
    [SerializeField]
    public int total_slot_number;

    // 限界突破可能スロット開放数
    [SerializeField]
    public int unlocked_slot_number_to_limit_break;

    [SerializeField]
    private bool limit_break_item_combination_id_has_value;
    [SerializeField]
    private int limit_break_item_combination_id_value;
    // 限界突破必要アイテム組合せID
    public int? limit_break_item_combination_id;

    // 育成ボード形状Prefab指定
    [SerializeField]
    public string board_prefab_name;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        limit_break_item_combination_id = null;
        if(limit_break_item_combination_id_has_value) {
            limit_break_item_combination_id = limit_break_item_combination_id_value;
        }
        InitExtension();
    }
}
