using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPaymentsSubmitReceiptWithTimeout : BaseSendAPI
	{
		// request bodys
		public string Receipt;
		public string Signature;

		public SendPaymentsSubmitReceiptWithTimeout() : base()
		{
			URL = ClientDefine.URL_API + "/api/payments/submit_receipt_with_timeout";
		}
	}
}
