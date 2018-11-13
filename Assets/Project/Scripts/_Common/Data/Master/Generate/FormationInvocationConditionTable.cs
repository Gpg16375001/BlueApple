/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class FormationInvocationConditionTable : ScriptableObject
{
    [SerializeField]
    private List<FormationInvocationCondition> _dataList;

    public List<FormationInvocationCondition> DataList {
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
public partial class FormationInvocationCondition
{
    // 陣形ユニークID
    [SerializeField]
    public int formation_id;

    // ポジション
    [SerializeField]
    public int position;

    // None
    [SerializeField]
    public FormationInvocationConditionEnum condition;

    // 条件値
    [SerializeField]
    public string condition_value;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
