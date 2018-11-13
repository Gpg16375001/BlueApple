/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharaMaterialEvolutionByRoleDefinitionTable : ScriptableObject
{
    [SerializeField]
    private List<CharaMaterialEvolutionByRoleDefinition> _dataList;

    public List<CharaMaterialEvolutionByRoleDefinition> DataList {
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
        _dataDict = _dataList.ToDictionary (x => x.role);
        InitExtension();
    }

    private Dictionary<int, CharaMaterialEvolutionByRoleDefinition> _dataDict = null;
    public CharaMaterialEvolutionByRoleDefinition this[int key]
    {
        get {
            CharaMaterialEvolutionByRoleDefinition ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CharaMaterialEvolutionByRoleDefinition
{
    // None
    [SerializeField]
    public int role;

    // モンスター固有素材1
    [SerializeField]
    public int enemy_based_material_1;

    // モンスター固有素材2
    [SerializeField]
    public int enemy_based_material_2;

    // モンスター固有素材3
    [SerializeField]
    public int enemy_based_material_3;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
