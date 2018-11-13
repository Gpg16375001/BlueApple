using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;
using UniRx.Triggers;
using UniRx;


/// <summary>
/// View : キャラ進化演出.
/// </summary>
public class View_UnitEvolutionMovie : ViewBase
{   
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_UnitEvolutionMovie Create(CardData before, CardData after, Action didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_UnitEvolutionMovie");
		var c = go.GetOrAddComponent<View_UnitEvolutionMovie>();
		c.InitInternal(before, after, didClose);
		return c;
	}
	private void InitInternal(CardData before, CardData after, Action didClose)
	{
		View_PlayerMenu.IsEnableButtons = false;
		View_GlobalMenu.IsEnableButtons = false;
		
		m_didClose = didClose;

		// モーション管理システム初期化.
		m_motionModule = this.gameObject.GetOrAddComponent<UnitEvolutionMovieMotionModule>();
		m_motionModule.Init(() => {
            View_PlayerMenu.IsEnableButtons = true;
            View_GlobalMenu.IsEnableButtons = true;
			if(m_didClose != null){
				m_didClose();
			}
			this.Dispose();
		});
        m_motionModule.RegistStateChangeStream();

		// ユニットカードの表示
		var unitRoot = this.GetScript<RectTransform>("FirstCard/UnitCardAnchor");
        var unitCard = GameObjectEx.LoadAndCreateObject("_Common/View/UnitCard", unitRoot.gameObject);
		unitCard.GetComponent<UnitCard>().Init(before, null);
		unitRoot = this.GetScript<RectTransform>("ResultCard/UnitCardAnchor");
        unitCard = GameObjectEx.LoadAndCreateObject("_Common/View/UnitCard", unitRoot.gameObject);
		unitCard.GetComponent<UnitCard>().Init(after, null);
		// 音
		this.PlaySE(after);

		// レアリティ.
		for (var i = 1; i <= 6; ++i){
			this.GetScript<RectTransform>("Rarity_SRT/Star"+i.ToString()).gameObject.SetActive(before.Rarity >= i);
		}
		for (var i = 1; i <= 4; ++i){
			var rarity = i+after.Rarity;
			this.GetScript<RectTransform>("Rarity_SRT/StarEmpty"+i.ToString()).gameObject.SetActive(rarity <= after.MaxRarity);
		}

		// ボタン.
        var trigger = this.gameObject.GetOrAddComponent<ObservableEventTrigger>();
		trigger.OnPointerDownAsObservable().Subscribe(pointer => DidTap());
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	private void PlaySE(CardData card)
	{
		this.StartCoroutine(this.PlayAndWaidEndSE(card));
	}
	IEnumerator PlayAndWaidEndSE(CardData card)
	{
		SoundManager.SharedInstance.PlaySE(SoundClipName.se009);
		do {
			yield return null;
		} while (SoundManager.SharedInstance.IsPlaySE);
		if(!string.IsNullOrEmpty(card.Card.voice_sheet_name)){
			this.PlayVoice(card);
		}      
	}
	private void PlayVoice(CardData card)
	{
		var cue = SoundVoiceCueEnum.evolution1;      
		if(card.MinRarity <= 2){
			// 星2の特別なカード(=主人公など)以外のモブの星2カードの場合.
			if(card.MaxRarity < 6){
				if (card.RarityGrade >= 2) {
                    cue = SoundVoiceCueEnum.evolution2;
                }
			}
            // 星2の特別なカードの場合.
			else{
				if (card.RarityGrade >= 3) {
                    cue = SoundVoiceCueEnum.evolution3;
				}else if(card.RarityGrade >= 2){
					cue = SoundVoiceCueEnum.evolution2;
				}
			}     
		}
		else if(card.MinRarity <= 3){
			// 星3の特別なカード以外のモブの星3カードの場合.
			if(card.MaxRarity < 6){
				if (card.RarityGrade >= 2) {
                    cue = SoundVoiceCueEnum.evolution2;
                }
			}
            // 星3の特別なカードの場合.
			else{
				if (card.RarityGrade >= 3) {
                    cue = SoundVoiceCueEnum.evolution3;
				}else if(card.RarityGrade >= 2){
					cue = SoundVoiceCueEnum.evolution2;
				}
			}
        }
        // そのほかの一般的な進化段階が3未満のカード.
		else if(card.MaxRarity - card.MinRarity < 3){
			if (card.RarityGrade >= 2) {
                cue = SoundVoiceCueEnum.evolution2;
            }
		}
        Debug.Log("Evolution play voice. cue=" + cue + "/voice_sheet_name=" + card.Card.voice_sheet_name);
		SoundManager.SharedInstance.PlayVoice(card.Card.voice_sheet_name, cue);         
	}

	// ボタン : タップ.
    void DidTap()
	{
		m_motionModule.Animator.SetBool("Touch", true);      
	}

	void Awake()
	{
		var canvas = this.gameObject.GetOrAddComponent<Canvas>();
		if (canvas != null) {
			canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
		}
	}

	private Action m_didClose;
	private UnitEvolutionMovieMotionModule m_motionModule;
}
