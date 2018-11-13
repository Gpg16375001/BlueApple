using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendGachaGetProductList : BaseSendAPI
	{
		// request bodys


		public SendGachaGetProductList() : base()
		{
			URL = ClientDefine.URL_API + "/api/gacha/get_product_list";
		}
	}
}
