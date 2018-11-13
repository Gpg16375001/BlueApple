using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
    public class AtomSoundGroup : MonoBehaviour
    {
        // 宴SoundManager
        internal SoundManager SoundManager { get { return AtomSoundManagerSystem.SoundManager; } }
        // CriAdx2用SoundManagerSystem
        internal AtomSoundManagerSystem AtomSoundManagerSystem { get; private set; }

        // プレイヤー管理
        internal Dictionary<string, AtomSoundPlayer> PlayerList { get { return playerList; } }

        Dictionary<string, AtomSoundPlayer> playerList = new Dictionary<string, AtomSoundPlayer> ();

        // グループ名
        public string GroupName { get { return gameObject.name; } }

        //グループ内で複数のオーディオを鳴らすか
        public bool MultiPlay {
            get { return multiPlay; }
            set { multiPlay = value; }
        }

        [SerializeField]
        bool multiPlay;

        //プレイヤーが終了したら自動削除するか
        public bool AutoDestoryPlayer {
            get { return autoDestoryPlayer; }
            set { autoDestoryPlayer = value; }
        }

        [SerializeField]
        bool autoDestoryPlayer;

        //マスターボリューム
        public float MasterVolume { get { return masterVolume; } set { masterVolume = value; } }

        [Range (0, 1), SerializeField]
        float masterVolume = 1;

        //グループボリューム
        public float GroupVolume { get { return groupVolume; } set { groupVolume = value; } }

        [Range (0, 1), SerializeField]
        float groupVolume = 1;

        //ダッキングの影響を与えるグループ
        public List<AtomSoundGroup> DuckGroups { get { return duckGroups; } }

        [SerializeField]
        List<AtomSoundGroup> duckGroups = new List<AtomSoundGroup> ();

        float DuckVolume { get; set; }

        float duckVelocity = 1;

        internal void Init (AtomSoundManagerSystem criSoundManagerSystem)
        {
            AtomSoundManagerSystem = criSoundManagerSystem;
            DuckVolume = 1;
            duckVelocity = 1;
        }

        // マスターボリュームとダッキングボリュームを加味したボリュームの取得
        internal float GetVolume (string tag)
        {
            float masterVolume = this.GroupVolume * this.MasterVolume * SoundManager.MasterVolume;
            foreach (var taggedVolume in SoundManager.TaggedMasterVolumes) {
                if (taggedVolume.Tag == tag) {
                    masterVolume *= taggedVolume.Volume;
                }
            }
            return masterVolume * DuckVolume;
        }

        void Update ()
        {
            // SoundPlayerの更新処理
            foreach (var player in PlayerList.Values) {
                player.Update ();
            }

            //以下、ダッキング処理
            if (Mathf.Approximately (1.0f, SoundManager.DuckVolume)) {
                //ダッキングのボリュームが1なので常に影響受けない
                DuckVolume = 1;
                return;
            }

            //ダッキングの影響をうけるグループがない
            if (DuckGroups.Count <= 0) {
                DuckVolume = 1;
                return;
            }

            bool isPlaying = DuckGroups.Exists (x => x.IsPlaying ());
            float dukkingTo = (isPlaying) ? SoundManager.DuckVolume : 1;
            if (Mathf.Abs (dukkingTo - DuckVolume) < 0.001f) {
                //ダッキングの目標値に近づいた
                DuckVolume = dukkingTo;
                duckVelocity = 0;
            } else {
                DuckVolume = Mathf.SmoothDamp (DuckVolume, dukkingTo, ref duckVelocity, SoundManager.DuckFadeTime);
            }
        }

        void LateUpdate ()
        {
            // SoundPlayerの更新処理
            foreach (var player in PlayerList.Values.ToArray()) {
                player.LateUpdate ();
            }
        }

        // プレイヤーリストから削除する。
        internal void Remove (string label)
        {
            PlayerList.Remove (label);
        }

        // labelのプレイヤーを取得する。
        AtomSoundPlayer GetPlayer (string label)
        {
            AtomSoundPlayer player;
            if (PlayerList.TryGetValue (label, out player)) {
                return player;
            }
            return null;
        }

        // labelのプレイヤーがあれば取得しなければ作成する。
        AtomSoundPlayer GetPlayerOrCreateIfMissing (string label)
        {
            AtomSoundPlayer player = GetPlayer (label);
            if (player == null) {
                player = new AtomSoundPlayer ();
                player.Init (label, this);
                PlayerList.Add (label, player);
            }
            return player;
        }

        // labelのプレイヤーがあれば削除して作成する。
        AtomSoundPlayer GetOnlyOnePlayer (string label, float fadeOutTime)
        {
            AtomSoundPlayer player = GetPlayerOrCreateIfMissing (label);
            if (PlayerList.Count > 1) {
                foreach (var keyValue in PlayerList) {
                    if (keyValue.Value != player) {
                        keyValue.Value.Stop (fadeOutTime);
                    }
                }
            }
            return player;
        }

        // どこかのPlayerが再生中か
        internal bool IsPlaying ()
        {
            return PlayerList.Values.Any (x => x.IsPlaying ());
        }

        // 指定labelのPlayerが再生中か
        internal bool IsPlaying (string label)
        {
            AtomSoundPlayer player = GetPlayer (label);
            if (player == null)
                return false;
            return player.IsPlaying ();
        }

        // 指定labelのPlayerがループ再生中か
        internal bool IsPlayingLoop (string label)
        {
            AtomSoundPlayer player = GetPlayer (label);
            if (player == null)
                return false;
            return player.IsPlayingLoop ();
        }

        // 再生処理
        internal void Play (string label, SoundData data, float fadeInTime, float fadeOutTime)
        {
            AtomSoundPlayer player = (MultiPlay) ? GetPlayerOrCreateIfMissing (label) : GetOnlyOnePlayer (label, fadeOutTime);
            player.Play (data, fadeInTime, fadeOutTime);
        }

        // 停止処理
        internal void Stop (string label, float fadeTime)
        {
            AtomSoundPlayer player = GetPlayer (label);
            if (player == null)
                return;
            player.Stop (fadeTime);
        }

        // 全停止処理
        internal void StopAll (float fadeTime)
        {
            foreach (var player in PlayerList.Values) {
                player.Stop (fadeTime);
            }
        }

        // ループ再生以外の音声を停止
        internal void StopAllIgnoreLoop (float fadeTime)
        {
            foreach (var player in PlayerList.Values) {
                if (player.IsPlayingLoop ())
                    continue;
                player.Stop (fadeTime);
            }
        }

        // オーディオソースの取得
        internal AudioSource GetAudioSource (string label)
        {
            // CriAdx2使用のためAudioSourceは取得できない。
            return null;
        }

        //
        internal CriAtomExPlayerOutputAnalyzer GetAudioAnalyzer (string label)
        {
            //AtomSoundPlayer player = GetPlayer (label);
			AtomSoundPlayer player = GetPlayerOrCreateIfMissing(label);
            if (player == null)
                return null;
            return player.GetAudioAnalyzer ();
        }

        // 現在のボリュームの取得
        internal float GetSamplesVolume (string label)
        {
            AtomSoundPlayer player = GetPlayer (label);
            if (player == null)
                return 0;
            return player.GetSamplesVolume ();
        }

        const int Version = 0;
        //セーブデータ用のバイナリ書き込み
        internal void Write (BinaryWriter writer)
        {
            writer.Write (Version);
            writer.Write (GroupVolume);
            writer.Write (PlayerList.Count);
            foreach (var keyValue in PlayerList) {
                writer.Write (keyValue.Key);
                writer.WriteBuffer (keyValue.Value.Write);
            }
        }

        //セーブデータ用のバイナリ読み込み
        internal void Read (BinaryReader reader)
        {
            int version = reader.ReadInt32 ();
            if (version <= Version) {
                GroupVolume = reader.ReadSingle ();
                int playerCount = reader.ReadInt32 ();
                for (int i = 0; i < playerCount; ++i) {
                    string label = reader.ReadString ();
                    AtomSoundPlayer player = GetPlayerOrCreateIfMissing (label);
                    reader.ReadBuffer (player.Read);
                }
            } else {
                Debug.LogError (LanguageErrorMsg.LocalizeTextFormat (ErrorMsg.UnknownVersion, version));
            }
        }
    }
}