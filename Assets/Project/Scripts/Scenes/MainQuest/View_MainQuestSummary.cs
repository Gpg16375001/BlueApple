using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using TMPro;


/// <summary>
/// View : メインクエストのあらすじポップアップ.
/// </summary>
public class View_MainQuestSummary : PopupViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
	public static View_MainQuestSummary Create(Action didCose)
	{
		var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_MainQuestSummary");
		var c = go.GetOrAddComponent<View_MainQuestSummary>();
		c.InitInternal(didCose);
		return c;
	}
    private void InitInternal(Action didClose)
	{
		m_didClose = didClose;
        
		var quest = AwsModule.ProgressData.GetPlayableLatestMainQuest(AwsModule.ProgressData.PrevSelectedQuest.Country);
		this.GetScript<TextMeshProUGUI>("txtp_Summary").text = quest.summary;

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("Close/bt_CommonS02", DidTapClose);

        SetBackButton ();
	}

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

	// ボタン : 閉じる
    void DidTapClose()
	{
        this.PlayOpenCloseAnimation(false, () => {
            if (m_didClose != null){
                m_didClose();
            }
            base.Dispose();
        });
	}

	private Action m_didClose;
}
