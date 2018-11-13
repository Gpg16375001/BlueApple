using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendWeaponsLockWeapon : BaseSendAPI
	{
		// request bodys
		public long WeaponBagId;

		public SendWeaponsLockWeapon() : base()
		{
			URL = ClientDefine.URL_API + "/api/weapons/lock_weapon";
		}
	}
}
