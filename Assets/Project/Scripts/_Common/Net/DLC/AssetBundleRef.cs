using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロード済みのAssetBundleの参照管理をする
/// </summary>
public class AssetBundleRef
{
    public AssetBundle assetbundle {
        get;
        private set;
    }

    public bool IsAlive {
        get {
            return refCnt > 0;
        }
    }

    public bool IsLoading {
        get;
        private set;
    }

    public AssetBundleRef(AssetBundle assetbundle) {
        this.assetbundle = assetbundle;
        IsLoading = false;
        refCnt = 0;
    }

    public AssetBundleRef() {
        this.assetbundle = null;
        IsLoading = true;
        refCnt = 0;
    }

    ~AssetBundleRef()
    {
        if (assetbundle != null) {
            assetbundle.Unload (true);
            assetbundle = null;
        }
    }

    public void SetAssetBundle(AssetBundle assetbundle)
    {
        this.assetbundle = assetbundle;
        IsLoading = false;
    }

    public void AddRef(int count = 1)
    {
        refCnt += count;
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        if (--refCnt == 0) {
            assetbundle.Unload (unloadAllLoadedObjects);
            assetbundle = null;
        }
    }

    private int refCnt = 0;
}