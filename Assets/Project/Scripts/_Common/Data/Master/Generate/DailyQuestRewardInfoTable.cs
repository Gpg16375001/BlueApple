/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class DailyQuestRewardInfoTable : ScriptableObject
{
    [SerializeField]
    private List<DailyQuestRewardInfo> _dataList;

    public List<DailyQuestRewardInfo> DataList {
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
        _dataDict_day_of_week_daily_quest_type_array = _dataList.GroupBy (x => new UniRx.Tuple<int, int>(x.day_of_week, x.daily_quest_type)).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<UniRx.Tuple<int, int>, DailyQuestRewardInfo[]> _dataDict_day_of_week_daily_quest_type_array = null;
    public DailyQuestRewardInfo[] this[int day_of_week, int daily_quest_type]
    {
        get {
            DailyQuestRewardInfo[] ret;
            var key = new UniRx.Tuple<int, int>(day_of_week, daily_quest_type);
            _dataDict_day_of_week_daily_quest_type_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class DailyQuestRewardInfo
{
    // 曜日クエストID
    [SerializeField]
    public int day_of_week;

    // 4:強化　5:進化
    [SerializeField]
    public int daily_quest_type;

    // None
    [SerializeField]
    public ItemTypeEnum reward_type;

    // リワードID
    [SerializeField]
    public int reward_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
