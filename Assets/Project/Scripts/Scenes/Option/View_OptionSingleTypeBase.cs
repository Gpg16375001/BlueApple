using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : 設定画面一枚絵タイプ基底View.
/// </summary>
public class View_OptionSingleTypeBase : ViewBase, IViewOption
{

	/// <summary>
	/// 初期化.
	/// </summary>
	public virtual void Init(OptionBootMenu boot)
	{
		m_boot = boot;
        
		// グローバルメニュー関連の設定.
		View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
		View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
		View_PlayerMenu.DidTapBackButton += DidTapBack;
	}

	/// View切り替えの処理.
    protected virtual void ChangeView(OptionMenu menu)
    {
        if (menu.BootMenuType == OptionBootMenuEnum.HaveGem) {
            SwitchViewMode(ViewMode.HaveGem);
        } else if (menu.BootMenuType == OptionBootMenuEnum.History) {
            SwitchViewMode(ViewMode.History);
        }
    }

	/// Viewモード切り替え 
	protected void SwitchViewMode(ViewMode mode)
	{
		this.GetScript<RectTransform>("HaveGem").gameObject.SetActive(mode == ViewMode.HaveGem);
		this.GetScript<RectTransform>("History").gameObject.SetActive(mode == ViewMode.History);
	}

	#region ButtonDelegate.

	// ボタン : 戻る.
	void DidTapBack()
	{
		// オプションメニューを開いた状態で戻る.現状マイページからしから設定にいけないのでこれで.
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
			if (m_currentMiniView != null) {
				m_currentMiniView.AsyncDetach(() => {
					ScreenChanger.SharedInstance.GoToMyPage(() => View_OptionTopPop.Create());
				});
				return;
			}
			ScreenChanger.SharedInstance.GoToMyPage(() => View_OptionTopPop.Create());
		});
	}

    #endregion

	void Awake()
    {
        // 独立したUIなのでCanvas設定.
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }

	private OptionBootMenu m_boot;
    protected OptionMiniViewBase m_currentMiniView;

    // enum : 表示モード
    protected enum ViewMode
    {
        HaveGem,
        History,
    }
}
