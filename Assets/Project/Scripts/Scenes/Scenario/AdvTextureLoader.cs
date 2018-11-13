using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	/// <summary>
    /// シナリオパートの各種Textureを共通アセット化.
    /// </summary>
	public class AdvTextureLoader : MonoBehaviour
    {      
        void Awake()
        {
			AssetFileManager.GetCustomLoadManager().OnFindAsset += FindAsset;         
        }

		private void FindAsset(AssetFileManager manager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, ref AssetFileBase asset)
        {
			if(fileInfo.FileType != AssetFileType.Texture) {
				return;
			}         
			if(!(settingData is AdvGraphicInfo)){
				return;
			}
			if (fileInfo.FileName.Contains("BG")) {
				var bundleName = string.Format("scenariobg_{0}", Path.GetFileNameWithoutExtension(fileInfo.FileName).ToLower());
				asset = new AdvCommonTextureAssetFile(manager, fileInfo, settingData, fileInfo.FileName, DLCManager.DLC_FOLDER.ScenarioBG, bundleName);
            }
			if (fileInfo.FileName.Contains("Sprite")) {
				asset = new AdvCommonTextureAssetFile(manager, fileInfo, settingData, fileInfo.FileName, DLCManager.DLC_FOLDER.ScenarioSprite, "scenariosprite");
			}
		}
    }
}
