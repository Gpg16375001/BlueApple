using System.Text;
using UnityEngine;
using System.Collections;

namespace SmileLab
{
    /// <summary>
    /// メーラーを開く用クラス.
    /// </summary>
    public class OpenMailer
    {
        /// <summary>
        /// メーラーを起動する
        /// </summary>
        public static void Exec(string mailAdress, string title)
        {
            //本文は端末名、OS、アプリバージョン、言語
            var deviceName = SystemInfo.deviceModel;
#if UNITY_IOS && !UNITY_EDITOR
        deviceName = UnityEngine.iOS.Device.generation.ToString();
#endif

            // 本文
            var body = "※お問い合わせ内容をご記入の上、送信してください。\n\n";
            body += "お問い合わせ内容" + "\n";
            body += "\n\n\n\n\n\n\n\n\n\n\n";
            body += "\n\n※以下は変更せずにそのまま送信してください。\n\n";
            body += "お客様ID\t\t： " + AwsModule.UserData.CustomerID + "\n";
            body += "Device\t\t： " + deviceName + "\n";
            body += "OS\t\t\t： " + SystemInfo.operatingSystem + "\n";
            body += "Version\t\t： " + DefinePlayerSettings.BUNDLE_VERSION + "\n";

            //エスケープ処理
            mailAdress = System.Uri.EscapeDataString(mailAdress);
            body = System.Uri.EscapeDataString(body);
            title = System.Uri.EscapeDataString(title);

            Application.OpenURL("mailto:" + mailAdress + "?subject=" + title + "&body=" + body);
        }

    }
}
