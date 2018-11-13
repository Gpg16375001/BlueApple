using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsLimitBreakWithGem : BaseSendAPI
	{
		// request bodys
		public int CardId;

		public SendCardsLimitBreakWithGem() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/limit_break_with_gem";
		}
	}
}
