using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersSearchByCustomerId : BaseSendAPI
	{
		// request bodys
		public string CustomerId;

		public SendUsersSearchByCustomerId() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/search_by_customer_id";
		}
	}
}
