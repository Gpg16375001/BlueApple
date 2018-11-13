/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ElementAffinitySettingTable : ScriptableObject
{
    [SerializeField]
    private List<ElementAffinitySetting> _dataList;

    public List<ElementAffinitySetting> DataList {
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
public partial class ElementAffinitySetting
{
    // None
    [SerializeField]
    public ElementEnum element;

    // None
    [SerializeField]
    public ElementEnum targe_element;

    [SerializeField]
    private ElementAffinityEnum _affinity;
    // 相性情報
    public ElementAffinity affinity;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        affinity = MasterDataTable.element_affinity.DataList.First (x => x.Enum == _affinity);
        InitExtension();
    }
}
