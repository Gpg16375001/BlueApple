using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendFgidAssociateUser : BaseSendAPI
	{
		// request bodys
		public string GameToken;

		public SendFgidAssociateUser() : base()
		{
			URL = ClientDefine.URL_API + "/api/fgid/associate_user";
		}
	}
}
