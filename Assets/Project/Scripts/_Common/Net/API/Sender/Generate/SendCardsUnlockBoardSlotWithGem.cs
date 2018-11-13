using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsUnlockBoardSlotWithGem : BaseSendAPI
	{
		// request bodys
		public int CardId;
		public int BoardIndex;
		public int[] SlotIndexList;

		public SendCardsUnlockBoardSlotWithGem() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/unlock_board_slot_with_gem";
		}
	}
}
