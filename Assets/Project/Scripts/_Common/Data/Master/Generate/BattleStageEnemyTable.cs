/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleStageEnemyTable : ScriptableObject
{
    [SerializeField]
    private List<BattleStageEnemy> _dataList;

    public List<BattleStageEnemy> DataList {
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

    private Dictionary<int, BattleStageEnemy> _dataDict = null;
    public BattleStageEnemy this[int key]
    {
        get {
            BattleStageEnemy ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


    public static void LoadPartitionData(int stage_id, Action<BattleStageEnemyTable> didLoad) {
        MasterDataManager.LoadPartitionMasterData<BattleStageEnemyTable> (
            string.Format("stage_enemy_part_{0}", stage_id),
            (table) => {
                didLoad(table);
            }
        );
    }

}

[Serializable]
public partial class BattleStageEnemy
{
    // ステージ敵ユニークID
    [SerializeField]
    public int id;

    // 出現ステージ
    [SerializeField]
    public int stage_id;

    // 出現Wave
    [SerializeField]
    public int wave_num;

    // 出撃番号
    [SerializeField]
    public int number;

    [SerializeField]
    private int _monster;
    // モンスター情報
    public EnemyMonster monster;

    [SerializeField]
    private int _card;
    // カード情報
    public CardCard card;

    [SerializeField]
    private bool equip_weapon_has_value;
    [SerializeField]
    private int equip_weapon_value;
    // 武器ユニークID
    public int? equip_weapon;

    // ボスフラグ
    [SerializeField]
    public bool is_boss;

    [SerializeField]
    private int _parameter_table;
    // パラメータタイプ
    public BattleEnemyParameter parameter_table;

    // ステージ補正パーセント(百分率)
    [SerializeField]
    public int parameter_correction;

    // 運
    [SerializeField]
    public int luck;

    [SerializeField]
    private bool skill_group_id_has_value;
    [SerializeField]
    private int skill_group_id_value;
    // スキルグループ指定
    public int? skill_group_id;

    [SerializeField]
    private bool drop_table_id_has_value;
    [SerializeField]
    private int drop_table_id_value;
    // ドロップテーブル情報
    public int? drop_table_id;

    // ID
    [SerializeField]
    public int ai_setting;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        monster = MasterDataTable.monster.DataList.FirstOrDefault (x => x.id == _monster);
        card = MasterDataTable.card.DataList.FirstOrDefault (x => x.id == _card);
        equip_weapon = null;
        if(equip_weapon_has_value) {
            equip_weapon = equip_weapon_value;
        }
        parameter_table = MasterDataTable.enemy_parameter.DataList.First (x => x.id == _parameter_table);
        skill_group_id = null;
        if(skill_group_id_has_value) {
            skill_group_id = skill_group_id_value;
        }
        drop_table_id = null;
        if(drop_table_id_has_value) {
            drop_table_id = drop_table_id_value;
        }
        InitExtension();
    }
}
