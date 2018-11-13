using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMagikitesGetMagikiteList : BaseSendAPI
	{
		// request bodys


		public SendMagikitesGetMagikiteList() : base()
		{
			URL = ClientDefine.URL_API + "/api/magikites/get_magikite_list";
		}
	}
}
