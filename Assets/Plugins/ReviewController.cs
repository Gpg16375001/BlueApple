using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ReviewController {


#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _RequestReview();
#endif

#if UNITY_IOS
    static readonly string ReviewURL = "https://itunes.apple.com/app/id{0}?action=write-review";
#elif UNITY_ANDROID
    static readonly string ReviewURL = "market://details?id={0}";
#else
    static readonly string ReviewURL = string.Empty;
#endif

    public static float GetiOSVersion()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        string versionString = SystemInfo.operatingSystem.Replace( "iPhone OS ", "" );
        versionString = versionString.Replace( "iOS ", "" );
        float version = -1f;
        int idx = versionString.LastIndexOf('.');

        if(float.TryParse(versionString.Substring(0, idx), out version)) {
            return version;
        }
        return -1f;
#else
        return -1f;
#endif
    }

    public static bool OpenReviewiOS()
    {
#if !UNITY_EDITOR && UNITY_IOS
        if (ReviewController.GetiOSVersion () >= 10.3f) {
            _RequestReview();
            return true;
        }
#endif
        return false;
    }
    public static void OpenReview(string openID)
    {
#if !UNITY_EDITOR && UNITY_IOS

        if (ReviewController.GetiOSVersion () >= 10.3f) {
            _RequestReview();
            return;
        }
#endif
        Application.OpenURL(string.Format(ReviewURL, openID));
    }
}
