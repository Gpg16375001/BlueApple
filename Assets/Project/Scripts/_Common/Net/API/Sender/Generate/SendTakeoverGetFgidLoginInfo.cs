using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendTakeoverGetFgidLoginInfo : BaseSendAPI
	{
		// request bodys


		public SendTakeoverGetFgidLoginInfo() : base()
		{
			URL = ClientDefine.URL_API + "/takeover/get_fgid_login_info";
		}
	}
}
