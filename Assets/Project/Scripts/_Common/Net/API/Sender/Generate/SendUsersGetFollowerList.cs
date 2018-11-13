using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersGetFollowerList : BaseSendAPI
	{
		// request bodys
		public int From;
		public int Count;
		public int SortOrder;

		public SendUsersGetFollowerList() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/get_follower_list";
		}
	}
}
