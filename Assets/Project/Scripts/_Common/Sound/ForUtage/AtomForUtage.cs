using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Utage
{
    /// <summary>
    /// サウンド管理クラスと宴のつなぎ
    /// </summary>
    public class AtomForUtage : MonoBehaviour
    {
        //フェードの時間指定をデータでするか
        public bool EnableFadeOnData { get { return enableFadeOnData; } }

        [SerializeField]
        bool enableFadeOnData = true;

        //宴のコンフィグ設定でのマスターボリューム調整を無視する
        public bool IgnoreConfigMasterVolume { get { return ignoreConfigMasterVolume; } }

        [SerializeField]
        bool ignoreConfigMasterVolume = true;

        //宴のダッキング処理によるボリューム調整を無視する
        public bool IgnoreDuckVolume { get { return ignoreDuckVolume; } }

        [SerializeField]
        bool ignoreDuckVolume = true;

        //宴のダッキング処理によるボリューム調整を無視する
        public bool EnableLipSyncForCri { get { return enableLipSyncForCri; } }

        [SerializeField]
        bool enableLipSyncForCri = false;

        //キューシート名が空欄だった場合にこの名前を使う
        public string DefaultQueSheet { get { return defaultQueSheet; } }

        [SerializeField]
        string defaultQueSheet = "";

        //サウンド再生システムをAdx2用に上書き
        public void OnCreateSoundSystem (SoundManager soundManager)
        {
            soundManager.System = new AtomSoundManagerSystem (this);
        }

        //サウンドファイルのロードを上書きするコールバックを登録
        void Awake ()
        {
            AssetFileManager.GetCustomLoadManager ().OnFindAsset += FindAsset;
        }

        //サウンドファイルのロードをAdx2用に上書き
        void FindAsset (AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, ref AssetFileBase asset)
        {
            if (fileInfo.Setting.FileType == AssetFileType.Sound) {
                asset = new AtomAssetFile (mangager, fileInfo, settingData, this);
            }
        }
    }
}