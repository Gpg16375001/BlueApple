/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class WeaponTable : ScriptableObject
{
    [SerializeField]
    private List<Weapon> _dataList;

    public List<Weapon> DataList {
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

    private Dictionary<int, Weapon> _dataDict = null;
    public Weapon this[int key]
    {
        get {
            Weapon ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Weapon
{
    // 武器ユニークID
    [SerializeField]
    public int id;

    // 武器名
    [SerializeField]
    public string name;

    [SerializeField]
    private int _rarity;
    // 武器のレアリティ
    public WeaponRarity rarity;

    [SerializeField]
    private bool element_has_value;
    [SerializeField]
    private ElementEnum element_value;
    // 属性
    public ElementEnum? element;

    // テーブルID
    [SerializeField]
    public int level_table_id;

    // None
    [SerializeField]
    public int type;

    // None
    [SerializeField]
    public int motion_type;

    // 武具の詳細説明
    [SerializeField]
    public string details;

    // Lv1時のHP
    [SerializeField]
    public int initial_hp;

    // Lv最大時のHP
    [SerializeField]
    public int max_hp;

    // Lv1時の攻撃
    [SerializeField]
    public int initial_attack;

    // Lv最大時の攻撃
    [SerializeField]
    public int max_attack;

    // Lv1時の防御
    [SerializeField]
    public int initial_defence;

    // Lv最大時の防御
    [SerializeField]
    public int max_defence;

    // Lv1時の素早さ
    [SerializeField]
    public int initial_agility;

    // Lv最大時の素早さ
    [SerializeField]
    public int max_agility;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        rarity = MasterDataTable.weapon_rarity.DataList.First (x => x.rarity == _rarity);
        element = null;
        if(element_has_value) {
            element = element_value;
        }
        InitExtension();
    }
}
