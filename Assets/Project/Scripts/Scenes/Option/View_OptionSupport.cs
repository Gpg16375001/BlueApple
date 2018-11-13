using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// View : オプション画面 サポート.
/// </summary>
public class View_OptionSupport : View_OptionListTypeBase
{
    /// メニュー表示.
    protected override void ChangeView(OptionMenu menu)
    {
        base.ChangeView(menu);
        CreateSupportView();
    }

    // サポート > サポートリスト View生成.
    private void CreateSupportView()
    {
        this.SwitchViewMode(ViewMode.Friend);
        var c = this.gameObject.AddComponent<MiniView_OptionSupport>();
        c.Init(m_currentMenu);
        m_currentMiniView = c;
    }
}
