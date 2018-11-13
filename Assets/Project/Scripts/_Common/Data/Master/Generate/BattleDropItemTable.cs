/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleDropItemTable : ScriptableObject
{
    [SerializeField]
    private List<BattleDropItem> _dataList;

    public List<BattleDropItem> DataList {
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

    private Dictionary<int, BattleDropItem> _dataDict = null;
    public BattleDropItem this[int key]
    {
        get {
            BattleDropItem ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleDropItem
{
    // ID
    [SerializeField]
    public int id;

    // ドロップテーブルを定義しているID
    [SerializeField]
    public int drop_table_id;

    // アイテムドロップ確率.値が大きいほど出る確率が高い.
    [SerializeField]
    public int probability;

    // None
    [SerializeField]
    public ItemTypeEnum reward_type;

    // リワードID
    [SerializeField]
    public int reward_id;

    // 個数
    [SerializeField]
    public int quantity;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
