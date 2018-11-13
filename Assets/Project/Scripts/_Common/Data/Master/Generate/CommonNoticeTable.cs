/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CommonNoticeTable : ScriptableObject
{
    [SerializeField]
    private List<CommonNotice> _dataList;

    public List<CommonNotice> DataList {
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
public partial class CommonNotice
{
    // ID
    [SerializeField]
    public int id;

    // リストのヘッダー部分に記載するタイトル.
    [SerializeField]
    public string sun_title;

    // タイトル
    [SerializeField]
    public string title;

    // 1:iOSのみ、2:Androidのみ、4:auのみ、8:DMMのみとし、複数の場合はそれぞれの数字をbit加算した値となる
    [SerializeField]
    public int view_target_bit;

	[SerializeField]
    private string start_date_value;
    // 開始日時
    public DateTime start_date;

	[SerializeField]
    private string end_date_value;
    // 終了日時
    public DateTime end_date;

    // 優先度
    [SerializeField]
    public int priority;

    // お知らせURL
    [SerializeField]
    public string url;

    // None
    [SerializeField]
    public CommonNoticeCategoryEnum category;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
       start_date = DateTime.Parse(start_date_value);
       end_date = DateTime.Parse(end_date_value);
        InitExtension();
    }
}
