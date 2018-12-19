/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestPointNameTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestPointName> _dataList;

    public List<EventQuestPointName> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.event_id);
        InitExtension();
    }

    private Dictionary<int, EventQuestPointName> _dataDict = null;
    public EventQuestPointName this[int key]
    {
        get {
            EventQuestPointName ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestPointName
{
    // イベントID
    [SerializeField]
    public int event_id;

    // イベントポイント名
    [SerializeField]
    public string point_name;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
