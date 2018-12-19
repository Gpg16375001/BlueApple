using System;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.WebRequest;

#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

namespace SmileLab.Net
{
    /// <summary>
    /// class : ネット通信の統括管理.
    /// </summary>
    public class NetRequestManager
    {
        /// <summary>
        /// タイムアウトする時間.
        /// </summary>
        public static float TimeOutTime
        {
            get {
                return timeOutTime > 0f ? timeOutTime : TIME_TIMEOUT_DEFAULT;
            }
            set {
                timeOutTime = value;
            }
        }
        private static float timeOutTime = -1f;
        // サーバー側のタイムアウトが29秒なので
        private static readonly float TIME_TIMEOUT_DEFAULT = 35f;

        // retry回数
        public static int RetryCount
        {
            get {
                return retryCount > 0 ? retryCount : RETRY_COUNT_DEFAULT;
            }
            set {
                retryCount = value;
            }
        }
        private static int retryCount = -1;
        private static readonly int RETRY_COUNT_DEFAULT = 5;

#if UNITY_5_4_OR_NEWER
        /// <summary>
        /// UnityWebRequest通信リクエスト.Get通信.
        /// </summary>
        public static void Get(string url, Dictionary<string, string> header, Action<UnityWebRequest> didLoad, Action<Exception> didError = null)
        {
            ObservableWebRequest.GetRequest(url, header)
                                .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                                .Subscribe(didLoad, ex => {
                                    OnError(ex, didError);
                                });
        }
#else
        /// <summary>
        /// WWW通信リクエスト.Get通信.
        /// </summary>
        [Obsolete("WWWを使った通信は推奨されません。UnityWebRequestを用いたGetを使用してください。")]
        public static void Get(string url, Dictionary<string, string> header, Action<WWW> didLoad, Action<Exception> didError = null)
        {
            ObservableWWW.GetWWW(url, header)
                         .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                         .Subscribe(didLoad, ex => {
                             OnError(ex, didError);
                         });
        }
#endif

#if UNITY_5_4_OR_NEWER
        /// <summary>
        /// UnityWebRequest通信リクエスト.Post通信.
        /// </summary>
        public static void Post(string url, Dictionary<string, string> post, Dictionary<string, string> header, Action<UnityWebRequest> didLoad, Action<Exception> didError = null)
        {
            ObservableWebRequest.PostRequest(url, post, header)
                                .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                                .Subscribe(didLoad, ex => {
                                    OnError(ex, didError);
                                });
        }
#else
        /// <summary>
        /// WWW通信リクエスト.Post通信.
        /// </summary>
        [Obsolete("WWWを使った通信は推奨されません。UnityWebRequestを用いたPostを使用してください。")]
        public static void Post(string url, byte[] post, Dictionary<string, string> header, Action<WWW> didLoad, Action<Exception> didError = null)
        {

            ObservableWWW.PostWWW(url, post, header)
                         .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                         .Subscribe(didLoad, ex => {
                             OnError(ex, didError);
                         });
        }
#endif

#if UNITY_5_4_OR_NEWER
        /// <summary>
        /// UnityWebRequest通信リクエスト.Post通信.
        /// </summary>
        public static void Post(string url, byte[] post, Dictionary<string, string> header, Action<UnityWebRequest> didLoad, Action<Exception> didError = null)
        {
            ObservableWebRequest.PostRequest(url, post, header)
                                .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                                .Subscribe(didLoad, ex => {
                                    OnError(ex, didError);
                                });
        }
#endif

        /// <summary>
        /// 通常ダウンロード.
        /// </summary>
        public static void Download(string url, Action<byte[]/*www.bytes*/> didLoad,
            Action<Exception> didError = null, Action<float> progressAction = null)
        {
#if UNITY_5_4_OR_NEWER
            // ダウンロード.
            UniRx.Progress<float> progress = null;
            if (progressAction != null) {
                progress = new UniRx.Progress<float> (progressAction);
            }
            ObservableWebRequest.GetAndGetBytes(url, 0, null, progress)
                .OnErrorRetry((UnityWebRequestNetworkErrorException ex) => {
                    Debug.LogError(ex.Message);
                }, RetryCount)
                .Subscribe(bytes => didLoad(bytes), ex => OnError(ex, didError));
#else
            // ダウンロード.
            ObservableWWW.GetWWW(url)
                         .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                         .Subscribe(www => didLoad(www.bytes), ex => OnError(ex, didError));
#endif
        }

        /// <summary>
        /// 通常ダウンロード（pathへの書き込みも行う）
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="path">Path.</param>
        /// <param name="didLoad">Did load.</param>
        /// <param name="didError">Did error.</param>
        public static void Download(string url, string path, byte[] buffer, Action<bool> didLoad,
            Action<Exception> didError = null, Action<float> progressAction = null)
        {
            UniRx.Progress<float> progress = null;
            if (progressAction != null) {
                progress = new UniRx.Progress<float> (progressAction);
            }
            ObservableWebRequest.GetAndWriteFile (url, path, buffer, 0, progress:progress)
                .OnErrorRetry((UnityWebRequestNetworkErrorException ex) => {
                    Debug.LogError(ex.Message);
                }, RetryCount)
                .Subscribe (result => didLoad (result), ex => OnError (ex, didError));
        }

