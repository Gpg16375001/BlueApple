/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillEffectTable : ScriptableObject
{
    [SerializeField]
    private List<SkillEffect> _dataList;

    public List<SkillEffect> DataList {
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

    private Dictionary<int, SkillEffect> _dataDict = null;
    public SkillEffect this[int key]
    {
        get {
            SkillEffect ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SkillEffect
{
    // スキル効果ユニークID
    [SerializeField]
    public int id;

    // スキル効果ロジック名
    [SerializeField]
    public SkillEffectLogicEnum effect;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name1;

    // 引数1
    [SerializeField]
    public string arg_value1;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name2;

    // 引数2
    [SerializeField]
    public string arg_value2;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name3;

    // 引数3
    [SerializeField]
    public string arg_value3;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name4;

    // 引数4
    [SerializeField]
    public string arg_value4;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name5;

    // 引数5
    [SerializeField]
    public string arg_value5;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name6;

    // 引数6
    [SerializeField]
    public string arg_value6;

    // None
    [SerializeField]
    public SkillEffectLogicArgEnum arg_name7;

    // 引数7
    [SerializeField]
    public string arg_value7;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
