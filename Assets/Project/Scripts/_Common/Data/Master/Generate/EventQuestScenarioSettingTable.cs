/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestScenarioSettingTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestScenarioSetting> _dataList;

    public List<EventQuestScenarioSetting> DataList {
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
        _dataDict_event_quest_id_array = _dataList.GroupBy (x => x.event_quest_id).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<int, EventQuestScenarioSetting[]> _dataDict_event_quest_id_array = null;
    public EventQuestScenarioSetting[] this[int key]
    {
        get {
            EventQuestScenarioSetting[] ret;
            _dataDict_event_quest_id_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestScenarioSetting
{
    // 識別子
    [SerializeField]
    public int id;

    // イベントクエストID
    [SerializeField]
    public int event_quest_id;

    [SerializeField]
    private bool event_quest_schedule_id_has_value;
    [SerializeField]
    private int event_quest_schedule_id_value;
    // イベントクエストスケジュールID
    public int? event_quest_schedule_id;

    [SerializeField]
    private bool release_quest_id_has_value;
    [SerializeField]
    private int release_quest_id_value;
    // 指定クエストIDの開放後に再生する
    public int? release_quest_id;

    // None
    [SerializeField]
    public EventQuestScenarioPlayTimingEnum play_timing;

    // 宴プロジェクト名
    [SerializeField]
    public string adv_project_name;

    // シナリオ名
    [SerializeField]
    public string scenario_name;

    // シナリオ再生時演出Prefab
    [SerializeField]
    public string before_play_prefab;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        event_quest_schedule_id = null;
        if(event_quest_schedule_id_has_value) {
            event_quest_schedule_id = event_quest_schedule_id_value;
        }
        release_quest_id = null;
        if(release_quest_id_has_value) {
            release_quest_id = release_quest_id_value;
        }
        InitExtension();
    }
}
