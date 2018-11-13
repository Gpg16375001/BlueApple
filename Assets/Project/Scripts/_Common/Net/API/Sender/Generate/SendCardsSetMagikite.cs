using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsSetMagikite : BaseSendAPI
	{
		// request bodys
		public EquippedMagikite[] EquippedMagikiteList;

		public SendCardsSetMagikite() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/set_magikite";
		}
	}
}
