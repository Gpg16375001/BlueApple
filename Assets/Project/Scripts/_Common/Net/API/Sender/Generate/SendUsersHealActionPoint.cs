using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersHealActionPoint : BaseSendAPI
	{
		// request bodys
		public int ConsumerId;
		public int Quantity;

		public SendUsersHealActionPoint() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/heal_action_point";
		}
	}
}
