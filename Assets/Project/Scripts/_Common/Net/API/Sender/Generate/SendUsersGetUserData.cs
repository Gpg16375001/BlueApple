using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersGetUserData : BaseSendAPI
	{
		// request bodys
		public int[] UserIdList;

		public SendUsersGetUserData() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/get_user_data";
		}
	}
}
