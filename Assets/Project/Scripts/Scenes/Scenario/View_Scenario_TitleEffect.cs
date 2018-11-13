using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;


/// <summary>
/// View : シナリオ再生前のタイトル演出.
/// </summary>
public class View_Scenario_TitleEffect : ViewBase
{
    /// <summary>
    /// アニメーション再生中？
    /// </summary>
    public bool IsPlayingAnim { get { return m_anim.isPlaying; } }


    /// <summary>
    /// 引数のシナリオ情報に応じたタイトルエフェクトViewを生成..
    /// </summary>
    public static View_Scenario_TitleEffect Create(ScenarioSetting info, Action didEnd)
    {
        switch(info.eff_type){
            case ScenarioEffectTypeEnum.ChapterStart:
                return CreateUsedChapter(info, didEnd);
            case ScenarioEffectTypeEnum.ActStart:
                return CreateUsedAct(info, didEnd);
        }
        Debug.LogError("[View_Scenario_TitleEffect] Create Error!! : Unknown type. type="+info.eff_type);
        return null;
    }

    // 章演出で使いたいやつを生成.
    private static View_Scenario_TitleEffect CreateUsedChapter(ScenarioSetting info, Action didEnd)
    {
		var quest = MasterDataTable.quest_main[info.id];
		var go = GameObjectEx.LoadAndCreateObject(string.Format("Scenario/View_Scenario_ChapterTitle{0}", ((int)quest.Country.Enum).ToString("d2")));
        var c = go.GetOrAddComponent<View_Scenario_TitleEffect>();
        c.InitInternalChapter(info, didEnd);
        return c;
    }
    private void InitInternalChapter(ScenarioSetting info, Action didEnd)
    {
        m_anim = this.GetScript<Animation>("AnimParts");

        // ラベル類.
		this.GetScript<TextMeshProUGUI>("txtp_PickupCountryName").text = info.eff_country;
		this.GetScript<TextMeshProUGUI>("txtp_CountrySummary").text = info.eff_summary;
        
		m_anim.gameObject.SetActive(false);
		var quest = MasterDataTable.quest_main[info.id];
		this.DownloadBG(quest.Country.Enum, sprite => {
			if(sprite != null){
				this.GetScript<Image>("BG").sprite = sprite;
				foreach(var image in this.GetScript<RectTransform>("PartsRoot").GetComponentsInChildren<Image>(true)){
					image.sprite = sprite;
				}
				foreach (var mask in this.GetScript<RectTransform>("PartsRoot").GetComponentsInChildren<SpriteMask>(true)) {
					mask.sprite = sprite;
                }
			}
			this.StartCoroutine(WaitPlayingAnim(didEnd));
		});
    }
	private void DownloadBG(BelongingEnum belongingEnum, Action<Sprite> didLoad)
	{
		var bundleName = string.Format("scenariobg_eff_chaptertitle{0}", ((int)belongingEnum).ToString("d2"));
		DLCManager.AssetBundleFromFileOrDownload(DLCManager.DLC_FOLDER.ScenarioBG, bundleName, string.Format("eff_ChapterTitle{0}", ((int)belongingEnum).ToString("d2")), didLoad, (ex) => {
			didLoad(null);
            Debug.LogError(ex.Message);
        });
	}

	// TODO : ステージ演出で使いたいやつを生成.
    private static View_Scenario_TitleEffect CreateUsedAct(ScenarioSetting info, Action didEnd)
    {
        var go = GameObjectEx.LoadAndCreateObject("Scenario/View_Scenario_ActTitle");
        var c = go.GetOrAddComponent<View_Scenario_TitleEffect>();
        c.InitInternalAct(info, didEnd);
        return c;
    }
    private void InitInternalAct(ScenarioSetting info, Action didEnd)
    {
        m_anim = this.GetScript<Animation>("Contents");

        // ラベル類.
		this.GetScript<TextMeshProUGUI>("txtp_PickupCountryName").text = info.eff_country;
        this.GetScript<TextMeshProUGUI>("txtp_StageSummary").text = info.eff_summary;

        this.StartCoroutine(WaitPlayingAnim(didEnd));
    }

    // アニメーション再生、待機.
    IEnumerator WaitPlayingAnim(Action didEnd)
    {
        if(m_anim == null){
            didEnd();
            yield break;
        }

        LockInputManager.SharedInstance.IsLock = true;
  
        yield return null;  // 念のため1フレ待つ.
        m_anim.gameObject.SetActive(true);
        while(m_anim.isPlaying){
            yield return null;
        }
        didEnd();

        LockInputManager.SharedInstance.IsLock = false;
    }

    void Awake()
    {
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.FadeCamera);
    }

    private Animation m_anim;
}
