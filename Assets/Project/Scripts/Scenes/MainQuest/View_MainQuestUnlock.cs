using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : メインクエスト解放演出.開閉の概念がある.
/// </summary>
public class View_MainQuestUnlock : ViewBase
{
 
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_MainQuestUnlock Create(MainQuestReleaseCountryInfo releaseInfo)
	{
		if (instance != null) {
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_MainQuestUnlock");
		var cardMaster = MasterDataTable.card;
        instance = go.GetOrAddComponent<View_MainQuestUnlock>();
		instance.InitInternal(releaseInfo.country, new CardData(cardMaster[releaseInfo.card_id1]), new CardData(cardMaster[releaseInfo.card_id2]));
        return instance;
	}   
	private void InitInternal(Belonging belonging, CardData left, CardData right)
	{      
		// 国章
		var sptName = ((int)belonging.Enum).ToString()+"L";
		this.GetScript<uGUISprite>("EmblemIcon").ChangeSprite(sptName);
		this.GetScript<uGUISprite>("EmblemIcon_Light01").ChangeSprite(sptName);
		this.GetScript<uGUISprite>("EmblemIcon_Light02").ChangeSprite(sptName);
		this.GetScript<TextMeshProUGUI>("CountryName").text = belonging.name;     
		// キャラ立ち絵(Live2Dはダメ)
		var loader = new UnitResourceLoader(left.CardId);
		loader.IsLoadPortrait = true;
		loader.LoadResource(resouce => {
			var portrait = resouce.GetPortrait(left.Rarity);
			this.GetScript<Image>("CharacterAnchorL/Portrait").sprite = portrait;
			this.GetScript<Image>("CharacterAnchorL/Portrait").rectTransform.sizeDelta = new Vector2(portrait.rect.width, portrait.rect.height);
		});
		loader = new UnitResourceLoader(right.CardId);
		loader.IsLoadPortrait = true;
        loader.LoadResource(resouce => {
			var portrait = resouce.GetPortrait(right.Rarity);
			this.GetScript<Image>("CharacterAnchorR/Portrait").sprite = portrait;
			this.GetScript<Image>("CharacterAnchorR/Portrait").rectTransform.sizeDelta = new Vector2(portrait.rect.width, portrait.rect.height);
        });
        
		this.SetCanvasCustomButtonMsg("bt_Screen", DidTapScreen);
		this.GetScript<CustomButton>("bt_Screen").interactable = false;
	}

    /// <summary>
    /// 非同期削除.
    /// </summary>
    public static void DeleteAsync(Action didDelete)
	{
		instance.PlayInOutAnimation(false, () => {
			instance.Dispose();
			if(didDelete != null){
				didDelete();
			}
		});
	}

    /// <summary>
    /// 開閉アニメーション.
    /// </summary>
	public void PlayInOutAnimation(bool bIn, Action didEnd = null, Action didTapScreen = null)
	{
		m_didTapScreen = didTapScreen;
		this.StartCoroutine(this.CoPlayAnimation(bIn, didEnd));
	}
	IEnumerator CoPlayAnimation(bool bIn, Action didEnd)
	{
		LockInputManager.SharedInstance.IsLock = true;
		// 絵ロード待ち.
		if(bIn){
			if (this.GetScript<Image>("CharacterAnchorL/Portrait").sprite == null || this.GetScript<Image>("CharacterAnchorR/Portrait").sprite == null) {
                View_FadePanel.SharedInstance.IsLightLoading = true;
				while (this.GetScript<Image>("CharacterAnchorL/Portrait").sprite == null || this.GetScript<Image>("CharacterAnchorR/Portrait").sprite == null) {
                    yield return null;
                }
                View_FadePanel.SharedInstance.IsLightLoading = false;
            }
		}

		var anim = this.GetScript<Animation>("AnimParts");
		var animName = bIn ? "MainQuestUnlockIn" : "MainQuestUnlockOut";
		anim.Play(animName);
		do{
			yield return null;
		}while(anim.isPlaying);
		if(didEnd != null){
			didEnd();
		}
		if(bIn){
			this.GetScript<CustomButton>("bt_Screen").interactable = true;
		}
		LockInputManager.SharedInstance.IsLock = false;
	}

	// ボタン : 画面全体.
    void DidTapScreen()
	{
		if(m_didTapScreen != null){
			m_didTapScreen();
		}
	}

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private Action m_didTapScreen;
	private static View_MainQuestUnlock instance;
}
