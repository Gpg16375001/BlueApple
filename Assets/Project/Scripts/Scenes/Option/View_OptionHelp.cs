using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmileLab;


/// <summary>
/// View : オプション画面 ヘルプ.
/// </summary>
public class View_OptionHelp : View_OptionListTypeBase
{
    // ボタンタップ.
    protected override void DidTapMenu(OptionMenu menu)
    {
        Debug.Log("DidTap Menu : menu=" + menu.Enum + "/" + menu.name);
        base.DidTapMenu(menu);      
        switch (menu.Enum){
			case OptionMenuEnum.Help:
                CreateHelpView();
                break;
            case OptionMenuEnum.Glossary:
                CreateGlossaryView();
                break;
			case OptionMenuEnum.Inquiry:
                CreateInquiryView();
                break;
        }
    }

	// ヘルプ > ヘルプ View生成.
    private void CreateHelpView()
    {
		this.SwitchViewMode(ViewMode.Dictionary);
		var list = MasterDataTable.help_info.DataList.FindAll(i => i.subject == OptionMenuEnum.Help);
		var c = this.gameObject.AddComponent<MiniView_OptionHelp>();      
		c.Init(list);
    }
    // ヘルプ > 用語集 View生成.
    private void CreateGlossaryView()
    {
		this.SwitchViewMode(ViewMode.Dictionary);
		var list = MasterDataTable.help_info.DataList.FindAll(i => i.subject == OptionMenuEnum.Glossary);
        var c = this.gameObject.AddComponent<MiniView_OptionDictionary>();
        c.Init(list);
    }

    // ヘルプ > お問い合わせ View生成.
    private void CreateInquiryView()
    {
        this.SwitchViewMode(ViewMode.Contact);
        var c = this.gameObject.AddComponent<MiniView_OptionContact>();
        c.Init ();
    }
}
