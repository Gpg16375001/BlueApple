/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MissionTypeTable : ScriptableObject
{
    [SerializeField]
    private List<MissionType> _dataList;

    public List<MissionType> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.index);
        InitExtension();
    }

    private Dictionary<int, MissionType> _dataDict = null;
    public MissionType this[int key]
    {
        get {
            MissionType ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MissionType
{
    // ID
    [SerializeField]
    public int index;

    // 名前
    [SerializeField]
    public string name;

    // None
    [SerializeField]
    public MissionTypeEnum Enum;

    // 説明文テンプレート
    [SerializeField]
    public string text_template;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
