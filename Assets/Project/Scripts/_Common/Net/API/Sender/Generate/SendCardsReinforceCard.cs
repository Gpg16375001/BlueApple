using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsReinforceCard : BaseSendAPI
	{
		// request bodys
		public int CardId;
		public int[] MaterialIdList;

		public SendCardsReinforceCard() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/reinforce_card";
		}
	}
}
