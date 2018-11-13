using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendLoginbonusReceiveItem : BaseSendAPI
	{
		// request bodys
		public int[] LoginbonusIdList;

		public SendLoginbonusReceiveItem() : base()
		{
			URL = ClientDefine.URL_API + "/api/loginbonus/receive_item";
		}
	}
}
