using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersHealBattlePoint : BaseSendAPI
	{
		// request bodys
		public int ConsumerId;
		public int Quantity;

		public SendUsersHealBattlePoint() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/heal_battle_point";
		}
	}
}
