using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SafariView
{
#if UNITY_IOS
	[DllImport ("__Internal")]
	extern static void LaunchUrl(string url);
    [DllImport ("__Internal")]
    extern static void SafariViewInit(string gameObjectName);
    [DllImport ("__Internal")]
    extern static void Dismiss();
#endif


	public static float GetiOSVersion()
	{
#if UNITY_IPHONE && !UNITY_EDITOR
		string versionString = SystemInfo.operatingSystem.Replace( "iPhone OS ", "" );
		versionString = versionString.Replace( "iOS ", "" );
		float version = -1f;
		int idx = versionString.IndexOf('.');

        if(float.TryParse(versionString.Substring(0, idx), out version)) {
		    return version;
        }
        return -1f;
#else
    	return -1f;
#endif
	}

    public static void Init(GameObject go)
    {
#if UNITY_IOS && !UNITY_EDITOR
        SafariViewInit(go.name);
#endif
    }

	public static void LaunchURL(string url)
	{
#if UNITY_IOS && !UNITY_EDITOR
		// SFSafariViewControllerはios9から
		if( SafariView.GetiOSVersion() >= 9 )
		{
			LaunchUrl(url);
		}
		else
		{
			Application.OpenURL(url);
		}
#endif
	}

    public static void Close()
    {
#if UNITY_IOS && !UNITY_EDITOR
        // SFSafariViewControllerはios9から
        if( SafariView.GetiOSVersion() >= 9 )
        {
            Dismiss();
        }
#endif
    }
}