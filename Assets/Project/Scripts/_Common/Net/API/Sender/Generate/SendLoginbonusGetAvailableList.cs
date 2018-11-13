using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendLoginbonusGetAvailableList : BaseSendAPI
	{
		// request bodys


		public SendLoginbonusGetAvailableList() : base()
		{
			URL = ClientDefine.URL_API + "/api/loginbonus/get_available_list";
		}
	}
}
