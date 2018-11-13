using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsLimitBreak : BaseSendAPI
	{
		// request bodys
		public int CardId;

		public SendCardsLimitBreak() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/limit_break";
		}
	}
}
