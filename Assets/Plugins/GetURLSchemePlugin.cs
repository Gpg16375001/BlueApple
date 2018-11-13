using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// URLスキームからアプリが起動されたかの判定と、URLスキームから起動されていた際の共通処理を提供します.
/// </summary>
public static class GetURLSchemePlugin
{
    public static UriInfo GetUriInfo()
    {
        // 起動に使用されたURLスキームを解析.
        var urlScheme = UrlSchemeReader.GetBootUrlScheme();
        return UriInfo.Parse(urlScheme);
    }

    #region InternalClass

    /// <summary>
    /// 起動に使用されたURLスキームの取得を提供します.
    /// </summary>
    static class UrlSchemeReader
    {
        [DllImport("__Internal")]
        static extern string _getURLScheme();

        /// <summary>
        /// 起動に使用されたURLスキームを取得します。
        /// </summary>
        public static string GetBootUrlScheme()
        {
            switch(Application.platform) {
                case RuntimePlatform.Android: return GetBootUrlSchemeAndroid();
                case RuntimePlatform.IPhonePlayer: return GetBootUrlSchemeIos();
                default: return GetBootUrlSchemeEditor();
            }
        }

        // Android用の実装
        static string GetBootUrlSchemeAndroid()
        {
            try {
                var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                var intent = activity.Call<AndroidJavaObject>("getIntent");
                if(intent == null) return null; // インテントがなければ解析終了
                var action = intent.Call<string>("getAction");
                if(action != "android.intent.action.VIEW") return null; // 起動アクションでなければ解析終了
                var data = intent.Call<AndroidJavaObject>("getData");
                if(data == null) return null; // データが渡されていなければ解析終了
                var uri = data.Call<string>("toString");
                intent.Call<AndroidJavaObject>("setAction", "");
                return uri;
            }
            catch(Exception e) {
                Debug.LogError(e.Message);
                return null;
            }
        }

        // iOS用の実装
        static string GetBootUrlSchemeIos()
        {
            try {
                return _getURLScheme();
            }
            catch {
                return null;
            }
        }

        // エディタ用の実装
        static string GetBootUrlSchemeEditor()
        {
            // エディタでもURLスキームから起動された場合のテストをしたい場合にコメントアウトを外す
            //return "com.smilelab.step://chocobo?id=EDITOR&pw=123456";
            return null;
        }
    }

    #endregion
}

/// <summary>
/// URIのからのパラメーター取得を提供します.
/// </summary>
public class UriInfo
{
    /// <summary>URI</summary>
    public string uri { get; private set; }

    /// <summary>URIかどうか</summary>
    public bool isUri { get; private set; }

    /// <summary>スキーム名</summary>
    public string schemeName { get; private set; }

    /// <summary>ホスト名</summary>
    public string hostName { get; private set; }

    // パラメータ辞書
    Dictionary<string, string> parameters;

    // コンストラクタを隠蔽してnewを制限.
    UriInfo() { }

    /// <summary>
    /// 文字列を解析してURI情報を取得します.
    /// </summary>
    public static UriInfo Parse(string uri)
    {
        var uriInfo = new UriInfo();

        if(uri == null) return uriInfo;

        uriInfo.uri = uri;

        var schemeSeparator = "://";
        var schemeIndex = uri.IndexOf(schemeSeparator);

        if(schemeIndex == -1) return uriInfo;
        uriInfo.isUri = true;

        uriInfo.schemeName = uri.Substring(0, schemeIndex);

        var parameterSeparator = "?";
        var parametersIndex = uri.LastIndexOf(parameterSeparator);

        if(parametersIndex == -1) {
            uriInfo.hostName = uri.Substring(schemeIndex + schemeSeparator.Length, uri.Length - schemeIndex - schemeSeparator.Length);
        } else {
            uriInfo.hostName = uri.Substring(schemeIndex + schemeSeparator.Length, parametersIndex - schemeIndex - schemeSeparator.Length);
            uriInfo.parameters = uri.Substring(parametersIndex + parameterSeparator.Length, uri.Length - parametersIndex - parameterSeparator.Length)
                .Split('&')
                .Select(text => text.Split('='))
                .ToDictionary(pair => pair[0], pair => pair[1]);
        }

        return uriInfo;
    }

    /// <summary>
    /// GETパラメーターで渡された値を取得します.
    /// </summary>
    public string GetParameter(string key)
    {
        if(!isUri) {
            throw new Exception("パラメーター「" + key + "」を取得しようとしましたが、URIは正常な形式で読み込まれていませんでした。");
        }

        if(parameters == null || !parameters.ContainsKey(key)) {
            throw new ArgumentException("パラメーター「" + key + "」を取得しようとしましたが、パラメーターはURLスキームから渡されていませんでした。");
        }

        return parameters[key];
    }
}
