using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UtageExtensions;

namespace Utage
{
    /// <summary>
    /// CriAdx2用の宴サウンドマネージャーシステム
    /// </summary>
    public class AtomSoundManagerSystem : SoundManagerSystemInterface
    {
        // セーブデータのバージョン
        const int Version = 0;
        // 宴サウンドマネージャー
        internal SoundManager SoundManager { get; private set; }
        // サウンドマネージャーのTransformキャッシュ(ゲームオブジェクト作成時に使用)
        Transform CachedTransform { get; set; }
        // 宴とCriAdx2のブリッジ設定クラス
        internal AtomForUtage AtomForUtage { get; set; }

        //
        internal CriSoundControll CriSoundController { get; set; }

        //グループリスト
        Dictionary<string, AtomSoundGroup> groupTbl = new Dictionary<string, AtomSoundGroup> ();

        public AtomSoundManagerSystem (AtomForUtage atomForUtage)
        {
            this.AtomForUtage = atomForUtage;
        }

        public void Init (SoundManager soundManager, List<string> saveStreamNameList)
        {
            SoundManager = soundManager;
            CachedTransform = SoundManager.transform;
            CriSoundController = SmileLab.SoundManager.SharedInstance.GetSoundControll<CriSoundControll> ();
        }

        /// <summary>
        /// サウンドの再生
        /// </summary>
        public void Play (string groupName, string label, SoundData soundData, float fadeInTime, float fadeOutTime)
        {
            if (CriSoundController != null) {
                AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
                group.Play (label, soundData, fadeInTime, fadeOutTime);
            }
        }

        /// <summary>
        /// フェードアウトして曲を停止
        /// </summary>
        /// <param name="type">タイプ</param>
        /// <param name="fadeTime">フェードする時間</param>
        public void Stop (string groupName, string label, float fadeTime)
        {
            if (CriSoundController != null) {
                AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
                group.Stop (label, fadeTime);
            }
        }      

        /// <summary>
        /// 指定のサウンドが鳴っているか
        /// </summary>
        /// <param name="type">タイプ</param>
        /// <returns>鳴っていればtrue、鳴っていなければfalse</returns>
        public bool IsPlaying (string groupName, string label)
        {
            if (CriSoundController != null) {
                AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
                return group.IsPlaying (label);
            }
            return false;
        }

        /// <summary>
        /// 指定のオーディオを取得
        /// </summary>
        public AudioSource GetAudioSource (string groupName, string label)
        {
            // CriAdx2のためAudioSourceが取得できない。
            return null;
        }

        public CriAtomExPlayerOutputAnalyzer GetAudioAnalyzer (string groupName, string label)
        {
            if (CriSoundController != null) {
                AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
                return group.GetAudioAnalyzer (label);
            }
            return null;
        }

        /// <summary>
        /// 現在のボリュームを波形から計算して取得
        /// </summary>
        public float GetSamplesVolume (string groupName, string label)
        {
            // TODO: 実装可能なので対応中
            return 0.0f;
        }

        /// <summary>
        /// 指定のグループすべて停止
        /// </summary>
        /// <param name="fadeTime">フェードアウトの時間</param>
        public void StopGroup (string groupName, float fadeTime)
        {
            if (CriSoundController != null) {
				AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
                group.StopAll (fadeTime);
            }
        }

        /// <summary>
        /// 指定のグループのループ以外をすべて停止
        /// </summary>
        /// <param name="fadeTime">フェードアウトの時間</param>
        public void StopGroupIgnoreLoop (string groupName, float fadeTime)
        {
            if (CriSoundController != null) {
                AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
                group.StopAllIgnoreLoop (fadeTime);
            }
        }

        /// <summary>
        /// フェードアウトして曲全てを停止
        /// </summary>
        /// <param name="fadeTime">フェードアウトの時間</param>
        public void StopAll (float fadeTime)
        {
            if (CriSoundController != null) {
                foreach (var group in groupTbl.Values) {
                    group.StopAll (fadeTime);
                }
            }
        }

