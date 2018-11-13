/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class WeaponRarityTable : ScriptableObject
{
    [SerializeField]
    private List<WeaponRarity> _dataList;

    public List<WeaponRarity> DataList {
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

    private Dictionary<int, WeaponRarity> _dataDict = null;
    public WeaponRarity this[int key]
    {
        get {
            WeaponRarity ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class WeaponRarity
{
    // レアリティ
    [SerializeField]
    public int rarity;

    // このレアリティでの初期最大レベル
    [SerializeField]
    public int initial_max_level;

    // このレアリティの最大限界突破回数
    [SerializeField]
    public int breatkhrough_count;

    // このレアリティの限界突破時に上昇する最大レベル
    [SerializeField]
    public int add_max_level;

    // 素材にした時に入る基礎経験値
    [SerializeField]
    public int base_exp;

    // 素材専用武器を素材にいした時に入る経験値.この値がある時はbase_expの代わりに使用する.
    [SerializeField]
    public int material_weapon_exp;

    // 売却時の価格.
    [SerializeField]
    public int price;

    // 強化に必要なゲーム内通貨の量
    [SerializeField]
    public int cost;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
