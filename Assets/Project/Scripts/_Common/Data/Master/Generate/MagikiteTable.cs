/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MagikiteTable : ScriptableObject
{
    [SerializeField]
    private List<Magikite> _dataList;

    public List<Magikite> DataList {
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

    private Dictionary<int, Magikite> _dataDict = null;
    public Magikite this[int key]
    {
        get {
            Magikite ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Magikite
{
    // マギカイトユニークID
    [SerializeField]
    public int id;

    // 名前
    [SerializeField]
    public string name;

    // レアリティ
    [SerializeField]
    public int rarity;

    [SerializeField]
    private bool skill_id_has_value;
    [SerializeField]
    private int skill_id_value;
    // スキル識別ID
    public int? skill_id;

    // 所持しているスキルのレベル
    [SerializeField]
    public int skill_level;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        skill_id = null;
        if(skill_id_has_value) {
            skill_id = skill_id_value;
        }
        InitExtension();
    }
}
