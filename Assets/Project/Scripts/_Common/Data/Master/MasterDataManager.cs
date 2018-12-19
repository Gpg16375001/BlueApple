using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SmileLab;
using SmileLab.Net;

/// <summary>
/// マスタデータの取得を提供します。
/// </summary>
public static class MasterDataManager
{
    private static readonly string MasterDataFileName = "masterdata";

    /// <summary>
    /// マスタの使用準備ができていれば真を返します。
    /// </summary>
    private static bool _IsReady = false;
    public static bool IsReady { get { return _IsReady; } }

    private static string MasterDataPath;
    private static Dictionary<string, WeakReference> refs = new Dictionary<string, WeakReference>();

    /// <summary>
    /// 初期化処理を開始します。終了するとIsReadyプロパティがTrueになります。
    /// </summary>
    public static void Init()
    {
        _IsReady = false;


        MasterDataPath = DLCManager.GetDownloadPath (DLCManager.DLC_FOLDER.Master, MasterDataFileName);
		Debug.Log (MasterDataPath);
        DLCManager.DownloadFile (DLCManager.DLC_FOLDER.Master, MasterDataFileName, 
            (result) => {
                _IsReady = true;
            }
        );
    }

    public static T LoadMasterData<T>(string masterName) where T: ScriptableObject
    {
        WeakReference refObj;
        if (refs.TryGetValue (masterName, out refObj)) {
            if (refObj.IsAlive) {
                return refObj.Target as T;
            }
        }

        // assetBundleがnullであればロードする。
        var assetBundle = DLCManager.AssetBundleFromFile(MasterDataPath);

        if (assetBundle == null) {
            return null;
        }
        var masterData = assetBundle.assetbundle.LoadAsset<T> (masterName);
        // この関数ないでloadされていた場合は後片付け。
        assetBundle.Unload (false);
        refs [masterName] = new WeakReference (masterData);
        return masterData;
    }

    public static void LoadPartitionMasterData<T>(string partitionName, Action<T> didLoad) where T: ScriptableObject
    {
        WeakReference refObj;
        if (refs.TryGetValue (partitionName, out refObj)) {
            if (refObj.IsAlive) {
                didLoad(refObj.Target as T);
                return;
            }
        }
        DLCManager.AssetBundleFromDownloadOrCache(DLCManager.DLC_FOLDER.Master, partitionName,
            (assetBundle) => {
                T obj = assetBundle.assetbundle.LoadAsset<T>(partitionName);

                refs [partitionName] = new WeakReference (obj);
                didLoad(obj);

                assetBundle.Unload(false);
            }
        );
    }
}
