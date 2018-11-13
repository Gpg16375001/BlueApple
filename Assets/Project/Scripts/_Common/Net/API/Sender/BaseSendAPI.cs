using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

namespace SmileLab.Net.API
{
    /// <summary>
    /// AWS SDKによる通信リクエスト基底クラス.
    /// </summary>
    [Serializable]
    public class BaseSendAPI
    {
        /// <summary>通信URL.</summary>
        public string URL { get; protected set; }

        /// <summary>同名のリクエストヘッダ種に載せてユーザー認証に使用.</summary>
        public string Authorization { get; protected set; }

        /// <summary>バージョン番号文字列.</summary>
        public string Version;
        /// <summary>iphone or android.</summary>
        public string OS;
        /// <summary>日時文字列."yyyy-MM-ddTHH:mm:sszzzz"形式.</summary>
        public string Date;
        /// <summary>重複リクエスト防止</summary>
        public string RequestId;

        /// <summary>リトライ回数</summary>
        [NonSerialized]
        public int retryCount;

        public BaseSendAPI ()
        {
            Version = Application.version;
			OS = GameSystem.GetPlatformName();
            Date = GameTime.SharedInstance.Now.ToString ("yyyy-MM-ddTHH:mm:sszzzz");
            RequestId = Guid.NewGuid ().ToString ();
            retryCount = 0;
        }
    }
}
