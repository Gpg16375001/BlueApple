using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMissionsGetAchievement : BaseSendAPI
	{
		// request bodys


		public SendMissionsGetAchievement() : base()
		{
			URL = ClientDefine.URL_API + "/api/missions/get_achievement";
		}
	}
}
