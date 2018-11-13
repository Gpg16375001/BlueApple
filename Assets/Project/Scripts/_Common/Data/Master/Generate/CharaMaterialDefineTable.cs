/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharaMaterialDefineTable : ScriptableObject
{
    [SerializeField]
    private List<CharaMaterialDefine> _dataList;

    public List<CharaMaterialDefine> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.rarity);
        InitExtension();
    }

    private Dictionary<int, CharaMaterialDefine> _dataDict = null;
    public CharaMaterialDefine this[int key]
    {
        get {
            CharaMaterialDefine ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CharaMaterialDefine
{
    // レアリティ
    [SerializeField]
    public int rarity;

    // 使用時にもらえる経験値
    [SerializeField]
    public int base_point;

    // 素材と強化対象の属性が一致した場合の上昇百分率
    [SerializeField]
    public int same_element_ratio;

    // 強化費用
    [SerializeField]
    public int cost;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
