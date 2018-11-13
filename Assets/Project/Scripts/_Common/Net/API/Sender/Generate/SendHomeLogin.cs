using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendHomeLogin : BaseSendAPI
	{
		// request bodys


		public SendHomeLogin() : base()
		{
			URL = ClientDefine.URL_API + "/api/home/login";
		}
	}
}
