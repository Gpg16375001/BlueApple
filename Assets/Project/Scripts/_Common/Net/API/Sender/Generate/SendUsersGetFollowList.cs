using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersGetFollowList : BaseSendAPI
	{
		// request bodys
		public int From;
		public int Count;
		public int SortOrder;

		public SendUsersGetFollowList() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/get_follow_list";
		}
	}
}
