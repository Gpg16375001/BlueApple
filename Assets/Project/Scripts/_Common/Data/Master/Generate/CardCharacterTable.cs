/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CardCharacterTable : ScriptableObject
{
    [SerializeField]
    private List<CardCharacter> _dataList;

    public List<CardCharacter> DataList {
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

    private Dictionary<int, CardCharacter> _dataDict = null;
    public CardCharacter this[int key]
    {
        get {
            CardCharacter ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CardCharacter
{
    // キャラクターID
    [SerializeField]
    public int id;

    // キャラクター名
    [SerializeField]
    public string name;

    // キャラクター英名
    [SerializeField]
    public string name_en;

    // 担当声優の名前
    [SerializeField]
    public string cv;

    [SerializeField]
    private BelongingEnum _belonging;
    // 所属
    public Belonging belonging;

    [SerializeField]
    private FamilyEnum _family;
    // 種族
    public Family family;

    [SerializeField]
    private GenderEnum _gender;
    // 性別
    public Gender gender;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        belonging = MasterDataTable.belonging.DataList.First (x => x.Enum == _belonging);
        family = MasterDataTable.family.DataList.First (x => x.Enum == _family);
        gender = MasterDataTable.gender.DataList.First (x => x.Enum == _gender);
        InitExtension();
    }
}
