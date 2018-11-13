using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendCardsSetWeapon : BaseSendAPI
	{
		// request bodys
		public EquippedWeapon[] EquippedWeaponList;

		public SendCardsSetWeapon() : base()
		{
			URL = ClientDefine.URL_API + "/api/cards/set_weapon";
		}
	}
}
