/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestCommonInfoTable : ScriptableObject
{
    [SerializeField]
    private List<MainQuestCommonInfo> _dataList;

    public List<MainQuestCommonInfo> DataList {
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

    private Dictionary<int, MainQuestCommonInfo> _dataDict = null;
    public MainQuestCommonInfo this[int key]
    {
        get {
            MainQuestCommonInfo ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MainQuestCommonInfo
{
    // ID
    [SerializeField]
    public int id;

    // ティッカー文言
    [SerializeField]
    public string ticker_text;

	[SerializeField]
    private string start_date_value;
    // ティッカー表示開始日時
    public DateTime start_date;

	[SerializeField]
    private string end_date_value;
    // ティッカー表示終了日時
    public DateTime end_date;

    // ティッカー表示強制無効フラグ
    [SerializeField]
    public bool is_disabled;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       start_date = DateTime.Parse(start_date_value);
       end_date = DateTime.Parse(end_date_value);
        InitExtension();
    }
}
