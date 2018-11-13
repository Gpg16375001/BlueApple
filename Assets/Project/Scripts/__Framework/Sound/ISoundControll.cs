using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SmileLab {
    /// <summary>
    /// サウンド機能を提供するためのインターフェース
    /// </summary>
    public interface ISoundControll : IDisposable
    {

        /// <summary>
        /// BGMのボリューム
        /// </summary>
        float VolumeBGM {
            get;
            set;
        }

        /// <summary>
        /// SEのボリューム
        /// </summary>
        float VolumeSE {
            get;
            set;
        }

        /// <summary>
        /// ボイスのボリューム
        /// </summary>
        float VolumeVoice {
            get;
            set;
        }

        /// <summary>
        /// サウンドが再生中か
        /// </summary>
        bool IsPlay {
            get;
        }

        /// <summary>
        /// BGM再生中か
        /// </summary>
        bool IsPlayBGM {
            get;
        }

        /// <summary>
        /// SE再生中か
        /// </summary>
        bool IsPlaySE {
            get;
        }

        /// <summary>
        /// ボイス再生中か
        /// </summary>
        bool IsPlayVoice {
            get;
        }

        /// <summary>
        /// BGMの再生
        /// 基本的にすでに同じBGMがなっている時は鳴らし直しをしない。
        /// すでに同じBGMがなっている時に鳴らし直ししたい場合はforcePlayをtrueに指定する。
        /// </summary>
        /// <param name="clip">再生したいclip</param>
        /// <param name="bLoop">ループ再生指定</param>
        /// <param name="forcePlay">すでに同じBGMがなっていても再度最初から鳴らし直す</param>
        void PlayBGM (SoundClipName clip, bool bLoop, bool forcePlay);
        void PlayBGM (string clipName, bool bLoop, bool forcePlay);
        /// <summary>
        /// BGMの停止
        /// </summary>
        void StopBGM ();

        /// <summary>
        /// SEの再生
        /// </summary>
        /// <returns>管理番号</returns>
        /// <param name="clip">再生したいclip</param>
        /// <param name="bLoop">ループ再生指定</param>
        int PlaySE (SoundClipName clip, bool bLoop);
        int PlaySE (string clipName, bool bLoop);

        /// <summary>
        /// 指定されたindexの音を止める
        /// indexにはPlaySEで返された管理番号を指定する
        /// マイナス値を指定するとすべてのSEが止まる。
        /// </summary>
        /// <param name="index">止めたいSEの管理番号</param>
        void StopSE (int index);


        /// <summary>
        /// ボイスの再生
        /// </summary>
        void PlayVoice (string fileName, string cueName);

        /// <summary>
        /// Containses the voice.
        /// </summary>
        /// <returns><c>true</c>, if voice was containsed, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="cueName">Cue name.</param>
        bool ContainsVoice(string fileName, string cueName);

        /// <summary>
        /// ボイスの停止
        /// </summary>
        void StopVoice ();

        /// <summary>
        /// なっているすべてのサウンドを止める
        /// </summary>
        void StopAll ();

        /// <summary>
        /// Pause処理
        /// </summary>
        void Pause ();

        /// <summary>
        /// Resume処理
        /// </summary>
        void Resume ();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="manager">SoundManagerのインスタンス</param>
        void Init (SoundManager manager);

        /// <summary>
        /// Update時に行いたい処理
        /// </summary>
        void Update ();

        /// <summary>
        /// LateUpdate時に行いたい処理
        /// </summary>
        void LateUpdate ();


        /// <summary>
        /// 必要リソースのダウンロード処理
        /// </summary>
        /// <param name="clip">Clip.</param>
        /// <param name="didDownload">Did download.</param>
        void DownloadResource (SoundClipName clip, System.Action didDownload = null);
        void DownloadResource (string clipName, System.Action didDownload = null);
        void DownloadResource (string[] clipNames, System.Action didDownload = null);
    }
}