using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// MiniView : 設定のプッシュ通知.
/// </summary>
public class MiniView_OptionPush : OptionMiniViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {
		m_bDefaultPushAP = AwsModule.LocalData.IsNotificateAP;
		m_bDefaultPushBP = AwsModule.LocalData.IsNotificateBP;
		
        this.InitSettingButtons();
    }

    // 各ボタン初期化.
    private void InitSettingButtons()
    {
        // APが全快した時の通知.
        var tgl = this.GetScript<TwoToggleButtonGroup>("Switch_01");
		tgl.Init(bOn: AwsModule.LocalData.IsNotificateAP, bPairToggle: true);
        tgl.DidTapButtonEvent += DidTapPushAP;
        // イベントBPが全快した時の通知.
        tgl = this.GetScript<TwoToggleButtonGroup>("Switch_02");
		tgl.Init(bOn: AwsModule.LocalData.IsNotificateBP, bPairToggle: true);
        tgl.DidTapButtonEvent += DidTapPushBP;
    }

    #region ButtonDelegate.

    // TODO : 01ボタン押下.要名前変更.
	void DidTapPushAP(bool bOn)
    {
		Debug.Log("DidTapPushAP : "+bOn);
		AwsModule.LocalData.IsNotificateAP = bOn;
    }
    // TODO : 02ボタン押下.要名前変更.
	void DidTapPushBP(bool bOn)
    {
		Debug.Log("DidTapPushBP : "+bOn);
		AwsModule.LocalData.IsNotificateBP = bOn;
    }

	#endregion

	// デタッチ直前イベント.
	protected override void WillDetachProc(Action didProcEnd)
	{ 
		if(m_bDefaultPushAP == AwsModule.LocalData.IsNotificateAP && m_bDefaultPushBP == AwsModule.LocalData.IsNotificateBP){
			if(didProcEnd != null){
				didProcEnd();
			}
			return;
		}
		
		View_FadePanel.SharedInstance.IsLightLoading = true;
		AwsModule.LocalData.Sync((bSuccess, sender, eArgs) => {
            if (didProcEnd != null) {
                didProcEnd();
            }
            View_FadePanel.SharedInstance.IsLightLoading = false;
        });
	}

	private bool m_bDefaultPushAP;
	private bool m_bDefaultPushBP;
}
