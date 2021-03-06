﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DailyQuest : IQuestData
{
    /// <summary>
    /// クエストのタイプ
    /// </summary>
    public int QuestType { get { return this.id / 10000; } }

    /// <summary>
    /// from IQuestData interface : クエストID.
    /// </summary>
    public int ID { get { return this.id; } }

    /// <summary>
    /// from IQuestData interface : ステージID.
    /// </summary>
    public int BattleStageID { get { return this.stage_id; } }

    /// <summary>
    /// from IQuestData interface : 国情報.
    /// </summary>
    public Belonging Country
    {
        get {
            if (countryInfo == null) {
                countryInfo = MasterDataTable.belonging.DataList.Find(b => b.Enum == BelongingEnum.Unknown);
            }
            return countryInfo;
        }
    }
    private Belonging countryInfo;

    /// <summary>
    /// from IQuestData interface : 章番号.
    /// </summary>
    public int ChapterNum { get { return 0; } }

    /// <summary>
    /// from IQuestData interface : クエストインデックス.
    /// </summary>
    public int StageNum { get { return 0; } }

    /// <summary>
    /// from IQuestData interface : シーン番号.
    /// </summary>
    public int QuestNum { get { return 0; } }

    /// <summary>
    /// from IQuestData interface : クエスト名として表示する名前.
    /// </summary>
    public string QuestName { get { return this.quest_name; } }

    /// <summary>
    /// 必要AP.
    /// </summary>
    public int NeedAP { get { return this.cost_ap; } }

    /// <summary>
    /// 初回クリア報酬があるか？
    /// </summary>
    public bool HasClearReward { get { return false; } }

    /// <summary>
    /// 初回クリア報酬タイプ
    /// </summary>
    public ItemTypeEnum? ClearRewardType { get { return null; } }

    /// <summary>
    /// 初回クリア報酬ID
    /// </summary>
    public int ClearRewardId { get { return 0; } }

    /// <summary>
    /// 初回クリア報酬数
    /// </summary>
    public int ClearRewardQuantity { get { return 0; }  }

    /// <summary>
    /// 強制ロックか？
    /// </summary>
    public bool ForceLock { get { return false; } }

    public int[] ReleaseMissions { get { return new int[0]; } }
}
