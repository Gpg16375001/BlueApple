using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPvpUpdateOpponentList : BaseSendAPI
	{
		// request bodys


		public SendPvpUpdateOpponentList() : base()
		{
			URL = ClientDefine.URL_API + "/api/pvp/update_opponent_list";
		}
	}
}
