using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UtageExtensions;

namespace Utage
{

    /// <summary>
    /// Adx2用
    /// ラベルで区別された各オーディオを鳴らす
    /// 基本はシステム内部で使うので外から呼ばないこと
    /// </summary>
    internal class AtomSoundPlayer
    {
        /// <summary>
        /// 再生管理しているオーディオ
        /// </summary>
        public AtomSoundAudio Audio { get; private set; }

        //ラベル
        internal string Label { get; private set; }
        //グループ情報
        internal AtomSoundGroup Group { get; set; }

        // フェードアウト処理中のオーディオ
        private AtomSoundAudio FadeOutAudio { get; set; }

        // 再生管理しているオーディオのリスト
        private List<AtomSoundAudio> AudioList { get; set; }
        // 直近1フレーム中に再生が開始されたオーディオのリスト
        private List<AtomSoundAudio> CurrentFrameAudioList { get; set; }
        // 直近1フレーム中に再生停止されAudioListから削除予定のAudioのリスト
        private List<AtomSoundAudio> CurrentFrameRemoveList { get; set; }

        /// <summary>
        /// 再生終了しているか
        /// </summary>
        public bool IsStop ()
        {
            return !AudioList.Any (x => x != null);
        }

        /// <summary>
        /// 再生中か
        /// </summary>
        public bool IsPlaying ()
        {
            return AudioList.Any (x => x != null && x.IsPlaying ());
        }

        /// <summary>
        /// ループ再生中か
        /// </summary>
        public bool IsPlayingLoop ()
        {
            return AudioList.Any (x => x != null && x.IsPlayingLoop ());
        }

        /// <summary>
        /// 毎フレームの処理
        /// 自分の所属するAtomAudioGroupのUpdateから実行される。
        /// </summary>
        public void Update ()
        {
            // AudioのUpdate処理
            foreach (var audio in AudioList) {
                audio.Update ();
            }
        }

        /// <summary>
        /// フレームの最後に実行される処理
        /// 自分の所属するAtomAudioGroupのLateUpdateから実行される。
        /// </summary>
        public void LateUpdate ()
        {
            // AudioのLateUpdate処理
            foreach (var audio in AudioList) {
                audio.LateUpdate ();
            }

            // Removeに登録されているAudioを削除
            foreach (var audio in CurrentFrameRemoveList) {
                AudioList.Remove (audio);
            }
            CurrentFrameRemoveList.Clear ();

            CurrentFrameAudioList.Clear ();

            // 自動削除フラグが立っていた場合は管理しているAudioGroupからDetach
            if (this.Group.AutoDestoryPlayer && AudioList.Count == 0) {
                Detach ();
            }
        }

        /// <summary>
        /// 再生しているAudioを止める
        /// </summary>
        /// <param name="fadeTime">停止時のフェード時間</param>
        public void Stop (float fadeTime)
        {
            foreach (var audio in AudioList) {
                if (audio == null)
                    continue;
                audio.FadeOut (fadeTime);
            }
        }

        // 初期化
        internal void Init (string label, AtomSoundGroup group)
        {
            this.Group = group;
            this.Label = label;
            this.AudioList = new List<AtomSoundAudio> ();
            this.CurrentFrameAudioList = new List<AtomSoundAudio> ();
            this.CurrentFrameRemoveList = new List<AtomSoundAudio> ();
        }

        // 管理されているGroupからのDetach
        void Detach ()
        {
            this.Group.Remove (Label);
        }

        // AudioのRemove処理
        internal void Remove (AtomSoundAudio audio)
        {
            CurrentFrameRemoveList.Add (audio);
        }

