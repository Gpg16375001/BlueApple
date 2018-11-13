using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendGachaExecuteTutorialGacha : BaseSendAPI
	{
		// request bodys


		public SendGachaExecuteTutorialGacha() : base()
		{
			URL = ClientDefine.URL_API + "/api/gacha/execute_tutorial_gacha";
		}
	}
}
