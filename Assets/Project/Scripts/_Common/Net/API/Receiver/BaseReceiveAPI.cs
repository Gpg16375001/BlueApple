using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab.Net.API
{
    /// <summary>
    /// AWS SDKによる通信受信基底クラス.
    /// </summary>
    public class BaseReceiveAPI
    {
        // 共通レスポンス情報
        public int ResultCode = 0;
        public int MasterVersion = 0;
		public string ErrorMessage = "";
    }
}