using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPvpGetOpponentList : BaseSendAPI
	{
		// request bodys


		public SendPvpGetOpponentList() : base()
		{
			URL = ClientDefine.URL_API + "/api/pvp/get_opponent_list";
		}
	}
}
