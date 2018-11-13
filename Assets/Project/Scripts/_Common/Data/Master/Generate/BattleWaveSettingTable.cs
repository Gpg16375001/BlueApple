/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleWaveSettingTable : ScriptableObject
{
    [SerializeField]
    private List<BattleWaveSetting> _dataList;

    public List<BattleWaveSetting> DataList {
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

        InitExtension();
    }


    public static void LoadPartitionData(int stage_id, Action<BattleWaveSettingTable> didLoad) {
        MasterDataManager.LoadPartitionMasterData<BattleWaveSettingTable> (
            string.Format("battle_wave_setting_part_{0}", stage_id),
            (table) => {
                didLoad(table);
            }
        );
    }

}

[Serializable]
public partial class BattleWaveSetting
{
    // ステージID
    [SerializeField]
    public int stage_id;

    // ウェーブ数
    [SerializeField]
    public int wave_count;

    [SerializeField]
    private int _formation;
    // 使用陣形名
    public Formation formation;

    // 陣形レベル
    [SerializeField]
    public int formation_level;

    // ボス出現Waveか
    [SerializeField]
    public bool is_boss;

    // ここに設定があった場合はこのWaveに入った時にBGMが切り替わります。
    [SerializeField]
    public string wave_bgm;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        formation = MasterDataTable.formation.DataList.First (x => x.id == _formation);
        InitExtension();
    }
}
