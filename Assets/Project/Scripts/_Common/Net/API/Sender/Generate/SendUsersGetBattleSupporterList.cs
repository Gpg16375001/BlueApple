using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersGetBattleSupporterList : BaseSendAPI
	{
		// request bodys


		public SendUsersGetBattleSupporterList() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/get_battle_supporter_list";
		}
	}
}
