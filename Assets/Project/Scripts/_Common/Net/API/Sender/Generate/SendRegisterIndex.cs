using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendRegisterIndex : BaseSendAPI
	{
		// request bodys


		public SendRegisterIndex() : base()
		{
			URL = ClientDefine.URL_API + "/register/";
		}
	}
}
