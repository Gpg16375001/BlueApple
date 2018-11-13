using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetAchievementByType : BaseSendAPI
	{
		// request bodys
		public int QuestType;

		public SendQuestsGetAchievementByType() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_achievement_by_type";
		}
	}
}
