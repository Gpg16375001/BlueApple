using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendTeamsGetFormationList : BaseSendAPI
	{
		// request bodys


		public SendTeamsGetFormationList() : base()
		{
			URL = ClientDefine.URL_API + "/api/teams/get_formation_list";
		}
	}
}
