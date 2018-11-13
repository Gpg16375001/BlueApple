using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsUnlockBoardSlot : BaseSendAPI
	{
		// request bodys
		public int CardId;
		public int BoardIndex;
		public int[] SlotIndexList;

		public SendCardsUnlockBoardSlot() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/unlock_board_slot";
		}
	}
}
