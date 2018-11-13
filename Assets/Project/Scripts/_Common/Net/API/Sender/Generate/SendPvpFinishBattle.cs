using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPvpFinishBattle : BaseSendAPI
	{
		// request bodys
		public int EntryId;
		public bool IsWin;

		public SendPvpFinishBattle() : base()
		{
			URL = ClientDefine.URL_API + "/api/pvp/finish_battle";
		}
	}
}
