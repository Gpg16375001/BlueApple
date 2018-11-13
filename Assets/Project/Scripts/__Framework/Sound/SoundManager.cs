using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab
{
    /// <summary>
    /// サウンド処理を提供する。
    /// 内部処理はISoundControll側で実装している。
    /// </summary>
    public class SoundManager : ViewBase
    {
        /// <summary>
        /// 共通インスタンス.
        /// </summary>
        public static SoundManager SharedInstance { get; private set; }

        // SoundControllのインスタンス
        private ISoundControll SoundController;

        // 初期化済みか
        private bool IsInitialize {
            get {
                return SoundController != null;
            }
        }

        /// <summary>
        /// BGMのボリューム
        /// </summary>
        public float VolumeBGM {
            get {
                if (IsInitialize) {
                    return SoundController.VolumeBGM;
                }
                return 0.0f;
            }
            set {
                if (IsInitialize) {
                    SoundController.VolumeBGM = value;
                }
            }
        }

        /// <summary>
        /// SEのボリューム
        /// </summary>
        public float VolumeSE {
            get {
                if (IsInitialize) {
                    return SoundController.VolumeSE;
                }
                return 0.0f;
            }
            set {
                if (IsInitialize) {
                    SoundController.VolumeSE = value;
                }
            }
        }

        /// <summary>
        /// ボイスのボリューム
        /// </summary>
        public float VolumeVoice {
            get {
                if (IsInitialize) {
                    return SoundController.VolumeVoice;
                }
                return 0.0f;
            }
            set {
                if (IsInitialize) {
                    SoundController.VolumeVoice = value;
                }
            }
        }

        /// <summary>
        /// サウンドが再生中か
        /// </summary>
        public bool IsPlay {
            get {
                if (IsInitialize) {
                    return SoundController.IsPlay;
                }
                return false;
            }
        }

        /// <summary>
        /// BGM再生中か
        /// </summary>
        public bool IsPlayBGM {
            get {
                if (IsInitialize) {
                    return SoundController.IsPlayBGM;
                }
                return false;
            }
        }

        /// <summary>
        /// SE再生中か
        /// </summary>
        public bool IsPlaySE {
            get {
                if (IsInitialize) {
                    return SoundController.IsPlaySE;
                }
                return false;
            }
        }

        /// <summary>
        /// ボイス再生中か
        /// </summary>
        public bool IsPlayVoice {
            get {
                if (IsInitialize) {
                    return SoundController.IsPlayVoice;
                }
                return false;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <typeparam name="T">サウンドコントローラーの指定</typeparam>
        public void Initialize <T>() where T : ISoundControll
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            SoundController = (ISoundControll)System.Activator.CreateInstance(typeof(T));
            SoundController.Init(this);
        }

        /// <summary>
        /// 使用後は必ず呼び出すこと！内部でGameObjectも破棄してる.アンマネージド系のリソースを使うことを考慮.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="SmileLab.SoundManager"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="SmileLab.SoundManager"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="SmileLab.SoundManager"/> so
        /// the garbage collector can reclaim the memory that the <see cref="SmileLab.SoundManager"/> was occupying.</remarks>
        public override void Dispose ()
        {
            if (SoundController != null) {
                SoundController.Dispose ();
                SoundController = null;
            }
            base.Dispose ();
        }

        /// <summary>
        /// BGMの再生
        /// 基本的にすでに同じBGMがなっている時は鳴らし直しをしない。
        /// すでに同じBGMがなっている時に鳴らし直ししたい場合はforcePlayをtrueに指定する。
        /// </summary>
        /// <param name="clip">再生したいclip</param>
        /// <param name="bLoop">ループ再生指定</param>
        /// <param name="forcePlay">すでに同じBGMがなっていても再度最初から鳴らし直す</param>
        public void PlayBGM (SoundClipName clip, bool bLoop = false, bool forcePlay = false)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.PlayBGM (clip, bLoop, forcePlay);
        }

        public void PlayBGM (string clipName, bool bLoop = false, bool forcePlay = false)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.PlayBGM (clipName, bLoop, forcePlay);
        }

        /// <summary>
        /// BGMの停止
        /// </summary>
        public void StopBGM ()
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.StopBGM ();
        }

        /// <summary>
        /// SEの再生
        /// </summary>
        /// <returns>管理番号</returns>
        /// <param name="clip">再生したいclip</param>
        /// <param name="bLoop">ループ再生指定</param>
        public int PlaySE (SoundClipName clip, bool bLoop = false)
        {
            if (!IsInitialize) {
                return -1;
            }
            return SoundController.PlaySE (clip, bLoop);
        }

        public int PlaySE (string clipName, bool bLoop = false)
        {
            if (!IsInitialize) {
                return -1;
            }
            return SoundController.PlaySE (clipName, bLoop);
        }

        public void PlayVoice(string fileName, string cueName)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.PlayVoice (fileName, cueName);
        }

        public void PlayVoice (string fileName, SoundVoiceCueEnum cueName)
        {
            PlayVoice(fileName, cueName.ToString());
        }

        public bool ContainsVoice(string fileName, string cueName)
        {
            if (!IsInitialize) {
                return false;
            }
            return SoundController.ContainsVoice (fileName, cueName);
        }

        public bool ContainsVoice(string fileName, SoundVoiceCueEnum cueName)
        {
            return ContainsVoice (fileName, cueName.ToString ());
        }

        public void StopVoice()
        {
            SoundController.StopVoice ();
        }

        /// <summary>
        /// 指定されたindexの音を止める
        /// indexにはPlaySEで返された管理番号を指定する
        /// マイナス値を指定するとすべてのSEが止まる。
        /// </summary>
        /// <param name="index">止めたいSEの管理番号</param>
        public void StopSE (int index = -1)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.StopSE (index);
        }

        /// <summary>
        /// なっているすべてのサウンドを止める
        /// </summary>
        public void StopAll()
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.StopAll ();
        }

        /// <summary>
        /// サウンドのPause処理
        /// </summary>
        public void Pause()
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.Pause ();
        }

        /// <summary>
        /// サウンドのResume処理
        /// </summary>
        public void Resume()
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.Resume ();
        }

        /// <summary>
        /// 指定した型に変換をかけてSoundControllを取得
        /// 指定した型ではない時はExceptionが出るので気をつけてください。
        /// </summary>
        /// <returns>SoundControllを返す</returns>
        /// <typeparam name="T">取得したい型</typeparam>
        public T GetSoundControll<T>() where T : ISoundControll
        {
            return (T)SoundController;
        }

        public void DownloadResource(SoundClipName clip, System.Action didDownload=null)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.DownloadResource (clip, didDownload);
        }

        public void DownloadResource(string clipName, System.Action didDownload=null)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.DownloadResource (clipName, didDownload);
        }

        public void DownloadResource(string[] clipNames, System.Action didDownload=null)
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.DownloadResource (clipNames, didDownload);
        }

        void Awake()
        {
            if(SharedInstance != null) {
                SharedInstance.Dispose();
            }
            SharedInstance = this;
        }

        void Update()
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.Update ();
        }

        void LateUpdate()
        {
            if (!IsInitialize) {
                return;
            }
            SoundController.LateUpdate ();
        }

        // アプリポーズ時
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) {
                Pause ();
            } else {
                Resume ();
            }
        }

        #if UNITY_EDITOR
        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                if (SharedInstance != null) {
                    SharedInstance.StopAll ();
                }
            }
        }
        #endif
    }
}
