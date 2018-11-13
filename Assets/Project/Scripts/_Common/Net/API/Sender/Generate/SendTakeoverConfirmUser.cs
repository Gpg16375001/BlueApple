using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendTakeoverConfirmUser : BaseSendAPI
	{
		// request bodys
		public string GameToken;

		public SendTakeoverConfirmUser() : base()
		{
			URL = ClientDefine.URL_API + "/takeover/confirm_user";
		}
	}
}
