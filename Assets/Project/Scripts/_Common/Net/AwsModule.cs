using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using System;
using System.Collections;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// AWS操作用コンポーネント.GlareではAWS SDKも用いてクライアント側でデータ管理を行う.
/// </summary>
public class AwsModule : ViewBase
{
	#region クライアント側で保持するデータ群.

	/// <summary>
	/// AWS保存データ.ローカル保存したいデータを保存.
	/// </summary>
	public static AwsLocalSaveData LocalData { get; private set; }

	/// <summary>
	/// ローカル保存したいユーザーデータを保存.
	/// </summary>
	public static AwsLocalUserData UserData { get; private set; }

	/// <summary>
	/// ユーザーのゲーム進行情報データ.
	/// </summary>
	public static AwsUserProgressData ProgressData { get; private set; }

	/// <summary>
	/// バトル進行情報データ.
	/// </summary>
	public static AwsBattleProgressData BattleData { get; private set; }

	/// <summary>
	/// 編成データ.
	/// </summary>
	public static AwsPartyData PartyData { get; private set; }

	/// <summary>
	/// カード毎の変更データ.
	/// </summary>
	public static AwsCardModifiedData CardModifiedData { get; private set; }
 
    /// <summary>
    /// お知らせの見てる見てないデータ.
    /// </summary>
	public static AwsNotesModifiedData NotesModifiedData { get; private set; }

    #endregion

    /// <summary>
    /// AWSを用いたリクエスト.
    /// </summary>
    public static AwsRequestAPI Request { get; private set; }


    /// <summary>
    /// ログイン事後処理.各種CoginitoDataSetをシンクするために必要.
    /// </summary>
    public static void AuthLoggedin(string identityId, string token)
    {
        credentials.AuthLoggedin(identityId, token);
    }

    public static void AllDelete()
    {
        if (UserData.IsExistAcount) {
            UserData.Delete ();
            LocalData.Delete ();
            ProgressData.Delete ();
            BattleData.Delete ();
            PartyData.Delete ();
            CardModifiedData.Delete ();

            cognitoSyncManager.Dispose ();
            cognitoSyncManager = null;

            credentials.Clear ();

            // リクエストロジック初期化.
            AwsRequestAPI.AuthToken = string.Empty;
        }
    }

    private void Awake()
    {
        this.InitInternal();
    }
    // 内部初期化.
    private void InitInternal()
    {
        // AWS共通初期化.
        // リブート時に自身が破棄されるとUnityInitializerで再設定ができない為AppCore管轄外のDontDestroyなオブジェクトを生成しアタッチしておく.
        if(!isInit) {
            var go = new GameObject("AwsGameObject");
            DontDestroyOnLoad(go);
            UnityInitializer.AttachToGameObject(go);
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;    // DeleteObjectなど幾つかのリクエストは、UnityWebRequestを指定しないとエラーになる。
            isInit = true;
        }

        // 保存データ各種初期化.
		var identityId = ClientDefine.ID_AWS_IDENTITY;
        credentials = new AwsRequestCredentials(identityId, RegionEndpoint.APNortheast1, "Glare");
        cognitoSyncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig { RegionEndpoint = RegionEndpoint.APNortheast1 });
        UserData = new AwsLocalUserData(cognitoSyncManager);
        LocalData = new AwsLocalSaveData(cognitoSyncManager);
        ProgressData = new AwsUserProgressData(cognitoSyncManager);
        BattleData = new AwsBattleProgressData(cognitoSyncManager);
        PartyData = new AwsPartyData (cognitoSyncManager);
		CardModifiedData = new AwsCardModifiedData(cognitoSyncManager);
		NotesModifiedData = new AwsNotesModifiedData(cognitoSyncManager);

        // リクエストロジック初期化.
        Request = new AwsRequestAPI();
    }

    public static void ResetDataSet(Action resetEnd)
    {
        m_ResetEnd = resetEnd;

        // authが通っているはずなのでそのままSyncManagerを初期化する。
        cognitoSyncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig { RegionEndpoint = RegionEndpoint.APNortheast1 });

        // DataSetを作り直す
        UserData = new AwsLocalUserData(cognitoSyncManager);
        LocalData = new AwsLocalSaveData(cognitoSyncManager);
        ProgressData = new AwsUserProgressData(cognitoSyncManager);
        BattleData = new AwsBattleProgressData(cognitoSyncManager);
        PartyData = new AwsPartyData (cognitoSyncManager);
        CardModifiedData = new AwsCardModifiedData(cognitoSyncManager);
		NotesModifiedData = new AwsNotesModifiedData(cognitoSyncManager);

        UserData.CheckoutDateSet = true;
        LocalData.CheckoutDateSet = true;
        ProgressData.CheckoutDateSet = true;
        BattleData.CheckoutDateSet = true;
        PartyData.CheckoutDateSet = true;
        CardModifiedData.CheckoutDateSet = true;
		NotesModifiedData.CheckoutDateSet = true;

        // サーバーデータとSyncする。
        syncEndCount = 0;
        syncRequestCount = 7;
        RertySync (UserData.OriginalSync, SyncResult);
        RertySync (LocalData.Sync, SyncResult);
        RertySync (ProgressData.Sync, () => {
            if(ProgressData.TutorialStageNum >= 0) {
                ProgressData.UpdateTutorialPoint(-1, false);
            }
            SyncResult();
        });
        RertySync (BattleData.Sync, SyncResult);
        RertySync (PartyData.OriginalSync, SyncResult);
        RertySync (CardModifiedData.Sync, SyncResult);
        RertySync (NotesModifiedData.Sync, SyncResult);
    }


    static void RertySync(Action<AwsCognitoDatasetBase.DidSyncDelegate> sync, Action success)
    {
        sync ((isSuccess, sender, e) => {
            if(isSuccess) {
                success();
            } else {
                RertySync(sync, success);
            }
        });
    }

    static void SyncResult()
    {
        syncEndCount++;
        if (syncEndCount == syncRequestCount) {
            if (m_ResetEnd != null) {
                m_ResetEnd ();
            }
        }
    }
    private static int syncEndCount;
    private static int syncRequestCount;
    private static Action m_ResetEnd;

    private static bool isInit = false;
    private static AwsRequestCredentials credentials;
    private static CognitoSyncManager cognitoSyncManager;
}
