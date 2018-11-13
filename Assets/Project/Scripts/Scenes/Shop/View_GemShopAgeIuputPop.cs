using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class View_GemShopAgeIuputPop : PopupViewBase {
    private static View_GemShopAgeIuputPop instance;
    public static View_GemShopAgeIuputPop Create()
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Shop/View_GemShopAgeIuputPop");
        instance = go.GetOrAddComponent<View_GemShopAgeIuputPop>();
        instance.InitInternal();
        return instance;
    }

    private void InitInternal()
    {
        var yearPullDown = GetScript<TMP_Dropdown> ("Year/bt_PullDown");
        yearPullDown.onValueChanged.AddListener(YearChange);

        var monthPullDown = GetScript<TMP_Dropdown> ("Month/bt_PullDown");
        monthPullDown.onValueChanged.AddListener(MonthChange);

        // 1900~今年までのリストを作成して今年を初期値にする
        List<string> yearList = new List<string>();
        var currentYear = GameTime.SharedInstance.Now.Year;
        for (int y = 1900; y <= currentYear; ++y) {
            yearList.Add (y.ToString ());
        }
        yearPullDown.ClearOptions ();
        yearPullDown.AddOptions (yearList);
        yearPullDown.value = 90;
        yearPullDown.captionText.text = yearPullDown.options[90].text;
        YearChange (90);

        monthPullDown.value = 0;
        MonthChange (0);

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("Send/bt_CommonS02", DidTapSend);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    void YearChange(int value)
    {
        m_SetYear = 1900 + value;
    }

    void MonthChange(int value)
    {
        m_SetMonth = 1 + value;
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            this.Dispose();
        });
    }
    void DidTapSend()
    {
        if (IsClosed) {
            return;
        }

        // ローカルに保存する。
        AwsModule.UserData.SetAge(m_SetYear, m_SetMonth);
        if (AwsModule.UserData.GetAge () <= 0) {
            PopupManager.OpenPopupOK ("設定された年齢が不正です。設定を確認してください。");
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            if(AwsModule.UserData.GetAge() <= 15) {
                PopupManager.OpenPopupYN(TextData.GetText("GEM_AGE_CONFIRMATION"),
                    () => {
                        View_GemShop.Create();
                    },
                    () => {
                        AwsModule.UserData.DeleteAgeFile();
                    }
                );
            } else {
                View_GemShop.Create();
            }
            this.Dispose();
        });
    }


    private int m_SetYear;
    private int m_SetMonth;
}
