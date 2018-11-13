using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsContinueQuest : BaseSendAPI
	{
		// request bodys
		public int QuestId;
		public int EntryId;

		public SendQuestsContinueQuest() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/continue_quest";
		}
	}
}
