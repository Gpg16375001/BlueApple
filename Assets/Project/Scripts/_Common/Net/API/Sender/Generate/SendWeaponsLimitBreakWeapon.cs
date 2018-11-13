using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendWeaponsLimitBreakWeapon : BaseSendAPI
	{
		// request bodys
		public long WeaponBagId;
		public long[] MaterialWeaponBagIdList;

		public SendWeaponsLimitBreakWeapon() : base()
		{
			URL = ClientDefine.URL_API + "/api/weapons/limit_break_weapon";
		}
	}
}
