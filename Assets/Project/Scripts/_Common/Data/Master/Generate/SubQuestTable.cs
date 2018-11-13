/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SubQuestTable : ScriptableObject
{
    [SerializeField]
    private List<SubQuest> _dataList;

    public List<SubQuest> DataList {
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

    private Dictionary<int, SubQuest> _dataDict = null;
    public SubQuest this[int key]
    {
        get {
            SubQuest ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class SubQuest
{
    // クエストID
    [SerializeField]
    public int id;

    // サブクエスト識別用連番のインデックス
    [SerializeField]
    public int index;

    // 幕番号
    [SerializeField]
    public int act;

    // シーン番号
    [SerializeField]
    public int scene;

    // 消費AP
    [SerializeField]
    public int cost_ap;

    // プレイヤー経験値
    [SerializeField]
    public int user_experience;

    // ステージID
    [SerializeField]
    public int stage_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
