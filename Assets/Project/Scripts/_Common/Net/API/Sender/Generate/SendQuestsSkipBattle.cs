using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsSkipBattle : BaseSendAPI
	{
		// request bodys
		public int QuestId;
		public int StageId;
		public int[] MemberCardIdList;
		public int SkipCount;
		public int OverrideExpPercentage;
		public int OverrideGoldPercentage;
		public int OverrideItemDropPercentage;

		public SendQuestsSkipBattle() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/skip_battle";
		}
	}
}
