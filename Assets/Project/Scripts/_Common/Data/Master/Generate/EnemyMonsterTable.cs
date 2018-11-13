/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EnemyMonsterTable : ScriptableObject
{
    [SerializeField]
    private List<EnemyMonster> _dataList;

    public List<EnemyMonster> DataList {
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

    private Dictionary<int, EnemyMonster> _dataDict = null;
    public EnemyMonster this[int key]
    {
        get {
            EnemyMonster ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EnemyMonster
{
    // モンスターユニークID
    [SerializeField]
    public int id;

    [SerializeField]
    private bool resource_id_has_value;
    [SerializeField]
    private int resource_id_value;
    // リソース参照ID
    public int? resource_id;

    // モンスター名
    [SerializeField]
    public string name;

    [SerializeField]
    private ElementEnum _element;
    // 属性
    public Element element;

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

    // ボイス指定（指定がない場合はならない)
    [SerializeField]
    public string voice_sheet_name;

    [SerializeField]
    private bool size_has_value;
    [SerializeField]
    private int size_value;
    // ID
    public int? size;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        resource_id = null;
        if(resource_id_has_value) {
            resource_id = resource_id_value;
        }
        element = MasterDataTable.element.DataList.First (x => x.Enum == _element);
        belonging = MasterDataTable.belonging.DataList.First (x => x.Enum == _belonging);
        family = MasterDataTable.family.DataList.First (x => x.Enum == _family);
        gender = MasterDataTable.gender.DataList.First (x => x.Enum == _gender);
        size = null;
        if(size_has_value) {
            size = size_value;
        }
        InitExtension();
    }
}
