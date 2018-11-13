/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillTargetParameterTable : ScriptableObject
{
    [SerializeField]
    private List<SkillTargetParameter> _dataList;

    public List<SkillTargetParameter> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.Enum);
        InitExtension();
    }

    private Dictionary<SkillTargetParameterEnum, SkillTargetParameter> _dataDict = null;
    public SkillTargetParameter this[SkillTargetParameterEnum key]
    {
        get {
            SkillTargetParameter ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SkillTargetParameter
{
    // 名前
    [SerializeField]
    public string name;

    // 短縮名
    [SerializeField]
    public string short_name;

    // None
    [SerializeField]
    public SkillTargetParameterEnum Enum;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
