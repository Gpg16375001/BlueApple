/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ConditionTable : ScriptableObject
{
    [SerializeField]
    private List<Condition> _dataList;

    public List<Condition> DataList {
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

    private Dictionary<int, Condition> _dataDict = null;
    public Condition this[int key]
    {
        get {
            Condition ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Condition
{
    // ID
    [SerializeField]
    public int id;

    // 状態名
    [SerializeField]
    public string name;

    // デバフフラグ
    [SerializeField]
    public bool is_debuff;

    // 状態タイプ
    [SerializeField]
    public ConditionTypeEnum condition_type;

    [SerializeField]
    private bool condition_group_has_value;
    [SerializeField]
    private int condition_group_value;
    // グループID
    public int? condition_group;

    // 状態グループ内での優先順位
    [SerializeField]
    public int condition_group_priority;

    // Wave切替時持ち越しするか
    [SerializeField]
    public bool is_carryover;

    // 効果期間
    [SerializeField]
    public int period;

    // 効果期間(手番数)
    [SerializeField]
    public int turn_count;

    [SerializeField]
    private bool within_skill_id_has_value;
    [SerializeField]
    private int within_skill_id_value;
    // スキル識別ID
    public int? within_skill_id;

    // 発動タイミング
    [SerializeField]
    public int invoke_timing;

    [SerializeField]
    private bool timing_skill_id_has_value;
    [SerializeField]
    private int timing_skill_id_value;
    // スキル識別ID
    public int? timing_skill_id;

    // 行動不可
    [SerializeField]
    public bool is_no_action;

    // 行動不可時次の行動までの重さ
    [SerializeField]
    public int no_action_wait;

    // 行動毎の解除確率
    [SerializeField]
    public int action_calling_off_probability;

    // ダメージ時の解除確率
    [SerializeField]
    public int damage_calling_off_probability;

    // 最大発動回数
    [SerializeField]
    public int invoke_count;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        condition_group = null;
        if(condition_group_has_value) {
            condition_group = condition_group_value;
        }
        within_skill_id = null;
        if(within_skill_id_has_value) {
            within_skill_id = within_skill_id_value;
        }
        timing_skill_id = null;
        if(timing_skill_id_has_value) {
            timing_skill_id = timing_skill_id_value;
        }
        InitExtension();
    }
}
