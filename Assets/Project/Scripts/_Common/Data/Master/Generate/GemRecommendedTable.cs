/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GemRecommendedTable : ScriptableObject
{
    [SerializeField]
    private List<GemRecommended> _dataList;

    public List<GemRecommended> DataList {
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

    private Dictionary<int, GemRecommended> _dataDict = null;
    public GemRecommended this[int key]
    {
        get {
            GemRecommended ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class GemRecommended
{
    // ID
    [SerializeField]
    public int id;

    // None
    [SerializeField]
    public GemRecommendedEnum type;

    // メッセージ
    [SerializeField]
    public string message;

    // 優先順位
    [SerializeField]
    public int priority;

    [SerializeField]
    private string start_date_value;
    // 開始日時
    public DateTime? start_date;

    [SerializeField]
    private string end_date_value;
    // 終了日時
    public DateTime? end_date;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       if(string.IsNullOrEmpty(start_date_value)) { start_date = null; } else { start_date = DateTime.Parse(start_date_value); }
       if(string.IsNullOrEmpty(end_date_value)) { end_date = null; } else { end_date = DateTime.Parse(end_date_value); }
        InitExtension();
    }
}
