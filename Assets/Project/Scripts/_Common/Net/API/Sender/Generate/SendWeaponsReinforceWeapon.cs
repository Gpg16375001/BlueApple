using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendWeaponsReinforceWeapon : BaseSendAPI
	{
		// request bodys
		public long WeaponBagId;
		public long[] MaterialWeaponBagIdList;

		public SendWeaponsReinforceWeapon() : base()
		{
			URL = ClientDefine.URL_API + "/api/weapons/reinforce_weapon";
		}
	}
}
