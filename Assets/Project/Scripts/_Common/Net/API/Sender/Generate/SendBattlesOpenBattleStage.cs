using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendBattlesOpenBattleStage : BaseSendAPI
	{
		// request bodys
		public int StageId;
		public int[] MemberCardIdList;
		public int SupporterUserId;
		public int SupporterCardId;

		public SendBattlesOpenBattleStage() : base()
		{
			URL = ClientDefine.URL_API + "/api/battles/open_battle_stage";
		}
	}
}
