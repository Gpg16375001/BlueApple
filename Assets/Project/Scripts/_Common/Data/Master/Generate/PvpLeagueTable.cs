/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PvpLeagueTable : ScriptableObject
{
    [SerializeField]
    private List<PvpLeague> _dataList;

    public List<PvpLeague> DataList {
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
        _dataDict_table_id_array = _dataList.GroupBy (x => x.table_id).ToDictionary(x => x.Key, x => x.ToArray());
        InitExtension();
    }

    private Dictionary<int, PvpLeague[]> _dataDict_table_id_array = null;
    public PvpLeague[] this[int key]
    {
        get {
            PvpLeague[] ret;
            _dataDict_table_id_array.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class PvpLeague
{
    // None
    [SerializeField]
    public int table_id;

    // リーグID
    [SerializeField]
    public int league_id;

    // 名前
    [SerializeField]
    public string name;

    // 勝敗ポイント
    [SerializeField]
    public int winning_point;

    [SerializeField]
    private bool item_type_has_value;
    [SerializeField]
    private int item_type_value;
    // アイテムタイプ
    public int? item_type;

    [SerializeField]
    private bool item_id_has_value;
    [SerializeField]
    private int item_id_value;
    // アイテムID
    public int? item_id;

    [SerializeField]
    private bool quantity_has_value;
    [SerializeField]
    private int quantity_value;
    // 個数
    public int? quantity;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        item_type = null;
        if(item_type_has_value) {
            item_type = item_type_value;
        }
        item_id = null;
        if(item_id_has_value) {
            item_id = item_id_value;
        }
        quantity = null;
        if(quantity_has_value) {
            quantity = quantity_value;
        }
        InitExtension();
    }
}
