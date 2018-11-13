/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GachaGroupTable : ScriptableObject
{
    [SerializeField]
    private List<GachaGroup> _dataList;

    public List<GachaGroup> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.id);
        InitExtension();
    }

    private Dictionary<int, GachaGroup> _dataDict = null;
    public GachaGroup this[int key]
    {
        get {
            GachaGroup ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class GachaGroup
{
    // ガチャグループID
    [SerializeField]
    public int id;

    // グループ名
    [SerializeField]
    public string name;

    // ガチャID
    [SerializeField]
    public int gacha_id;

    // グループが選択される比、1000分比くらいが分かりやすい
    [SerializeField]
    public int hit_rate;

    // 10連確定分（10連最後の1回を想定）のグループ選択比
    [SerializeField]
    public int s10_last_hit_rate;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
