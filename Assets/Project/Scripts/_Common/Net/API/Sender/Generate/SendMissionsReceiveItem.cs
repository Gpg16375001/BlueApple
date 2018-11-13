using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMissionsReceiveItem : BaseSendAPI
	{
		// request bodys
		public int[] MissionIdList;

		public SendMissionsReceiveItem() : base()
		{
			URL = ClientDefine.URL_API + "/api/missions/receive_item";
		}
	}
}
