using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPvpBeginBattle : BaseSendAPI
	{
		// request bodys
		public int ContestId;
		public int OpponentUserId;

		public SendPvpBeginBattle() : base()
		{
			URL = ClientDefine.URL_API + "/api/pvp/begin_battle";
		}
	}
}
