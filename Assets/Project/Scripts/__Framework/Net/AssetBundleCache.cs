using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SmileLab.Net;


namespace SmileLab
{
    /// <summary>
    /// クラス：アセットバンドルのローカルキャッシュクラス.
    ///        メモリ、ストレージ、サーバーの順にロードする.
    ///        キーはstringで固定.型は使用するアセットに合わせて設定する.
    /// </summary>
    public class AssetBundleCache : IDisposable
    {
        /// <summary>
        /// コンストラクタ.メモリキャッシュ容量とストレージに保存する際のディレクトリを指定.
        /// </summary>
        public AssetBundleCache(uint memCapa)
        {
            m_cacheCapacity = memCapa;
            m_cache = new AssetBundleLRUCache(m_cacheCapacity);
            CoroutineAgent.Execute(Update());
        }

        /// <summary>
        /// 外部から解放できるようにDisposeパターンで実装している.
        /// </summary>
        public void Dispose()
        {
            this.ClearMemory();
        }

        /// <summary>
        /// メモリクリア.ストレージには干渉しない.
        /// </summary>
        public void ClearMemory()
        {
            m_cache.Dispose();
        }

        /// <summary>
        /// 読み込み.
        /// </summary>
        public void Load(string url, uint bundleVer, Action<AssetBundle> didLoad)
        {
            Debug.Log("[AssetBundleCache] Load add queue with uint version.");
            Action<AssetBundle> didLoadEx = asset => {
                if(didLoad != null) {
                    didLoad(asset);
                }
                m_bLoadStorage = false;   // ストレージから読み込んでいた場合読み込みフラグが立っているので下げる.
            };
            // リクエストキューに追加してロード待ち.
            m_loadQueue.Add(new LoadInfo { URL = url, BundleVersion = bundleVer, DidLoadProc = didLoadEx });
        }
        /// <summary>
        /// 読み込み.
        /// </summary>
        public void Load(string url, Hash128 bundleVer, Action<AssetBundle> didLoad)
        {
            Debug.Log("[AssetBundleCache] Load add queue with hash version.");
            Action<AssetBundle> didLoadEx = asset => {
                if(didLoad != null) {
                    didLoad(asset);
                }
                m_bLoadStorage = false;
            };
            m_loadQueue.Add(new LoadInfo { URL = url, Hash = bundleVer, DidLoadProc = didLoadEx });
        }

        #region internal proc.

        // 毎フレームキューの中身を確認してロードしていく.
        IEnumerator Update()
        {
            while(true) {
                while(m_bLoadStorage || m_loadQueue.Count <= 0) {
                    yield return null;
                }

                // キューなので上から取り出すイメージ.
                var info = m_loadQueue[0];
                m_loadQueue.RemoveAt(0);

                // メモリ上にあればメモリから.なければストレージから読み込む.
                if(m_cache.IsExsits(info.URL)) {
                    Debug.Log("[AssetBundleCache] Load from cache.");
                    info.DidLoadProc(m_cache[info.URL]);
                } else {
                    Debug.Log("[AssetBundleCache] Download. url=" + info.URL + "/ver=" + info.BundleVersion);
                    m_bLoadStorage = true;
                    CoroutineAgent.Execute(this.LoadOrDownload(info));
                }
                yield return null;
            }

        }
        private IEnumerator LoadOrDownload(LoadInfo info)
        {
            while(!Caching.ready) {
                yield return null;
            }

            // 読み込み正常終了コールバック.
            Action<AssetBundle> didLoad = bundle => {
                m_cache[info.URL] = bundle;
                info.DidLoadProc(bundle);
                bundle.Unload(false);
                Debug.Log("[AssetBundleCache] LoadOrDownload  Success.");
            };
            // 読み込みエラーコールバック.
            Action<Exception> didError = e => {
                Debug.LogError("[AssetBundleCache] LoadOrDownload  Error!! : " + e.Message);
                info.DidLoadProc(null);
            };

            // バージョンハッシュが有効ならそちらを使用する.
            if(info.Hash.isValid){
                NetRequestManager.LoadFromCacheOrDownload(info.URL, info.Hash, didLoad, didError);
            }else{
                NetRequestManager.LoadFromCacheOrDownload(info.URL, info.BundleVersion, didLoad, didError);    
            }
        }

        private List<LoadInfo> m_loadQueue = new List<LoadInfo>();  // ロード用のキューリスト.
        private AssetBundleLRUCache m_cache;    // キャッシュデータ.
        private uint m_cacheCapacity;
        private bool m_bLoadStorage = false;

        #endregion

        #region internal class.

        // プライベートクラス：アセットロード用情報.
        private sealed class LoadInfo
        {
            public string URL;
            public Hash128 Hash;    // こちらが設定されている場合はBundleVersionより優先して使われる.
            public uint BundleVersion;
            public Action<AssetBundle> DidLoadProc;
        }

        // プライベートクラス：アセットバンドル用LRUキャッシュ.
        private sealed class AssetBundleLRUCache : LRUCache<string, AssetBundle>
        {
            public AssetBundleLRUCache(uint capacity) : base(capacity) { }

            /// <summary>指定キーのアイテムが存在する？</summary>
            public override bool IsExsits(string key)
            {
                return this[key] != null;
            }
            // アセットバンドルなので解放=Unload.
            protected override void DisposeItem(AssetBundle item)
            {
                if(item == null) {
                    return;
                }
                item.Unload(false);
            }
        }

        #endregion
    }
}
