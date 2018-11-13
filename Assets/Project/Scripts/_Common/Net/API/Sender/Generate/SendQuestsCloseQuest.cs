using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsCloseQuest : BaseSendAPI
	{
		// request bodys
		public int QuestId;
		public bool IsAchieved;
		public int EntryId;
		public int[] SelectionIdList;
		public int[] MissionIdList;

		public SendQuestsCloseQuest() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/close_quest";
		}
	}
}
