using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

using SmileLab;

#if !UNITY_EDITOR && UNITY_IPHONE
using UnityEngine.iOS;
using CalendarUnit = UnityEngine.iOS.CalendarUnit;
using NotificationServices = UnityEngine.iOS.NotificationServices;
#endif

/// <summary>
/// ローカル通知のスケジューラ.
/// </summary>
public class LocalNotificationScheduler
{
    /// <summary>
    /// プッシュ通知の許可を求める
    /// </summary>
    public static void RegisterNotificationSettings()
    {
#if !UNITY_EDITOR && UNITY_IPHONE
        NotificationServices.RegisterForNotifications(
            NotificationType.Alert |
            NotificationType.Badge |
            NotificationType.Sound);
#endif
    }

#if !UNITY_EDITOR
#if UNITY_ANDROID
    private static AndroidJavaObject _androidObject;
    private static AndroidJavaObject AndroidObj
    {
        get{
            if( _androidObject == null ){
                _androidObject = new AndroidJavaObject("com.smilelab.localpush.LocalPushSender");
                _androidObject.Call("Init");
            }
            return _androidObject;
        }
    }
#elif UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void LocalNotificationPlugin_ClearBadge();
#endif
#endif

    /// <summary>
    /// スケジュールされた通知を削除
    /// </summary>
    public static void ClearLocalNotification()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        AndroidObj.Call("ClearAllNotification");
#elif UNITY_IPHONE
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
#endif
    }

    /// <summary>
    /// スケジュールされた通知を削除(既にステータスバーに通知された情報も削除)
    /// </summary>
    public static void CancelAllLocalNotifications()
    {
#if !UNITY_EDITOR
#if UNITY_ANDROID
        AndroidObj.Call("ClearAllNotification");
#elif UNITY_IPHONE
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
#endif
    }

    /// <summary>
    /// 通知を設定
    /// </summary>
    /// <param name="id">通知ID(Androidのみ。同IDは上書きされる)</param>
    /// <param name="tickerText">通知時のテキスト(Androidのみ)</param>
    /// <param name="contentTitle">通知バーに表示されるタイトル</param>
    /// <param name="contentText">通知バーに表示されるテキスト</param>
    /// <param name="delaySec">通知する時間(xx秒後)</param>
    /// <param name="interval">繰り返すスパン</param>
    public static void Schedule(int id, string tickerText, string contentTitle, string contentText, int delaySec, NotificationScheduleType interval, int badgeNumber = 0)
    {
        Debug.Log("[LocalNotificationScheduler] Schedule. fire time="+delaySec+" sec later.");
#if !UNITY_EDITOR
#if UNITY_ANDROID
        Debug.Log("Android interval sec="+interval.ToSecond());
        AndroidObj.Call("SetDelayPush", id, tickerText, contentTitle, contentText, delaySec, interval.ToSecond());
#elif UNITY_IPHONE
        var no = new UnityEngine.iOS.LocalNotification();
        no.alertAction = contentTitle;
        no.alertBody = contentText;
        no.fireDate =  GameTime.SharedInstance.Now.AddSeconds(delaySec);
        var repeat = interval.ToiOSCalendarUnit();
        if(repeat != CalendarUnit.Era){
            Debug.Log("iOS interval : repeat");
            no.repeatInterval = repeat;
        }
        no.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
        no.applicationIconBadgeNumber = badgeNumber;
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(no);
#endif
#endif
    }


    /// <summary>
    /// バッジ削除(iOSのみ)
    /// </summary>
    public static void ClearBadge()
    {
#if !UNITY_EDITOR && UNITY_IPHONE
        LocalNotificationPlugin_ClearBadge() ;
#endif
    }
}

/// <summary>
/// enum : ローカル通知スケジュールタイプ.
/// </summary>
public enum NotificationScheduleType
{
    None = -1,
    Second,
    Minute,
    Day,
    Month,
}
/// <summary>
/// NotificationScheduleType列挙体操作用拡張クラス.
/// </summary>
public static class NotificationScheduleTypeHelper
{
    /// <summary>秒(int)に変換.対応いているのはSecond〜Monthまでなので必要に応じて拡張.無効値は0を返す</summary>
    public static int ToSecond(this NotificationScheduleType type)
    {
        switch(type) {
            case NotificationScheduleType.Second:
                return 1;
            case NotificationScheduleType.Minute:
                return (int)TimeSpan.FromMinutes(1.0).TotalSeconds;
            case NotificationScheduleType.Day:
                return (int)TimeSpan.FromDays(1.0).TotalSeconds;
            case NotificationScheduleType.Month:
                var now = GameTime.SharedInstance.Now;
                var days = DateTime.DaysInMonth(now.Year, now.Month);
                return (int)TimeSpan.FromDays(days).TotalSeconds;
        }
        return 0;
    }
#if !UNITY_EDITOR && UNITY_IPHONE
    /// <summary>iOSのカレンダー情報に変換.</summary>
    public static CalendarUnit ToiOSCalendarUnit(this NotificationScheduleType type)
    {
        switch(type) {
            case NotificationScheduleType.Second:
                return CalendarUnit.Second;
            case NotificationScheduleType.Minute:
                return CalendarUnit.Minute;
            case NotificationScheduleType.Day:
                return CalendarUnit.Day;
            case NotificationScheduleType.Month:
                return CalendarUnit.Month;
        }
        return CalendarUnit.Era;
    }
#endif
}