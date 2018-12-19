using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendFriendsGetFollowList : BaseSendAPI
	{
		// request bodys
		public int From;
		public int Count;
		public int SortOrder;

		public SendFriendsGetFollowList() : base()
		{
			URL = ClientDefine.URL_API + "/api/friends/get_follow_list";
		}
	}
}
