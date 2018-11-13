using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab.Net.API
{
    public partial class DailyQuestAchievement
    {
        public DailyQuest Info {
            get {
                return MasterDataTable.quest_daily [QuestId];
            }
        }
    }
}