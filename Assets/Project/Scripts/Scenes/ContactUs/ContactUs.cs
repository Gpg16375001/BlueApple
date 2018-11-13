using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public static class ContactUs {

    [Serializable]
    public class UserIdToken {
        public string sub;
    }

    public static string CreateUrl()
    {
        if (AwsModule.UserData.UserData != null) {
            var userID = AwsModule.UserData.UserData.UserId;
            var userIdClass = new UserIdToken ();
            userIdClass.sub = userID.ToString ();
            var userIdJson = JsonUtility.ToJson (userIdClass);

            var userIdBase64 = Convert.ToBase64String (Encoding.UTF8.GetBytes (userIdJson));

            var head = Guid.NewGuid ();
            var headBase64 = Convert.ToBase64String (Encoding.UTF8.GetBytes (head.ToString ()));

            var foot = Guid.NewGuid ();
            var footBase64 = Convert.ToBase64String (Encoding.UTF8.GetBytes (foot.ToString ()));

            var idToken = string.Format ("{0}.{1}.{2}", headBase64, userIdBase64, footBase64);

            var userName = AwsModule.UserData.UserData.Nickname;


            var osVersion = SystemInfo.operatingSystem;


            string os = null;
            string device = null;
            #if !UNITY_EDITOR && UNITY_IPHONE
            os = "iOS";
            device = UnityEngine.iOS.Device.generation.ToString();
            #else
            #if !UNITY_EDITOR && UNITY_ANDROID
            os = "Android";
            #else
            os = SystemInfo.operatingSystem;
            #endif
            device = SystemInfo.deviceModel;
            #endif

            var appVersion = Application.version;

            var customerId = AwsModule.UserData.UserData.CustomerId;

            return string.Format ("https://precatus.com/contact/index.html?user_name={0}&os={1}&os_version={2}&device={3}&id_token={4}&user_id={5}&app_version={6}&platform_type={7}",
                Uri.EscapeUriString (userName),
                Uri.EscapeUriString (os),
                Uri.EscapeUriString (osVersion),
                Uri.EscapeUriString (device),
                Uri.EscapeUriString (idToken),
                Uri.EscapeUriString (customerId),
                Uri.EscapeUriString (appVersion),
                SmileLab.GameSystem.GetPlatformIndex()
            );
        } 
        {
            var userID = AwsModule.UserData.UserID;
            var userIdClass = new UserIdToken ();
            userIdClass.sub = userID.ToString ();
            var userIdJson = JsonUtility.ToJson (userIdClass);

            var userIdBase64 = Convert.ToBase64String (Encoding.UTF8.GetBytes (userIdJson));

            var head = Guid.NewGuid ();
            var headBase64 = Convert.ToBase64String (Encoding.UTF8.GetBytes (head.ToString ()));

            var foot = Guid.NewGuid ();
            var footBase64 = Convert.ToBase64String (Encoding.UTF8.GetBytes (foot.ToString ()));

            var idToken = string.Format ("{0}.{1}.{2}", headBase64, userIdBase64, footBase64);

            Debug.Log (Encoding.UTF8.GetString (Convert.FromBase64String (userIdBase64)));
            var osVersion = SystemInfo.operatingSystem;
            var userName = AwsModule.UserData.UserName;

            string os = null;
            string device = null;
            #if !UNITY_EDITOR && UNITY_IPHONE
            os = "iOS";
            device = UnityEngine.iOS.Device.generation.ToString();
            #else
            #if !UNITY_EDITOR && UNITY_ANDROID
            os = "Android";
            #else
            os = SystemInfo.operatingSystem;
            #endif
            device = SystemInfo.deviceModel;
            #endif

            var appVersion = Application.version;

            var customerId = AwsModule.UserData.CustomerID;
            if (userName != null) {
                return string.Format ("https://precatus.com/contact/index.html?user_name={0}&os={1}&os_version={2}&device={3}&id_token={4}&user_id={5}&app_version={6}&platform_type={7}",
                    Uri.EscapeUriString (userName),
                    Uri.EscapeUriString (os),
                    Uri.EscapeUriString (osVersion),
                    Uri.EscapeUriString (device),
                    Uri.EscapeUriString (idToken),
                    Uri.EscapeUriString (customerId),
                    Uri.EscapeUriString (appVersion),
                    SmileLab.GameSystem.GetPlatformIndex()
                );
            } else {
                return string.Format ("https://precatus.com/contact/index.html?os={0}&os_version={1}&device={2}&id_token={3}&user_id={4}&app_version={5}&platform_type={6}",
                    Uri.EscapeUriString (os),
                    Uri.EscapeUriString (osVersion),
                    Uri.EscapeUriString (device),
                    Uri.EscapeUriString (idToken),
                    Uri.EscapeUriString (customerId),
                    Uri.EscapeUriString (appVersion),
                    SmileLab.GameSystem.GetPlatformIndex()
                );
            }
        }
    }
}
