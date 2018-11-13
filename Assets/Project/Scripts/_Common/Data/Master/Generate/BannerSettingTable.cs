/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BannerSettingTable : ScriptableObject
{
    [SerializeField]
    private List<BannerSetting> _dataList;

    public List<BannerSetting> DataList {
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
public partial class BannerSetting
{
    // ID
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

    // None
    [SerializeField]
    public ApplicationTransitionEnum transition;

    [SerializeField]
    private bool transition_detail_has_value;
    [SerializeField]
    private int transition_detail_value;
    // 遷移先詳細
    public int? transition_detail;

    // 画像指定
    [SerializeField]
    public string image_path;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       if(string.IsNullOrEmpty(start_at_value)) { start_at = null; } else { start_at = DateTime.Parse(start_at_value); }
       if(string.IsNullOrEmpty(end_at_value)) { end_at = null; } else { end_at = DateTime.Parse(end_at_value); }
        transition_detail = null;
        if(transition_detail_has_value) {
            transition_detail = transition_detail_value;
        }
        InitExtension();
    }
}
