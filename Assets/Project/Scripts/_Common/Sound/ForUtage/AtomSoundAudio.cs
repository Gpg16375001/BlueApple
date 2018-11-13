using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Utage
{
    public class AtomSoundAudio
    {
        /// <summary>
        /// DataのNameを取得
        /// セーブデータ用の処理
        /// </summary>
        public string Name {
            get {
                if (Data != null) {
                    return Data.Name;
                }
                return string.Empty;
            }
        }

        // 自信を管理しているPlayer
        AtomSoundPlayer Player { get; set; }
        // Atom用の詳細設定を取得するためのインスタンス
        AtomForUtage AtomForUtage {
            get {
                return Player.Group.AtomSoundManagerSystem.AtomForUtage;
            }
        }
        // 管理しているグループ名を取得
        string GroupName {
            get {
                return Player.Group.GroupName;
            }
        }

        // ボイスタイプか確認
        bool IsVoice {
            get {
                return GroupName == SoundManager.IdVoice;
            }
        }

        bool IsBGM 
        {
            get {
                return GroupName == SoundManager.IdBgm;
            }
        }

        private CriSoundControll CriSoundController {
            get {
                return Player.Group.AtomSoundManagerSystem.CriSoundController;
            }
        }

        // 再生するアセットファイル
        AtomAssetFile AtomFile;

        //オーディオの情報
        internal SoundData Data { get; private set; }
        // 再生用のCriAdx2用のPlayer
        int criPlayerIndex = -1;

        // 自信の再生ボリューム
        public float Volume {
            get {
                return volume;
            }
            private set {
                if (criPlayerIndex >= 0 && volume != value) {
                    volume = value;
                    CriSoundController.SetVolumeFromIndex (criPlayerIndex, volume);
                }
            }
        }

        float volume;

        // フェード中か
        bool isFadeOuting = false;
        // 再生ウェイト中
        bool waitDelay;
        // フェード値
        float fadeValue = 1.0f;

        // フェード管理クラス
        class FadeInfo
        {
            public bool Enable { get; private set; }

            public float From { get; set; }

            public float To { get; private set; }

            public float Time { get; private set; }

            public float Duration { get; private set; }

            public float Value { get; private set; }

            public void Start (float from, float to, float duration)
            {
                Enable = true;
                Value = From = from;
                To = to;
                Duration = duration;
                Time = 0;
            }

            public bool Update ()
            {
                if (!Enable)
                    return false;

                Time += UnityEngine.Time.deltaTime;
                if (Time >= Duration) {
                    Value = To;
                    Enable = false;
                } else {
                    Value = Mathf.Lerp (From, To, Time / Duration);
                }
                return true;
            }
        }

        FadeInfo fadeInfo = new FadeInfo ();

        //セーブが有効かどうか
        internal bool EnableSave {
            get {
                return !isFadeOuting;
            }
        }

        /// <summary>
        /// 再生中か
        /// </summary>
        public bool IsPlaying ()
        {
            return !isFadeOuting && CriSoundController.IsPlayFromIndex (criPlayerIndex);
        }

        /// <summary>
        /// 指定データが再生中か
        /// </summary>
        public bool IsPlaying (SoundData data)
        {
            return (IsEqualSoundData (data) && IsPlaying ());
        }

        /// <summary>
        /// ループ再生中か
        /// </summary>
        public bool IsPlayingLoop ()
        {
            return IsPlaying () && Data.IsLoop;
        }

        /// <summary>
        /// 毎フレーム処理
        /// 管理しているPlayerから呼び出される
        /// </summary>
        public void Update ()
        {
            if (criPlayerIndex < 0) {
                return;
            }

            if (fadeInfo.Enable) {
                if (fadeInfo.Update ()) {
                    fadeValue = fadeInfo.Value;
                }
                if(!fadeInfo.Enable && isFadeOuting) {
                    Stop ();
                }
            }

            if (!waitDelay) {
                switch (CriSoundController.GetPlayerStatusFromIndex (criPlayerIndex)) {
                case CriAtomExPlayer.Status.Error:
                case CriAtomExPlayer.Status.Stop:
                case CriAtomExPlayer.Status.PlayEnd:
                    Detach ();
                    break;
                }
            }
        }

        /// <summary>
        /// 毎フレーム終了時処理
        /// 管理しているPlayerから呼び出される
        /// </summary>
        public void LateUpdate ()
        {
            //ボリュームの更新
            Volume = GetVolume ();
        }

        //初期化
        internal void Init (AtomSoundPlayer player, SoundData soundData)
        {
            this.Player = player;
            this.Data = soundData;
            this.AtomFile = Data.File as AtomAssetFile;
        }

        // 現在再生音声のボリューム
        internal float GetSamplesVolume ()
        {
            throw new NotImplementedException ();
        }

        internal CriAtomExPlayerOutputAnalyzer GetAudioAnalyzer ()
        {
            return CriSoundController.GetAnalyzerFromIndex (criPlayerIndex);
        }

        // サウンドデータのチェック
        internal bool IsEqualSoundData (SoundData data)
        {
            AtomAssetFile atomFile = data.File as AtomAssetFile;
            if (atomFile == null || AtomFile == null) {
                return false;
            }
            return (atomFile.CueName == AtomFile.CueName) && (atomFile.CueSheet == AtomFile.CueSheet);
        }

        // 鳴らす
        internal void Play (float fadeTime, float delay = 0)
        {
            if (this.AtomFile == null) {
                Debug.LogError ("Not Support Audio Clip");
                Detach ();
                return;
            }
            Player.Group.StartCoroutine (CoWaitDelay (fadeTime, delay));
        }

        // フェードアウト処理
        internal void FadeOut (float fadeTime = 0)
        {
            if (IsPlaying ()) {
                if (!AtomForUtage.EnableFadeOnData && fadeTime > 0) {
                    isFadeOuting = true;
                    fadeInfo.Start (Volume, 0, fadeTime);
                } else {
                    Stop ();
                }
            } else {
                Detach ();
            }
        }

        // PlayerからのDetach
        void Detach ()
        {
            Player.Remove (this);
            if (IsVoice && AtomForUtage.EnableLipSyncForCri) {
                CriSoundController.RemoveAnalyzerFromIndex (criPlayerIndex);
            }
        }

        // 実際の再生処理
        IEnumerator CoWaitDelay (float fadeTime, float delay)
        {
            isFadeOuting = false;
            var acb = CriSoundController.LoadACB (AtomFile.CueSheet, IsBGM);

            if (!IsBGM) {
                criPlayerIndex = CriSoundController.SoundPrepare (acb, AtomFile.CueName,
                    Data.IsLoop, IsVoice && AtomForUtage.EnableLipSyncForCri, 0, 0);
            }
            Volume = 0.0f;
            if (delay > 0) {
                waitDelay = true;
                yield return new WaitForSeconds (delay);
                waitDelay = false;
            }

            if (IsBGM) {
                criPlayerIndex = CriSoundController.PlayBGMReturnIndex (AtomFile.CueName);
            } else {
                CriSoundController.StartFromIndex (criPlayerIndex);
            }

            if (!AtomForUtage.EnableFadeOnData && fadeTime > 0) {
                fadeInfo.Start (0, Data.Volume, fadeTime);
            } else {
                Volume = Data.Volume;
            }
        }

        //ボリューム計算
        float GetVolume ()
        {
            float volume = fadeValue * Data.Volume * Player.Group.GetVolume (Data.Tag);
            return volume;
        }

        void Stop ()
        {
            if (criPlayerIndex >= 0) {
                if (IsBGM) {
                    CriSoundController.StopBGM ();
                } else {
                    CriSoundController.StopFromIndex (criPlayerIndex, true);
                }
                criPlayerIndex = -1;
            }
            Detach ();
        }
    }
}
