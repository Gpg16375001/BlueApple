using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendTakeoverResumeUser : BaseSendAPI
	{
		// request bodys
		public string GameToken;

		public SendTakeoverResumeUser() : base()
		{
			URL = ClientDefine.URL_API + "/takeover/resume_user";
		}
	}
}
