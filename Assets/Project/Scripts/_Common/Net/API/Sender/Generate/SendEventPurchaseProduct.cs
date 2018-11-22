using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendEventPurchaseProduct : BaseSendAPI
	{
		// request bodys
		public int EventQuestId;
		public int ShopProductId;
		public int Quantity;

		public SendEventPurchaseProduct() : base()
		{
			URL = ClientDefine.URL_API + "/api/event/purchase_product";
		}
	}
}
