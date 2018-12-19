/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestScheduleTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestSchedule> _dataList;

    public List<EventQuestSchedule> DataList {
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

    private Dictionary<int, EventQuestSchedule> _dataDict = null;
    public EventQuestSchedule this[int key]
    {
        get {
            EventQuestSchedule ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestSchedule
{
    // イベントクエストスケジュールID
    [SerializeField]
    public int id;

    // イベントクエストID
    [SerializeField]
    public int event_quest_id;

    // 開催期間名
    [SerializeField]
    public string name;

	[SerializeField]
    private string start_at_value;
    // 開始日時
    public DateTime start_at;

	[SerializeField]
    private string end_at_value;
    // 終了日時
    public DateTime end_at;

    // 背景Prefab
    [SerializeField]
    public string background_prefab;

    [SerializeField]
    private bool top_display_card_1_has_value;
    [SerializeField]
    private int top_display_card_1_value;
    // 表示カード指定1
    public int? top_display_card_1;

    [SerializeField]
    private bool top_display_card_2_has_value;
    [SerializeField]
    private int top_display_card_2_value;
    // 表示カード指定2
    public int? top_display_card_2;

    // BGM指定
    [SerializeField]
    public string bgm_name;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       start_at = DateTime.Parse(start_at_value);
       end_at = DateTime.Parse(end_at_value);
        top_display_card_1 = null;
        if(top_display_card_1_has_value) {
            top_display_card_1 = top_display_card_1_value;
        }
        top_display_card_2 = null;
        if(top_display_card_2_has_value) {
            top_display_card_2 = top_display_card_2_value;
        }
        InitExtension();
    }
}
