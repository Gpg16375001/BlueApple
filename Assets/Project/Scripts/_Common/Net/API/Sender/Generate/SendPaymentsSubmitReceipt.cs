using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPaymentsSubmitReceipt : BaseSendAPI
	{
		// request bodys
		public string Receipt;
		public string Signature;

		public SendPaymentsSubmitReceipt() : base()
		{
			URL = ClientDefine.URL_API + "/api/payments/submit_receipt";
		}
	}
}
