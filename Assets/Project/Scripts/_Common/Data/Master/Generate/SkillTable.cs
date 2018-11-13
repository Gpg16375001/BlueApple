/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillTable : ScriptableObject
{
    [SerializeField]
    private List<Skill> _dataList;

    public List<Skill> DataList {
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

    private Dictionary<int, Skill> _dataDict = null;
    public Skill this[int key]
    {
        get {
            Skill ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Skill
{
    // スキル識別ID
    [SerializeField]
    public int id;

    // スキル表示名
    [SerializeField]
    public string display_name;

    // None
    [SerializeField]
    public SkillTypeEnum skill_type;

    // 通常攻撃を示すフラグ
    [SerializeField]
    public bool is_normal_action;

    [SerializeField]
    private bool attack_element_has_value;
    [SerializeField]
    private AttackElementEnum attack_element_value;
    // None
    public AttackElementEnum? attack_element;

    // None
    [SerializeField]
    public AttackRangeEnum attack_range;

    // 重さ
    [SerializeField]
    public int weight;

    // チャージタイム
    [SerializeField]
    public int charge_time;

    // 最大レベル
    [SerializeField]
    public int max_level;

    // 再生アニメイタートリガー
    [SerializeField]
    public string play_animator_trigger;

    // 攻撃パターン番号
    [SerializeField]
    public int attack_pattern_index;

    // 演出パターン
    [SerializeField]
    public int performance_pattern;

    // フレーバー
    [SerializeField]
    public string flavor;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        attack_element = null;
        if(attack_element_has_value) {
            attack_element = attack_element_value;
        }
        InitExtension();
    }
}
