using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

namespace UniRx.WebRequest
{
    /// <summary>
    /// UniRx ObservableWWWのWebRequest使用版のようなもの.基本的にこちらの方が性能が上のはず.
    /// ソース : http://qiita.com/Marimoiro/items/7f007805ab8825c43a80
    /// </summary>
    public static class ObservableWebRequest
    {
        public static IObservable<UnityWebRequest> ToRequestObservable(this UnityWebRequest request, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(request, null, observer, progress, cancellation));
        }

        public static IObservable<string> ToObservable(this UnityWebRequest request, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(request, null, observer, progress, cancellation));
        }

        public static IObservable<byte[]> ToBytesObservable(this UnityWebRequest request, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => Fetch(request, null, observer, progress, cancellation));
        }

        public static IObservable<string> Get(string url, IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return
                ObservableUnity.FromCoroutine<string>(
                    (observer, cancellation) =>
                        FetchText(UnityWebRequest.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<byte[]> GetAndGetBytes(string url, int timeout = 0, IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(UnityWebRequest.Get(url), timeout, headers, observer, progress, cancellation));
        }
        public static IObservable<bool> GetAndWriteFile(string url, string path, byte[] buffer, int timeout = 0, IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<bool>((observer, cancellation) => FetchFile(UnityWebRequest.Get(url), path, buffer, timeout, headers, observer, progress, cancellation));
        }
        public static IObservable<Sprite> GetSprite(string url, int timeout = 0, IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<Sprite>((observer, cancellation) => FetchSprite(UnityWebRequestTexture.GetTexture(url), timeout, headers, observer, progress, cancellation));
        }
        public static IObservable<UnityWebRequest> GetRequest(string url, IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(UnityWebRequest.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, Dictionary<string, string> postData,
            IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(UnityWebRequest.Post(url, postData), headers, observer, progress, cancellation));

        }

        public static IObservable<byte[]> PostAndGetBytes(string url, Dictionary<string, string> postData, int timeout = 0, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(UnityWebRequest.Post(url, postData), timeout, null, observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, Dictionary<string, string> postData, IDictionary<string, string> headers, int timeout = 0, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(UnityWebRequest.Post(url, postData), timeout, headers, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> PostRequest(string url, Dictionary<string, string> postData, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(UnityWebRequest.Post(url, postData), null, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> PostRequest(string url, Dictionary<string, string> postData, IDictionary<string, string> headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(UnityWebRequest.Post(url, postData), headers, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> PostRequest(string url, WWWForm postData, IDictionary<string, string> headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => Fetch(UnityWebRequest.Post(url, postData), headers, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> PostRequest(string url, byte[] postData, IDictionary<string, string> headers, IProgress<float> progress = null)
        {
            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) => FetchRequest(request, headers, observer, progress, cancellation));
        }

        public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, uint version, uint crc, int timeout = 0, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) => FetchAssetBundle(UnityWebRequest.GetAssetBundle(url, version, crc), timeout, null, observer, progress, cancellation));
        }
        public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, Hash128 version, uint crc, int timeout = 0, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) => FetchAssetBundle(UnityWebRequest.GetAssetBundle(url, version, crc), timeout, null, observer, progress, cancellation));
        }

        static IEnumerator Fetch<T>(UnityWebRequest request, IDictionary<string, string> headers, IObserver<T> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {

            if(headers != null) {
                foreach(var header in headers) {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            if(reportProgress != null) {
                var operation = request.SendWebRequest();
                while(!operation.isDone && !cancel.IsCancellationRequested) {
                    try {
                        reportProgress.Report(operation.progress);
                    } catch(Exception ex) {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return null;
                }
            } else {
                yield return request.SendWebRequest();
            }

            if(cancel.IsCancellationRequested) {
                yield break;
            }
                
            if(reportProgress != null) {
                try {
                    reportProgress.Report(request.downloadProgress);
                } catch(Exception ex) {
                    observer.OnError(ex);
                    yield break;
                }
            }
        }


        static IEnumerator FetchRequest(UnityWebRequest request, IDictionary<string, string> headers, IObserver<UnityWebRequest> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using(request) {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if(cancel.IsCancellationRequested) {
                    yield break;
                }

                if(!string.IsNullOrEmpty(request.error)) {
                    observer.OnError(new UnityWebRequestErrorException(request));
                } else {
                    observer.OnNext(request);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchText(UnityWebRequest request, IDictionary<string, string> headers, IObserver<string> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using(request) {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if(cancel.IsCancellationRequested) {
                    yield break;
                }

                if(!string.IsNullOrEmpty(request.error)) {
                    observer.OnError(new UnityWebRequestErrorException(request));
                } else {
                    var text = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                    observer.OnNext(text);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchAssetBundle(UnityWebRequest request, int timeout, IDictionary<string, string> headers, IObserver<AssetBundle> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using(request) {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if(cancel.IsCancellationRequested) {
                    yield break;
                }

                if(!string.IsNullOrEmpty(request.error)) {
                    if (request.error == "Request timeout") {
                        observer.OnError (new TimeoutException ());
                    } else if (request.isNetworkError) { 
                        observer.OnError(new UnityWebRequestNetworkErrorException(request));
                    } else {
                        observer.OnError(new UnityWebRequestErrorException(request));
                    }
                } else {
                    var handler = request.downloadHandler as DownloadHandlerAssetBundle;
                    var assetBundle = (handler != null) ? handler.assetBundle : null;

                    observer.OnNext(assetBundle);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchBytes(UnityWebRequest request, int timeout, IDictionary<string, string> headers, IObserver<byte[]> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using(request) {
                request.timeout = timeout;
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if(cancel.IsCancellationRequested) {
                    yield break;
                }

                if(!string.IsNullOrEmpty(request.error)) {
                    if (request.error == "Request timeout") {
                        observer.OnError(new TimeoutException());
                    } else if (request.isNetworkError) { 
                        observer.OnError(new UnityWebRequestNetworkErrorException(request));
                    } else {
                        observer.OnError(new UnityWebRequestErrorException(request));
                    }
                } else {
                    observer.OnNext(request.downloadHandler.data);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchFile(UnityWebRequest request, string path, byte[] buffer, int timeout, IDictionary<string, string> headers, IObserver<bool> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using(request) {
                request.timeout = timeout;
                DownloadAndWriteFileHandler handler = buffer == null ? 
                    new DownloadAndWriteFileHandler(path) :
                    new DownloadAndWriteFileHandler(path, buffer);
                request.downloadHandler = handler;

                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if(cancel.IsCancellationRequested) {
                    yield break;
                }

                if(!string.IsNullOrEmpty(request.error)) {
                    if (request.error == "Request timeout") {
                        observer.OnError(new TimeoutException());
                    } else if (request.isNetworkError) { 
                        observer.OnError(new UnityWebRequestNetworkErrorException(request));
                    } else {
                        observer.OnError(new UnityWebRequestErrorException(request));
                    }
                } else {
                    observer.OnNext(handler.Result);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchSprite(UnityWebRequest request, int timeout, IDictionary<string, string> headers, IObserver<Sprite> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using(request) {
                request.timeout = timeout;

                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if(cancel.IsCancellationRequested) {
                    yield break;
                }

                if(!string.IsNullOrEmpty(request.error)) {
                    if (request.error == "Request timeout") {
                        observer.OnError(new TimeoutException());
                    } else if (request.isNetworkError) { 
                        observer.OnError(new UnityWebRequestNetworkErrorException(request));
                    } else {
                        observer.OnError(new UnityWebRequestErrorException(request));
                    }
                } else {

                    var textureHandler = request.downloadHandler as DownloadHandlerTexture;
                    var texture = textureHandler.texture;
                    observer.OnNext(Sprite.Create(texture, new Rect() {x = 0, y = 0, width=texture.width, height=texture.height}, new Vector2(0.5f, 0.5f)));
                    observer.OnCompleted();
                }
            }
        }


    }

    public class UnityWebRequestErrorException : Exception
    {
        public string RawErrorMessage { get; private set; }
        public bool HasResponse { get; private set; }
        public string Text { get; private set; }
        public System.Net.HttpStatusCode StatusCode { get; private set; }
        public System.Collections.Generic.Dictionary<string, string> ResponseHeaders { get; private set; }
        public UnityWebRequest Request { get; private set; }

        // cache the text because if www was disposed, can't access it.
        public UnityWebRequestErrorException(UnityWebRequest request)
        {
            this.Request = request;
            this.RawErrorMessage = request.error;
            this.ResponseHeaders = request.GetResponseHeaders();
            this.HasResponse = false;

            StatusCode = (System.Net.HttpStatusCode)request.responseCode;


            if(request.downloadHandler is DownloadHandlerBuffer) {
                Text = request.downloadHandler.text;
            }

            if(request.responseCode != 0) {
                this.HasResponse = true;
            }
        }

        public override string Message {
            get {
                return base.Message + " " + ToString();
            }
        }

        public override string ToString()
        {
            var text = this.Text;
            if(string.IsNullOrEmpty(text)) {
                return RawErrorMessage;
            } else {
                return RawErrorMessage + " " + text;
            }
        }
    }

    public class UnityWebRequestNetworkErrorException : Exception
    {
        public string RawErrorMessage { get; private set; }
        public UnityWebRequest Request { get; private set; }

        // cache the text because if www was disposed, can't access it.
        public UnityWebRequestNetworkErrorException(UnityWebRequest request)
        {
            this.Request = request;
            this.RawErrorMessage = request.error;
        }

        public override string Message {
            get {
                return base.Message + " " + ToString();
            }
        }

        public override string ToString()
        {
            return RawErrorMessage;
        }
    }
}
