using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendPresentboxGetReceivableList : BaseSendAPI
	{
		// request bodys
		public int From;
		public int Count;
		public int SortOrder;

		public SendPresentboxGetReceivableList() : base()
		{
			URL = ClientDefine.URL_API + "/api/presentbox/get_receivable_list";
		}
	}
}
