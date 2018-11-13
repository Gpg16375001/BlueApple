using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetMainAchievementByCountry : BaseSendAPI
	{
		// request bodys
		public int MainQuestCountry;

		public SendQuestsGetMainAchievementByCountry() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_main_achievement_by_country";
		}
	}
}
