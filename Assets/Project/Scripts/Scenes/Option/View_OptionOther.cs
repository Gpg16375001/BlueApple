using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : オプション画面 その他.
/// </summary>
public class View_OptionOther : View_OptionListTypeBase
{
    // メニュー表示.
    protected override void ChangeView(OptionMenu menu)
    {
        base.ChangeView(menu);
        switch (menu.Enum){
            case OptionMenuEnum.TermsOfService:
            case OptionMenuEnum.PrivacyPolicy:            
            case OptionMenuEnum.PaymentService:
            case OptionMenuEnum.SpecifiedCommercialTransactions:
			case OptionMenuEnum.License:
                CreateTextOnlyNoticeView();
                break;
            case OptionMenuEnum.BackTitle:
                {
                    var info = MasterDataTable.help_notice.DataList.Find(d => d.subject == OptionMenuEnum.BackTitle);
                    PopupManager.OpenPopupYN(info.text, () => ScreenChanger.SharedInstance.GoToTitle());
                }
                break;
        }
    }

    // テキストのみの注意書き文表示.
    private void CreateTextOnlyNoticeView()
    {
        this.SwitchViewMode(ViewMode.TextOnly);
        var info = MasterDataTable.help_notice.DataList.Find(d => d.subject == m_currentMenu.Enum);
        var c = this.gameObject.AddComponent<MiniView_OptionNotice>();
        c.Init(info.text);
        m_currentMiniView = c;
    }
}
