using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
    public class AtomAssetFile : AssetFileBase
    {
        public AtomAssetFile (AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, AtomForUtage criForUtage)
            : base (mangager, fileInfo, settingData)
        {
            this.CueName = settingData.RowData.ParseCellOptional<string> ((int)AdvColumnName.Voice, "");
            if (string.IsNullOrEmpty(this.CueName)) {
                this.CueName = FilePathUtil.GetFileNameWithoutExtension (this.FileName);
            }
            // 拡張子を削除する。
            this.CueName = System.IO.Path.GetFileNameWithoutExtension (this.CueName);

            if (string.IsNullOrEmpty (this.CueName)) {
                return;
            }

            this.SoundSetting = settingData as IAssetFileSoundSettingData;
            if (SoundSetting.RowData != null) {
                this.CueSheet = settingData.RowData.ParseCellOptional<string> ("CueSheet", criForUtage.DefaultQueSheet);
            } else {
                this.CueSheet = criForUtage.DefaultQueSheet;
            }
        }

        public string CueSheet { get; protected set; }

        public string CueName { get; protected set; }

        public IAssetFileSoundSettingData SoundSetting { get; protected set; }

        private CriSoundControll CriSoundController {
            get {
                if (criSoundController == null) {
                    criSoundController = SmileLab.SoundManager.SharedInstance.GetSoundControll<CriSoundControll> ();
                }
                return criSoundController;
            }
        }

        private CriSoundControll criSoundController;

        public override bool CheckCacheOrLocal ()
        {
            if (CriSoundController != null) {
                // 指定のキューシートがダウンロードされていれば何もしない。
                return CriSoundController.DownloadedACB (CueSheet);
            }
            return true;
        }

        public override IEnumerator LoadAsync (Action onComplete, Action onFailed)
        {
            this.IsLoadEnd = false;
            this.IsLoadError = false;

            yield return LoadAsyncSub (onComplete, onFailed);
        }

        //ロード
        IEnumerator LoadAsyncSub (Action onComplete, Action onFailed)
        {
            // CriSoundControllerが存在しない場合に進行不能にならないようにする
            if (CriSoundController == null) {
                this.IsLoadEnd = true;
                yield break;
            }

            switch (FileInfo.StrageType) {
            case AssetFileStrageType.Resources:
                // リソースであることはありえない
                yield break;
            case AssetFileStrageType.StreamingAssets:
                break;
            case AssetFileStrageType.Server:
                bool isEnd = false;
                CriSoundController.DownloadResourceFromSheetName (CueSheet, () => {
                    isEnd = true;
                });
                while (!isEnd) {
                    yield return null;
                }
                break;
            }
            onComplete ();

            this.IsLoadEnd = true;
        }

        public override void Unload ()
        {
            IsLoadEnd = false;
        }
    }
}