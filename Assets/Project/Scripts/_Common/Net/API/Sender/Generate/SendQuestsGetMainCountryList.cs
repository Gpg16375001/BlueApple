using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetMainCountryList : BaseSendAPI
	{
		// request bodys


		public SendQuestsGetMainCountryList() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_main_country_list";
		}
	}
}
