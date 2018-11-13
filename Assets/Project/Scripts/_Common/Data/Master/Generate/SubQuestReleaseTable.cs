/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SubQuestReleaseTable : ScriptableObject
{
    [SerializeField]
    private List<SubQuestRelease> _dataList;

    public List<SubQuestRelease> DataList {
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

    private Dictionary<int, SubQuestRelease> _dataDict = null;
    public SubQuestRelease this[int key]
    {
        get {
            SubQuestRelease ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SubQuestRelease
{
    // サブクエスト識別用連番のインデックス
    [SerializeField]
    public int index;

    // 解放条件となる選択肢ID その１
    [SerializeField]
    public int select_1;

    // 解放条件となる選択肢ID その２
    [SerializeField]
    public int select_2;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
