using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendShopGetProductList : BaseSendAPI
	{
		// request bodys


		public SendShopGetProductList() : base()
		{
			URL = ClientDefine.URL_API + "/api/shop/get_product_list";
		}
	}
}
