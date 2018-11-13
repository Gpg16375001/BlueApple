/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillRangeTable : ScriptableObject
{
    [SerializeField]
    private List<SkillRange> _dataList;

    public List<SkillRange> DataList {
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
public partial class SkillRange
{
    // スキル範囲ID
    [SerializeField]
    public int id;

    // None
    [SerializeField]
    public SkillTargetEnum target;

    // None
    [SerializeField]
    public int range_setting;

    // 範囲内ランダム最小選択数
    [SerializeField]
    public int random_min_target;

    // 範囲内ランダム最大選択数
    [SerializeField]
    public int random_max_target;

    // ターゲット重複あり
    [SerializeField]
    public bool is_duplication_target;

    [SerializeField]
    private int _element_target;
    // None
    public SkillElementTargetSetting element_target;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        element_target = MasterDataTable.skill_element_target_setting.DataList.FirstOrDefault (x => x.id == _element_target);
        InitExtension();
    }
}
