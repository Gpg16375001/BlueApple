﻿/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GachaItemTable : ScriptableObject
{
    [SerializeField]
    private List<GachaItem> _dataList;

    public List<GachaItem> DataList {
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

    private Dictionary<int, GachaItem> _dataDict = null;
    public GachaItem this[int key]
    {
        get {
            GachaItem ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class GachaItem
{
    // ガチャアイテムID
    [SerializeField]
    public int id;

    // グループID
    [SerializeField]
    public int group_id;

    // アイテム数把握用
    [SerializeField]
    public int index;

    // ガチャID
    [SerializeField]
    public int gacha_id;

    // アイテムタイプ名
    [SerializeField]
    public string item_type;

    // アイテムID
    [SerializeField]
    public int item_id;

    // 個数
    [SerializeField]
    public int quantity;

    // 同一グループ内での選択比、同じグループ内で傾斜をかける場合に101、99等指定
    [SerializeField]
    public int hit_rate;

    // 優先順位
    [SerializeField]
    public int priority;

    [SerializeField]
    private string priority_start_date_value;
    // 優先順位開始日時
    public DateTime? priority_start_date;

    [SerializeField]
    private string priority_end_date_value;
    // 優先順位終了日時
    public DateTime? priority_end_date;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       if(string.IsNullOrEmpty(priority_start_date_value)) { priority_start_date = null; } else { priority_start_date = DateTime.Parse(priority_start_date_value); }
       if(string.IsNullOrEmpty(priority_end_date_value)) { priority_end_date = null; } else { priority_end_date = DateTime.Parse(priority_end_date_value); }
        InitExtension();
    }
}