        /// <summary>
        /// ネットワーク上から直接Spriteを生成する。
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="path">Path.</param>
        /// <param name="buffer">Buffer.</param>
        /// <param name="didLoad">Did load.</param>
        /// <param name="didError">Did error.</param>
        /// <param name="progressAction">Progress action.</param>
        public static void DownloadSprite(string url, Action<Sprite> didLoad,
            Action<Exception> didError = null, Action<float> progressAction = null)
        {
            UniRx.Progress<float> progress = null;
            if (progressAction != null) {
                progress = new UniRx.Progress<float> (progressAction);
            }
            ObservableWebRequest.GetSprite (url, (int)TimeOutTime, progress:progress)
                .OnErrorRetry((UnityWebRequestNetworkErrorException ex) => {
                    Debug.LogError(ex.Message);
                }, RetryCount)
                .Subscribe (result => didLoad (result), ex => OnError (ex, didError));
        }

        /// <summary>
        /// アセットバンドルダウンロード.version=uint
        /// </summary>
        public static void LoadFromCacheOrDownload(string url, uint version, uint crc, Action<AssetBundle> didLoad, Action<Exception> didError = null,  Action<float> progressAction = null)
        {
#if UNITY_5_4_OR_NEWER
            UniRx.Progress<float> progress = null;
            if (progressAction != null) {
                progress = new UniRx.Progress<float> (progressAction);
            }
            ObservableWebRequest.LoadFromCacheOrDownload(url, version, crc, 0, progress)
                .OnErrorRetry((UnityWebRequestNetworkErrorException ex) => {
                    Debug.LogError(ex.Message);
                }, RetryCount)
                .Subscribe(didLoad, ex => {
                    OnError(ex, didError);
                });
#else
            ObservableWWW.LoadFromCacheOrDownload(url, (int)version, crc)
                         .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                         .Subscribe(didLoad, ex => {
                             OnError(ex, didError);
                         });
#endif

        }

        /// <summary>
        /// アセットバンドルダウンロード.version=uint(CRC使用しないVer.)
        /// </summary>
        public static void LoadFromCacheOrDownload(string url, uint version, Action<AssetBundle> didLoad, Action<Exception> didError = null, Action<float> progress = null)
        {
            LoadFromCacheOrDownload(url, version, 0, didLoad, didError, progress);
        }

        /// <summary>
        /// アセットバンドルダウンロード.version=hash128
        /// </summary>
        public static void LoadFromCacheOrDownload(string url, Hash128 version, uint crc, Action<AssetBundle> didLoad, Action<Exception> didError = null, Action<float> progressAction = null)
        {
#if UNITY_5_4_OR_NEWER
            UniRx.Progress<float> progress = null;
            if (progressAction != null) {
                progress = new UniRx.Progress<float> (progressAction);
            }
            ObservableWebRequest.LoadFromCacheOrDownload(url, version, crc, 0, progress)
                .OnErrorRetry((UnityWebRequestNetworkErrorException ex) => {
                    Debug.LogError(ex.Message);
                }, RetryCount)
                .Subscribe(didLoad, ex => {
                    OnError(ex, didError);
                });
#else
            ObservableWWW.LoadFromCacheOrDownload(url, version, crc)
                         .Timeout(TimeSpan.FromSeconds(TimeOutTime))
                         .Subscribe(didLoad, ex => {
                             OnError(ex, didError);
                         });
#endif
        }

        /// <summary>
        /// アセットバンドルダウンロード.version=hash128(CRC使用しないVer.)
        /// </summary>
        public static void LoadFromCacheOrDownload(string url, Hash128 version, Action<AssetBundle> didLoad, Action<Exception> didError = null, Action<float> progress = null)
        {
            LoadFromCacheOrDownload(url, version, 0, didLoad, didError, progress);
        }

        // エラー時のコールバック.
        static void OnError(Exception exception, Action<Exception> didError)
        {
            var eType = exception.GetType();

            // エラー発生時の共通動作.設定がなければタイトルリブート.
            Action didErrorEx = () => {
                if(didError != null) {
                    didError(exception);
                }else{
                    PopupManager.OpenPopupSystemOK("エラーが発生しました。\nタイトルに戻ります。", () => ScreenChanger.SharedInstance.Reboot());
                }
            };

            // タイムアウト.
            if(eType == typeof(TimeoutException)){
                Debug.LogError("[NetRequestManager] OnError : Timeout.");
                didErrorEx();
                return;
            }
#if UNITY_5_4_OR_NEWER
            // UnityWebRequestエラー.
            if(eType == typeof(UnityWebRequestErrorException)){
                var e = exception as UnityWebRequestErrorException;
                Debug.LogError("[NetRequestManager] OnError : " + e.Message + " / " +e.Request.error+"\nURL="+e.Request.url);
                didErrorEx();
                return;
            }

#endif
            // WWWエラー.
            if(eType == typeof(WWWErrorException)){
                var e = exception as WWWErrorException;
                Debug.LogError("[NetRequestManager] OnError : " + e.Message + " / " +e.WWW.error+"\nURL="+e.WWW.url);
                didErrorEx();
                return;
            }
            // その他なんらかのエラー.
            Debug.LogError("[NetRequestManager] OnError : " + exception.Message);
            didErrorEx();
        }
    }
}