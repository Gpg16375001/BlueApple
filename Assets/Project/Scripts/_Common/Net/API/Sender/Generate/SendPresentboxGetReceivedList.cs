using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPresentboxGetReceivedList : BaseSendAPI
	{
		// request bodys
		public int From;
		public int Count;

		public SendPresentboxGetReceivedList() : base()
		{
			URL = ClientDefine.URL_API + "/api/presentbox/get_received_list";
		}
	}
}
