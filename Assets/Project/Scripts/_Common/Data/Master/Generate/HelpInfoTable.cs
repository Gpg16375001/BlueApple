/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class HelpInfoTable : ScriptableObject
{
    [SerializeField]
    private List<HelpInfo> _dataList;

    public List<HelpInfo> DataList {
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
public partial class HelpInfo
{
    // リスト上の項目名
    [SerializeField]
    public OptionMenuEnum subject;

    // ヘルプ内で各項目のヘッダーとなる大項目
    [SerializeField]
    public string subject_large;

    // 大項目押下で展開される各種詳細項目
    [SerializeField]
    public string subject_small;

    // ヘルプ内容
    [SerializeField]
    public string text_detail;

    // ヘルプ内に表示する画像でtext_detailより優先してこちらを表示する
    [SerializeField]
    public string name_texture;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
