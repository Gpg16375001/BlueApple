using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendUsersSearchByNickname : BaseSendAPI
	{
		// request bodys
		public string Keyword;

		public SendUsersSearchByNickname() : base()
		{
			URL = ClientDefine.URL_API + "/api/users/search_by_nickname";
		}
	}
}
