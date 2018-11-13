using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsOpenQuest : BaseSendAPI
	{
		// request bodys
		public int QuestId;
		public int StageId;
		public int[] MemberCardIdList;
		public int SupporterUserId;
		public int SupporterCardId;
		public int OverrideExpPercentage;
		public int OverrideGoldPercentage;
		public int OverrideItemDropPercentage;

		public SendQuestsOpenQuest() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/open_quest";
		}
	}
}
