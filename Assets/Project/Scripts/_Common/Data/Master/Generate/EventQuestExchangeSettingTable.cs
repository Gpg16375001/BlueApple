/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestExchangeSettingTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestExchangeSetting> _dataList;

    public List<EventQuestExchangeSetting> DataList {
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

    private Dictionary<int, EventQuestExchangeSetting[]> _dataDict_event_quest_id_array = null;
    public EventQuestExchangeSetting[] this[int key]
    {
        get {
            EventQuestExchangeSetting[] ret;
            _dataDict_event_quest_id_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestExchangeSetting
{
    // イベント交換所ユニークID
    [SerializeField]
    public int id;

    // イベントクエストID
    [SerializeField]
    public int event_quest_id;

    // 交換に使用する消費戦績ポイント
    [SerializeField]
    public int use_point;

    // 商品説明
    [SerializeField]
    public string details;

    // None
    [SerializeField]
    public ItemTypeEnum item_type;

    // アイテムID
    [SerializeField]
    public int item_id;

    // アイテム個数
    [SerializeField]
    public int quantity;

    // 購入上限数
    [SerializeField]
    public int limitaion_count;

    [SerializeField]
    private string start_date_value;
    // 開始日時
    public DateTime? start_date;

    [SerializeField]
    private string end_date_value;
    // 終了日時
    public DateTime? end_date;

    [SerializeField]
    private bool release_condition_has_value;
    [SerializeField]
    private int release_condition_value;
    // 購入後開放ID
    public int? release_condition;

    // 利用可フラグ
    [SerializeField]
    public bool is_enalbe;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       if(string.IsNullOrEmpty(start_date_value)) { start_date = null; } else { start_date = DateTime.Parse(start_date_value); }
       if(string.IsNullOrEmpty(end_date_value)) { end_date = null; } else { end_date = DateTime.Parse(end_date_value); }
        release_condition = null;
        if(release_condition_has_value) {
            release_condition = release_condition_value;
        }
        InitExtension();
    }
}
