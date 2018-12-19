using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_EventSelect : ViewBase {

    public void Init (int EventId, EventQuestStageTypeEnum? StageType, Screen_EventQuest root, int eventPoint, EventQuest questData, EventQuestAchievement[] questAchievement)
    {
		m_EventID = EventId;
        m_QuestAchievements = questAchievement;
        m_QuestData = questData;
        m_Root = root;

		// イベントポイント名の設定
		GetScript<TextMeshProUGUI> ("txtp_StockTitle").SetTextFormat("所持{0}", ItemTypeEnum.event_point.GetName(EventId));
		GetScript<Image> ("IconEventPoint").overrideSprite = null;
		IconLoader.LoadEventPoint(m_EventID, DidLoadIcon);

        GetScript<TextMeshProUGUI> ("txtp_GemNum").SetText(eventPoint);

        m_TabSettings = GetScript<RectTransform> ("Tab").GetComponentsInChildren<EventQuestTabSetting> (true);
        foreach (var tab in m_TabSettings) {
            tab.SetTabCallback (DidTapTab);
        }
        // 解放条件を満たしかつイベント
        m_StageDatas = MasterDataTable.event_quest_stage.DataList.Where(x => {
            var schedule = MasterDataTable.event_quest_schedule[x.schedule];
            if(schedule != null) {
                return schedule.event_quest_id == EventId && schedule.Enable();
            }
            return false;
        }).ToArray();

        ActiveTab (StageType.HasValue ? StageType.Value : EventQuestStageTypeEnum.Scenario, true);

        SetCanvasCustomButtonMsg ("HowToPlay/bt_Sub", DidTapHowToPlay);
        SetCanvasCustomButtonMsg ("Shop/bt_CommonS02", DidTapShop);
    }

    public bool BackProc()
    {
        return true;
    }

    public void SetEventPoint (int eventPoint)
    {
        GetScript<TextMeshProUGUI> ("txtp_GemNum").SetText(eventPoint);
    }

	public override void Dispose ()
	{
		IconLoader.RemoveLoadedEvent (ItemTypeEnum.event_point, m_EventID, DidLoadIcon);
		base.Dispose ();
	}

	private void DidLoadIcon(IconLoadSetting data, Sprite icon)
	{
		if (data.type == ItemTypeEnum.event_point && data.id == m_EventID) {
			GetScript<Image> ("IconEventPoint").overrideSprite = icon;
		}
	}

    private void ActiveTab(EventQuestStageTypeEnum type, bool forceCreate=false)
    {
        if (!forceCreate && m_ActiveType == type) {
            return;
        }
        System.Array.ForEach (m_TabSettings, x => x.IsHighlight = type == x.StageType);
        CreateQuestList (type);
        m_ActiveType = type;
    }

    private void CreateQuestList(EventQuestStageTypeEnum type)
    {
        ScrollRect sr = GetScript<ScrollRect> ("ScrollAreaEvent");

        // リストの削除とスクロール位置を一番上に戻す。
        sr.content.gameObject.DestroyChildren ();
        sr.verticalNormalizedPosition = 1.0f;

        if (m_StageDatas == null) {
            return;
        }

        var typeStageDatas = m_StageDatas.Where (x => x.stage_type == type).OrderByDescending(x => {
            var schedule = MasterDataTable.event_quest_schedule[x.schedule];
            return schedule.start_at;
        }).ThenByDescending(x => x.index);
        if (typeStageDatas.Count() <= 0) {
            return;
        }

        EventStageItems.Clear ();
        var rootObj = sr.content.gameObject;
        foreach (var stage in typeStageDatas) {
            CreateQuestItem (stage, rootObj);
        }
            
        if (EventStageItems.Count > 0) {
            var go = EventStageItems.FirstOrDefault(x => x.IsReleased);
            if (go != null) {
                go.Show ();
            }
            StartCoroutine (CalcLayout (go));
        }

        // スクロール位置の反映
        sr.SetLayoutVertical ();
    }

    IEnumerator CalcLayout(ListItem_EventStage target)
    {
        // レイアウトの反映をまつ
        yield return null;

        ScrollRect sr = GetScript<ScrollRect> ("ScrollAreaEvent");
        var go = target;
        if (go != null) {
            var rectTran = go.GetScript<RectTransform>("bt_Stage");
            var pos = sr.content.localPosition;
            pos.y = -1.0f * go.transform.localPosition.y;
            sr.content.localPosition = pos;
            // スクロール位置の反映
            sr.SetLayoutVertical ();
        }
    }

    private GameObject CreateQuestItem(EventQuestStage stage, GameObject root)
    {
        GameObject item = null;
        if (stage.stage_type == EventQuestStageTypeEnum.Scenario) {
            item = GameObjectEx.LoadAndCreateObject ("EventQuest/ListItem_EventStage", root);
            var beh = item.GetOrAddComponent<ListItem_EventStage> ();
            beh.Init (stage, DidTapQuestList, DidTapEventStage, m_QuestAchievements);
            EventStageItems.Add (beh);
        } else {
            item = GameObjectEx.LoadAndCreateObject ("EventQuest/ListItem_EventChallenge", root);
            item.GetOrAddComponent<ListItem_EventChallenge> ().Init (stage, DidTapQuestList, m_QuestAchievements);
        }
        return item;
    }

    void DidTapEventStage(ListItem_EventStage stageObject, bool isOpen)
    {
        ScrollRect sr = GetScript<ScrollRect> ("ScrollAreaEvent");

        if (isOpen) {
            EventStageItems.ForEach (x => {
                if (x != stageObject && x.IsOpen) {
                    x.Hide();
                }
            });
        }
        // スクロール位置の反映
        sr.SetLayoutVertical ();
    }

    void DidTapTab(EventQuestStageTypeEnum type)
    {
        ActiveTab (type);
    }

    void DidTapQuestList(EventQuestStageDetails stageDetails)
    {
        if (stageDetails == null) {
            return;
        }

        AwsModule.ProgressData.CurrentQuest = stageDetails;
        var achiemnet = m_QuestAchievements.Where(a => a.StageDetailId == stageDetails.ID).FirstOrDefault();
        if (achiemnet != null) {
            AwsModule.ProgressData.CurrentQuestAchievedMissionIdList = achiemnet.AchievedMissionIdList;
        }
        ScenarioProvider.CurrentSituation = ScenarioSituation.Event;
        ScenarioProvider.CurrentScenarioState = ScenarioProgressState.PrevBattle;
        AwsModule.BattleData.SetStage(AwsModule.ProgressData.CurrentQuest.BattleStageID);

        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToScenario();
        });
    }

    void DidTapHowToPlay()
    {
        if (!string.IsNullOrEmpty (m_QuestData.how_to_play_url)) {
            View_WebView.Open (m_QuestData.how_to_play_url);
        }
    }

    void DidTapShop()
    {
        m_Root.OpenShop ();
    }

    void DidTapEventMission()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToMission();
            }
        );
    }

    List<ListItem_EventStage> EventStageItems = new List<ListItem_EventStage> ();

	int m_EventID;
    Screen_EventQuest m_Root;
    EventQuestStage[] m_StageDatas;
    EventQuest m_QuestData;

    EventQuestStageTypeEnum m_ActiveType;
    EventQuestTabSetting[] m_TabSettings;
    EventQuestAchievement[] m_QuestAchievements;
}