        //再生（直前があればフェードアウトしてから再生）
        internal void Play (SoundData data, float fadeInTime, float fadeOutTime)
        {
            switch (data.PlayMode) {
            case SoundPlayMode.Add:
                //重複して鳴らす（SEなど）
                PlayAdd (data, fadeInTime, fadeOutTime);
                break;
            case SoundPlayMode.Replay:
                //直前のをフェードアウトし、同時に先頭から鳴らしなおす（一部のSEなど）
                PlayFade (data, fadeInTime, fadeOutTime, true);
                break;
            case SoundPlayMode.NotPlaySame:
                //同じサウンドが鳴っている場合は、そのままにしてなにもしない（BGMや一部のSEなど）
                if ((Audio != null && Audio.IsPlaying (data))) {
                    return;
                }
                PlayFade (data, fadeInTime, fadeOutTime, false);
                break;
            }
        }

        //再生（直前があればフェードアウトしてから再生）
        void PlayAdd (SoundData data, float fadeInTime, float fadeOutTime)
        {
            //今のフレームで同じサウンドを鳴らしていたらもう鳴らさない
            foreach (var item in CurrentFrameAudioList) {
                if (item != null && item.IsEqualSoundData (data)) {
                    return;
                }
            }

            AtomSoundAudio audio = CreateNewAudio (data);
            //即時再生
            audio.Play (fadeInTime);
            CurrentFrameAudioList.Add (audio);
        }

        //再生（直前があればフェードアウトしてから再生）
        void PlayFade (SoundData data, float fadeInTime, float fadeOutTime, bool corssFade)
        {
            //フェードアウト中のがあったら消す
            if (FadeOutAudio != null) {
                Remove (FadeOutAudio);
                FadeOutAudio = null;
            }

            // 現在のオーディオがないなら即座に鳴らす
            if (Audio == null) {
                Audio = CreateNewAudio (data);
                // 即時再生
                Audio.Play (fadeInTime);
            } else {
                // 今鳴っているものをフェードアウト
                FadeOutAudio = Audio;
                Audio = CreateNewAudio (data);
                FadeOutAudio.FadeOut (fadeOutTime);
                if (corssFade) {
                    // 即座に鳴らす
                    Audio.Play (fadeInTime);
                } else {
                    // フェードアウトを待ってから鳴らす
                    if (Audio != null) {
                        Audio.Play (fadeInTime, fadeOutTime);
                    }
                }
            }
        }

        //新規でオーディオ作成
        AtomSoundAudio CreateNewAudio (SoundData soundData)
        {
            AtomSoundAudio audio = new AtomSoundAudio ();
            audio.Init (this, soundData);
            AudioList.Add (audio);
            return audio;
        }

        // 現在のボリュームの取得
        internal float GetSamplesVolume ()
        {
            return IsPlaying () ? Audio.GetSamplesVolume () : 0;
        }

        internal CriAtomExPlayerOutputAnalyzer GetAudioAnalyzer ()
        {
            return Audio.GetAudioAnalyzer ();
        }

        const int Version = 0;
        // セーブデータ用のバイナリ書き込み
        internal void Write (BinaryWriter writer)
        {
            writer.Write (Version);
            writer.Write (AudioList.Count);
            foreach (var audio in AudioList) {
                bool enableSave = audio.EnableSave;
                writer.Write (enableSave);
                if (!enableSave)
                    continue;
                writer.WriteBuffer (audio.Data.Write);
            }
            writer.Write (Audio == null ? "" : Audio.Name);
        }

        // セーブデータ用のバイナリ読み込み
        internal void Read (BinaryReader reader)
        {
            int version = reader.ReadInt32 ();
            if (version <= Version) {
                int audioCount = reader.ReadInt32 ();
                for (int i = 0; i < audioCount; ++i) {
                    bool enableSave = reader.ReadBoolean ();
                    if (!enableSave)
                        continue;

                    SoundData soundData = new SoundData ();
                    reader.ReadBuffer (soundData.Read);
                    Play (soundData, 0.1f, 0);
                }
                string audioName = reader.ReadString ();
                if (!string.IsNullOrEmpty (audioName)) {
                    Audio = AudioList.Find (x => x.Name == audioName);
                }
                if (this.Group.AutoDestoryPlayer && AudioList.Count == 0) {
                    Detach ();
                }
            } else {
                Debug.LogError (LanguageErrorMsg.LocalizeTextFormat (ErrorMsg.UnknownVersion, version));
            }
        }
    }
}