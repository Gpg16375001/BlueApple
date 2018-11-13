using System;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab.Net.API
{
    public partial class UserData
    {
        /// <summary>
        /// 課金、フリーの総ジェム所持数
        /// </summary>
        /// <value>The gem count.</value>
        public int GemCount {
            get {
				return Math.Min((PaidGemCount + FreeGemCount), MasterDataTable.CommonDefine["CURRENCY_CAPACITY"].define_value);
            }
        }
    }
}
