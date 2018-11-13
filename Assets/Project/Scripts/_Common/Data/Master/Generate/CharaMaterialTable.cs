/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharaMaterialTable : ScriptableObject
{
    [SerializeField]
    private List<CharaMaterial> _dataList;

    public List<CharaMaterial> DataList {
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

    private Dictionary<int, CharaMaterial> _dataDict = null;
    public CharaMaterial this[int key]
    {
        get {
            CharaMaterial ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CharaMaterial
{
    // ID
    [SerializeField]
    public int id;

    // 素材名
    [SerializeField]
    public string name;

    // 素材の種類
    [SerializeField]
    public string type;

    // レアリティ
    [SerializeField]
    public int rarity;

    [SerializeField]
    private bool element_has_value;
    [SerializeField]
    private ElementEnum element_value;
    // None
    public ElementEnum? element;

    [SerializeField]
    private bool role_has_value;
    [SerializeField]
    private CardRoleEnum role_value;
    // ロール
    public CardRoleEnum? role;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        element = null;
        if(element_has_value) {
            element = element_value;
        }
        role = null;
        if(role_has_value) {
            role = role_value;
        }
        InitExtension();
    }
}
