using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendEventGetProductList : BaseSendAPI
	{
		// request bodys
		public int EventQuestId;

		public SendEventGetProductList() : base()
		{
			URL = ClientDefine.URL_API + "/api/event/get_product_list";
		}
	}
}
