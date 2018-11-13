using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendAuthIndex : BaseSendAPI
	{
		// request bodys
		public string Username;
		public string Password;

		public SendAuthIndex() : base()
		{
			URL = ClientDefine.URL_API + "/auth/";
		}
	}
}
