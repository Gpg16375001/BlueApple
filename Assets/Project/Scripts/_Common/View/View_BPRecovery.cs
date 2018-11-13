using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;
using SmileLab;
using SmileLab.Net.API;

public class View_BPRecovery : PopupViewBase {
    const int BP_MAX = 5;

    public static View_BPRecovery Create(Action<PvpUserData> didHeal)
    {
        if (AwsModule.UserData.BattlePoint >= BP_MAX) {
            PopupManager.OpenPopupSystemOK ("BPは最大です。");
            return null;
        }

        var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_BPRecovery");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_BPRecovery>();
        c.InitInternal(didHeal);
        return c;
    }

    private void InitInternal(Action<PvpUserData> didHeal)
    {
        m_DidHeal = didHeal;

        var bpRecoveryItems = MasterDataTable.consumer_item.DataList.Where (x => x.sub_type == "BP回復薬").ToArray();

        m_BattlePointTimeToFull = AwsModule.UserData.BattlePointTimeToFull;
        m_UpdateStartupTime = AwsModule.UserData.UpdateStartupFromTime;
        SetLimitTime ();

        // 回復アイテムの登録
        var GridObj = GetScript<RectTransform>("ItemGrid").gameObject;
        foreach (var item in bpRecoveryItems) {
            var consumerData = ConsumerData.CacheGet (item.index);
            int count = consumerData != null ? consumerData.Count : 0;

            if (count > 0) {
                var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_PointRecoveryItem", GridObj);
                var behaviour = go.GetOrAddComponent<ListItem_PointRecoveryItem> ();
                behaviour.Init (UserPointTypeEnum.BP, item, count, DidHeal);
            }
        }

        // ジェム回復の登録
        {
            var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_PointRecoveryItem", GridObj);
            var behaviour = go.GetOrAddComponent<ListItem_PointRecoveryItem> ();
            behaviour.Init (UserPointTypeEnum.BP, 5, DidHeal);
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
        var fullRecoveryTime = Mathf.Max(0, m_BattlePointTimeToFull - (int)(Time.realtimeSinceStartup - m_UpdateStartupTime));
        var recoveryTime = MasterDataTable.userpoint_recovery_time[UserPointTypeEnum.BP].recovery_time;
        var oneRecoveryTime = Mathf.Max(0, fullRecoveryTime % recoveryTime);

        if (fullRecoveryTime <= 0) {
            // オーバー回復できないのでAPI発行してBPだけ更新して閉じる
            PopupManager.OpenPopupSystemOK ("BP回復時間が過ぎましたので画面更新します。",
                () => {
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black,
                        () => {
                            ScreenChanger.SharedInstance.GoToPVP();
                        }
                    );
                }
            );
        }

        // 1回復までの時間表示
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
        if (IsClosed) {
            return;
        }
        if (m_DidHeal != null) {
            m_DidHeal (userData as PvpUserData);
        }
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

    float m_UpdateStartupTime;
    int m_BattlePointTimeToFull;
    Action<PvpUserData> m_DidHeal;
}