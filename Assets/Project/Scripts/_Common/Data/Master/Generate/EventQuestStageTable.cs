/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestStageTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestStage> _dataList;

    public List<EventQuestStage> DataList {
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

    private Dictionary<int, EventQuestStage> _dataDict = null;
    public EventQuestStage this[int key]
    {
        get {
            EventQuestStage ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestStage
{
    // イベントクエストID
    [SerializeField]
    public int id;

    // イベントクエストスケジュールID
    [SerializeField]
    public int schedule;

    // ステージタイプ
    [SerializeField]
    public EventQuestStageTypeEnum stage_type;

    // ステージ名
    [SerializeField]
    public string name;

    // ステージ番号
    [SerializeField]
    public int index;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
