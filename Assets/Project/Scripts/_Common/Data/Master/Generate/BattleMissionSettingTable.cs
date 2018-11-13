/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleMissionSettingTable : ScriptableObject
{
    [SerializeField]
    private List<BattleMissionSetting> _dataList;

    public List<BattleMissionSetting> DataList {
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

        InitExtension();
    }


}

[Serializable]
public partial class BattleMissionSetting
{
    // ミッションセットID
    [SerializeField]
    public int id;

    // ステージID
    [SerializeField]
    public int stage_id;

    [SerializeField]
    private bool condition_1_has_value;
    [SerializeField]
    private int condition_1_value;
    // 条件ID
    public int? condition_1;

    // ミッション1の変数
    [SerializeField]
    public int variable_1;

    [SerializeField]
    private bool condition_2_has_value;
    [SerializeField]
    private int condition_2_value;
    // 条件ID
    public int? condition_2;

    // ミッション2の変数
    [SerializeField]
    public int variable_2;

    [SerializeField]
    private bool condition_3_has_value;
    [SerializeField]
    private int condition_3_value;
    // 条件ID
    public int? condition_3;

    // ミッション3の変数
    [SerializeField]
    public int variable_3;

    [SerializeField]
    private bool item_type_1_has_value;
    [SerializeField]
    private ItemTypeEnum item_type_1_value;
    // 1つ目の報酬のアイテムタイプ
    public ItemTypeEnum? item_type_1;

    // 1つ目の報酬のアイテムID
    [SerializeField]
    public int item_id_1;

    // 1つ目の報酬のアイテム個数
    [SerializeField]
    public int item_quantity_1;

    [SerializeField]
    private bool item_type_2_has_value;
    [SerializeField]
    private ItemTypeEnum item_type_2_value;
    // 2つ目の報酬のアイテムタイプ
    public ItemTypeEnum? item_type_2;

    // 2つ目の報酬のアイテムID
    [SerializeField]
    public int item_id_2;

    // 2つ目の報酬のアイテム個数
    [SerializeField]
    public int item_quantity_2;

    [SerializeField]
    private bool item_type_3_has_value;
    [SerializeField]
    private ItemTypeEnum item_type_3_value;
    // 3つ目の報酬のアイテムタイプ
    public ItemTypeEnum? item_type_3;

    // 3つ目の報酬のアイテムID
    [SerializeField]
    public int item_id_3;

    // 3つ目の報酬のアイテム個数
    [SerializeField]
    public int item_quantity_3;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        condition_1 = null;
        if(condition_1_has_value) {
            condition_1 = condition_1_value;
        }
        condition_2 = null;
        if(condition_2_has_value) {
            condition_2 = condition_2_value;
        }
        condition_3 = null;
        if(condition_3_has_value) {
            condition_3 = condition_3_value;
        }
        item_type_1 = null;
        if(item_type_1_has_value) {
            item_type_1 = item_type_1_value;
        }
        item_type_2 = null;
        if(item_type_2_has_value) {
            item_type_2 = item_type_2_value;
        }
        item_type_3 = null;
        if(item_type_3_has_value) {
            item_type_3 = item_type_3_value;
        }
        InitExtension();
    }
}
