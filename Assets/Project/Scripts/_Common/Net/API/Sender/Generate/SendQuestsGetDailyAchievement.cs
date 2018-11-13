using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetDailyAchievement : BaseSendAPI
	{
		// request bodys


		public SendQuestsGetDailyAchievement() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_daily_achievement";
		}
	}
}
