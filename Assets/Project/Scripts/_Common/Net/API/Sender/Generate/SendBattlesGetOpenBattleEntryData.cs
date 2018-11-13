using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendBattlesGetOpenBattleEntryData : BaseSendAPI
	{
		// request bodys


		public SendBattlesGetOpenBattleEntryData() : base()
		{
			URL = ClientDefine.URL_API + "/api/battles/get_open_battle_entry_data";
		}
	}
}
