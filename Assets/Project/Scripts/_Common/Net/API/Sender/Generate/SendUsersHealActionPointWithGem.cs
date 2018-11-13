using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersHealActionPointWithGem : BaseSendAPI
	{
		// request bodys


		public SendUsersHealActionPointWithGem() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/heal_action_point_with_gem";
		}
	}
}
