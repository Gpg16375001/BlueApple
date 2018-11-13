/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleAISettingTable : ScriptableObject
{
    [SerializeField]
    private List<BattleAISetting> _dataList;

    public List<BattleAISetting> DataList {
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
        _dataDict_ai_define_array = _dataList.GroupBy (x => x.ai_define).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<int, BattleAISetting[]> _dataDict_ai_define_array = null;
    public BattleAISetting[] this[int key]
    {
        get {
            BattleAISetting[] ret;
            _dataDict_ai_define_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleAISetting
{
    // ID
    [SerializeField]
    public int ai_define;

    // 判定優先順位
    [SerializeField]
    public int priority;

    // None
    [SerializeField]
    public SkillTargetEnum target;

    [SerializeField]
    private bool action_condition_has_value;
    [SerializeField]
    private BattleAIConditionEnum action_condition_value;
    // None
    public BattleAIConditionEnum? action_condition;

    // 行動条件値
    [SerializeField]
    public string action_condition_arg;

    [SerializeField]
    private bool condition_has_value;
    [SerializeField]
    private BattleAITargetConditionEnum condition_value;
    // None
    public BattleAITargetConditionEnum? condition;

    // 条件値
    [SerializeField]
    public string condition_arg;

    // None
    [SerializeField]
    public BattleAIActionTypeEnum action_type;

    [SerializeField]
    private bool action_selection_has_value;
    [SerializeField]
    private BattleAIActionSelectionEnum action_selection_value;
    // None
    public BattleAIActionSelectionEnum? action_selection;

    // 行動詳細指定値
    [SerializeField]
    public string action_selection_arg;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        action_condition = null;
        if(action_condition_has_value) {
            action_condition = action_condition_value;
        }
        condition = null;
        if(condition_has_value) {
            condition = condition_value;
        }
        action_selection = null;
        if(action_selection_has_value) {
            action_selection = action_selection_value;
        }
        InitExtension();
    }
}
