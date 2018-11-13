/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class UnitQuestTable : ScriptableObject
{
    [SerializeField]
    private List<UnitQuest> _dataList;

    public List<UnitQuest> DataList {
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

    private Dictionary<int, UnitQuest> _dataDict = null;
    public UnitQuest this[int key]
    {
        get {
            UnitQuest ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class UnitQuest
{
    // クエストID
    [SerializeField]
    public int id;

    // キャラではなくカードのIDである点に注意.
    [SerializeField]
    public int card_id;

    // ステージ番号
    [SerializeField]
    public int stage;

    // クエスト番号
    [SerializeField]
    public int quest;

    // 消費AP
    [SerializeField]
    public int cost_ap;

    // プレイヤー経験値
    [SerializeField]
    public int user_experience;

    // ステージID
    [SerializeField]
    public int stage_id;

    // このクエストが解放されるレベル
    [SerializeField]
    public int release_lv;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