        /// <summary>
        /// マスターボリュームの設定
        /// </summary>
        public float GetMasterVolume (string groupName)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
            return group.MasterVolume;
        }

        public void SetMasterVolume (string groupName, float volume)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
            group.MasterVolume = volume;
        }

        /// <summary>
        /// グループボリュームの設定
        /// </summary>
        public float GetGroupVolume (string groupName)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
            return group.GroupVolume;
        }

        public void SetGroupVolume (string groupName, float volume)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
            group.GroupVolume = volume;
        }

        /// <summary>
        /// グループ内で複数のオーディオを再生するかどうか
        /// </summary>
        public bool IsMultiPlay (string groupName)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
            return group.MultiPlay;
        }

        public void SetMultiPlay (string groupName, bool multiPlay)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing (groupName);
            group.MultiPlay = multiPlay;
        }

		/// <summary>
        /// グループ内でオーディオ再生終了時自動破棄するかどうか
        /// </summary>
        public bool IsAutoDestroyPlayer(string groupName)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing(groupName);
			return group.AutoDestoryPlayer;
        }

		public void SetAutoDestroyPlayer(string groupName, bool autoDestroy)
        {
            AtomSoundGroup group = GetGroupOrCreateIfMissing(groupName);
			group.AutoDestoryPlayer = autoDestroy;
        }

        /// <summary>
        /// セーブデータ用のバイナリ変換
        /// 再生中のBGMのファイル情報などをバイナリ化
        /// </summary>
        public void WriteSaveData (BinaryWriter writer)
        {
            writer.Write (Version);
            writer.Write (groupTbl.Count);
            foreach (var keyValue in groupTbl) {
                writer.Write (keyValue.Key);
            }
            foreach (var keyValue in groupTbl) {
                writer.WriteBuffer (keyValue.Value.Write);
            }
        }

        /// <summary>
        /// セーブデータを読みこみ
        /// </summary>
        public void ReadSaveDataBuffer (BinaryReader reader)
        {
            int version = reader.ReadInt32 ();
            if (version <= Version) {
                int count = reader.ReadInt32 ();
                //グループは初期化前にすべて作成済みである必要があるので、いったん実行
                List<AtomSoundGroup> list = new List<AtomSoundGroup> ();
                for (int i = 0; i < count; ++i) {
                    string name = reader.ReadString ();
                    list.Add (GetGroupOrCreateIfMissing (name));
                }
                for (int i = 0; i < count; ++i) {
                    reader.ReadBuffer (list [i].Read);
                }
            } else {
                Debug.LogError (LanguageErrorMsg.LocalizeTextFormat (ErrorMsg.UnknownVersion, version));
            }
        }

        /// <summary>
        /// ロード中か
        /// </summary>
        public bool IsLoading { 
            get {
                return false;
            }
        }

        //指定の名前のストリームを取得。なければ作成。
        AtomSoundGroup GetGroupOrCreateIfMissing (string groupName)
        {
            AtomSoundGroup group;
            if (!groupTbl.TryGetValue (groupName, out group)) {
                group = CachedTransform.AddChildGameObjectComponent<AtomSoundGroup> (groupName);
                group.Init (this);
                groupTbl.Add (groupName, group);

                switch (groupName) {
                case SoundManager.IdBgm:
                    group.DuckGroups.Add (GetGroupOrCreateIfMissing (SoundManager.IdVoice));
                    break;
                case SoundManager.IdAmbience:
                    group.DuckGroups.Add (GetGroupOrCreateIfMissing (SoundManager.IdVoice));
                    break;
                case SoundManager.IdVoice:
                    group.AutoDestoryPlayer = true;
                    break;
                case SoundManager.IdSe:
                    group.AutoDestoryPlayer = true;
                    group.MultiPlay = true;
                    break;
                }
            }
            return group;
        }
    }
}