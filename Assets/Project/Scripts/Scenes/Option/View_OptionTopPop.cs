using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// View : オプション画面起動時のポップ.
/// </summary>
public class View_OptionTopPop : PopupViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_OptionTopPop Create(Action didTapClose = null)
    {
        var go = GameObjectEx.LoadAndCreateObject("Option/View_OptionTopPop");
        var c = go.GetOrAddComponent<View_OptionTopPop>();
        c.InitInternal(didTapClose);
        return c;
    }
    // 内部初期化.
    private void InitInternal(Action didTapClose)
    {
        m_didTapClose = didTapClose;

        // ボタン設定.
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("GameSetting/bt_SubLarge", DidTapGameSetting);
		this.SetCanvasCustomButtonMsg("Friend/bt_SubLarge", DidTapSupport);      
		this.SetCanvasCustomButtonMsg("Gem/bt_SubLarge", DidTapHaveGem);
		this.SetCanvasCustomButtonMsg("History/bt_SubLarge", DidTapReceipyHistory);
		this.SetCanvasCustomButtonMsg("Help/bt_SubLarge", DidTapHelp);
		this.SetCanvasCustomButtonMsg("Other/bt_SubLarge", DidTapOther);
        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

    #region ButtonDelegate.

    // ボタン : 閉じる.
    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            this.Dispose ();
            if (m_didTapClose != null) {
                m_didTapClose ();
            }
        });
    }

	// ボタン : ゲーム設定.
    void DidTapGameSetting()
    {
        if (IsClosed) {
            return;
        }
        var data = MasterDataTable.option_boot_menu.DataList.Find(d => d.Enum == OptionBootMenuEnum.GameSetting);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToOption(data);
        });
    }

    // ボタン : サポート.
    void DidTapSupport()
    {
        if (IsClosed) {
            return;
        }
        var data = MasterDataTable.option_boot_menu.DataList.Find(d => d.Enum == OptionBootMenuEnum.Support);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToOption(data);
        });
    }
    
	// ボタン : ジェム所有数.
    void DidTapHaveGem()
    {
        if (IsClosed) {
            return;
        }
		var data = MasterDataTable.option_boot_menu.DataList.Find(d => d.Enum == OptionBootMenuEnum.HaveGem);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToOption(data);
        });
    }

	// ボタン : 受け取り履歴.
    void DidTapReceipyHistory()
    {
        if (IsClosed) {
            return;
        }
		var data = MasterDataTable.option_boot_menu.DataList.Find(d => d.Enum == OptionBootMenuEnum.History);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToOption(data);
        });
    }

    // ボタン : ヘルプ・お問い合わせ.
    void DidTapHelp()
    {
        if (IsClosed) {
            return;
        }
        var data = MasterDataTable.option_boot_menu.DataList.Find(d => d.Enum == OptionBootMenuEnum.Help);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToOption(data);
        });
    }

    // ボタン : その他.
    void DidTapOther()
    {
        if (IsClosed) {
            return;
        }
        var data = MasterDataTable.option_boot_menu.DataList.Find(d => d.Enum == OptionBootMenuEnum.Other);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToOption(data);
        });
    }
    
    #endregion

    private Action m_didTapClose;
}
