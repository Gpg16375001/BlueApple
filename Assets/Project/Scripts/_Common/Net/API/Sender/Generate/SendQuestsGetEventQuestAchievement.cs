using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetEventQuestAchievement : BaseSendAPI
	{
		// request bodys
		public int EventQuestId;

		public SendQuestsGetEventQuestAchievement() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_event_quest_achievement";
		}
	}
}
