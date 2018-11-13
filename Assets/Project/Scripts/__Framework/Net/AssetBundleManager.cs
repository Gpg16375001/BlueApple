using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab 
{
    /// <summary>
    /// アセットバンドル管理.
    /// </summary>
    public class AssetBundleManager : ViewBase
    {
        /// <summary>共通インスタンス.</summary>
        public static AssetBundleManager SharedInstance { get; private set; }


        /// <summary>
        /// 破棄処理.
        /// </summary>
        public override void Dispose()
        {
            if(this.IsDestroyed){
                return;
            }
            this.ClearCacheMemory();
            base.Dispose();
        }

        /// <summary>
        /// 初期化.
        /// </summary>
        public void Init(uint capacity)
        {
            m_cache = new AssetBundleCache(capacity);
        }

        /// <summary>
        /// ロード.
        /// </summary>
        public void Load(string serverUrl, Hash128 bundleVer, Action<AssetBundle> didLoad)
        {
            m_cache.Load(serverUrl, bundleVer, didLoad);
        }

        /// <summary>
        /// ロードして生成.
        /// </summary>
        public void LoadWithInstantiate<T>(string url, Hash128 bundleVer, string name, Action<T> didLoad) where T : UnityEngine.Object
        {
            this.Load(url, bundleVer, bundle => {
                var prefab = bundle.LoadAsset<T>(name);
                if(didLoad != null){
                    var instance = Instantiate(prefab) as T;
                    didLoad(instance);
                }
            });
        }

        /// <summary>
        /// メモリキャッシュの削除.
        /// </summary>
        public void ClearCacheMemory()
        {
            m_cache.ClearMemory();
        }

        void Awake()
        {
            if(SharedInstance != null) {
                SharedInstance.Dispose();
            }
            SharedInstance = this;
        }

        private AssetBundleCache m_cache;
    }
}
