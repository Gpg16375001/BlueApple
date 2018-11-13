using System;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

abstract public class AwsCognitoDatasetBase : IDisposable
{
    public bool CheckoutDateSet = false;
    /// <summary>
    /// Sync終了時呼び出し関数の定義
    /// success true: Sync成功 false: Sync失敗
    /// </summary>
    public delegate void DidSyncDelegate(bool success, object sender, EventArgs e);

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="mng">CognitoSyncManager</param>
    /// <param name="datasetName">Dataset名</param>
    public AwsCognitoDatasetBase(CognitoSyncManager mng, string datasetName)
    {
        // 作成済みか判断する。
        bool isCreated = mng.ListDatasets ().Any (x => x.DatasetName == datasetName);

        m_Data = mng.OpenOrCreateDataset(datasetName);
        m_Data.OnSyncSuccess += OnSyncSuccess;
        m_Data.OnSyncFailure += OnSyncFailure;
        m_Data.OnSyncConflict = OnSyncConflict;
        m_Data.OnDatasetMerged = DatasetMerged;
        m_Data.OnDatasetDeleted = DatasetDeleted;

        // 初回作成時
        if (!isCreated) {
            ClearValues ();
        }
    }

    /// <summary>
    /// DatesetのSync処理
    /// </summary>
    /// <param name="didCallback">Sync終了時呼び出し</param>
    public virtual void Sync(DidSyncDelegate didCallback = null)
    {
        m_DidCallback = didCallback;
        m_Data.SynchronizeAsync();
    }

    /// <summary>
    /// Releases all resource used by the <see cref="AwsCognitoDatasetBase"/> object.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="AwsCognitoDatasetBase"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="AwsCognitoDatasetBase"/> in an unusable state. After calling
    /// <see cref="Dispose"/>, you must release all references to the <see cref="AwsCognitoDatasetBase"/> so the garbage
    /// collector can reclaim the memory that the <see cref="AwsCognitoDatasetBase"/> was occupying.</remarks>
    public virtual void Dispose()
    {
        m_Data.OnSyncSuccess -= OnSyncSuccess;
        m_Data.OnSyncFailure -= OnSyncFailure;
        m_Data.OnSyncConflict = null;
        m_Data.OnDatasetMerged = null;
        m_Data.OnDatasetDeleted = null;
        m_Data.ClearAllDelegates ();
        m_Data.Dispose ();
        m_Data = null;
    }

    public virtual void Delete()
    {
        m_Data.Delete ();
        Dispose ();
    }

    public virtual bool Modify()
    {
        foreach(var key in m_Data.ActiveRecords.Keys) {
            if(m_Data.IsModified(key)) {
                return true;
            }
        }
        return false;
    }
    #region Dataset Controll
    /// <summary>
    /// 初期化時呼び出し関数
    /// </summary>
    protected abstract void ClearValues();

    public void RemoveAllKey()
    {
        foreach (var recode in m_Data.Records) {
            m_Data.Remove (recode.Key);
        }
    }
    /// <summary>
    /// Datasetからの値の取得
    /// </summary>
    /// <param name="key">Key.</param>
    /// <typeparam name="T">取得データのType.</typeparam>
    protected T Get<T>(string key)
    {
        string data = m_Data.Get (key);
        if (!string.IsNullOrEmpty(data)) {
            var type = typeof(T);
            if (type != typeof(string) && !type.IsPrimitive) {
                return UnityEngine.JsonUtility.FromJson<T> (data);
            } else {
                return (T)Convert.ChangeType (data, type);
            }
        }
        return default(T);
    }

    /// <summary>
    /// Datasetへの値の設定
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="obj">Object.</param>
    protected void Put(string key, object obj)
    {
        var type = obj.GetType();
        if (type != typeof(string) && !type.IsPrimitive) {
            m_Data.Put (key, UnityEngine.JsonUtility.ToJson (obj));
        } else {
            m_Data.Put (key, obj.ToString ());
        }
    }

    /// <summary>
    /// キーが存在するか。
    /// </summary>
    /// <returns><c>true</c>, if key was existed, <c>false</c> otherwise.</returns>
    /// <param name="key">Key.</param>
    protected bool ExistKey(string key)
    {
        return m_Data.Records.Any (x => x.Key == key);
    }

    /// <summary>
    /// 指定したキーを削除
    /// </summary>
    /// <param name="key">Key.</param>
    protected void Remove(string key)
    {
        m_Data.Remove (key);
    }

    #endregion

    #region CognitoSync Callbacks
    // Sync成功
    protected virtual void OnSyncSuccess(object sender, SyncSuccessEventArgs e)
    {
        if (m_DidCallback == null) {
            return;
        }
        m_DidCallback (true, sender, e);
        m_DidCallback = null;
    }
    // Sync失敗
    protected virtual void OnSyncFailure(object sender, SyncFailureEventArgs e)
    {
        UnityEngine.Debug.LogWarning(e.Exception.Message);
        if (m_DidCallback == null) {
            return;
        }
        m_DidCallback (false, sender, e);
        m_DidCallback = null;
    }
    // Sync時衝突発生
    protected virtual bool OnSyncConflict(Dataset dataset, List<SyncConflict> conflicts)
    {
        if (CheckoutDateSet) {
            dataset.Resolve (
                conflicts.Select (x => {
                    if(x.RemoteRecord != null) {
                        return x.ResolveWithRemoteRecord();
                    }
                    return x.ResolveWithLocalRecord();
                }).ToList ()
            );
            return true;
        }

        // データのコンフリクトが起きた場合は基本的にLocalを優先する。ModifiedDateがある場合は新しい方を優先する
        dataset.Resolve (
            conflicts.Select (x => {
                if(x.LocalRecord.LastModifiedDate.HasValue && x.RemoteRecord.LastModifiedDate.HasValue) {
                    if(x.RemoteRecord != null && x.LocalRecord.LastModifiedDate.Value < x.RemoteRecord.LastModifiedDate.Value) {
                        return x.ResolveWithRemoteRecord();
                    }
                }
                return x.ResolveWithLocalRecord();
            }).ToList ()
        );
        // return true so that synchronize() is retried after conflicts are resolved
        return true;
    }
    // 二つのアカウントデータのマージ時に発生
    protected virtual bool DatasetMerged (Dataset dataset, List<string> datasetNames)
    {
        // このコールバックの外部でデータセットのマージを処理する場合はfalseを返します。
        return true;
    }
    // 削除時に発生
    protected virtual bool DatasetDeleted (Dataset dataset)
    {
        // return true to delete the local copy of the dataset
        return false;
    }
    #endregion

    protected Dataset m_Data;
    protected DidSyncDelegate m_DidCallback;
}
