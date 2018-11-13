using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;
using TMPro;
using Live2D.Cubism.Rendering;


/// <summary>
/// Screen : キャラシナリオクエスト.
/// </summary>
public class Screen_UnitQuest : ViewBase
{
	public override void Dispose()
	{
		View_GlobalMenu.DidSetEnableButton -= SetEnableButton;
        View_PlayerMenu.DidSetEnableButton -= SetEnableButton;
        View_PlayerMenu.DidTapBackButton -= DidTapBack;
		base.Dispose();
	}

	/// <summary>
	/// 初期化.
	/// </summary>
	public void Init(UnitQuest unitQuest, List<int> achiveQuestIdList, Action didTapBack)
	{
		m_info = unitQuest;
		m_didTapBack = didTapBack;
		m_achiveIdList = achiveQuestIdList;

		// グローバルメニューイベント.
		View_GlobalMenu.DidSetEnableButton += SetEnableButton;
		View_PlayerMenu.DidSetEnableButton += SetEnableButton;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

		this.CreateList();
		this.RequestUnitModel();

		// ラベル
		this.GetScript<TextMeshProUGUI>("txtp_UnitName").text = CardData.CacheGet(m_info.card_id).Card.nickname;
            
		View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
	}
    private void SetEnableButton(bool bActive)
	{
		this.IsEnableButton = bActive;
	}

    // リスト生成.
    private void CreateList()
	{
		var rootObj = this.GetScript<RectTransform>("QuestListRoot").gameObject;
		rootObj.DestroyChildren();
		var viewObj = GameObjectEx.LoadAndCreateObject("UnitQuest/View_UnitQuest_StageSelect", rootObj);
		viewObj.GetOrAddComponent<View_UnitQuest_StageSelect>().Init(m_info, m_achiveIdList);
	}

	// キャラモデルリクエスト.
    private void RequestUnitModel()
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        this.GetScript<RectTransform>("CharacterAnchor").gameObject.DestroyChildren();
		var loader = new UnitResourceLoader(m_info.card_id);
        loader.LoadResource(resouce => {
            var go = Instantiate(resouce.Live2DModel) as GameObject;
            go.transform.SetParent(this.GetScript<RectTransform>("CharacterAnchor"));
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
			var cubismRender = go.GetComponentsInChildren<CubismRenderController>()[0];
            if (cubismRender != null) {
                var rootCanvas = this.GetScript<Canvas>("CharacterAnchor");
                cubismRender.gameObject.SetLayerRecursively(rootCanvas.gameObject.layer);
                cubismRender.SortingLayer = rootCanvas.sortingLayerName;
                cubismRender.SortingOrder = rootCanvas.sortingOrder;
            }
            View_FadePanel.SharedInstance.IsLightLoading = false;
        });
    }

	#region ButtonDelegate.

	// ボタン:戻る.
    void DidTapBack()
	{
		if(m_didTapBack != null){
			m_didTapBack();
		}else{
			View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
		}      
	}

    #endregion

	void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	private UnitQuest m_info;
	private Action m_didTapBack;
	private List<int> m_achiveIdList;
}
