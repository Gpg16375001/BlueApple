using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendBattlesGetBattleEntryData : BaseSendAPI
	{
		// request bodys
		public int EntryId;

		public SendBattlesGetBattleEntryData() : base()
		{
			URL = ClientDefine.URL_API + "/api/battles/get_battle_entry_data";
		}
	}
}
