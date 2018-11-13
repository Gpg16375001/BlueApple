using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using System;


/// <summary>
/// ScreenController : キャラシナリオ
/// </summary>
public class UnitQuestSController : ScreenControllerBase
{
	public override void Init(Action<bool> didConnectEnd)
	{
		SendAPI.QuestsGetAchievement((bSuccess, res) => {
			if(!bSuccess || res == null){
				didConnectEnd(false);
				return;
			}
			res.QuestAchievementList.CacheSet();
			foreach (var info in res.QuestAchievementList) {
				if (!info.IsAchieved) {
					continue;
				}
				// 3 = キャラ
                if (info.QuestType == 3) {
					m_achivedQuestIdList.Add(info.QuestId);
                }
			}         
			didConnectEnd(true);
		});
		base.Init(didConnectEnd);
	}

	public override void CreateBootScreen()
	{
		this.CreateQuestList();
	}
 
    // クエストリスト画面作成.
    private void CreateQuestList()
	{
		if(m_currentView != null){
			m_currentView.Dispose();
		}
		var go = GameObjectEx.LoadAndCreateObject("UnitQuest/Screen_UnitQuestList", this.gameObject);
		var c = go.GetOrAddComponent<Screen_UnitQuestList>();
		c.Init(quest => {
			// クエスト選択.
			View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
				this.CreateStageSelectView(quest);
				View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
			});
		});
		m_currentView = c;
	}
    // ユニットクエスト内のステージ選択画面生成.
	private void CreateStageSelectView(UnitQuest unitQuest)
	{
		if (m_currentView != null) {
            m_currentView.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("UnitQuest/Screen_UnitQuest", this.gameObject);
		var c = go.GetOrAddComponent<Screen_UnitQuest>();
		var questList = MasterDataTable.quest_unit.DataList.FindAll(q => q.card_id == unitQuest.card_id);
		var achiveList = m_achivedQuestIdList.Where(id => questList.Exists(q => q.id == id)).ToList();
		c.Init(unitQuest, achiveList, () => {
			// 戻る.
			View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
				this.CreateQuestList();
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
            });
		});
        m_currentView = c;
	}

	private ViewBase m_currentView;
	private List<int> m_achivedQuestIdList = new List<int>();
}
