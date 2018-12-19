/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CommonSeasonTable : ScriptableObject
{
    [SerializeField]
    private List<CommonSeason> _dataList;

    public List<CommonSeason> DataList {
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

        InitExtension();
    }


}

[Serializable]
public partial class CommonSeason
{
    // ID
    [SerializeField]
    public int id;

    // None
    [SerializeField]
    public CommonSeasonEnum type;

    // True:毎年の時期
    [SerializeField]
    public bool is_default;

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
