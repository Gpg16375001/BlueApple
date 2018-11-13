using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// View : 国解放ポップ.
/// </summary>
public class View_CountryOpenPop : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
	public static void Create(Belonging belonging, Action didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_CountryOpenPop");
		var c = go.GetOrAddComponent<View_CountryOpenPop>();
		c.InitInternal(belonging, didClose);
	}
	private void InitInternal(Belonging belonging, Action didClose)
	{
		m_didTapClose = didClose;

		this.GetScript<TextMeshProUGUI>("txtp_Country").text = belonging.name;

		// ボタン.
		this.SetCanvasCustomButtonMsg("Close/bt_Common", DidTapClose);
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("CountrySelect/bt_Common", DidTapGoToCountrySelect);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
	}

    protected override void DidBackButton ()
    {
        DidTapClose();
    }
	#region ButtonDelegate.

    // 閉じる.
    void DidTapClose()
	{
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            if(m_didTapClose != null){
                m_didTapClose();
            }
            Dispose();
        });
	}

    // 国選択へ.
    void DidTapGoToCountrySelect()
	{
		if (IsClosed) {
            return;
        }
		var current = AwsModule.ProgressData.CurrentQuest;
		AwsModule.ProgressData.IsForceIgnoreOnceCheckMainQuestSummary = true;
		AwsModule.ProgressData.PrevSelectedQuest = AwsModule.ProgressData.CurrentQuest = null;
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
			View_PlayerMenu.CreateIfMissing().IsEnableButton = true;
			View_GlobalMenu.CreateIfMissing().IsEnableButton = true;         
			ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Country, true, false, () => {
				AwsModule.ProgressData.PrevSelectedQuest = AwsModule.ProgressData.CurrentQuest = current;
			});         
		});
	}

    #endregion


	private Action m_didTapClose;
}
