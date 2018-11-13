using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendBattlesCloseBattleStage : BaseSendAPI
	{
		// request bodys
		public int EntryId;
		public int CloseStatus;

		public SendBattlesCloseBattleStage() : base()
		{
			URL = ClientDefine.URL_API + "/api/battles/close_battle_stage";
		}
	}
}
