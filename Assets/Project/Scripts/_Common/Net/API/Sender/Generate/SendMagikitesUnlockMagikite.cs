using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMagikitesUnlockMagikite : BaseSendAPI
	{
		// request bodys
		public long MagikiteBagId;

		public SendMagikitesUnlockMagikite() : base()
		{
			URL = ClientDefine.URL_API + "/api/magikites/unlock_magikite";
		}
	}
}
