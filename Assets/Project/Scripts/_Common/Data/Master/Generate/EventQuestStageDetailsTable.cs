/*
このファイルは自動生成されたファイルです。
クラスを拡張したい場合は別ファイルを用意しparcial宣言して拡張してください。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestStageDetailsTable : ScriptableObject
{
    [SerializeField]
    private List<EventQuestStageDetails> _dataList;

    public List<EventQuestStageDetails> DataList {
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

    private Dictionary<int, EventQuestStageDetails> _dataDict = null;
    public EventQuestStageDetails this[int key]
    {
        get {
            EventQuestStageDetails ret;
            _dataDict.TryGetValue (key, out ret);
            return ret;
        }
    }


}

[Serializable]
public partial class EventQuestStageDetails
{
    // イベントクエストステージ詳細ID
    [SerializeField]
    public int id;

    // イベントクエストID
    [SerializeField]
    public int stage_id;

    // クエスト
    [SerializeField]
    public int stage_index;

    // 消費AP
    [SerializeField]
    public int cost_ap;

    // バトルのステージID
    [SerializeField]
    public int battle_stage_id;

    [SerializeField]
    private bool release_condition_has_value;
    [SerializeField]
    private int release_condition_value;
    // 指定しているステージ詳細IDをクリア済みであれば解放
    public int? release_condition;

    [SerializeField]
    private bool clear_reward_type_has_value;
    [SerializeField]
    private ItemTypeEnum clear_reward_type_value;
    // 初回クリア報酬アイテムタイプ
    public ItemTypeEnum? clear_reward_type;

    // 初回クリア報酬アイテムID
    [SerializeField]
    public int clear_reward_id;

    // 初回クリア報酬数
    [SerializeField]
    public int clear_reward_quantity;

    // 推奨レベル
    [SerializeField]
    public int recommended_level;


    // 初期化関数をpartial化して拡張できるようにする。
    partial void InitExtension();
    public void Init() {
        if(!Application.isPlaying) return;
        release_condition = null;
        if(release_condition_has_value) {
            release_condition = release_condition_value;
        }
        clear_reward_type = null;
        if(clear_reward_type_has_value) {
            clear_reward_type = clear_reward_type_value;
        }
        InitExtension();
    }
}
