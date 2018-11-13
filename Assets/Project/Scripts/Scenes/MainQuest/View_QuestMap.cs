﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : クエスト上のマップ.
/// </summary>
public class View_QuestMap : ViewBase
{
	/// <summary>
    /// 拡大表示しているか.
    /// </summary>
    public bool IsZoom { get; private set; }

	/// <summary>
    /// 詳細マップ表示しているか.
    /// </summary>
	public bool IsForcusDetail { get; private set; }
 
    /// <summary>
    /// 詳細マップ.詳細マップ表示中であれば.
    /// </summary>
	public View_QuestMap_Detail DetailMap { get; private set; }


	public override void Dispose()
	{
		if(this.DetailMap != null){
			this.DetailMap.Dispose();
		}      
		base.Dispose();
	}

	/// <summary>
	/// 初期化.
	/// </summary>
	public void Init()
	{
		this.GetScript<RectTransform>("ZoomMapRoot").gameObject.SetActive(false);
		this.GetScript<RectTransform>("QuestMap").gameObject.SetActive(false);
	}

    /// <summary>
    /// 表示リセット.
    /// </summary>
	public void ResetZoomAndForcus(Action didPlayEnd = null)
	{
		if (this.DetailMap != null) {
            this.DetailMap.Dispose();
        }
		this.StartCoroutine(this.CoPlayBackCountryAnimation(didPlayEnd));
	}
	IEnumerator CoPlayBackCountryAnimation(Action didPlayEnd = null)
	{
		IsForcusDetail = false;
        IsZoom = false;
		var anim = this.GetScript<Animation>("Map");
		anim.Play("QuestMapSelectBackCountry");
		LockInputManager.SharedInstance.IsLock = true;
		do {
            yield return null;
        } while (anim.isPlaying);
        if (didPlayEnd != null) {
            didPlayEnd();
        }
		LockInputManager.SharedInstance.IsLock = false;
	}

    /// <summary>
    /// 国詳細Viewを作成、表示切り替え演出を実施.
    /// </summary>
	public void DrawAndForcusDetailMap(MainQuest questData, bool bReleaseRoot = false, Action didPlayEnd = null)
	{
		// アニメーションを再生してから作る.
		this.StartCoroutine(this.CoPlayForcusMapAnimation(true, questData.Country, () => {

			// TODO : このプレハブは最終的にバンドルになる想定.
			var prefabName = "View_QuestMap_"+(int)questData.Country.Enum+"_"+questData.ChapterNum;
            var go = GameObjectEx.LoadAndCreateObject("MainQuest/" + prefabName, this.gameObject);
			this.DetailMap = go.GetOrAddComponent<View_QuestMap_Detail>();
			this.DetailMap.Init(questData, bReleaseRoot);

			if(didPlayEnd != null){
				didPlayEnd();
			}
		}));
	}
    /// <summary>
    /// 詳細マップフォーカスを解除..
    /// </summary>
	public void ReleaseDetailMap(Belonging belonging, Action didPlayEnd = null)
	{
		if(this.DetailMap != null){
			this.DetailMap.Dispose();
		}
		this.StartCoroutine(this.CoPlayForcusMapAnimation(false, belonging, didPlayEnd));
	}
	private IEnumerator CoPlayForcusMapAnimation(bool bForcus, Belonging belonging, Action didPlayEnd = null)
	{
		IsForcusDetail = bForcus;
		IsZoom |= IsForcusDetail;
		if (belonging != null && belonging.Enum != BelongingEnum.Unknown) {
			foreach (var bel in Enum.GetValues(typeof(BelongingEnum)) as BelongingEnum[]) {
                if (bel == BelongingEnum.Unknown) {
                    continue;
                }
				this.GetScript<RectTransform>("ZoomMap_"+(int)bel).gameObject.SetActive(bel == belonging.Enum);
            }
        }
		var anim = this.GetScript<Animation>("Map");
		var animName = IsForcusDetail ? "QuestMapSelectIn" : "QuestMapSelectOut";
		anim.Play(animName);
		LockInputManager.SharedInstance.IsLock = true;
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didPlayEnd != null) {
            didPlayEnd();
        }
		LockInputManager.SharedInstance.IsLock = false;
	}

    /// <summary>
    /// マップズームアニメーション.
    /// </summary>
	public void PlayZoomAnimation(bool bZoom, Belonging belonging = null, Action didPlayEnd = null)
	{
		this.StartCoroutine(this.CoPlayMapZoomAnimation(bZoom, belonging, didPlayEnd));
	}
	private IEnumerator CoPlayMapZoomAnimation(bool bZoom, Belonging belonging = null, Action didPlayEnd = null)
    {
        IsZoom = bZoom;
		IsForcusDetail &= IsZoom;
        if (belonging != null && belonging.Enum != BelongingEnum.Unknown) {
			foreach(var bel in Enum.GetValues(typeof(BelongingEnum)) as BelongingEnum[]){
				if(bel == BelongingEnum.Unknown){
					continue;
				}
				this.GetScript<RectTransform>("ZoomMap_"+(int)bel).gameObject.SetActive(bel == belonging.Enum);
			}
			m_currentZoomCountry = belonging;
        }
        var anim = this.GetScript<Animation>("Map");
        var animName = IsZoom ? "StorySelectMapZoomIn" : "StorySelectMapZoomOut";
		if (m_currentZoomCountry != null && m_currentZoomCountry.Enum != BelongingEnum.Unknown) {
			animName += ((int)m_currentZoomCountry.Enum).ToString();
		}
        anim.Play(animName);
		LockInputManager.SharedInstance.IsLock = true;
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didPlayEnd != null) {
            didPlayEnd();
        }
		LockInputManager.SharedInstance.IsLock = false;
    }
	private Belonging m_currentZoomCountry;
}