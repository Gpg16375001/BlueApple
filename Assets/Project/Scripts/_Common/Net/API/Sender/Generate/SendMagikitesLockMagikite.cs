using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMagikitesLockMagikite : BaseSendAPI
	{
		// request bodys
		public long MagikiteBagId;

		public SendMagikitesLockMagikite() : base()
		{
			URL = ClientDefine.URL_API + "/api/magikites/lock_magikite";
		}
	}
}
