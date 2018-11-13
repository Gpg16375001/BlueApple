using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FileDownloadRequest : ILoadProgress
{
    public delegate void OnSuccess(bool ret);
    public delegate void OnError(Exception exception);
    public delegate void OnProgress(float value);

    public string URL {
        get;
        private set;
    }

    public event OnSuccess DownloadedEvent;
    public event OnError ErrorEvent;
    public event OnProgress ProgressEvent;

    public FileDownloadRequest(string url)
    {
        URL = url;
    }

    private float _progress;
    public float ProgressValue {
        get {
            return _progress;
        }
    }

    public void Success(bool ret)
    {
        if (DownloadedEvent != null) {
            DownloadedEvent.Invoke (ret);
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
