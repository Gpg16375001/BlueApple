using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPresentboxReceiveItem : BaseSendAPI
	{
		// request bodys
		public int[] PresentIdList;

		public SendPresentboxReceiveItem() : base()
		{
			URL = ClientDefine.URL_API + "/api/presentbox/receive_item";
		}
	}
}
