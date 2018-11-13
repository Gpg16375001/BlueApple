using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;


/// <summary>
/// MiniView : サウンド設定.
/// </summary>
public class MiniView_OptionSoundSetting : OptionMiniViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {
        m_sliderBGM = this.GetScript<Slider>("SliderBGM/img_SliderBase");
        m_sliderSE = this.GetScript<Slider>("SliderSE/img_SliderBase");
        m_sliderVoice = this.GetScript<Slider>("SliderVoice/img_SliderBase");

        // 現在の設定に合わせてスライダー初期化.
        m_sliderBGM.value = SoundManager.SharedInstance.VolumeBGM;
        m_sliderSE.value = SoundManager.SharedInstance.VolumeSE;
        m_sliderVoice.value = SoundManager.SharedInstance.VolumeVoice;

        // ラベル初期化.
        var bgm = Mathf.FloorToInt(m_sliderBGM.value * 10f) == 0 && m_sliderBGM.value > 0 ? 1 : Mathf.FloorToInt(m_sliderBGM.value * 10f);
        var se = Mathf.FloorToInt(m_sliderSE.value * 10f) == 0 && m_sliderSE.value > 0 ? 1 : Mathf.FloorToInt(m_sliderSE.value * 10f);
        var voice = Mathf.FloorToInt(m_sliderVoice.value * 10f) == 0 && m_sliderVoice.value > 0 ? 1 : Mathf.FloorToInt(m_sliderVoice.value * 10f);
        this.GetScript<TextMeshProUGUI>("txtp_ValueBGM").text = bgm.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_ValueSE").text = se.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_ValueVoice").text = voice.ToString();

        // スライダーの値変化イベントを登録.
        m_sliderBGM.onValueChanged.AddListener(DidChangeValueBGM);
        m_sliderSE.onValueChanged.AddListener(DidChangeValueSE);
        m_sliderVoice.onValueChanged.AddListener(DidChangeValueVoice);

        // ボタン.
		this.SetCanvasCustomButtonMsg("ResetDefault/bt_CommonS", DidTapReset);
    }

    #region CallbackEvents.

    // スライダー : BGM音量変化.
    void DidChangeValueBGM(float val)
    {
        var viewVal = Mathf.FloorToInt(val * 10f) == 0 && val > 0 ? 1 : Mathf.FloorToInt(val * 10f);
        this.GetScript<TextMeshProUGUI>("txtp_ValueBGM").text = viewVal.ToString();
        SoundManager.SharedInstance.VolumeBGM = AwsModule.LocalData.Volume_BGM = val;
    }

    // スライダー : SE音量変化.
    void DidChangeValueSE(float val)
    {
        var viewVal = Mathf.FloorToInt(val * 10f) == 0 && val > 0 ? 1 : Mathf.FloorToInt(val * 10f);
        this.GetScript<TextMeshProUGUI>("txtp_ValueSE").text = viewVal.ToString();
        SoundManager.SharedInstance.VolumeSE = AwsModule.LocalData.Volume_SE = val;
    }

    // スライダー : Voice音量変化.
    void DidChangeValueVoice(float val)
    {
        var viewVal = Mathf.FloorToInt(val * 10f) == 0 && val > 0 ? 1 : Mathf.FloorToInt(val * 10f);
        this.GetScript<TextMeshProUGUI>("txtp_ValueVoice").text = viewVal.ToString();
        SoundManager.SharedInstance.VolumeVoice = AwsModule.LocalData.Volume_Voice = val;
    }

    // ボタン : 初期設定に戻す.
    void DidTapReset()
    {
        m_sliderBGM.value = SoundManager.SharedInstance.VolumeBGM = 1f;
        m_sliderSE.value = SoundManager.SharedInstance.VolumeSE = 1f;
        m_sliderVoice.value = SoundManager.SharedInstance.VolumeVoice = 1f;
        this.GetScript<TextMeshProUGUI>("txtp_ValueBGM").text = Mathf.FloorToInt(m_sliderBGM.value * 10f).ToString();
        this.GetScript<TextMeshProUGUI>("txtp_ValueSE").text = Mathf.FloorToInt(m_sliderSE.value * 10f).ToString();
        this.GetScript<TextMeshProUGUI>("txtp_ValueVoice").text = Mathf.FloorToInt(m_sliderVoice.value * 10f).ToString();
    }

    #endregion

    // デタッチ直前イベント.
    protected override void WillDetachProc(Action didProcEnd)
    {
		if(Mathf.Approximately(AwsModule.LocalData.Volume_BGM, SoundManager.SharedInstance.VolumeBGM) && Mathf.Approximately(AwsModule.LocalData.Volume_SE, SoundManager.SharedInstance.VolumeSE) && Mathf.Approximately(AwsModule.LocalData.Volume_Voice, SoundManager.SharedInstance.VolumeVoice)){
			if (didProcEnd != null) {
                didProcEnd();
            }
			return;
		}
		
        View_FadePanel.SharedInstance.IsLightLoading = true;
        AwsModule.LocalData.Volume_BGM = SoundManager.SharedInstance.VolumeBGM;
        AwsModule.LocalData.Volume_SE = SoundManager.SharedInstance.VolumeSE;
        AwsModule.LocalData.Volume_Voice = SoundManager.SharedInstance.VolumeVoice;
        AwsModule.LocalData.Sync((bSuccess, sender, eArgs) => { 
            if(didProcEnd != null){
                didProcEnd();
            }
            View_FadePanel.SharedInstance.IsLightLoading = false;
        });
    }

    public override void Dispose()
    {
        AwsModule.LocalData.Sync((bSuccess, sender, eArgs) => {
            base.Dispose();
        });
    }

    private Slider m_sliderBGM;
    private Slider m_sliderSE;
    private Slider m_sliderVoice;
}
