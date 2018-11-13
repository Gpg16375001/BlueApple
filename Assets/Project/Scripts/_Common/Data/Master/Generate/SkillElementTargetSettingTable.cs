/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SkillElementTargetSettingTable : ScriptableObject
{
    [SerializeField]
    private List<SkillElementTargetSetting> _dataList;

    public List<SkillElementTargetSetting> DataList {
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

    private Dictionary<int, SkillElementTargetSetting> _dataDict = null;
    public SkillElementTargetSetting this[int key]
    {
        get {
            SkillElementTargetSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SkillElementTargetSetting
{
    // スキル属性ターゲットID
    [SerializeField]
    public int id;

    // 火属性ターゲット
    [SerializeField]
    public bool is_fire;

    // 水属性ターゲット
    [SerializeField]
    public bool is_water;

    // 風属性ターゲット
    [SerializeField]
    public bool is_wind;

    // 土属性ターゲット
    [SerializeField]
    public bool is_soil;

    // 光属性ターゲット
    [SerializeField]
    public bool is_light;

    // 闇属性ターゲット
    [SerializeField]
    public bool is_dark;

    // 無属性ターゲット
    [SerializeField]
    public bool is_naught;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
