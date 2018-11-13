/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharaMaterialEvolutionDefinitionTable : ScriptableObject
{
    [SerializeField]
    private List<CharaMaterialEvolutionDefinition> _dataList;

    public List<CharaMaterialEvolutionDefinition> DataList {
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

    private Dictionary<int, CharaMaterialEvolutionDefinition> _dataDict = null;
    public CharaMaterialEvolutionDefinition this[int key]
    {
        get {
            CharaMaterialEvolutionDefinition ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class CharaMaterialEvolutionDefinition
{
    // レアリティ
    [SerializeField]
    public int rarity;

    // 進化に必要なゲーム内通貨の量
    [SerializeField]
    public int evolution_cost;

    // 属性固有進化素材1必要数
    [SerializeField]
    public int element_based_evolution_material_count_1;

    // 属性固有進化素材2必要数
    [SerializeField]
    public int element_based_evolution_material_count_2;

    // 属性固有進化素材3必要数
    [SerializeField]
    public int element_based_evolution_material_count_3;

    // 虹属性進化素材1必要数
    [SerializeField]
    public int rainbow_evolution_material_count_1;

    // ロール固有素材1必要数
    [SerializeField]
    public int role_based_material_count_1;

    // ロール固有素材2必要数
    [SerializeField]
    public int role_based_material_count_2;

    // ロール固有素材3必要数
    [SerializeField]
    public int role_based_material_count_3;

    // モンスター固有素材1必要数
    [SerializeField]
    public int enemy_based_material_count_1;

    // モンスター固有素材2必要数
    [SerializeField]
    public int enemy_based_material_count_2;

    // モンスター固有素材3必要数
    [SerializeField]
    public int enemy_based_material_count_3;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;

        InitExtension();
    }
}
