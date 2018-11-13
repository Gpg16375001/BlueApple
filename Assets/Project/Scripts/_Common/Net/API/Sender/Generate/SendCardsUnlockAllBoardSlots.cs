using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsUnlockAllBoardSlots : BaseSendAPI
	{
		// request bodys
		public int CardId;
		public int BoardIndex;

		public SendCardsUnlockAllBoardSlots() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/unlock_all_board_slots";
		}
	}
}
