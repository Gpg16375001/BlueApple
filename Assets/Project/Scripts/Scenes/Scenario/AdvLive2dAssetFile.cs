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
    /// シナリオパートで使用するLive2Dアセット.
    /// </summary>
	public class AdvLive2dAssetFile : AssetFileBase
	{
		/// カードIDを元にロードしてくる.
		public AdvLive2dAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, int cardId) : base(mangager, fileInfo, settingData)
		{
			m_loader = new UnitResourceLoader(cardId);
            m_loader.LoadFlagReset ();
            m_loader.IsLoadLive2DModel = true;
		}
        /// その他リソース名を指定した初期化.
		public AdvLive2dAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, string url) : base(mangager, fileInfo, settingData)
		{
			m_assetName = Path.GetFileName(url);
		}

		/// ローカルまたはキャッシュあるか（つまりサーバーからDLする必要があるか）
		public override bool CheckCacheOrLocal()
		{
			if(m_loader != null){
				return m_loader.Live2DModel != null;
			}
			return m_live2DModel!= null;      
		}
  
		/// ロード処理.ストレージタイプなどに関わらずユニットリソースとして取得するようにする.
		public override IEnumerator LoadAsync(Action onComplete, Action onFailed)
		{
			this.IsLoadEnd = false;
			var bLoading = true;

            // 通常.
			if(m_loader == null && !string.IsNullOrEmpty(m_assetName)){
				this.CommonDownloadLive2D(m_assetName, live2dObj => {
					live2dObj.GetOrAddComponent<CubismAutoEyeBlinkInput>();       // 目パチオート
					live2dObj.GetOrAddComponent<CubismAutoMouthInput>();          // 口パクオート
					live2dObj.GetOrAddComponent<AdvGraphicObjectLive2D>().enabled = true;  // 宴でLive2D表示用
					live2dObj.GetOrAddComponent<Live2DLipSynchForCri>().enabled = true;    // 宴でCRI含むセリフと口パク連動用
					live2dObj.GetOrAddComponent<CubismAudioMouthInputFromCri>().enabled = true;  // 宴でCRI含む音声と口パク連動用
					UnityObject = live2dObj;
					bLoading = false;
				});            
			}else{
                // ユニットリソースあり.
    			m_loader.LoadResource(resouce => {
    				if(resouce == null){
    					bLoading = false;
    					return;
    				}
    				var go = resouce.Live2DModel;
    				go.GetOrAddComponent<CubismAutoEyeBlinkInput>();       // 目パチオート
    				go.GetOrAddComponent<CubismAutoMouthInput>();          // 口パクオート
    				go.GetOrAddComponent<AdvGraphicObjectLive2D>().enabled = true;  // 宴でLive2D表示用
    				go.GetOrAddComponent<Live2DLipSynchForCri>().enabled = true;    // 宴でCRI含むセリフと口パク連動用
    				go.GetOrAddComponent<CubismAudioMouthInputFromCri>().enabled = true;  // 宴でCRI含む音声と口パク連動用
    				UnityObject = go;
    				bLoading = false;
    			});
			}
			while(bLoading){
				yield return null;
			}
			if(UnityObject == null){
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
            if (m_loader != null) {
                m_loader.Dispose ();
            }
		}
  
        // 通常ダウンロード.
		private void CommonDownloadLive2D(string assetName, Action<GameObject> didLoad)
		{
			var s3Path = DLCManager.GetS3Path(DLCManager.DLC_FOLDER.NPC, "npc_"+m_assetName);         
			DLCManager.AssetBundleFromFileOrDownload(s3Path, (ABRef) => {
				ObservableUnity.FromCoroutine<GameObject>(
                    (observer, cancellatio) => CoLoadResource(ABRef, observer, cancellatio)
				).Subscribe(didLoad, ex => { Debug.LogException(ex); });
            });
		}
		private IEnumerator CoLoadResource(AssetBundleRef ABRef, IObserver<GameObject> observer, CancellationToken cancel)
		{ 
			var assetLoad = ABRef.assetbundle.LoadAssetAsync<GameObject>("live2dmodel");
            yield return assetLoad;
            m_live2DModel = assetLoad.asset as GameObject;
			ABRef.Unload(false);
			observer.OnNext(m_live2DModel);
            observer.OnCompleted();
		}

		private string m_assetName;
		private UnitResourceLoader m_loader;
		private GameObject m_live2DModel;
	}
}
