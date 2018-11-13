/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MaintenanceTable : ScriptableObject
{
    [SerializeField]
    private List<Maintenance> _dataList;

    public List<Maintenance> DataList {
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
public partial class Maintenance
{
    // None
    [SerializeField]
    public StoreTypeEnum store_type;

    // 文言
    [SerializeField]
    public string text;

	[SerializeField]
    private string start_date_value;
    // ティッカー表示開始日時
    public DateTime start_date;

	[SerializeField]
    private string end_date_value;
    // ティッカー表示終了日時
    public DateTime end_date;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       start_date = DateTime.Parse(start_date_value);
       end_date = DateTime.Parse(end_date_value);
        InitExtension();
    }
}
