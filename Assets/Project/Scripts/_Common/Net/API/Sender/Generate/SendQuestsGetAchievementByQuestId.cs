using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetAchievementByQuestId : BaseSendAPI
	{
		// request bodys
		public int QuestId;

		public SendQuestsGetAchievementByQuestId() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_achievement_by_quest_id";
		}
	}
}
