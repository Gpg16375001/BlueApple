/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleEnemyParameterTable : ScriptableObject
{
    [SerializeField]
    private List<BattleEnemyParameter> _dataList;

    public List<BattleEnemyParameter> DataList {
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

    private Dictionary<int, BattleEnemyParameter> _dataDict = null;
    public BattleEnemyParameter this[int key]
    {
        get {
            BattleEnemyParameter ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class BattleEnemyParameter
{
    // パラメータタイプID
    [SerializeField]
    public int id;

    // HP
    [SerializeField]
    public int hp;

    // 攻撃力
    [SerializeField]
    public int attack;

    // 防御力
    [SerializeField]
    public int defence;

    // 素早さ
    [SerializeField]
    public int agility;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
