using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendShopPurchaseProduct : BaseSendAPI
	{
		// request bodys
		public int ShopProductId;
		public int Quantity;

		public SendShopPurchaseProduct() : base()
		{
			URL = ClientDefine.URL_API + "/api/shop/purchase_product";
		}
	}
}
