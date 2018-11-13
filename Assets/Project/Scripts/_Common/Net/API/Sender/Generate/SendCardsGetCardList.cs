using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsGetCardList : BaseSendAPI
	{
		// request bodys


		public SendCardsGetCardList() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/get_card_list";
		}
	}
}
