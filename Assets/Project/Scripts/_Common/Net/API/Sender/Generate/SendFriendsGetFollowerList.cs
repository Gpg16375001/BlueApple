using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendFriendsGetFollowerList : BaseSendAPI
	{
		// request bodys
		public int From;
		public int Count;
		public int SortOrder;

		public SendFriendsGetFollowerList() : base()
		{
			URL = ClientDefine.URL_API + "/api/friends/get_follower_list";
		}
	}
}
