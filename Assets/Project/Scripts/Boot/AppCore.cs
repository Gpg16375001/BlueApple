using UnityEngine;
using UnityEngine.EventSystems;
﻿using System.Collections;
using System.Linq;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// アプリのコアとなる通しで存在させておく必要のあるアプリをまとめたコンポーネント.
/// </summary>
public class AppCore : MonoBehaviour
{

    /// <summary>初期化済み？</summary>
    public static bool IsInit { get; private set; }


    // 破棄,アプリの中核を担うモジュールなのでむやみに呼ばないこと.
    private static void DestroyComponents()
    {
        if(instance == null){
            return;
        }
        foreach(var vb in instance.GetComponentsInChildren<ViewBase>()){
            vb.Dispose();
        }

        DLCManager.DisposeProc();    // バージョン情報の破棄.

        SqliteConnectionManager.ShardInstanse.Dispose();

        IconLoader.Dispose ();

        // イベントシステムの重複登録は許されない.
        var eSys = instance.GetComponentInChildren<EventSystem>();
        if(eSys != null){
            Destroy(eSys.gameObject);    
        }
    }

    void Awake()
    {
        if(instance != null) {
            IsInit = false;
            DestroyComponents();
        }      
    }

    IEnumerator Start()
    {
        // システムパスとフォルダの初期化.
        GameSystem.Init();

        // 各アプリケーションコアコンポーネントの初期化.
        while(CameraHelper.SharedInstance == null){
            yield return null;
        }
        while(ScreenChanger.SharedInstance == null){
            yield return null;
        }
        while(View_FadePanel.SharedInstance == null){
            yield return null;
        }
        while(SoundManager.SharedInstance == null){
            yield return null;
        }
        while(PopupManager.SharedInstance == null) {
            yield return null;
        }
        while(LockInputManager.SharedInstance == null){
            yield return null;
        }
        while(GameTime.SharedInstance == null){
            yield return null;
        }
        while(UtageModule.SharedInstance == null){
            yield return null;
        }
        while(!AndroidBackButton.IsReady){
            yield return null;
        }
        while (PurchaseManager.SharedInstance == null) {
            yield return null;
        }
        // Fadeの設定.カメラの初期化を待たないと出来ないのでここでやる.
		View_FadePanel.SharedInstance.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.FadeCamera);

        SoundManager.SharedInstance.Initialize<CriSoundControll> ();
        SoundManager.SharedInstance.VolumeBGM = AwsModule.LocalData.Volume_BGM;
        SoundManager.SharedInstance.VolumeSE = AwsModule.LocalData.Volume_SE;
        SoundManager.SharedInstance.VolumeVoice = AwsModule.LocalData.Volume_Voice;

        // DLCバージョンシステム初期化.
        DLCManager.Init();
        while(!DLCManager.IsReady) {
            yield return null;
        }

        // MasterData
        MasterDataManager.Init ();
        while(!MasterDataManager.IsReady) {
            yield return null;
        }

        AdditionalFontLoader.SharedInstance.Init ();
        while(!AdditionalFontLoader.SharedInstance.IsReady) {
            yield return null;
        }

        GrowthPushModule.SharedInstance.Init ();
        while (!GrowthPushModule.SharedInstance.IsReady) {
            yield return null;
        }      

		Debug.Log("Android version code=" + DefinePlayerSettings.ANDROID_VERSION_CODE);
        if (MasterDataTable.gem_product != null) {         
#if UNITY_EDITOR
            // 特に何もなし
            string[] productIDs = MasterDataTable.gem_store_setting.DataList.Where(x => x.platform == PlatformEnum.iOS && x.store_type == StoreTypeEnum.AppStore).Select(x => x.store_product_id).ToArray();
            PurchaseManager.SharedInstance.Initialize<EditorDummyPurchaseControll>(productIDs);
#else
#if UNITY_ANDROID
            // GooglePlay or AuMarket
			var bMarket = GameSystem.GetPlatformName() == StoreTypeEnum.AUMarket.ToString();
            if (bMarket) {
                string[] productIDs = MasterDataTable.gem_store_setting.DataList.Where(x => x.platform == PlatformEnum.Android && x.store_type == StoreTypeEnum.AUMarket).Select(x => x.store_product_id).ToArray();
                PurchaseManager.SharedInstance.Initialize<AuMarketControll>(productIDs);
            } else {
                string[] productIDs = MasterDataTable.gem_store_setting.DataList.Where(x => x.platform == PlatformEnum.Android && x.store_type == StoreTypeEnum.GooglePlay).Select(x => x.store_product_id).ToArray();
                PurchaseManager.SharedInstance.Initialize<Prime31GoogleIABControll>(productIDs);
            }
#elif UNITY_IPHONE
            string[] productIDs = MasterDataTable.gem_store_setting.DataList.Where(x => x.platform == PlatformEnum.iOS && x.store_type == StoreTypeEnum.AppStore).Select(x => x.store_product_id).ToArray();
            PurchaseManager.SharedInstance.Initialize<Prime31StoreKitControll>(productIDs);
#elif UNITY_WEBGL
            // TODO: あとでDMMの初期化も必要になる
            string[] productIDs = MasterDataTable.gem_product[PlatformEnum.WebGL, StoreTypeEnum.DMM].Select(x => x.product_id);
#endif
#endif
			// バリデート用の処理を登録する
			PurchaseManager.SharedInstance.ValidateEvent += PurchaseEventListener.Validate;
            PurchaseManager.SharedInstance.SucceedEvent += PurchaseEventListener.OnSucceed;
            PurchaseManager.SharedInstance.ErrorEvent += PurchaseEventListener.OnError;
        }

        // Adjustの初期化
        // PaymentManagerのイベント登録をするためにPaymentの初期化後に初期化
        AdjustModule.SharedInstance.Init ();

        ServerModelCache.Init ();

        IconLoader.Init ();

		// TODO : Authユーザー名が存在すれば書き込む.どっかのタイミングで消す.
        var authUserName = PlayerPrefs.GetString("AuthUserName", "");
        var authUserPass = PlayerPrefs.GetString("AuthPassword", "");
        if (!string.IsNullOrEmpty(authUserName) && !string.IsNullOrEmpty(authUserPass)) {
			AwsModule.UserData.SetAuthUserInfo(authUserName, authUserPass);
            PlayerPrefs.DeleteKey("AuthUserName");
            PlayerPrefs.DeleteKey("AuthPassword");
        }

        if(instance != null && instance.gameObject != null){
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        IsInit = true;
    }

    private static AppCore instance;
}
