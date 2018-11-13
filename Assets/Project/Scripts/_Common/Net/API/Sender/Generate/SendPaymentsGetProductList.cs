using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPaymentsGetProductList : BaseSendAPI
	{
		// request bodys


		public SendPaymentsGetProductList() : base()
		{
			URL = ClientDefine.URL_API + "/api/payments/get_product_list";
		}
	}
}
