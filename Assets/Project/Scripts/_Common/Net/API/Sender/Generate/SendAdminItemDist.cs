using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendAdminItemDist : BaseSendAPI
	{
		// request bodys
		public string Dummy;

		public SendAdminItemDist() : base()
		{
			URL = ClientDefine.URL_API + "/admin/item_dist/<path:query>";
		}
	}
}
