/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class DailyQuestTable : ScriptableObject
{
    [SerializeField]
    private List<DailyQuest> _dataList;

    public List<DailyQuest> DataList {
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

    private Dictionary<int, DailyQuest> _dataDict = null;
    public DailyQuest this[int key]
    {
        get {
            DailyQuest ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class DailyQuest
{
    // クエストID
    [SerializeField]
    public int id;

    // クエスト表示名
    [SerializeField]
    public string quest_name;

    // None
    [SerializeField]
    public int element;

    // クエストの難易度、1:初級、2:中級、3:上級、4:超級
    [SerializeField]
    public int difficulty;

    // 単独開催する曜日ID（1:月、2:火、3:水、4:木、5:金、6:土）、日曜日は全クエスト開放
    [SerializeField]
    public int day_of_week;

    // 消費AP
    [SerializeField]
    public int cost_ap;

    // バトルのステージID
    [SerializeField]
    public int stage_id;

    [SerializeField]
    private bool release_condition_has_value;
    [SerializeField]
    private int release_condition_value;
    // 指定しているクエストIDをクリア済みであれば解放
    public int? release_condition;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        release_condition = null;
        if(release_condition_has_value) {
            release_condition = release_condition_value;
        }
        InitExtension();
    }
}
