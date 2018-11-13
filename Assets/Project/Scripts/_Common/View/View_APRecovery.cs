using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;
using SmileLab;
using SmileLab.Net.API;

public class View_APRecovery : PopupViewBase {
    public static View_APRecovery Create()
    {
        int level = MasterDataTable.user_level.GetLevel(AwsModule.UserData.UserData.Exp);
        if (AwsModule.UserData.UserData.ActionPoint >= MasterDataTable.user_level.GetMaxAp (level)) {
            PopupManager.OpenPopupSystemOK ("APは最大です。");
            return null;
        }
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_APRecovery");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_APRecovery>();
        c.InitInternal();
        return c;
    }

    private void InitInternal()
    {
        var apRecoveryItems = MasterDataTable.consumer_item.DataList.Where (x => x.sub_type == "AP回復薬").ToArray();

        SetLimitTime ();

        // 回復アイテムの登録
        var GridObj = GetScript<RectTransform>("ItemGrid").gameObject;
        foreach (var item in apRecoveryItems) {
            var consumerData = ConsumerData.CacheGet (item.index);
            int count = consumerData != null ? consumerData.Count : 0;

            if (count > 0) {
                var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_PointRecoveryItem", GridObj);
                var behaviour = go.GetOrAddComponent<ListItem_PointRecoveryItem> ();
                behaviour.Init (UserPointTypeEnum.AP, item, count, DidHeal);
            }
        }

        // ジェム回復の登録
        {
            var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_PointRecoveryItem", GridObj);
            var behaviour = go.GetOrAddComponent<ListItem_PointRecoveryItem> ();
            behaviour.Init (UserPointTypeEnum.AP, 5, DidHeal);
        }

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    void SetLimitTime()
    {
        var fullRecoveryTime = Mathf.Max(0, AwsModule.UserData.ActionPointTimeToFull);
        var recoveryTime = MasterDataTable.userpoint_recovery_time[UserPointTypeEnum.AP].recovery_time;
        // 1回復までの時間表示
        var oneRecoveryTime = Mathf.Max(0, fullRecoveryTime % recoveryTime);
        TimeSpan oneRecoveryTimeSpan = new TimeSpan(0, 0, oneRecoveryTime);
        GetScript<TextMeshProUGUI>("TimeCount/txtp_Time").SetTextFormat("{0:D2}:{1:D2}", oneRecoveryTimeSpan.Minutes, oneRecoveryTimeSpan.Seconds);


        // 全体回復までの時間表示
        TimeSpan fullRecoveryTimeSpan = new TimeSpan(0, 0, fullRecoveryTime);
        if (fullRecoveryTimeSpan.Hours > 0) {
            GetScript<TextMeshProUGUI> ("TimeCountAll/txtp_Time").SetTextFormat ("{0:D2}:{1:D2}", fullRecoveryTimeSpan.Hours, fullRecoveryTimeSpan.Minutes);
        } else {
            GetScript<TextMeshProUGUI> ("TimeCountAll/txtp_Time").SetTextFormat ("{0:D2}:{1:D2}", fullRecoveryTimeSpan.Minutes, fullRecoveryTimeSpan.Seconds);
        }
    }

    void DidHeal(object userData)
    {
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

    void FixedUpdate()
    {
        SetLimitTime ();
    }
}
