/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestReleaseCountryInfoTable : ScriptableObject
{
    [SerializeField]
    private List<MainQuestReleaseCountryInfo> _dataList;

    public List<MainQuestReleaseCountryInfo> DataList {
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
public partial class MainQuestReleaseCountryInfo
{
    [SerializeField]
    private string _country;
    // 国名
    public Belonging country;

    // 代表カード1
    [SerializeField]
    public int card_id1;

    // 代表カード2
    [SerializeField]
    public int card_id2;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        country = MasterDataTable.belonging.DataList.First (x => x.name == _country);
        InitExtension();
    }
}
