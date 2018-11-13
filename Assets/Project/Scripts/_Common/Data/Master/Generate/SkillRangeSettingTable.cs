/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillRangeSettingTable : ScriptableObject
{
    [SerializeField]
    private List<SkillRangeSetting> _dataList;

    public List<SkillRangeSetting> DataList {
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
public partial class SkillRangeSetting
{
    // None
    [SerializeField]
    public int range;

    // スキル範囲基準Enum
    [SerializeField]
    public SkillRangeBaseEnum range_base;

    // X位置
    [SerializeField]
    public int x;

    // Y位置
    [SerializeField]
    public int y;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
