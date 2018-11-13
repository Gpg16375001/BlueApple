using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;
using TMPro;
using Live2D.Cubism.Rendering;


/// <summary>
/// View : キャラ強化結果画面.
/// </summary>
public class View_UnitEnhanceResult : ViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_UnitEnhanceResult Create(CardData current, CardData result, Action<CardData> didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_UnitEnhanceResult");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_UnitEnhanceResult>();
		c.InitInternal(current, result, didClose);
		return c;
	}
        
	private void InitInternal(CardData current, CardData result, Action<CardData> didClose)
	{
		m_currentData = current;
		m_resultData = result;
		m_didClose = didClose;

        // ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_UnitName").text = m_currentData.Card.nickname;      
  
		this.GetScript<TextMeshProUGUI>("BeforeUnitHP/txtp_HP").text = m_currentData.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeUnitATK/txtp_ATK").text = m_currentData.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeUnitDEF/txtp_DEF").text = m_currentData.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeUnitSPD/txtp_SPD").text = m_currentData.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeUnitLv/txtp_UnitLv").text = m_currentData.Level.ToString();

		this.GetScript<TextMeshProUGUI>("AfterUnitHP/txtp_HP").text = m_resultData.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("AfterUnitATK/txtp_ATK").text = m_resultData.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("AfterUnitDEF/txtp_DEF").text = m_resultData.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("AfterUnitSPD/txtp_SPD").text = m_resultData.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("AfterUnitLv/txtp_UnitLv").text = m_resultData.Level.ToString();

		// キャラクター表示にはLive2Dを使用.
		LockInputManager.SharedInstance.IsLock = true;
		loader = new UnitResourceLoader(m_currentData);
		loader.LoadResource(resouce => {
			var canvas = this.gameObject.GetOrAddComponent<Canvas>();
			var go = Instantiate(resouce.Live2DModel) as GameObject;         
			go.transform.SetParent(this.GetScript<RectTransform>("CharacterAnchor"));
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.GetComponent<CubismRenderController>().SortingOrder = canvas.sortingOrder;
			var sheet = current.Card.voice_sheet_name;
			Debug.Log(current.Card.voice_sheet_name);
			if(!string.IsNullOrEmpty(sheet)){
                voicePlayer = go.GetOrAddComponent<Live2dVoicePlayer>();            
				voicePlayer.Play(sheet, result.IsMaxLevel && result.IsMaxRarity ? SoundVoiceCueEnum.level_max: SoundVoiceCueEnum.levelup);
			}
			LockInputManager.SharedInstance.ForceUnlockInput();
		});

        if (string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButtonInfoId = AndroidBackButton.SetEventInThisScene (Dispose);
        }
	}
        
	/// <summary>
    /// 破棄メソッド.
    /// </summary>
    public override void Dispose()
    {
        if (voicePlayer != null) {
            voicePlayer.Stop ();
        }
        loader.Dispose ();
        if (!string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButton.DeleteEvent (AndroidBackButtonInfoId);
            AndroidBackButtonInfoId = null;
        }
        this.StartCoroutine(CoPlayClose());
    }
    IEnumerator CoPlayClose()
    {
		LockInputManager.SharedInstance.IsLock = true;
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		if(m_didClose != null){
			m_didClose(m_resultData);
		}
        base.Dispose();
		LockInputManager.SharedInstance.IsLock = false;
    }

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    UnitResourceLoader loader;
    Live2dVoicePlayer voicePlayer;
	private CardData m_currentData;
	private CardData m_resultData;
	private Action<CardData> m_didClose;

    private string AndroidBackButtonInfoId;
}
