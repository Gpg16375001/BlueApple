using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendWeaponsGetWeaponList : BaseSendAPI
	{
		// request bodys


		public SendWeaponsGetWeaponList() : base()
		{
			URL = ClientDefine.URL_API + "/api/weapons/get_weapon_list";
		}
	}
}
