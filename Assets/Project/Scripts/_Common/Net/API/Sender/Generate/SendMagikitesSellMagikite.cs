using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMagikitesSellMagikite : BaseSendAPI
	{
		// request bodys
		public long[] MagikiteBagIdList;

		public SendMagikitesSellMagikite() : base()
		{
			URL = ClientDefine.URL_API + "/api/magikites/sell_magikite";
		}
	}
}
