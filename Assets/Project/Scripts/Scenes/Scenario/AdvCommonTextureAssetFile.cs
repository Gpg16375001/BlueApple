using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.MouthMovement;

using UniRx;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif


namespace Utage
{

    /// <summary>
    /// シナリオパートで使用する共通Textureアセット.
    /// </summary>
	public class AdvCommonTextureAssetFile : AssetFileBase
	{
        /// その他リソース名を指定した初期化.
		public AdvCommonTextureAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, string url, DLCManager.DLC_FOLDER dlcFoler, string bundleName) : base(mangager, fileInfo, settingData)
		{
			m_assetName = Path.GetFileName(url);
			m_dlcFolder = dlcFoler;
			m_bundleName = bundleName;
		}

		/// ローカルまたはキャッシュあるか（つまりサーバーからDLする必要があるか）
		public override bool CheckCacheOrLocal()
		{
			return Texture != null;
		}
  
		/// ロード処理.ストレージタイプなどに関わらずユニットリソースとして取得するようにする.
		public override IEnumerator LoadAsync(Action onComplete, Action onFailed)
		{
			this.IsLoadEnd = false;
			var bLoading = true;
   
			this.CommonDownloadBG(m_assetName, tex => {
				Texture = tex;
				bLoading = false;
			});            
			
			while(bLoading){
				yield return null;
			}
			if(Texture == null){
				if(onFailed != null){
					onFailed();
				}
			}else{
				if (onComplete != null) {
                    onComplete();
                }	
			}         
			this.IsLoadEnd = true;
		}

		/// アンロード処理.
		public override void Unload()
		{
			// 特に何もしない.
		}
  
        // 通常ダウンロード.
		private void CommonDownloadBG(string assetName, Action<Texture2D> didLoad)
		{         
			DLCManager.AssetBundleFromFileOrDownload(m_dlcFolder, m_bundleName, Path.GetFileNameWithoutExtension(m_assetName), didLoad, (ex) => {
				Debug.LogError(ex.Message);
			});
		}

		private DLCManager.DLC_FOLDER m_dlcFolder;
		private string m_bundleName;
		private string m_assetName;
		private UnitResourceLoader m_loader;
	}
}
