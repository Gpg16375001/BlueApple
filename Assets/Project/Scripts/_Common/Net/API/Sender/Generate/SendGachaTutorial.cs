using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendGachaTutorial : BaseSendAPI
	{
		// request bodys


		public SendGachaTutorial() : base()
		{
			URL = ClientDefine.URL_API + "/api/gacha/execute_tutorial_gacha";
		}
	}
}
