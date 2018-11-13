using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsUnlockDailyQuest : BaseSendAPI
	{
		// request bodys
		public int QuestType;
		public int UnlockDayOfWeek;

		public SendQuestsUnlockDailyQuest() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/unlock_daily_quest";
		}
	}
}
