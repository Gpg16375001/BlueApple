using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoadRequest : ILoadProgress
{
    public delegate void OnSuccess(AssetBundleRef assetBundleRef);
    public delegate void OnError(Exception exception);
    public delegate void OnProgress(float value);

    private float _progress;
    public float ProgressValue {
        get {
            return _progress;
        }
    }

    public string URL {
        get;
        private set;
    }
    public int Count {
        get;
        private set;
    }

    private event OnSuccess _DownloadedEvent;
    public event OnSuccess DownloadedEvent {
        add {
            _DownloadedEvent += value;
            Count++;
        }
        remove {
            _DownloadedEvent -= value;
            Count--;
        }
    }
    public event OnError ErrorEvent;
    public event OnProgress ProgressEvent;

    public AssetBundleLoadRequest(string url)
    {
        URL = url;
        Count = 0;
    }

    public void Success(AssetBundleRef assetBundleRef)
    {
        if (_DownloadedEvent != null) {
            _DownloadedEvent.Invoke (assetBundleRef);
        }
    }

    public bool HasErrorEvent {
        get {
            return ErrorEvent != null;
        }
    }
    public void Error(Exception ex)
    {
        if (ErrorEvent != null) {
            ErrorEvent.Invoke (ex);
        }
    }

    public void Progress(float value)
    {
        _progress = value;
        if (ProgressEvent != null) {
            ProgressEvent.Invoke (value);
        }
    }
}
