/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class FormationTable : ScriptableObject
{
    [SerializeField]
    private List<Formation> _dataList;

    public List<Formation> DataList {
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

    private Dictionary<int, Formation> _dataDict = null;
    public Formation this[int key]
    {
        get {
            Formation ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class Formation
{
    // 陣形ユニークID
    [SerializeField]
    public int id;

    // 陣形名
    [SerializeField]
    public string name;

    // 説明
    [SerializeField]
    public string explain;

    // 陣形レベルテーブルID
    [SerializeField]
    public int level_table_id;

    // A1
    [SerializeField]
    public int A1;

    // A2
    [SerializeField]
    public int A2;

    // A3
    [SerializeField]
    public int A3;

    // B1
    [SerializeField]
    public int B1;

    // B2
    [SerializeField]
    public int B2;

    // B3
    [SerializeField]
    public int B3;

    // C1
    [SerializeField]
    public int C1;

    // C2
    [SerializeField]
    public int C2;

    // C3
    [SerializeField]
    public int C3;

    [SerializeField]
    private bool A1_skill_id_has_value;
    [SerializeField]
    private int A1_skill_id_value;
    // スキル識別ID
    public int? A1_skill_id;

    [SerializeField]
    private bool A2_skill_id_has_value;
    [SerializeField]
    private int A2_skill_id_value;
    // スキル識別ID
    public int? A2_skill_id;

    [SerializeField]
    private bool A3_skill_id_has_value;
    [SerializeField]
    private int A3_skill_id_value;
    // スキル識別ID
    public int? A3_skill_id;

    [SerializeField]
    private bool B1_skill_id_has_value;
    [SerializeField]
    private int B1_skill_id_value;
    // スキル識別ID
    public int? B1_skill_id;

    [SerializeField]
    private bool B2_skill_id_has_value;
    [SerializeField]
    private int B2_skill_id_value;
    // スキル識別ID
    public int? B2_skill_id;

    [SerializeField]
    private bool B3_skill_id_has_value;
    [SerializeField]
    private int B3_skill_id_value;
    // スキル識別ID
    public int? B3_skill_id;

    [SerializeField]
    private bool C1_skill_id_has_value;
    [SerializeField]
    private int C1_skill_id_value;
    // スキル識別ID
    public int? C1_skill_id;

    [SerializeField]
    private bool C2_skill_id_has_value;
    [SerializeField]
    private int C2_skill_id_value;
    // スキル識別ID
    public int? C2_skill_id;

    [SerializeField]
    private bool C3_skill_id_has_value;
    [SerializeField]
    private int C3_skill_id_value;
    // スキル識別ID
    public int? C3_skill_id;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        A1_skill_id = null;
        if(A1_skill_id_has_value) {
            A1_skill_id = A1_skill_id_value;
        }
        A2_skill_id = null;
        if(A2_skill_id_has_value) {
            A2_skill_id = A2_skill_id_value;
        }
        A3_skill_id = null;
        if(A3_skill_id_has_value) {
            A3_skill_id = A3_skill_id_value;
        }
        B1_skill_id = null;
        if(B1_skill_id_has_value) {
            B1_skill_id = B1_skill_id_value;
        }
        B2_skill_id = null;
        if(B2_skill_id_has_value) {
            B2_skill_id = B2_skill_id_value;
        }
        B3_skill_id = null;
        if(B3_skill_id_has_value) {
            B3_skill_id = B3_skill_id_value;
        }
        C1_skill_id = null;
        if(C1_skill_id_has_value) {
            C1_skill_id = C1_skill_id_value;
        }
        C2_skill_id = null;
        if(C2_skill_id_has_value) {
            C2_skill_id = C2_skill_id_value;
        }
        C3_skill_id = null;
        if(C3_skill_id_has_value) {
            C3_skill_id = C3_skill_id_value;
        }
        InitExtension();
    }
}
