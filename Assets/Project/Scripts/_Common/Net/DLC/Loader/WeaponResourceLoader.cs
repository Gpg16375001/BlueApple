using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine.Unity;

using UniRx;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

public class WeaponResourceLoader : IDisposable {

    /// <summary>
    /// Spineモデルの読み込みをするかのフラグ
    /// </summary>
    public bool IsLoadSpineAtlas;

	/// <summary>
    /// 立ち絵ロード
    /// </summary>
    public bool IsLoadPortrait;
    /// <summary>
    /// 立ち絵画像
    /// </summary>
    public Sprite PortraitImage { get; private set; }

    /// <summary>
    /// SpineAtlasのインスタンス
    /// </summary>
    public AtlasAsset WeaponAtlas {
        get;
        private set;
    }

    // ロードするファイルのS3上のパス
    private string S3Path;

    private int m_WeaponID;

    // ロードするファイルのS3上のパス
    public bool IsLoaded {
        get;
        private set;
    }

    private void Init(int weaponID)
    {
        m_WeaponID = weaponID;
        S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Weapon, string.Format ("weapon_{0}", weaponID));
    }

    public WeaponResourceLoader(int weaponID)
    {
        IsLoadSpineAtlas = true;
		IsLoadPortrait = true;
        Init (weaponID);
    }

    public WeaponResourceLoader(Weapon weapon)
    {
        IsLoadSpineAtlas = true;
		IsLoadPortrait = true;
        Init (weapon.id);
    }

    public WeaponResourceLoader(BattleLogic.WeaponParameter weapon)
    {
        IsLoadSpineAtlas = true;
		IsLoadPortrait = true;
        Init (weapon.weapon.id);
    }

    // ロード開始
    public void LoadResource(Action<WeaponResourceLoader> didLoad, MonoBehaviour behaviour = null)
    {
		if (!IsLoadSpineAtlas && !IsLoadPortrait) {
            return;
        }

        if (IsLoaded) {
            return;
        }

        DLCManager.AssetBundleFromFileOrDownload (S3Path,
            (ABRef) => {
                ObservableUnity.FromCoroutine<WeaponResourceLoader>(
                    (observer, cancellatio) => CoLoadResource(ABRef, observer, cancellatio)
                ).Subscribe(didLoad, ex => { Debug.LogException(ex); });
            }
        );
    }

    private static readonly string WeaponSpineAtlasNameOld = "weaponatlas";
    private static readonly string WeaponSpineAtlasNameFormat = "{0}_atlas";
	private static readonly string PortraitName = "portrait";
    private IEnumerator CoLoadResource(AssetBundleRef ABRef, IObserver<WeaponResourceLoader> observer, CancellationToken cancel)
    {
        if (IsLoadSpineAtlas) {
            // 武器画像
            AssetBundleRequest assetLoad = null;
            var weaponSpineAssetName = string.Format (WeaponSpineAtlasNameFormat, m_WeaponID);
            if(ABRef.assetbundle.Contains(weaponSpineAssetName)) {
                assetLoad = ABRef.assetbundle.LoadAssetAsync<AtlasAsset> (weaponSpineAssetName);
            } else {
                assetLoad = ABRef.assetbundle.LoadAssetAsync<AtlasAsset> (WeaponSpineAtlasNameOld);
            }

            if (assetLoad != null) {
                yield return assetLoad;
                WeaponAtlas = assetLoad.asset as AtlasAsset;
                if (WeaponAtlas == null) {
                    ABRef.Unload (false);
                    observer.OnError (new Exception ("WeaponAtlas Load faield"));
                    yield break;                
                }
            }
        }

		if(IsLoadPortrait){
			// 武器立ち絵画像
			AssetBundleRequest assetLoad = null;
			assetLoad = ABRef.assetbundle.LoadAssetAsync<Sprite>(PortraitName);
			if(assetLoad != null){
				yield return assetLoad;
				PortraitImage = assetLoad.asset as Sprite;
				if (PortraitImage == null){
					ABRef.Unload(false);
					observer.OnError(new Exception("WeaponPortraitImage Load faield"));
					yield break;
				}
			}
		}

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        IsLoaded = true;

        ABRef.Unload (false);
        observer.OnNext(this);
        observer.OnCompleted();
    }

    public void Dispose()
    {
        WeaponAtlas = null;
        IsLoaded = false;
    }
}
