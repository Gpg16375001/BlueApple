using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersUnfollowUser : BaseSendAPI
	{
		// request bodys
		public int UserId;

		public SendUsersUnfollowUser() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/unfollow_user";
		}
	}
}
