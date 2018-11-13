using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendFgidGetLoginInfo : BaseSendAPI
	{
		// request bodys


		public SendFgidGetLoginInfo() : base()
		{
			URL = ClientDefine.URL_API + "/api/fgid/get_login_info";
		}
	}
}
