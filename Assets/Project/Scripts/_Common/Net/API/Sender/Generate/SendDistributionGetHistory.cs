using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendDistributionGetHistory : BaseSendAPI
	{
		// request bodys


		public SendDistributionGetHistory() : base()
		{
			URL = ClientDefine.URL_API + "/api/distribution/get_history";
		}
	}
}
