using UnityEngine;

/// <summary>
/// クライアント側で使用するシステム共通定数一覧.
/// ※注意：ゲーム中で使用する様々な変数や定数をここに書かないこと！
/// </summary>
public static class ClientDefine
{
	#region URL.
#if DEFINE_RELEASE || DEFINE_RELEASE_ADHOC
    /// <summary>サーバー通信URL:本番環境のDLC.</summary>
	public const string URL_DLC = "https://d2ohftagsuzxeo.cloudfront.net/";
	/// <summary>サーバー通信URL:本番環境のAPI通信用.</summary>
	public const string URL_API = "https://yweo0xa5q6.execute-api.ap-northeast-1.amazonaws.com/v1/";
#elif DEFINE_BETA
	/// <summary>サーバー通信URL:β環境のDLC.</summary>
    public const string URL_DLC = "https://d3bhcks4u2tq5l.cloudfront.net/";
	/// <summary>サーバー通信URL:β環境のAPI通信用.</summary>
	public const string URL_API = "https://qjnozfatoc.execute-api.ap-northeast-1.amazonaws.com/v1_beta";
#else
	/// <summary>サーバー通信URL:α環境のDLC.</summary>
	public const string URL_DLC = "https://d3bhcks4u2tq5l.cloudfront.net/";
	/// <summary>サーバー通信URL:α環境のAPI通信用.</summary>
	public const string URL_API = "https://03owsy7786.execute-api.ap-northeast-1.amazonaws.com/v1_alpha";
#endif
	#endregion

	#region AWS API key.
#if DEFINE_RELEASE || DEFINE_RELEASE_ADHOC
	public const string KEY_AWS_API = "NYA21cEYk1HS9JBo6WHJ8zWTDClIPTk5Qpb6t535";
#elif DEFINE_BETA
	public const string KEY_AWS_API = "NBUKAKLAnP2FaTgTzkSuz1popyEzKhHp3KzXyB9w";
#else
	public const string KEY_AWS_API = "BHBQBzrUcB32MDnGB2lVs2tU92zDj3Bj6FbBPkha";
#endif
	#endregion

	#region AWS IdentityPoolID.
#if DEFINE_RELEASE || DEFINE_RELEASE_ADHOC
	public const string ID_AWS_IDENTITY = "ap-northeast-1:6a36a084-00a7-4e34-86e6-7a13e4f93301";
#elif DEFINE_BETA
	public const string ID_AWS_IDENTITY = "ap-northeast-1:2025de1f-2fcb-49d3-8739-f22582d97925";
#else
	public const string ID_AWS_IDENTITY = "ap-northeast-1:088870f7-94d2-4c96-b0db-5fc8173ed8e7";
#endif
	#endregion


	/// <summary>接続サーバー環境定義.</summary>
#if DEFINE_RELEASE || DEFINE_RELEASE_ADHOC
    public const string SERVER_ENVIRONMENT = "real";
#elif DEFINE_BETA
	public const string SERVER_ENVIRONMENT = "beta";
#else
	public const string SERVER_ENVIRONMENT = "alpha";
#endif

	/// <summary>URLスキーマの設定.</summary>   
#if DEFINE_RELEASE
	public static readonly string URL_SCHEME = "jp.fg.precatus";
#elif DEFINE_RELEASE_ADHOC
	public static readonly string URL_SCHEME = "com.smilelab.devseven"; // プロビジョニングはアルファのものを使用する.
#elif DEFINE_BETA
	public static readonly string URL_SCHEME = "com.smilelab.seven";
#else
	public static readonly string URL_SCHEME = "com.smilelab.devseven";
#endif
}
