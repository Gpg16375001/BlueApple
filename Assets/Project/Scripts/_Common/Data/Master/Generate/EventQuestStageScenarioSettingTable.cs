/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestStageScenarioSettingTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestStageScenarioSetting> _dataList;

    public List<EventQuestStageScenarioSetting> DataList {
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

    private Dictionary<int, EventQuestStageScenarioSetting> _dataDict = null;
    public EventQuestStageScenarioSetting this[int key]
    {
        get {
            EventQuestStageScenarioSetting ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestStageScenarioSetting
{
    // イベントクエストステージ詳細ID
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


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
