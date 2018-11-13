using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsEvolveCard : BaseSendAPI
	{
		// request bodys
		public int CardId;

		public SendCardsEvolveCard() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/evolve_card";
		}
	}
}
