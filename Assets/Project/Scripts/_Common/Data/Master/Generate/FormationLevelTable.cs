/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class FormationLevelTable : ScriptableObject
{
    [SerializeField]
    private List<FormationLevel> _dataList;

    public List<FormationLevel> DataList {
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
public partial class FormationLevel
{
    // 陣形レベルテーブルID
    [SerializeField]
    public int level_table_id;

    // レベル
    [SerializeField]
    public int level;

    // レベルアップコスト
    [SerializeField]
    public int levelup_cost;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
