using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using MessagePack;
using System.Linq;

using SmileLab;
using SmileLab.Net;


/// <summary>
/// DLCのバージョン管理クラス.
/// TODO: RequestをQueueに積んで実行数を制御するロジックは必要かもしれない。
/// </summary>
public static class DLCManager
{
    /// <summary>
    /// 準備OK？
    /// </summary>
    public static bool IsReady { get; private set; }

    // 
    public enum DLC_FOLDER
    {
        UI = 0,
        Utage,
        Master,
        Sound,
        Font,
        Unit,
        Enemy,
        NPC,
		ScenarioSprite,
        ScenarioBG,
        GachaBG,
        BattleBG,
        Weapon,
        BattleEffect,
        ScreenBG,
        Icon,

        MAX
    }

    private static string[] DLCFolderPaths;
    private static Dictionary<Hash128/*ファイル名のHash128*/, DLCVersionData/*version*/> VersionDataList; // DLCバージョン情報ファイルリスト.
    private static string FileListHash;
    //プラットフォームに対応した格納パス名.
    private static string NamePathPlatform 
    { 
        get {
            if(Application.platform == RuntimePlatform.IPhonePlayer) {
                return "iOS/";
            } else if(Application.platform == RuntimePlatform.Android) {
                return "Android/";
            } else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
                return "OSX/";
            } else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
                return "Windows/";
            } else if(Application.platform == RuntimePlatform.WebGLPlayer) {
                return "WebGL/";
            }
            return "";
        } 
    }

    /// <summary>宴ダウンロードパス.(宴については内部でプラットフォームごとにパスを切り替えてくれる.)</summary>
    private static readonly string _url_asset_utage;
    public static string URL_ASSET_UTAGE { get { return _url_asset_utage; } }

    private static readonly string _url_base;

    private static CoroutineAgent.CoroutineInfo coInfo = null;

    private static int MasterVersion;

    // staticコンストラクタ
    static DLCManager()
    {
        string baseUrl = ClientDefine.URL_DLC;
        string platform = NamePathPlatform;

        DLCFolderPaths = new string[(int)DLC_FOLDER.MAX];

        var s3_asset_ui_path = "UI/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.UI] = s3_asset_ui_path;
        var s3_asset_utage_path = "Utage/";
        DLCFolderPaths[(int)DLC_FOLDER.Utage] = s3_asset_utage_path;
        var s3_asset_master_path = "Master/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Master] = s3_asset_master_path;
        var s3_asset_sound_path = "Sound/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Sound] = s3_asset_sound_path;
        var s3_asset_font_path = "Font/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Font] = s3_asset_font_path;
        var s3_asset_unit_path = "Unit/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Unit] = s3_asset_unit_path;
        var s3_asset_enemy_path = "Enemy/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Enemy] = s3_asset_enemy_path;
		var s3_asset_npc_path = "NPC/" + platform;
		DLCFolderPaths[(int)DLC_FOLDER.NPC] = s3_asset_npc_path;
		var s3_asset_scenariosprite_path = "ScenarioSprite/" + platform;
		DLCFolderPaths[(int)DLC_FOLDER.ScenarioSprite] = s3_asset_scenariosprite_path;
		var s3_asset_scenariobg_path = "ScenarioBG/" + platform;
		DLCFolderPaths[(int)DLC_FOLDER.ScenarioBG] = s3_asset_scenariobg_path;
		var s3_asset_gachabg_path = "GachaBG/" + platform;
		DLCFolderPaths[(int)DLC_FOLDER.GachaBG] = s3_asset_gachabg_path;
        var s3_asset_battlebg_path = "BattleBG/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.BattleBG] = s3_asset_battlebg_path;
        var s3_asset_weapon_path = "Weapon/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Weapon] = s3_asset_weapon_path;
        var s3_asset_battleeffect_path = "BattleEffect/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.BattleEffect] = s3_asset_battleeffect_path;
        var s3_asset_screenbg_path = "ScreenBG/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.ScreenBG] = s3_asset_screenbg_path;
        var s3_asset_magikite_path = "Icon/" + platform;
        DLCFolderPaths[(int)DLC_FOLDER.Icon] = s3_asset_magikite_path;

        _url_asset_utage = baseUrl + s3_asset_utage_path;

        _url_base = string.Format ("{0}Contents/{1}", baseUrl, platform);

        coInfo = null;
    }

    /// <summary>
    /// 破棄する際に読んで欲しい処理.
    /// </summary>
    public static void DisposeProc()
    {
        IsReady = false;
        if (VersionDataList != null) {
            VersionDataList.Clear ();
            VersionDataList = null;
        }

        if (coInfo != null) {
            CoroutineAgent.Stop (coInfo);
            coInfo = null;
        }
        AssetBundleRefs.Clear ();
    }

    static readonly string DBConnectionName = "DLCVersion";
    static readonly string DBFileName = "DLCVersion.db";

    /// <summary>
    /// 初期化.
    /// </summary>
    public static void Init()
    {
        var s = UniRx.Scheduler.MainThread;
        // FileListHashの取得
        SendAPI.FilelistIndex ((sucess, result) => {
            if(!sucess) {
                PopupManager.OpenPopupSystemOK("ファイルバージョン情報の取得に失敗しました。再度情報の取得を行います。",
                    () => {
                        RetryInit();
                    }
                );
                return;
            }

            MasterVersion = result.MasterVersion;
            FileListHash = result.FileListHash;
            // Version管理用DBの初期化
            var dbFilePath = System.IO.Path.Combine (GameSystem.LocalSaveDirectoryPath, DBFileName);
            var connection = SqliteConnectionManager.ShardInstanse.CreateConnection (DBConnectionName, dbFilePath);
            connection.MakeTransaction ((con, transaction) => {
                DLCVersionData.CreateTable (con, transaction);
            });

            // FileList生成.ファイル名(MD5ハッシュ),CRCでリストアップされているのでこれをデータ化してセーブする.
            VersionDataList = new Dictionary<Hash128, DLCVersionData>();
            var fileName = string.Format("{0}_{1}.dat", FileListHash, GetRuntimePlatformID()).ToHashMD5();
            var url = string.Format("{0}FileList/{1}?v={2}", ClientDefine.URL_DLC, fileName, FileListHash);
#if DEBUG
            Debug.Log("FileListHash: " + FileListHash);
			Debug.Log("cipher: " + result.Key);
            Debug.Log(string.Format("{0}_{1}.dat", FileListHash, GetRuntimePlatformID()));
            Debug.Log("Version File Url: " + url);
#endif
            NetRequestManager.Download(url, bytes => {
				var dlcVersionTables = MessagePack.MessagePackSerializer.Deserialize<DLCVersionDataTables>(EncryptHelper.DecryptData(result.Key, bytes));
                VersionDataList = dlcVersionTables.VersionDataList.ToDictionary(x => x.FileNameHash);
                // マスターバージョンのチェックを登録する。
                AwsModule.Request.CheckMasterVersion = CheckMasterVersion;
                IsReady = true;
#if UNITY_EDITOR
                var fileNameHashs = string.Join("\n", VersionDataList.Keys.Select(x => x.ToString()).ToArray());
                Debug.Log(VersionDataList.Count + ": [ \n" + fileNameHashs + "]");
#endif
            });
        });

    }

    private static bool CheckMasterVersion(int masterVersion, Action didCheckThrough, Action didTakesCheck)
    {
        if (masterVersion <= 0) {
            return false;
        }
        if (MasterVersion <= 0) {
            MasterVersion = masterVersion;
            return false;
        }

        var check = MasterVersion != masterVersion;
        if (check) {
            // バージョンを取り直す
            SendAPI.FilelistIndex ((sucess, result) => {
				if(result == null){
					return;     // TODO : どうしようもないエラーなので何もできない.成功ともせずに抜ける.
				}
                if(!sucess) {
                    didCheckThrough();
                }

                MasterVersion = result.MasterVersion;
                if(FileListHash != result.FileListHash) {
                    LockInputManager.SharedInstance.IsLock = false;
                    PopupManager.OpenPopupSystemOK("バージョン更新があります。\nアプリを再起動します。", () => {
                        didTakesCheck();
                        ScreenChanger.SharedInstance.Reboot();
                    });
                } else {
                    didCheckThrough();
                }
            });
        }
        return check;
    }

    private static void RetryInit()
    {
        Init ();
    }

    public static void ClearDLCVaersionTable()
    {
        var connt = SqliteConnectionManager.ShardInstanse.GetConnection (DBConnectionName);
        connt.MakeTransaction ((con, transaction) => {
            DLCVersionData.DropTable(con, transaction);
            DLCVersionData.CreateTable(con, transaction);
        });
    }

    // 現在のプラットフォームのID(RuntimePlatformのenum依存)を取得する.ファイルリストはOS毎に出力されているため識別するのに使用する.
    private static int GetRuntimePlatformID()
    {
        // 開発時に確認できるようにEditorの場合はPlayerとして識別してDLCの確認ができるようにしておく.
        if(Application.platform == RuntimePlatform.OSXEditor){
            return (int)RuntimePlatform.OSXPlayer;
        }
        if(Application.platform == RuntimePlatform.WindowsEditor) {
            return (int)RuntimePlatform.WindowsPlayer;
        }
        return (int)Application.platform;
    }

    #region Parameters
    /// <summary>
    /// Get s3 path.
    /// </summary>
    /// <returns>The s3 path.</returns>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    public static string GetS3Path(DLC_FOLDER folderType, string fileName)
    {
        return string.Format ("{0}{1}", DLCFolderPaths[(int)folderType], fileName);
    }

    /// <summary>
    /// Get download URL.
    /// </summary>
    /// <returns>The download URL.</returns>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="version">Version.</param>
    public static string GetDownloadUrl(DLC_FOLDER folderType, string fileName, Hash128? version = null)
    {
        var s3Path = GetS3Path(folderType, fileName);
        if (!version.HasValue) {
            version = GetVersionFromS3Path (s3Path);
        }
        return GetDownloadUrl (s3Path, version.Value);
    }

    /// <summary>
    /// Get download URL.
    /// </summary>
    /// <returns>The download URL.</returns>
    /// <param name="s3Path">S3 path.</param>
    /// <param name="version">Version.</param>
    public static string GetDownloadUrl(string s3Path, Hash128 version)
    {
        return string.Format ("{0}{1}.{2}", _url_base, s3Path.ToHashMD5(), version);
    }

    public static string GetDownloadUrl(Hash128 fileNameHash, Hash128 version)
    {
        return string.Format ("{0}{1}.{2}", _url_base, fileNameHash.ToString(), version);
    }

    /// <summary>
    /// Creates the DLC URL.
    /// </summary>
    /// <returns>The DLC URL.</returns>
    /// <param name="url">URL.</param>
    public static string CreateDLCUrlForUtage(string url)
    {
        if (!url.StartsWith (ClientDefine.URL_DLC)) {
            return url;
        }
        
        var s3Path = url.Replace (ClientDefine.URL_DLC, "").Split('?')[0];
        var ret = GetDownloadUrl (s3Path, GetVersionFromS3Path (s3Path));      
#if UNITY_EDITOR
        Debug.Log (url +" => "+ ret);
#endif
        return ret;
    }

    /// <summary>
    /// Get download path.
    /// </summary>
    /// <returns>The download path.</returns>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    public static string GetDownloadPath(DLC_FOLDER folderType, string fileName)
    {
        var s3Path = GetS3Path (folderType, fileName);
        return GetDownloadPath(s3Path);
    }

    /// <summary>
    /// Get download path.
    /// </summary>
    /// <returns>The download path.</returns>
    /// <param name="s3Path">S3 path.</param>
    public static string GetDownloadPath(string s3Path)
    {
        // S3のパスをMD5Hashとして保存する。
        return string.Format ("{0}/{1}", GameSystem.DownloadDirectoryPath, s3Path.ToHashMD5());
    }

    public static string GetDownloadPath(Hash128 fileHash)
    {
        // S3のパスをMD5Hashとして保存する。
        return string.Format ("{0}/{1}", GameSystem.DownloadDirectoryPath, fileHash.ToString());
    }

    /// <summary>
    /// Get version.
    /// </summary>
    /// <returns>The version.</returns>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    public static Hash128 GetVersion(DLC_FOLDER folderType, string fileName)
    {
        var s3Path = GetS3Path (folderType, fileName);
        return GetVersionFromS3Path(s3Path);
    }

    /// <summary>
    /// URLから指定ファイルのバージョン番号(Hash128)取得.
    /// </summary>
    public static Hash128 GetVersion(string url)
    {
        if(!IsReady || VersionDataList.Count <= 0){
#if UNITY_EDITOR
            Debug.LogError("[DLCVersionManager] GetVersion Error!! : fileList is null or empty or not ready yet.");
#endif
            return default(Hash128);
        }

        // 各カテゴリー毎のルートディレクトからのパスがキーで入っているのでDLCのURL指定なら取り除く.
        if(url.StartsWith(ClientDefine.URL_DLC, StringComparison.Ordinal)){
            url = url.Remove(0, ClientDefine.URL_DLC.Length);
        }
        // パラメータは無視.
        url = url.Split('?')[0];

        return GetVersionFromS3Path (url);
    }

    /// <summary>
    /// Get version from s3 path.
    /// </summary>
    /// <returns>The version from s3 path.</returns>
    /// <param name="s3Path">S3 path.</param>
    public static Hash128 GetVersionFromS3Path(string s3Path)
    {
        DLCVersionData data = null;
        if(!VersionDataList.TryGetValue(s3Path.ToHash128MD5(), out data)) {
#if UNITY_EDITOR
            Debug.LogError(string.Format("[DLCManager] GetVersion Error!! : not found path = {0}.", s3Path));
#endif
            return default(Hash128);
        }
        return data.FileHash;
    }

    /// <summary>
    /// Get the version data from s3 path.
    /// </summary>
    /// <returns>The version data from s3 path.</returns>
    /// <param name="s3Path">S3 path.</param>
    public static DLCVersionData GetVersionDataFromS3Path(string s3Path)
    {
        DLCVersionData data = null;
        if(!VersionDataList.TryGetValue(s3Path.ToHash128MD5(), out data)) {
#if UNITY_EDITOR
            Debug.LogError(string.Format("[DLCVersionManager] GetVersion Error!! : {0}({1})fileList is null or empty or not ready yet.", s3Path, s3Path.ToHash128MD5()));
#endif
            return null;
        }
        return data;
    }
        
    public static DLCVersionData GetVersionDataFromFileNameHash(Hash128 fileNameHash)
    {
        DLCVersionData data = null;
        if(!VersionDataList.TryGetValue(fileNameHash, out data)) {
            return null;
        }
        return data;
    }

    /// <summary>
    /// 指定URLにマスターバージョンを付与する.すでにバージョンがurlについている場合は何もしない.
    /// </summary>
	public static string AddUrlMasterVersion(string url)
    {
        var parameters = url.Split('?');
        if (parameters.Any(p => p.Contains("v="))) {
            return url;
        }
        var rtn = string.Format("{0}?v={1}", url, MasterVersion);
        return rtn;
    }
    #endregion

    #region Load Methods
    /// <summary>
    /// Default download error.
    /// </summary>
    /// <param name="ex">Ex.</param>
    private static void DefaultDownloadError(Exception ex)
    {
		// TODO: あとでエラー時共通処理を考える。
		PopupManager.OpenPopupSystemOK("通信エラー。再起動します。", () => ScreenChanger.SharedInstance.Reboot());
        Debug.LogException (ex);
    }

    /// <summary>
    /// Download the bytes.
    /// </summary>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="didDownload">Did download.</param>
    /// <param name="doError">Do error.</param>
    public static void DownloadBytes(DLC_FOLDER folderType, string fileName, Action<byte[]> didDownload, Action<Exception> didError = null)
    {
        var s3Path = GetS3Path (folderType, fileName);
        DownloadBytes (s3Path, didDownload, didError);
    }

    public static void DownloadBytes(string s3Path, Action<byte[]> didDownload, Action<Exception> didError = null)
    {
        var fileVersion = GetVersionFromS3Path (s3Path);
        var url = GetDownloadUrl (s3Path, fileVersion);

        NetRequestManager.Download (url, didDownload, didError ?? DefaultDownloadError);
    }


    public static bool IsNeedDownload(string s3Path)
    {
        var versionData = GetVersionDataFromS3Path (s3Path);
        if (versionData == null) {
            return false;
        }
        var path = GetDownloadPath (s3Path);

        return IsNeedDownload (path, s3Path, versionData);
    }

    public static bool IsNeedDownload(string path, string s3Path, DLCVersionData versionData)
    {
        // ローカルファイルのバージョン判定
        if (FileUtility.Exists (path)) {
            var localVestionData = DLCVersionData.Get (SqliteConnectionManager.ShardInstanse.GetConnection(DBConnectionName), s3Path.ToHash128MD5 ());
            if (localVestionData != null && localVestionData.FileHash == versionData.FileHash) {
#if UNITY_EDITOR
                Debug.Log (string.Format(" {0} is local version lastest", s3Path));
#endif
                return false;
            }
        }
        return true;
    }

    public static bool IsNeedDownload(Hash128 fileNameHash)
    {
        var versionData = GetVersionDataFromFileNameHash (fileNameHash);
        if (versionData == null) {
            return false;
        }
        var path = GetDownloadPath (fileNameHash);

        return IsNeedDownload (path, fileNameHash, versionData);
    }

    public static bool IsNeedDownload(string path, Hash128 fileNameHash, DLCVersionData versionData)
    {
        // ローカルファイルのバージョン判定
        if (FileUtility.Exists (path)) {
            var localVestionData = DLCVersionData.Get (SqliteConnectionManager.ShardInstanse.GetConnection(DBConnectionName), fileNameHash);
            if (localVestionData != null && localVestionData.FileHash == versionData.FileHash) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Download the file.
    /// </summary>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="didDownload">Did download.</param>
    /// <param name="doError">Do error.</param>
    public static void DownloadFile(DLC_FOLDER folderType, string fileName, FileDownloadRequest.OnSuccess didDownload, FileDownloadRequest.OnError didError = null, FileDownloadRequest.OnProgress progress = null)
    {
        var s3Path = GetS3Path (folderType, fileName);
        DownloadFile (s3Path, didDownload, didError, progress);
    }

    private static Dictionary<string, FileDownloadRequest> DownloadFileRequestCache = new Dictionary<string, FileDownloadRequest>();
    public static void DownloadFile(string s3Path, FileDownloadRequest.OnSuccess didDownload, FileDownloadRequest.OnError didError = null, FileDownloadRequest.OnProgress progress = null)
    {
        var versionData = GetVersionDataFromS3Path (s3Path);
        if (versionData == null) {
            didError (new System.IO.FileNotFoundException ("Not Found in DLC", s3Path));
            return;
        }
        var url = GetDownloadUrl (s3Path, versionData.FileHash);
        var path = GetDownloadPath (s3Path);

        if (IsNeedDownload (path, s3Path, versionData)) {
            if (DownloadFileRequestCache.ContainsKey (url)) {
                DownloadFileRequestCache [url].DownloadedEvent += didDownload;
                if (didError != null) {
                    DownloadFileRequestCache [url].ErrorEvent += didError;
                }
                if (progress != null) {
                    DownloadFileRequestCache [url].ProgressEvent += progress;
                }
            } else {
                DownloadFileRequestCache.Add (url, new FileDownloadRequest (url));
                DownloadFileRequestCache [url].DownloadedEvent += didDownload;
                if (didError != null) {
                    DownloadFileRequestCache [url].ErrorEvent += didError;
                }
                if (progress != null) {
                    DownloadFileRequestCache [url].ProgressEvent += progress;
                }
                AddLoadProgress (DownloadFileRequestCache [url]);
                NetRequestManager.Download (url, path, null,
                    (result) => {
                        var connection = SqliteConnectionManager.ShardInstanse.GetConnection (DBConnectionName);
                        connection.MakeTransaction ((conn, transaction) => {
                            versionData.Save (conn, transaction);
                        });
                        DownloadFileRequestCache [url].Success(result);
                        DownloadFileRequestCache.Remove(url);
                    },
                    (ex) => {
                        if(DownloadFileRequestCache[url].HasErrorEvent) {
                            DownloadFileRequestCache[url].Error(ex);
                        } else {
                            DefaultDownloadError(ex);
                        }
                        DownloadFileRequestCache.Remove(url);
                    },
                    (value) => {
                        DownloadFileRequestCache[url].Progress(value);
                    }
                );
            }
        } else {
            if (progress != null) {
                progress (1.0f);
            }
            didDownload (true);
        }
    }
    public static void DownloadFile(Hash128 fileNameHash, FileDownloadRequest.OnSuccess didDownload, FileDownloadRequest.OnError didError = null, FileDownloadRequest.OnProgress progress = null)
    {
        var versionData = GetVersionDataFromFileNameHash (fileNameHash);
        if (versionData == null) {
            return;
        }
        var url = GetDownloadUrl (fileNameHash, versionData.FileHash);
        var path = GetDownloadPath (fileNameHash);

        if (IsNeedDownload (path, fileNameHash, versionData)) {
            if (DownloadFileRequestCache.ContainsKey (url)) {
                DownloadFileRequestCache [url].DownloadedEvent += didDownload;
                if (didError != null) {
                    DownloadFileRequestCache [url].ErrorEvent += didError;
                }
                if (progress != null) {
                    DownloadFileRequestCache [url].ProgressEvent += progress;
                }
            } else {
                DownloadFileRequestCache.Add (url, new FileDownloadRequest (url));
                DownloadFileRequestCache [url].DownloadedEvent += didDownload;
                if (didError != null) {
                    DownloadFileRequestCache [url].ErrorEvent += didError;
                }
                if (progress != null) {
                    DownloadFileRequestCache [url].ProgressEvent += progress;
                }
                AddLoadProgress (DownloadFileRequestCache [url]);
                NetRequestManager.Download (url, path, null,
                    (result) => {
                        var connection = SqliteConnectionManager.ShardInstanse.GetConnection (DBConnectionName);
                        connection.MakeTransaction ((conn, transaction) => {
                            versionData.Save (conn, transaction);
                        });
                        DownloadFileRequestCache [url].Success(result);
                        DownloadFileRequestCache.Remove(url);
                    },
                    (ex) => {
                        if(DownloadFileRequestCache[url].HasErrorEvent) {
                            DownloadFileRequestCache[url].Error(ex);
                        } else {
                            DefaultDownloadError(ex);
                        }
                        DownloadFileRequestCache.Remove(url);
                    },
                    (value) => {
                        DownloadFileRequestCache[url].Progress(value);
                    }
                );
            }
        } else {
            if (progress != null) {
                progress (1.0f);
            }
            didDownload (true);
        }
    }

    /// <summary>
    /// Downloads the files.
    /// </summary>
    /// <param name="s3Paths">S3 paths.</param>
    /// <param name="didDownload">Did download.</param>
    /// <param name="didError">Did error.</param>
    /// <param name="progress">Progress.</param>
    public static void DownloadFiles(IEnumerable<string> s3Paths, Action<bool> didDownload,
        Action<Exception> didError = null, Action<float> progress = null)
    {
        // ダウンロードが必要なファイル一覧を取得
        var downloadS3Paths = s3Paths.Distinct().Where(x => !string.IsNullOrEmpty(x) && IsNeedDownload(x));

        // 適当な個数ずつDownloadを行う。
        int downloadCount = downloadS3Paths.Count ();
        if (downloadCount <= 0) {
            if(progress != null) progress (1.0f);
            didDownload (true);
            return;
        }

        coInfo = CoroutineAgent.CreateCoroutine (CoDownloadFiles(downloadS3Paths, didDownload, didError, progress));
        CoroutineAgent.Execute (coInfo);
    }

    private static IEnumerator CoDownloadFiles(IEnumerable<string> downloadS3Paths, Action<bool> didDownload,
        Action<Exception> didError = null, Action<float> progress = null)
    {
        float MaxValue = (float)downloadS3Paths.Count ();
        float Value = 0.0f;
        float EndCount = 0.0f;
        foreach (var path in downloadS3Paths) {
            bool isDownloadWait = true;

            DownloadFile (path, 
                (result) => {
                    EndCount += 1.0f;
                    isDownloadWait = false;
                },
                (ex) => {
                    if(didError != null) { didError(ex); }
                    else { DefaultDownloadError(ex); }
                    isDownloadWait = false;
                },
                (value) => {
                    Value = EndCount + value;
                    if(progress != null) {
                        progress(Value / MaxValue);
                    }
                }
            );
                
            yield return new WaitUntil(() => !isDownloadWait);
            if(progress != null) progress(EndCount / MaxValue);
        }
        didDownload (true);
        coInfo = null;
    }

    public static long DownloadMinimumContentsFilesSize()
    {
        var downloadFileSize = VersionDataList.Where (x => x.Value.IsMinimumContents && IsNeedDownload (x.Key)).Sum (x => x.Value.FileSize);
        return downloadFileSize;
    }

    public static void DownloadMinimumContentsFiles(Action<bool> didDownload,
        Action<Exception> didError = null, Action<float, int, int> progress = null)
    {
        var downloadFileNameHashs = VersionDataList.Where(x => x.Value.IsMinimumContents && !x.Value.IsExcludeDownload && IsNeedDownload(x.Key)).Select(x => x.Key);
        int downloadCount = downloadFileNameHashs.Count ();
        if (downloadCount <= 0) {
            if(progress != null) progress (1.0f, 0, 0);
            didDownload (true);
            return;
        }

        if (progress != null) {
            progress (0.0f, 0, downloadCount);
        }
        coInfo = CoroutineAgent.CreateCoroutine (CoDownloadFiles(downloadFileNameHashs, didDownload, didError, progress));
        CoroutineAgent.Execute (coInfo);
    }

    public static long DownloadAllFilesSize()
    {
        var downloadFileSize = VersionDataList.Where (x => !x.Value.IsExcludeDownload && IsNeedDownload (x.Key)).Sum (x => x.Value.FileSize);
        return downloadFileSize;
    }

    public static void DownloadAllFiles(Action<bool> didDownload,
        Action<Exception> didError = null, Action<float, int, int> progress = null)
    {
        var downloadFileNameHashs = VersionDataList.Where(x => !x.Value.IsExcludeDownload && IsNeedDownload(x.Key)).Select(x => x.Key);
        int downloadCount = downloadFileNameHashs.Count ();
        if (downloadCount <= 0) {
            if(progress != null) progress (1.0f, 0, 0);
            didDownload (true);
            return;
        }

        if (progress != null) {
            progress (0.0f, 0, downloadCount);
        }
        coInfo = CoroutineAgent.CreateCoroutine (CoDownloadFiles(downloadFileNameHashs, didDownload, didError, progress));
        CoroutineAgent.Execute (coInfo);
    }

    private static IEnumerator CoDownloadFiles(IEnumerable<Hash128> downloadS3Paths, Action<bool> didDownload,
        Action<Exception> didError = null, Action<float, int, int> progress = null)
    {
        float MaxValue = (float)downloadS3Paths.Count ();
        float Value = 0.0f;
        float EndCount = 0.0f;
        foreach (var path in downloadS3Paths) {
            bool isDownloadWait = true;

            DownloadFile (path, 
                (result) => {
                    EndCount += 1.0f;
                    isDownloadWait = false;
                },
                (ex) => {
                    if(didError != null) { didError(ex); }
                    else { DefaultDownloadError(ex); }
                    isDownloadWait = false;
                },
                (value) => {
                    Value = EndCount + value;
                    if(progress != null) {
                        progress(Value / MaxValue, (int)EndCount, (int)MaxValue);
                    }
                }
            );

            yield return new WaitUntil(() => !isDownloadWait);
            if(progress != null) progress(EndCount / MaxValue, (int)EndCount, (int)MaxValue);
        }
        didDownload (true);
        coInfo = null;
    }

    static Dictionary<string, AssetBundleRef> AssetBundleRefs = new Dictionary<string, AssetBundleRef>();
    private static AssetBundleRef GetAssetBundleReference(string key)
    {
        foreach(var removeKey in AssetBundleRefs.Where (x => !x.Value.IsAlive).Select (x => x.Key).ToArray ()) {
            AssetBundleRefs.Remove (removeKey);
        }

        AssetBundleRef reference = null;
        AssetBundleRefs.TryGetValue (key, out reference);
        return reference;
    }
    /// <summary>
    /// AssetsBundle from file.
    /// </summary>
    /// <returns>The bundle from file.</returns>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    public static AssetBundleRef AssetBundleFromFile(DLC_FOLDER folderType, string fileName)
    {
        var path = GetDownloadPath (folderType, fileName);
        return AssetBundleFromFile (path);
    }

    /// <summary>
    /// AssetsBundle from file.
    /// </summary>
    /// <returns>The bundle from file.</returns>
    /// <param name="path">Path.</param>
    public static AssetBundleRef AssetBundleFromFile(string path)
    {
        if (!FileUtility.Exists (path)) {
            return null;
        }
        AssetBundleRef assetBundleRef = GetAssetBundleReference(path);
        if (assetBundleRef == null || (assetBundleRef != null && !assetBundleRef.IsAlive)) {
            assetBundleRef = new AssetBundleRef(AssetBundle.LoadFromFile (path));
            if (AssetBundleRefs.ContainsKey (path)) {
                AssetBundleRefs.Remove (path);
            }
            AssetBundleRefs.Add (path, assetBundleRef);
        }
        assetBundleRef.AddRef ();
        return assetBundleRef;
    }

    public static IEnumerator CoAssetBundleFromFileAsync(string path, Action<AssetBundleRef> didLoad)
    {
        if (didLoad == null) {
            yield break;
        }

        if (!FileUtility.Exists (path)) {
            didLoad(null);
            yield break;
        }
        AssetBundleRef assetBundleRef = GetAssetBundleReference(path);
        if (assetBundleRef != null && assetBundleRef.IsAlive) {
            assetBundleRef.AddRef ();
            if (assetBundleRef.IsLoading) {
                yield return new WaitUntil (() => !assetBundleRef.IsLoading);
            }
            didLoad(assetBundleRef);
            yield break;
        }

        assetBundleRef = new AssetBundleRef ();
        assetBundleRef.AddRef ();
        AssetBundleRefs.Add (path, assetBundleRef);

        var creater = AssetBundle.LoadFromFileAsync (path);
        yield return creater;

        if (creater.assetBundle == null) {
            // エラー発生
            // 該当ファイルを削除する。
            if (FileUtility.Exists (path)) {
                FileUtility.Delete (path);
            }
        }

        assetBundleRef.SetAssetBundle (creater.assetBundle);

        didLoad(assetBundleRef);
    }

    public static IEnumerator CoAssetBundleFromFileAsync(string path, Func<AssetBundleRef, IEnumerator> didLoad)
    {
        if (didLoad == null) {
            yield break;
        }

        if (!FileUtility.Exists (path)) {
            didLoad(null);
            yield break;
        }
        AssetBundleRef assetBundleRef = GetAssetBundleReference(path);
        if (assetBundleRef != null && assetBundleRef.IsAlive) {
            assetBundleRef.AddRef ();
            if (assetBundleRef.IsLoading) {
                yield return new WaitUntil (() => !assetBundleRef.IsLoading);
            }
            yield return didLoad(assetBundleRef);
            yield break;
        }

        assetBundleRef = new AssetBundleRef ();
        assetBundleRef.AddRef ();
        AssetBundleRefs.Add (path, assetBundleRef);

        var creater = AssetBundle.LoadFromFileAsync (path);
        yield return creater;

        assetBundleRef.SetAssetBundle (creater.assetBundle);
        yield return didLoad(assetBundleRef);
    }

    /// <summary>
    /// AssetsBundle from file or download.
    /// </summary>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="didLoad">Did load.</param>
    public static void AssetBundleFromFileOrDownload(DLC_FOLDER folderType, string fileName, AssetBundleLoadRequest.OnSuccess didLoad, FileDownloadRequest.OnError didError = null)
    {
        var path = GetDownloadPath (folderType, fileName);
        // ファイルが存在しない場合はダウンロードを行いロードして返す。
        DownloadFile (folderType, fileName, (result) => {
            AssetBundleRef assetbundleRef = GetAssetBundleReference(path);
            if(assetbundleRef != null && assetbundleRef.IsAlive && !assetbundleRef.IsLoading) {
                assetbundleRef.AddRef();
                didLoad(assetbundleRef);
            } else {
                CoroutineAgent.Execute(CoAssetBundleFromFileAsync(path, (abref) => {
                    didLoad(abref);
                }));
            }
        }, didError);
    }

    public static void AssetBundleFromFileOrDownload(string s3path, AssetBundleLoadRequest.OnSuccess didLoad, FileDownloadRequest.OnError didError = null)
    {
        // ファイルが存在しない場合はダウンロードを行いロードして返す。
        DownloadFile (s3path, (result) => {
            var path = GetDownloadPath (s3path);
            AssetBundleRef assetbundleRef = GetAssetBundleReference(path);
            if(assetbundleRef != null && assetbundleRef.IsAlive && !assetbundleRef.IsLoading) {
                assetbundleRef.AddRef();
                didLoad(assetbundleRef);
            } else {
                CoroutineAgent.Execute(CoAssetBundleFromFileAsync(path, (abref) => {
                    didLoad(abref);
                }));
            }
        }, didError);
    }

    public static void AssetBundleFromFileOrDownload<T>(DLC_FOLDER folderType, string fileName, string assetName, Action<T> didLoad, FileDownloadRequest.OnError didError = null) where T : UnityEngine.Object
    {
        var path = GetDownloadPath (folderType, fileName);
        // ファイルが存在しない場合はダウンロードを行いロードして返す。
        DownloadFile (folderType, fileName, (result) => {
            AssetBundleRef assetbundleRef = GetAssetBundleReference(path);
            if(assetbundleRef != null && assetbundleRef.IsAlive && !assetbundleRef.IsLoading) {
                assetbundleRef.AddRef();
                CoroutineAgent.Execute(CoLoadAssetAsync(assetbundleRef, assetName, didLoad));
            } else {
                CoroutineAgent.Execute(CoAssetBundleFromFileAsync(path, (abref) => CoLoadAssetAsync(abref, assetName, didLoad)));
            }
        }, didError);
    }

    public static void AssetBundleFromFileOrDownload<T>(string s3path, string assetName, Action<T> didLoad, FileDownloadRequest.OnError didError = null) where T : UnityEngine.Object
    {
        // ファイルが存在しない場合はダウンロードを行いロードして返す。
        DownloadFile (s3path, (result) => {
            var path = GetDownloadPath (s3path);
            AssetBundleRef assetbundleRef = GetAssetBundleReference(path);
            if(assetbundleRef != null && assetbundleRef.IsAlive && !assetbundleRef.IsLoading) {
                assetbundleRef.AddRef();
                CoroutineAgent.Execute(CoLoadAssetAsync(assetbundleRef, assetName, didLoad));
            } else {
                CoroutineAgent.Execute(CoAssetBundleFromFileAsync(path, (abref) => CoLoadAssetAsync(abref, assetName, didLoad)));
            }

        }, didError);
    }

    /// <summary>
    /// AssetsBundle from download or cache.
    /// 他のコールバックからも結果をLoadすることがあるからコールバックないからUnloadはしないでください！
    /// </summary>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="didLoad">Did load.</param>
    public static void AssetBundleFromDownloadOrCache(DLC_FOLDER folderType, string fileName,
        AssetBundleLoadRequest.OnSuccess didLoad, AssetBundleLoadRequest.OnError didError = null, AssetBundleLoadRequest.OnProgress progress = null)
    {
        var s3Path = GetS3Path (folderType, fileName);
        AssetBundleFromDownloadOrCache(s3Path, didLoad, didError, progress);
    }

    static Dictionary<string, AssetBundleLoadRequest> AssetBundleLoadRequestCache = new Dictionary<string, AssetBundleLoadRequest> ();
    public static void AssetBundleFromDownloadOrCache(string s3Path,
        AssetBundleLoadRequest.OnSuccess didLoad, AssetBundleLoadRequest.OnError didError = null, AssetBundleLoadRequest.OnProgress progress = null)
    {
        var fileVersion = GetVersionFromS3Path (s3Path);
        if (fileVersion == default(Hash128)) {
            var errorFunc = didError ?? DefaultDownloadError;
            errorFunc (new System.IO.FileNotFoundException ("Not Found in DLC", s3Path));
            return;
        }
        var url = GetDownloadUrl (s3Path, fileVersion);

        AssetBundleRef assetBundleRef = GetAssetBundleReference(url);
        if (assetBundleRef != null && assetBundleRef.IsAlive) {
            assetBundleRef.AddRef();
            if (progress != null) {
                progress (1.0f);
            }
            didLoad (assetBundleRef);
            return;
        }

        if (AssetBundleLoadRequestCache.ContainsKey (url)) {
            AssetBundleLoadRequestCache [url].DownloadedEvent += didLoad;
            AssetBundleLoadRequestCache [url].ErrorEvent += didError;
            AssetBundleLoadRequestCache [url].ProgressEvent += progress;
            return;
        }
        AssetBundleLoadRequestCache.Add (url, new AssetBundleLoadRequest(url));
        AssetBundleLoadRequestCache [url].DownloadedEvent += didLoad;
        AssetBundleLoadRequestCache [url].ErrorEvent += didError;
        AssetBundleLoadRequestCache [url].ProgressEvent += progress;
        AddLoadProgress (AssetBundleLoadRequestCache [url]);

        NetRequestManager.LoadFromCacheOrDownload (url, fileVersion,
            (assetbundle) => {
                assetBundleRef = new AssetBundleRef(assetbundle);

                assetBundleRef.AddRef(AssetBundleLoadRequestCache [url].Count);

                AssetBundleRefs.Add (url, assetBundleRef);

                AssetBundleLoadRequestCache [url].Success(assetBundleRef);
                AssetBundleLoadRequestCache.Remove(url);
            },
            (ex) => {
                if(!AssetBundleLoadRequestCache [url].HasErrorEvent) {
                    DefaultDownloadError(ex);
                } else {
                    AssetBundleLoadRequestCache [url].Error(ex);
                }
            },
            (value) => {
                AssetBundleLoadRequestCache [url].Progress(value);
            }
        );
    }

    /// <summary>
    /// AssetsBundle from download or cache.
    /// </summary>
    /// <param name="folderType">Folder type.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="assetName">Asset name.</param>
    /// <param name="didLoad">Did load.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static void AssetBundleFromDownloadOrCache<T>(DLC_FOLDER folderType, string fileName,
        string assetName, Action<T> didLoad, AssetBundleLoadRequest.OnError didError = null, AssetBundleLoadRequest.OnProgress progress = null) where T : UnityEngine.Object
    {
        var s3Path = GetS3Path (folderType, fileName);
        AssetBundleFromDownloadOrCache<T>(s3Path, assetName, didLoad, didError);
    }

    public static void AssetBundleFromDownloadOrCache<T>(string s3Path, string assetName,
        Action<T> didLoad, AssetBundleLoadRequest.OnError didError = null, AssetBundleLoadRequest.OnProgress progress = null) where T : UnityEngine.Object
    {
        AssetBundleFromDownloadOrCache (s3Path, (assetBundleRef) => {
            CoroutineAgent.Execute(CoLoadAssetAsync(assetBundleRef, assetName, didLoad));
        }, didError ?? DefaultDownloadError, progress);
    }

    private static IEnumerator CoLoadAssetAsync<T>(AssetBundleRef assetBundleRef, string assetName, Action<T> didLoad) where T : UnityEngine.Object
    {
        var assetAsync = assetBundleRef.assetbundle.LoadAssetAsync<T>(assetName);
        yield return assetAsync;
        var asset = assetAsync.asset as T;
        if((asset is GameObject)) {
            asset = UnityEngine.Object.Instantiate(asset) as T;
        }
        didLoad(asset);
        assetBundleRef.Unload(false);
    }

    static List<ILoadProgress> m_LoadProgresss;
    public static void StartProgress()
    {
        m_LoadProgresss = new List<ILoadProgress> ();
    }

    private static void AddLoadProgress(ILoadProgress progress)
    {
        if (m_LoadProgresss != null) {
            m_LoadProgresss.Add (progress);
        }
    }

    public static float AllProgress()
    {
        if (m_LoadProgresss == null || m_LoadProgresss.Count <= 0) {
            return 1.0f;
        }
        float maxValue = (float)m_LoadProgresss.Count();
        float nowValue = m_LoadProgresss.Sum(x => x.ProgressValue);

        return nowValue / maxValue;
    }

    public static void EndProgress()
    {
        if (m_LoadProgresss != null) {
            m_LoadProgresss.Clear ();
        }
        m_LoadProgresss = null;
    }

    /// <summary>
    /// バナーのダウンロード処理
    /// </summary>
    /// <param name="imageNames">Image names.</param>
    /// <param name="didDownload">Did download.</param>
    public static void StartBannerDownload(string[] imageNames, Action<string, Sprite> didDownload)
    {
        if (coBannerDownload != null) {
            CoroutineAgent.Stop (coBannerDownload);
            coBannerDownload = null;
        }
        coBannerDownload = CoroutineAgent.Execute (CoBannerDowload(imageNames, didDownload));
    }

    public static void StopBannerDownload()
    {
        if (coBannerDownload != null) {
            CoroutineAgent.Stop (coBannerDownload);
            coBannerDownload = null;
        }
    }

    private static CoroutineAgent.CoroutineInfo coBannerDownload;
    private static IEnumerator CoBannerDowload(string[] imageNames, Action<string, Sprite> didDownload)
    {
        int downloadCount = imageNames.Length;

        for (int i = 0; i < downloadCount; ++i) {
            Sprite ret = null;
            bool wait = true;
			NetRequestManager.DownloadSprite (string.Format("{0}/Banner/{1}?v={2}", ClientDefine.URL_DLC, imageNames[i], MasterVersion),
                (spt) => {
                    ret = spt;
                    wait = false;
                },
                (ex) => {
                    wait = false;
                }
            );

            yield return new WaitUntil (() => !wait);

            didDownload (imageNames[i], ret);
        }
        coBannerDownload = null;
    }

    /// .mp4形式のファイルをダウンロード or キャッシュからロードする.VideoPlayerでの使用を考慮してローカルファイルのパスを返す.
	public static void Mp4FromDownloadOrCache(string movieName, Action<string/*localURL*/> didDownload)
	{
		if (coMovieDownload != null) {
			CoroutineAgent.Stop(coMovieDownload);
			coMovieDownload = null;
        }

		string savePath = string.Format("{0}/{1}", GameSystem.DownloadDirectoryPath, movieName);
		savePath += savePath.EndsWith(".mp4", StringComparison.Ordinal) ? "" : ".mp4";
		if(System.IO.File.Exists(savePath)){
			didDownload(savePath);
			return;
		}
		coMovieDownload = CoroutineAgent.Execute(CoMovieDowload(movieName, savePath, didDownload));
	}
 
	private static CoroutineAgent.CoroutineInfo coMovieDownload = null;
	private static IEnumerator CoMovieDowload(string movieName, string savePath, Action<string/*localURL*/> didDownload)
    {
        bool wait = true;      
		NetRequestManager.Download(string.Format("{0}/Movie/{1}?v={2}", ClientDefine.URL_DLC, movieName, MasterVersion),
            (bytes) => {
				System.IO.File.WriteAllBytes(savePath, bytes);
                wait = false;
            },
            (ex) => {
                wait = false;
            }
        );

        yield return new WaitUntil(() => !wait);

		didDownload(savePath);
		coMovieDownload = null;
    }

    /// <summary>
    /// 別途あげているお知らせの単体ロード.ファイルリストのバージョン管理外.
    /// </summary>
	public static void NoticeFromDownloadOrCache(Action<CommonNoticeTable> didDownload, bool bErrorCheck = false)
	{
        var fileName = "masterdata_noticeonly";      
		var url_base = string.Format("{0}Notice/{1}", ClientDefine.URL_DLC, NamePathPlatform);
		var s3Path = string.Format("{0}{1}", url_base, fileName);
		var path = GetDownloadPath(s3Path);
		var url = AddUrlMasterVersion(s3Path);
		Debug.Log("NoticeFromDownloadOrCache : "+url);

		NetRequestManager.Download(url, path, null,
            (bSuccess) => {
			    if (!bSuccess) {
                    didDownload(null);
                    return;
                }
			    var assetBundleRef = AssetBundleFromFile(path);
                if (assetBundleRef == null) {
                    didDownload(null);
                    return;
                }
				var masterData = assetBundleRef.assetbundle.LoadAsset<CommonNoticeTable>("notice.asset");
                assetBundleRef.Unload(false);
                didDownload(masterData);
            },
            (ex) => {
    			if(bErrorCheck){
				    DefaultDownloadError(ex);
    			}
				didDownload(null);
            }
        );
	}
    #endregion
}
