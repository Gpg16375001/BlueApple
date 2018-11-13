using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersRemoveFollower : BaseSendAPI
	{
		// request bodys
		public int UserId;

		public SendUsersRemoveFollower() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/remove_follower";
		}
	}
}
