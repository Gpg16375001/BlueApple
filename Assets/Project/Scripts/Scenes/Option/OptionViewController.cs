using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// オプション画面のViewを操作するクラス.
/// </summary>
public class OptionViewController : ViewBase
{
    /// <summary>
    /// View切り替え.
    /// </summary>
    public IViewOption ChangeView(OptionBootMenu mode)
    {
        switch (mode.Enum){
			case OptionBootMenuEnum.GameSetting:
                return ChangeViewGameSetting(mode);
            case OptionBootMenuEnum.Support:
                return ChangeViewSupport(mode);
			case OptionBootMenuEnum.HaveGem:
				return ChangeHaveGem(mode);
			case OptionBootMenuEnum.History:
				return ChangeHistory(mode);
            case OptionBootMenuEnum.Help:
                return ChangeViewHelp(mode);
            case OptionBootMenuEnum.Other:
                return ChangeViewOther(mode);
        }
        return null;
    }

    #region Internal proc.
 
	// ゲーム設定画面に切り替え.
    private IViewOption ChangeViewGameSetting(OptionBootMenu mode)
    {
        var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionListType");
        var c = go.GetOrAddComponent<View_OptionGameSetting>();
        c.Init(mode);
        return c;
    }

    // フレンド画面に切り替え.
    private IViewOption ChangeViewSupport(OptionBootMenu mode)
    {
        var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionListType");
        var c = go.GetOrAddComponent<View_OptionSupport>();
        c.Init(mode);
        return c;
    }

	// ジェム所有数.
	private IViewOption ChangeHaveGem(OptionBootMenu mode)
	{
		var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionSingleType");
		var c = go.GetOrAddComponent<View_OptionHaveGem>();
		c.Init(mode);
		return c;
	}

	// 受け取り履歴.
	private IViewOption ChangeHistory(OptionBootMenu mode)
	{
		var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionSingleType");
		var c = go.GetOrAddComponent<View_OptionReceiptHistory>();
        c.Init(mode);
        return c;
	}

    // ヘルプ画面に切り替え.
    private IViewOption ChangeViewHelp(OptionBootMenu mode)
    {
        var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionListType");
        var c = go.GetOrAddComponent<View_OptionHelp>();
        c.Init(mode);
        return c;
    }

    // その他画面に切り替え.
    private IViewOption ChangeViewOther(OptionBootMenu mode)
    {
        var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionListType");
        var c = go.GetOrAddComponent<View_OptionOther>();
        c.Init(mode);
        return c;
    }

    #endregion
}
