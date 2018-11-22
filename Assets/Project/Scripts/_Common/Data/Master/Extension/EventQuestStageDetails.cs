using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EventQuestStageDetails : IQuestData {
    /// <summary>
    /// クエストのタイプ
    /// 1: メインクエスト
    /// 2: サブクエスト
    /// 3: ユニットクエスト
    /// 4: 強化クエスト
    /// 5: 進化クエスト
    /// </summary>
    public int QuestType {
        get{ return 6; }
    }

    /// <summary>
    /// クエストID.
    /// </summary>
    public int ID { 
        get { return id; }
    }

    /// <summary>
    /// バトルがある場合のバトルステージID.
    /// </summary>
    public int BattleStageID { 
        get { return battle_stage_id; }
    }

    /// <summary>
    /// 国情報.
    /// </summary>
    public Belonging Country { 
        get { return null; }
    }

    /// <summary>
    /// 章番号.
    /// </summary>
    public int ChapterNum { get { return 1; } }

    /// <summary>
    /// ステージ or クエストIndex番号.
    /// </summary>
    public int StageNum { get { return MasterDataTable.event_quest_stage[stage_id].index; } }

    /// <summary>
    /// クエスト番号.
    /// </summary>
    public int QuestNum { get { return stage_index; }  }

    /// <summary>
    /// クエスト名.
    /// </summary>
    public string QuestName { get { return MasterDataTable.event_quest_stage[stage_id].name; } }

    /// <summary>
    /// 開始に必要なAP.
    /// </summary>
    public int NeedAP { get { return cost_ap; } }


    /// <summary>
    /// 初回クリア報酬があるか？
    /// </summary>
    public bool HasClearReward { get { return clear_reward_type.HasValue; } }

    /// <summary>
    /// 初回クリア報酬タイプ
    /// </summary>
    public ItemTypeEnum? ClearRewardType { get { return clear_reward_type; } }

    /// <summary>
    /// 初回クリア報酬ID
    /// </summary>
    public int ClearRewardId { get { return clear_reward_id; } }

    /// <summary>
    /// 初回クリア報酬数
    /// </summary>
    public int ClearRewardQuantity { get { return clear_reward_quantity; }  }

    /// <summary>
    /// 強制ロックか？
    /// </summary>
    public bool ForceLock { get { return false; } }

    private EventQuest _eventQuest;
    public EventQuest EventQuestData {
        get {
            if (_eventQuest == null) {
                if (EventQuestScheduleData != null) {
                    _eventQuest = MasterDataTable.event_quest [EventQuestScheduleData.event_quest_id];
                }
            }
            return _eventQuest;
        }
    }

    private EventQuestSchedule _eventScheduleQuest;
    public EventQuestSchedule EventQuestScheduleData {
        get {
            if (_eventScheduleQuest == null) {
                if (EventQuestStageData != null) {
                    _eventScheduleQuest = MasterDataTable.event_quest_schedule [EventQuestStageData.schedule];
                }
            }
            return _eventScheduleQuest;
        }
    }

    private EventQuestStage _eventQuestStage;
    public EventQuestStage EventQuestStageData {
        get {
            if (_eventQuestStage == null) {
                _eventQuestStage = MasterDataTable.event_quest_stage [stage_id];
            }
            return _eventQuestStage;
        }
    }

    public int[] ReleaseMissions { get; set; }

    public bool IsClear { get; set; }
     
}
