using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
	public class SendMaterialsGetMaterialList : BaseSendAPI
	{
		// request bodys


		public SendMaterialsGetMaterialList() : base()
		{
			URL = ClientDefine.URL_API + "/api/materials/get_material_list";
		}
	}
}
