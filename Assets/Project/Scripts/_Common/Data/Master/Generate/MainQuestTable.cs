/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestTable : ScriptableObject
{
    [SerializeField]
    private List<MainQuest> _dataList;

    public List<MainQuest> DataList {
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

    private Dictionary<int, MainQuest> _dataDict = null;
    public MainQuest this[int key]
    {
        get {
            MainQuest ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class MainQuest
{
    // クエストID
    [SerializeField]
    public int id;

    // 国名
    [SerializeField]
    public string country;

    [SerializeField]
    private int _stage_info;
    // クエストのステージ情報
    public MainQuestStageInfo stage_info;

    [SerializeField]
    private int _map_land_point;
    // マップ上の地点情報
    public MainQuestMapLandPoint map_land_point;

    // マップ上のカメラフォーカス地点番号
    [SerializeField]
    public int camera_forcus_index;

    // クエスト番号
    [SerializeField]
    public int quest;

    // 消費AP
    [SerializeField]
    public int cost_ap;

    // プレイヤー経験値
    [SerializeField]
    public int user_experience;

    [SerializeField]
    private int _reward_item_type;
    // None
    public ItemType reward_item_type;

    // 報酬アイテムID。使う必要がなければ0。
    [SerializeField]
    public int reward_item_id;

    // 報酬アイテムのもらえる個数
    [SerializeField]
    public int reward_item_count;

    // バトルステージID
    [SerializeField]
    public int stage_id;

    // クリア時に解放されるクエストID。ない場合は0。
    [SerializeField]
    public int release_quest_id;

    // trueで強制的に進行不可になる
    [SerializeField]
    public bool is_force_lock;

    // 前回までのあらすじ文言
    [SerializeField]
    public string summary;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        stage_info = MasterDataTable.quest_main_stage_info.DataList.First (x => x.id == _stage_info);
        map_land_point = MasterDataTable.quest_main_map_land_info.DataList.First (x => x.id == _map_land_point);
        reward_item_type = MasterDataTable.item_type.DataList.First (x => x.index == _reward_item_type);
        InitExtension();
    }
}
