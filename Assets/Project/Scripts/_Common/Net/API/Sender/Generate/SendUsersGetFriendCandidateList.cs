using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersGetFriendCandidateList : BaseSendAPI
	{
		// request bodys


		public SendUsersGetFriendCandidateList() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/get_friend_candidate_list";
		}
	}
}
