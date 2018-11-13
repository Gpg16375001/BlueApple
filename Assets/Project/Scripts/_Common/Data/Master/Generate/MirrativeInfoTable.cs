/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MirrativeInfoTable : ScriptableObject
{
    [SerializeField]
    private List<MirrativeInfo> _dataList;

    public List<MirrativeInfo> DataList {
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
public partial class MirrativeInfo
{
    // タイトル
    [SerializeField]
    public string title;

    // 見出し
    [SerializeField]
    public string topic;

    // 小タイトル1
    [SerializeField]
    public string min_title_1;

    // 内容1
    [SerializeField]
    public string text_1;

    // 小タイトル2
    [SerializeField]
    public string min_title_2;

    // 内容2
    [SerializeField]
    public string text_2;

    // 下部説明
    [SerializeField]
    public string bottom_text;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
