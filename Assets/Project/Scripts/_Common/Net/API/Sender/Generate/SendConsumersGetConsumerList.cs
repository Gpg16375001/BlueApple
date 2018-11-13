using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendConsumersGetConsumerList : BaseSendAPI
	{
		// request bodys


		public SendConsumersGetConsumerList() : base()
		{
			URL = ClientDefine.URL_API + "/api/consumers/get_consumer_list";
		}
	}
}
