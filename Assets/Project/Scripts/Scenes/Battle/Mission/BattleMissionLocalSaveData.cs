using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// バトルミッションの進捗ローカル保存クラス.
/// </summary>
[Serializable]
public class BattleMissionLocalSaveData
{
    /// <summary>クエスト単位のミッション情報.</summary>
    [Serializable]
    public class QuestMissionInfo
    {
        public int QuestType;
        /// <summary>クエストID.</summary>
        public int ID;

        /// <summary>達成済みIndexリスト.1始まりなので注意.</summary>
        public List<int> AchiveIndexList { get { return idxList; } }
        [SerializeField]
        List<int> idxList = new List<int>();

        /// <summary>既に報酬を最大まで貰っている？</summary>
        public bool IsAchivedReward { get { return MaxRewardCount > 0 && RewardCount >= MaxRewardCount; } }

        /// <summary>既に貰ってる報酬数.</summary>
        public int RewardCount;

        /// <summary>報酬の貰える想定最大数.</summary>
        public int MaxRewardCount 
        { 
            get {
                var rtn = 0;
                if(MissionSetting == null){
                    return rtn;
                }
                if(MissionSetting.item_type_1.HasValue) rtn = 1;
                if(MissionSetting.item_type_2.HasValue) rtn = 2;
                if(MissionSetting.item_type_3.HasValue) rtn = 3;
                return rtn;
            } 
        }

        public BattleMissionSetting MissionSetting {
            get {
                int stageId;
                if(QuestType == 6) {
                    stageId = MasterDataTable.event_quest_stage_details[ID].battle_stage_id;
                } else if (QuestType == 4 || QuestType == 5) {
                    stageId = MasterDataTable.quest_daily[ID].stage_id;
                } else if (QuestType == 3) {
                    stageId = MasterDataTable.quest_unit[ID].stage_id;
                } else if (QuestType == 2) {
                    stageId = MasterDataTable.quest_sub[ID].stage_id;
                } else {
                    stageId = MasterDataTable.quest_main[ID].stage_id;
                }
                return MasterDataTable.battle_mission_setting.DataList.Find(m => m.stage_id == stageId);
            }
        }

        // ダミーコンストラクタ.
        public QuestMissionInfo()
        {
            ID = 0;
            QuestType = 0;
            RewardCount = 0;
            idxList = new List<int>();
        }
        public QuestMissionInfo(int questType, int questId, List<int> indexList)
        {
            QuestType = questType;
            ID = questId;
            RewardCount = 0;
            idxList = indexList;
        }
        public QuestMissionInfo(int questType, int questId, int rewardCount, List<int> indexList)
        {
            QuestType = questType;
            ID = questId;
            RewardCount = rewardCount;
            idxList = indexList;
        }

        public QuestMissionInfo(SmileLab.Net.API.QuestAchievement achivement)
        {
            QuestType = achivement.QuestType;
            ID = achivement.QuestId;
            RewardCount = achivement.ReceivedMissionRewardCount;
            idxList = achivement.AchievedMissionIdList.ToList();
        }
    }
}
