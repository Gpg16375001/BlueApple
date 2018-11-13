using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// ListItem : キャラクエストのステージ一覧.
/// </summary>
public class ListItem_UnitQuest_Stage : ViewBase
{
 
    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(int stageNum, List<IQuestData> questList, Action<int> didTapStage, Action<IQuestData> didTapQuest)
	{
		m_numStage = stageNum;
		m_questList = questList;
		m_didTapStage = didTapStage;
		m_didTapQuest = didTapQuest;

		// リスト準備.      
        this.GetComponent<MainQuestListInDropDown>().options.Clear();
		this.GetComponent<MainQuestListInDropDown>().AddOptions(m_questList);

		// コールバックイベント類.
        this.GetComponent<MainQuestListInDropDown>().RootButtonName = "bt_Stage";
        this.GetComponent<MainQuestListInDropDown>().onClickRoot = DidTapStage;
        this.GetComponent<MainQuestListInDropDown>().onValueChanged.AddListener(DidTapQuest);      
	}

	#region ButtonDelegate.

    // ステージをタップ
	void DidTapStage()
	{
		if (m_didTapStage != null) {
            m_didTapStage(m_numStage);
        }
	}

    // クエストをタップ.
	void DidTapQuest(int questIdx)
    {
        if (m_didTapQuest != null) {
            m_didTapQuest(m_questList[questIdx]);
        }
    }

	#endregion.

	private List<IQuestData> m_questList;
	private Action<IQuestData> m_didTapQuest;
	private Action<int> m_didTapStage;
	private int m_numStage;
}
