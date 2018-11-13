using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCognitosyncwatchdogIndex : BaseSendAPI
	{
		// request bodys
		public string Username;
		public string Password;

		public SendCognitosyncwatchdogIndex() : base()
		{
			URL = ClientDefine.URL_API + "//";
		}
	}
}
