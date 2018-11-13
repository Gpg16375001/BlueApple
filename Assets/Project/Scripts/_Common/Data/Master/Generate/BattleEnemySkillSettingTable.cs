/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleEnemySkillSettingTable : ScriptableObject
{
    [SerializeField]
    private List<BattleEnemySkillSetting> _dataList;

    public List<BattleEnemySkillSetting> DataList {
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
public partial class BattleEnemySkillSetting
{
    // グループID
    [SerializeField]
    public int group_id;

    [SerializeField]
    private int _skill;
    // None
    public Skill skill;

    // スキルレベル
    [SerializeField]
    public int skill_level;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        skill = MasterDataTable.skill.DataList.First (x => x.id == _skill);
        InitExtension();
    }
}
