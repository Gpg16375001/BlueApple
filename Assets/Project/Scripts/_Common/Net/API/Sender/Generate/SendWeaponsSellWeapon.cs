using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendWeaponsSellWeapon : BaseSendAPI
	{
		// request bodys
		public long[] WeaponBagIdList;

		public SendWeaponsSellWeapon() : base()
		{
			URL = ClientDefine.URL_API + "/api/weapons/sell_weapon";
		}
	}
}
