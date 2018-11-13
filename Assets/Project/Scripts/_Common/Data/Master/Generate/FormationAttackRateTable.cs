/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class FormationAttackRateTable : ScriptableObject
{
    [SerializeField]
    private List<FormationAttackRate> _dataList;

    public List<FormationAttackRate> DataList {
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
public partial class FormationAttackRate
{
    // 前列数
    [SerializeField]
    public int number_of_front;

    // 物理攻撃倍率(百分率)
    [SerializeField]
    public int physics_attack_rate;

    // 魔法攻撃倍率(百分率)
    [SerializeField]
    public int magic_attack_rate;

    // 防御倍率(百分率)
    [SerializeField]
    public int defense_rate;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
