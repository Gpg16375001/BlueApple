using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendQuestsGetAchievement : BaseSendAPI
	{
		// request bodys


		public SendQuestsGetAchievement() : base()
		{
			URL = ClientDefine.URL_API + "/api/quests/get_achievement";
		}
	}
}
