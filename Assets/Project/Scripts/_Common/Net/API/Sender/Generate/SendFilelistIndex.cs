using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendFilelistIndex : BaseSendAPI
	{
		// request bodys


		public SendFilelistIndex() : base()
		{
			URL = ClientDefine.URL_API + "/filelist/";
		}
	}
}
