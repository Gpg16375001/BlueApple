/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardBasedMaterialTable : ScriptableObject
{
    [SerializeField]
    private List<CardBasedMaterial> _dataList;

    public List<CardBasedMaterial> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.card_id);
        InitExtension();
    }

    private Dictionary<int, CardBasedMaterial> _dataDict = null;
    public CardBasedMaterial this[int key]
    {
        get {
            CardBasedMaterial ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardBasedMaterial
{
    // カードID
    [SerializeField]
    public int card_id;

    // カード固有原素材ID
    [SerializeField]
    public int card_based_row_material_id;

    // 必要数
    [SerializeField]
    public int card_based_row_material_need_number;

    // カード固有素材ID
    [SerializeField]
    public int card_based_material_id;

    [SerializeField]
    private string exchange_display_start_at_value;
    // 交換リスト表示開始日時
    public DateTime? exchange_display_start_at;

    [SerializeField]
    private string exchange_display_end_at_value;
    // 交換リスト表示終了日時
    public DateTime? exchange_display_end_at;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       if(string.IsNullOrEmpty(exchange_display_start_at_value)) { exchange_display_start_at = null; } else { exchange_display_start_at = DateTime.Parse(exchange_display_start_at_value); }
       if(string.IsNullOrEmpty(exchange_display_end_at_value)) { exchange_display_end_at = null; } else { exchange_display_end_at = DateTime.Parse(exchange_display_end_at_value); }
        InitExtension();
    }
}
