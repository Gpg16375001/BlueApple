/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventInfoTable : ScriptableObject
{
    [SerializeField]
    private List<EventInfo> _dataList;

    public List<EventInfo> DataList {
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

    private Dictionary<int, EventInfo> _dataDict = null;
    public EventInfo this[int key]
    {
        get {
            EventInfo ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventInfo
{
    // イベント開催情報識別子
    [SerializeField]
    public int id;

    // 表示優先順位
    [SerializeField]
    public int priority;

    [SerializeField]
    private string start_at_value;
    // 開始日時
    public DateTime? start_at;

    [SerializeField]
    private string end_at_value;
    // 終了日時
    public DateTime? end_at;

    // イベントタイプ
    [SerializeField]
    public EventTypeEnum event_type;

    [SerializeField]
    private bool event_arg_has_value;
    [SerializeField]
    private int event_arg_value;
    // イベント引数
    public int? event_arg;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       if(string.IsNullOrEmpty(start_at_value)) { start_at = null; } else { start_at = DateTime.Parse(start_at_value); }
       if(string.IsNullOrEmpty(end_at_value)) { end_at = null; } else { end_at = DateTime.Parse(end_at_value); }
        event_arg = null;
        if(event_arg_has_value) {
            event_arg = event_arg_value;
        }
        InitExtension();
    }
}
