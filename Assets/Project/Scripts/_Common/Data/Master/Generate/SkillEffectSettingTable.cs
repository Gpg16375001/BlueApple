/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillEffectSettingTable : ScriptableObject
{
    [SerializeField]
    private List<SkillEffectSetting> _dataList;

    public List<SkillEffectSetting> DataList {
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
public partial class SkillEffectSetting
{
    // スキル識別ID
    [SerializeField]
    public int skill_id;

    [SerializeField]
    private int _skill_effect;
    // None
    public SkillEffect skill_effect;

    [SerializeField]
    private int _range;
    // スキル効果範囲
    public SkillRange range;

    // スキル演出パターン
    [SerializeField]
    public int performance_pattern_1;

    // スキル演出パターン
    [SerializeField]
    public int performance_pattern_2;

    // スキル演出パターン
    [SerializeField]
    public int performance_pattern_3;

    // スキル演出パターン
    [SerializeField]
    public int performance_pattern_4;

    // スキル演出待ち時間
    [SerializeField]
    public float performance_wait_time_1_2;

    // スキル演出待ち時間
    [SerializeField]
    public float performance_wait_time_2_3;

    // スキル演出待ち時間
    [SerializeField]
    public float performance_wait_time_3_4;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        skill_effect = MasterDataTable.skill_effect.DataList.First (x => x.id == _skill_effect);
        range = MasterDataTable.skill_range.DataList.First (x => x.id == _range);
        InitExtension();
    }
}
