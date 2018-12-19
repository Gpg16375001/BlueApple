using UnityEngine;
using System;
using System.Collections;
using SmileLab;

/// <summary>
/// ローカル通知管理クラス.
/// </summary>
public class LocalNotificationManager : MonoBehaviour 
{
    public static LocalNotificationManager  SharedInstance  { get ; private set ; }

    void Awake()
    {
        if( SharedInstance != null ){
            GameObject.Destroy(this.gameObject);
            return;
        }
        
        SharedInstance  = this;

        // OnApplicationPauseが呼ばれないこともあるのでAwakeタイミングでもやる.
        LocalNotificationScheduler.CancelAllLocalNotifications();
        LocalNotificationScheduler.ClearLocalNotification();
        LocalNotificationScheduler.ClearBadge();
    }

    void Start()
    {
        // プッシュ通知許可.
        LocalNotificationScheduler.RegisterNotificationSettings();
    }

    void OnApplicationPause(bool bPause)
    {
        if (AwsModule.LocalData == null) {
            return;
        }
		// TODO : 通知設定確認.
		if(!AwsModule.LocalData.IsNotificateAP && !AwsModule.LocalData.IsNotificateBP){
            return;
        }
        
        LocalNotificationScheduler.CancelAllLocalNotifications();
        LocalNotificationScheduler.ClearLocalNotification();
        LocalNotificationScheduler.ClearBadge();

        // TODO : 通知.
        if( bPause ){
			LocalNotificationSchedule();
        }
    }

	void LocalNotificationSchedule()
	{
        if (AwsModule.LocalData.IsNotificateAP && AwsModule.UserData.ActionPointTimeToFull > 0) {
            LocalNotificationScheduler.Schedule (0, "", "AP回復", "APが全回復しました。物語を読み進めましょう。", AwsModule.UserData.ActionPointTimeToFull, NotificationScheduleType.None, 1);
        }
        if (AwsModule.LocalData.IsNotificateBP && AwsModule.UserData.BattlePointTimeToFull > 0) {
            LocalNotificationScheduler.Schedule (1, "", "BP回復", "BPが全回復しました。演習に向かいましょう。", AwsModule.UserData.BattlePointTimeToFull, NotificationScheduleType.None, 1);
        }
	}
}
