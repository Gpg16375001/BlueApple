using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class View_WebView : ViewBase {
    static View_WebView instance = null;

    /// <summary>
    /// iOS/Android共にWebViewで開く.
    /// </summary>
    public static View_WebView Open(string url, Action<string> startedCallback = null)
    {
        if (instance != null) {
            instance.m_StartedCallback = startedCallback;
            instance.OpenUrl (url);
            return instance;
        }

        var go = GameObjectEx.LoadAndCreateObject ("_Common/View/View_WebView");
        instance = go.GetOrAddComponent<View_WebView> ();
        instance.InitInternal (url, startedCallback);
        return instance;
    }

    public static void Close()
    {
        if (instance != null) {
            instance.Dispose ();
            instance = null;
        }
    }

    public override void Dispose ()
    {
        if (m_WebViewObject != null) {
            m_WebViewObject.SetVisibility (false);
        }
        base.Dispose ();
    }

    void InitInternal(string url, Action<string> startedCallback)
    {
        m_StartedCallback = startedCallback;
        var contents = GetScript<RectTransform> ("Contents").gameObject;
		this.SetCanvasCustomButtonMsg("bt_WebViewClose", DidTapCloseWebView);
        contents.SetActive(true);
        if (m_WebViewObject == null) {
            m_WebViewObject = GetScript<WebViewObject>("WebView");
            m_WebViewObject.Init(
                (str) => {
                    try {
                        TapWebViewLink(str);
                    }
                    catch {
                        Debug.LogFormat("WebView Callback Err: {0}", str);
                    }
                },
                err: (str) => {
                    Debug.LogErrorFormat("WebView Err: {0}", str);
                },
                httpErr: (str) => {
                    Debug.LogErrorFormat("WebView HttpErr: {0}", str);
                },
                enableWKWebView: false,
                started: m_StartedCallback
            );
            m_WebViewObject.SetMargins(0, 0, 0, 100);
        }
        OpenUrl(url);
    }

    void OpenUrl(string url)
    {
        //Debug.LogFormat ("Open URL: {0}", url);
		if (m_WebViewObject != null) {
            m_WebViewObject.LoadURL(url);
            m_WebViewObject.SetVisibility(true);
        }
    }

    void TapWebViewLink(string message)
    {
        if (string.IsNullOrEmpty (message)) {
            Debug.Log ("Message Is NULL");
            return;
        }

        var parameters = message.Split ('|');
        int enumIndex = 0;

        if (parameters == null || parameters.Length == 0) {
            Debug.Log ("データが不正です");
            return;
        } else if (!int.TryParse (parameters [0], out enumIndex)) {
            Debug.Log ("データが不正です");
            return;
        }

        var transitionEnum = (ApplicationTransitionEnum)enumIndex;
        transitionEnum.Transition (parameters);
    }

    void DidTapCloseWebView()
    {
        Close ();
    }

    void Awake()
    {
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }

    private WebViewObject m_WebViewObject;
    private Action<string> m_StartedCallback;
}
