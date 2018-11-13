using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	/// <summary>
    /// シナリオパートでLive2dのAssetBundleをゲーム中で使用しているものと共有するためのローダー.
    /// </summary>
	public class AdvLive2dObjectLoader : MonoBehaviour
    {      
        void Awake()
        {
			AssetFileManager.GetCustomLoadManager().OnFindAsset += FindAsset;         
        }

		private void FindAsset(AssetFileManager manager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, ref AssetFileBase asset)
        {
			if(fileInfo.FileType != AssetFileType.UnityObject) {
				return;
			}
			// TODO : Live2Dモデルとそれ以外を判別するためにCharacter内にさらにLive2Dフォルダを切る必要性あり.
			if(!fileInfo.FileName.Contains("Character") || !fileInfo.FileName.EndsWith(".prefab", System.StringComparison.Ordinal)){
				return;
			}
			if(!(settingData is AdvGraphicInfo)){
				return;
			}
			var parentDir = Directory.GetParent(fileInfo.FileName);

			// 親ディレクトリ=npc_***であればNPCキャラクター.
			if(parentDir.Name.StartsWith("npc_", StringComparison.Ordinal)){
				Debug.Log("AdvLive2dObjectLoader FindAsset : "+fileInfo.FileName);
				asset = new AdvLive2dAssetFile(manager, fileInfo, settingData, parentDir.Name);
				return;
			}
            // 親ディレクトリ=カードIDであれば味方ユニットリソースから.         
			var cardId = -1;
			if(int.TryParse(parentDir.Name, out cardId)){
				asset = new AdvLive2dAssetFile(manager, fileInfo, settingData, cardId);         
				return;
			}         
		}
    }
}
