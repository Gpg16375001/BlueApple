using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetOpenBattle : BaseSendAPI
	{
		// request bodys


		public SendQuestsGetOpenBattle() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_open_battle";
		}
	}
}
