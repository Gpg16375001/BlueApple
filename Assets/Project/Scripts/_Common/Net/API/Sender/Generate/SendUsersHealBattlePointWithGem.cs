using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersHealBattlePointWithGem : BaseSendAPI
	{
		// request bodys


		public SendUsersHealBattlePointWithGem() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/heal_battle_point_with_gem";
		}
	}
}
