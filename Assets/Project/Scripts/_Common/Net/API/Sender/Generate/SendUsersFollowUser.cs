using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersFollowUser : BaseSendAPI
	{
		// request bodys
		public int UserId;

		public SendUsersFollowUser() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/follow_user";
		}
	}
}
