using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmileLab;


/// <summary>
/// View : オプション画面 ゲーム設定.
/// </summary>
public class View_OptionGameSetting : View_OptionListTypeBase
{   
    // メニュー表示.
    protected override void ChangeView(OptionMenu menu)
    {
        base.ChangeView(menu);
        switch (menu.Enum) {
            case OptionMenuEnum.SoundSetting:
                CreateSoundSettingView();
                break;
            case OptionMenuEnum.PushNotificate:
                CreatePushNotificationView();
                break;
            case OptionMenuEnum.Inherit:
                CreateInheritView();
                break;
        }
    }

    // ゲーム設定 > サウンド設定 View生成.
    private void CreateSoundSettingView()
    {
        this.SwitchViewMode(ViewMode.SoundSetting);
        var c = this.gameObject.AddComponent<MiniView_OptionSoundSetting>();
        c.Init();
        m_currentMiniView = c;
    }
    // ゲーム設定 > プッシュ通知 View生成.
    private void CreatePushNotificationView()
    {
        this.SwitchViewMode(ViewMode.PushSetting);
        var c = this.gameObject.AddComponent<MiniView_OptionPush>();
        c.Init();
        m_currentMiniView = c;
    }
    // ゲーム設定 > 引き継ぎ View生成.
    private void CreateInheritView()
    {
        this.SwitchViewMode(ViewMode.Inherit);
        var c = this.gameObject.AddComponent<MiniView_OptionFGIDLogin> ();
        c.Init ();
        m_currentMiniView = c;
    }
}
