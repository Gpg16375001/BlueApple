using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendTeamsGetCardList : BaseSendAPI
	{
		// request bodys


		public SendTeamsGetCardList() : base()
		{
			URL = ClientDefine.URL_API + "/api/teams/get_card_list";
		}
	}
}
