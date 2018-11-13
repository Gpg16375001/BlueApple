using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendShopTradeMaterial : BaseSendAPI
	{
		// request bodys
		public int CardId;
		public int Quantity;

		public SendShopTradeMaterial() : base()
		{
			URL = ClientDefine.URL_API + "/api/shop/trade_material";
		}
	}
}
