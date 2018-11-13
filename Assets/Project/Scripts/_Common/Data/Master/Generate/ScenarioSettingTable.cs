/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ScenarioSettingTable : ScriptableObject
{
    [SerializeField]
    private List<ScenarioSetting> _dataList;

    public List<ScenarioSetting> DataList {
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

    private Dictionary<int, ScenarioSetting> _dataDict = null;
    public ScenarioSetting this[int key]
    {
        get {
            ScenarioSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class ScenarioSetting
{
    // クエストID
    [SerializeField]
    public int id;

    // 宴の各国章幕ごとのプロジェクト名
    [SerializeField]
    public string adv_project_name;

    // バトル前に再生するシナリオ名
    [SerializeField]
    public string scenario_pre_battle;

    // バトル中の冒頭に再生するシナリオ名
    [SerializeField]
    public string scenario_in_battle;

    // バトル中の終了時に再生するシナリオ名
    [SerializeField]
    public string scenario_out_battle;

    // バトル後に再生するシナリオ名
    [SerializeField]
    public string scenario_aft_battle;

    // 演出時に使用する国名
    [SerializeField]
    public string eff_country;

    // 演出い使用するパネルに表示するあらすじ
    [SerializeField]
    public string eff_summary;

    // 演出タイプ（空欄で何もしない）
    [SerializeField]
    public ScenarioEffectTypeEnum eff_type;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
