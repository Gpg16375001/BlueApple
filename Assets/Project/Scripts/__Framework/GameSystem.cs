/**
 * @file    GameSystem.cs
 * @brief
 *
 * @author  $Author$
 * @date    $Date$
 * @version $Rev$
 */
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SmileLab 
{
    /// <summary>
    /// クラス：システム周り
    ///    対応プラットフォームは Editor, iOS, Android のみ.
    /// </summary>
    public static class GameSystem
    {
    	#region ディレクトリ関係
        
    	public static void Init()
    	{
    		InitRootPath();
#if		UNITY_EDITOR
    		CachesDirectoryPath	= ApplicationRootPath + "/Caches";
#elif	UNITY_ANDROID
    		CachesDirectoryPath	= Application.persistentDataPath + "/Caches";
#elif	UNITY_IPHONE
    		// /Library/Caches だとメモリ不足の時などに消されてしまう場合あり。なので iCloud にバックアップがとられないディレクトリを自作。
    		CachesDirectoryPath	= ApplicationRootPath + "/Library/PrivateDocuments";
#else
    		CachesDirectoryPath	= ApplicationRootPath + "/Caches";
#endif
    		InitDirectoryPath();
    	}
    	
        /// <summary>
        /// アプリケーション用　永続保存用ディレクトリの絶対パスを返す.
        ///         このディレクトリは、iOS5 では iCloud を通してクラウドサーバーに保存されます。
        /// </summary>
        public static string PermanentDirectoryPath
        {
            get {
#if UNITY_EDITOR
                return ApplicationRootPath + "/Documents";
#elif UNITY_IPHONE
                return Application.persistentDataPath;
#elif UNITY_ANDROID
                return Application.persistentDataPath + "/Documents";
#else
                return ApplicationRootPath + "/Documents";
#endif
            }
        }

        /// <summary>
        /// アプリケーション用　オフラインデータ保存ディレクトリの絶対パスを返す.
        ///
        /// </summary>
        public static string CachesDirectoryPath;

        /// <summary>
        /// アプリケーション用　一時保存ディレクトリの絶対パスを返す.
        ///         iOS ではアプリ起動中の一時保存用
        /// </summary>
        public static string TempDirectoryPath
        {
            get {
#if UNITY_EDITOR
                return ApplicationRootPath + "/tmp";
#elif UNITY_ANDROID
                return Application.persistentDataPath + "/tmp";
#else
                return ApplicationRootPath + "/tmp";
#endif
            }
        }

        /// <summary>
        /// ローカルストレージ内 デバッグ用ファイル配置ディレクトリパス
        /// </summary>
        public static string DebugDirectoryPath
        {
            get {
                return CachesDirectoryPath + "/Debug";
            }
        }

        /// <summary>
        /// ローカルストレージ内 ダウンロードデータのキャッシュ保存ディレクトリパス
        /// </summary>
        public static string DownloadDirectoryPath
        {
            get {
                return CachesDirectoryPath + "/0001";
            }
        }

    	/// <summary>
    	/// ローカルストレージ内 ローカルセーブデータの保存ディレクトリパス
    	/// </summary>
    	public static string LocalSaveDirectoryPath
    	{
    		get { 
    			return CachesDirectoryPath + "/0002";
    		}
    	}

        /// <summary>
        /// アプリケーションディレクトリの絶対パスを返す.
        /// </summary>
        public static string ApplicationRootPath
        {
            get {
                return APPLICATION_ROOT_PATH;
            }
        }
        private static string APPLICATION_ROOT_PATH = null;

        private static void InitRootPath()
        {
            // アプリディレクトリパス作成
#if UNITY_EDITOR
            string path = Application.dataPath;
            APPLICATION_ROOT_PATH = path.Substring(0, path.LastIndexOf('/'));
#elif UNITY_IPHONE
            string path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf('/'));   // /Documents 削除
            APPLICATION_ROOT_PATH = path;
#elif UNITY_ANDROID
            APPLICATION_ROOT_PATH = Application.dataPath;
#endif
            Debug.Log("APPLICATION_ROOT_PATH = " + APPLICATION_ROOT_PATH);
        }
        
        private static void InitDirectoryPath()
        {
            // ディレクトリパス作成
            Debug.Log("***CachesDirectoryPath"+CachesDirectoryPath);
            if( !Directory.Exists(CachesDirectoryPath) ){
                Directory.CreateDirectory(CachesDirectoryPath);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(CachesDirectoryPath);
#endif
            }
            if( !Directory.Exists(DebugDirectoryPath) ){
                Directory.CreateDirectory(DebugDirectoryPath);
            }
            if(!Directory.Exists(DownloadDirectoryPath)) {
                Directory.CreateDirectory(DownloadDirectoryPath);
            }
    		if (!Directory.Exists(LocalSaveDirectoryPath) ) {
    			Directory.CreateDirectory(LocalSaveDirectoryPath);
    		}
#if UNITY_EDITOR || !UNITY_IPHONE
            if( !Directory.Exists(PermanentDirectoryPath) ){
                Directory.CreateDirectory(PermanentDirectoryPath);
            }
            if( !Directory.Exists(TempDirectoryPath) ){
                Directory.CreateDirectory(TempDirectoryPath);
            }
#endif
            Debug.Log("[GameSystem] Init Directory"
                      + "\n\tCache Directory     : " + CachesDirectoryPath
                      + "\n\tDownload Directory     : " + DownloadDirectoryPath
                      + "\n\tLocalSave Directory     : " + LocalSaveDirectoryPath
                      + "\n\tPermanent Directory : " + PermanentDirectoryPath
                      + "\n\tTemp Directory      : " + TempDirectoryPath);
        }

        /// <summary>
        /// 指定ディレクトリのキャッシュ削除.何も指定がなければ全キャッシュクリア.
        /// </summary>
        public static void ClearChache(string directoryPath = null)
        {
            // 引数がNULLなら全消し.
            if (string.IsNullOrEmpty(directoryPath)) {
                if (Directory.Exists(CachesDirectoryPath)) {
                    Directory.Delete(CachesDirectoryPath, true);
                }
                DLCManager.ClearDLCVaersionTable ();
            }
            // 引数が指定されていれば指定されたディレクトリを削除.
            else {
                if (Directory.Exists(directoryPath)) {
                    Directory.Delete(directoryPath, true);
                }
            }
        }
        #endregion


        /// <summary>
        /// プロパティ：StreamingAssets URL
        /// </summary>
        public static string StreamingAssetsURL
        {
            get{
#if UNITY_EDITOR
                return "file://" + Application.dataPath + "/StreamingAssets";
#elif UNITY_IPHONE
                return "file://" + Application.dataPath + "/Raw";
#elif UNITY_ANDROID
                return "jar:file://" + Application.dataPath + "!/assets";
#else
                return "file://" + Application.dataPath + "/StreamingAssets";
#endif
            }
        }

        /// <summary>
        /// 展開しているストア名を取得.
        /// </summary>
		public static string GetPlatformName()
        {
            switch (Application.platform) {
                case RuntimePlatform.Android:
			        {
                        var bAu = DefinePlayerSettings.ANDROID_VERSION_CODE.Substring(0, 1) == "2";
                        return bAu ? StoreTypeEnum.AUMarket.ToString() : StoreTypeEnum.GooglePlay.ToString();
                    }
                case RuntimePlatform.IPhonePlayer:
                    return StoreTypeEnum.AppStore.ToString();
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "windows";    // TODO : 今は使わない
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "osx";       // TODO : 今は使わない
                case RuntimePlatform.WebGLPlayer:
                    return StoreTypeEnum.DMM.ToString();
            }
            Debug.LogError("[GameSystem] GetPlatformName : Not suported platform = " + Application.platform);
            return "other_platform";
        }

            /// <summary>
            /// 展開しているストア名を取得.
            /// </summary>
        public static int GetPlatformIndex()
            {
            switch (Application.platform) {
            case RuntimePlatform.Android:
            {
                var bAu = DefinePlayerSettings.ANDROID_VERSION_CODE.Substring(0, 1) == "2";
                return bAu ? (int)StoreTypeEnum.AUMarket : (int)StoreTypeEnum.GooglePlay;
            }
            case RuntimePlatform.IPhonePlayer:
                return (int)StoreTypeEnum.AppStore;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return 0;    // TODO : 今は使わない
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return 0;       // TODO : 今は使わない
            case RuntimePlatform.WebGLPlayer:
                return (int)StoreTypeEnum.DMM;
            }
            Debug.LogError("[GameSystem] GetPlatformName : Not suported platform = " + Application.platform);
            return 0;
        }

        /// <summary>
        /// ユーザ識別用 ID を生成する.  呼び出すたびに毎回値が変わるため、生成したあとはセーブデータとかキーチェーンに保存すべし.
        /// </summary>
        public static string GenerateUUID()
        {
            return System.Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}