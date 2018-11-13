/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestMapLandPointTable : ScriptableObject
{
    [SerializeField]
    private List<MainQuestMapLandPoint> _dataList;

    public List<MainQuestMapLandPoint> DataList {
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
public partial class MainQuestMapLandPoint
{
    // 地点ID
    [SerializeField]
    public int id;

    // 所在地の副名。装飾として使われたりもする。
    [SerializeField]
    public string land_name_sub;

    // 所在地の主名。その素材地の名前。
    [SerializeField]
    public string land_name_main;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
