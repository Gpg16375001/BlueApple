using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendTextInspect : BaseSendAPI
	{
		// request bodys
		public string Text;

		public SendTextInspect() : base()
		{
			URL = ClientDefine.URL_API + "/api/text/inspect";
		}
	}
}
