using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendWeaponsUnlockWeapon : BaseSendAPI
	{
		// request bodys
		public long WeaponBagId;

		public SendWeaponsUnlockWeapon() : base()
		{
			URL = ClientDefine.URL_API + "/api/weapons/unlock_weapon";
		}
	}
}
