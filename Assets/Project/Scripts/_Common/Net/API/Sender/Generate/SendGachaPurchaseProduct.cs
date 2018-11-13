using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendGachaPurchaseProduct : BaseSendAPI
	{
		// request bodys
		public int GachaProductId;

		public SendGachaPurchaseProduct() : base()
		{
			URL = ClientDefine.URL_API + "/api/gacha/purchase_product";
		}
	}
}
