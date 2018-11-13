using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

/// <summary>
/// Screen : キャラシナリオクエストリスト.
/// </summary>
public class Screen_UnitQuestList : ViewBase
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
	public void Init(Action<UnitQuest> didTapQuest)
    {
		m_didTapQuest = didTapQuest;
		m_prefab = Resources.Load("UnitQuest/ListItem_UnitQuestList") as GameObject;

		// グローバルメニューイベント.
		View_GlobalMenu.DidSetEnableButton += SetEnableButton;
		View_PlayerMenu.DidSetEnableButton += SetEnableButton;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

		// リスト.
		this.CreateList();

        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
    }
    // ※イベント解除用.
    private void SetEnableButton(bool bActive)
	{
		this.IsEnableButton = bActive;
	}

    private void CreateList()
	{      
		m_list = MasterDataTable.quest_unit.DistinctFirstList;
		var grid = this.GetScript<InfiniteGridLayoutGroup>("ContentQuestSelect");
		grid.gameObject.DestroyChildren();
		grid.OnUpdateItemEvent.RemoveAllListeners();
        grid.OnUpdateItemEvent.AddListener(CallbackUpdateListItem);
		grid.Initialize(m_prefab, 5, m_list.Count, false);
    }
	void CallbackUpdateListItem(int index, GameObject go)
	{
		var item = go.GetComponent<ListItem_UnitQuestList>();
		if(item == null){
			item = go.AddComponent<ListItem_UnitQuestList>();
			item.Init(m_list[index], DidTapUnitQuest);
		}
	}

    #region ButtonDelegate.

    // ボタン:戻る.
    void DidTapBack()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
    }

	// ボタン:ユニットクエスト選択
	void DidTapUnitQuest(UnitQuest quest)
	{
		if(m_didTapQuest != null){
			m_didTapQuest(quest);
		}
	}

    #endregion

    void Awake()
    {
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }

	private GameObject m_prefab;
	private List<UnitQuest> m_list;
	private Action<UnitQuest> m_didTapQuest;
}
