using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_EventStage : ViewBase {

    public bool IsOpen {
        get {
            return m_IsOpen;
        }
    }

    public bool IsReleased{
        get {
            return m_Released;
        }
    }
    public void Init(EventQuestStage stage, Action<EventQuestStageDetails> tapAction, Action<ListItem_EventStage, bool> didTapStage, EventQuestAchievement[] questAchievements)
    {
        m_StageDetails = MasterDataTable.event_quest_stage_details.DataList.Where (x => x.stage_id == stage.id).ToArray();

        m_QuestAchievements = new EventQuestAchievement[m_StageDetails.Length];
        m_ReleaseStageDetails = new List<EventQuestStageDetails>();
        for (int count = m_StageDetails.Length - 1; count >= 0; count--) 
        {
            var detail = m_StageDetails [count];
            var achievement = questAchievements.FirstOrDefault (x => x.StageDetailId == detail.id);
            if (achievement != null) {
                m_QuestAchievements [count] = achievement;
                detail.ReleaseMissions = achievement.AchievedMissionIdList;
                detail.IsClear = achievement.IsAchieved;
                if (achievement.IsOpen) {
                    m_ReleaseStageDetails.Add(detail);
                }
            }
        }


        // リスト準備. 
        StageDropListItem = this.GetComponent<MainQuestListInDropDown>();

        GetScript<TextMeshProUGUI> ("txtp_Stage").SetText (stage.name);
        GetScript<TextMeshProUGUI> ("txtp_StageNum").SetText (stage.index);
        GetScript<TextMeshProUGUI> ("txtp_StageNumComplete").SetText (stage.index);

        ChangeCompleteBatch ();
        if (m_ReleaseStageDetails.Count <= 0) {
            ChangeButtonLockSprite ();

            StageDropListItem.RootButtonName = "bt_Stage";
            StageDropListItem.onClickRoot = DidTapStageClose;
            StageDropListItem.IsEnableToggle = false;

            m_Released = false;
            return;
        }

        m_Released = true;
        m_TapAction = tapAction;
        m_DidTapStage = didTapStage;

        StageDropListItem.options.Clear();
        StageDropListItem.AddOptions(m_ReleaseStageDetails.OrderByDescending(x => x.stage_index).ToArray(), null);      
        // コールバックイベント類.
        StageDropListItem.RootButtonName = "bt_Stage";
        StageDropListItem.onClickRoot = DidTapStage;
        StageDropListItem.onValueChanged.AddListener(ValueChange);

        var isNew = m_QuestAchievements != null && m_QuestAchievements.All (x => x != null && !x.IsAchieved);
        var isClear = m_QuestAchievements != null && m_QuestAchievements.All (x => x != null && x.IsAchieved);
        m_Complete = m_QuestAchievements != null && m_QuestAchievements.All (x => x != null && x.ReceivedMissionRewardCount >= 3);

        ChangeButtonSprite (false);
        GetScript<RectTransform> ("Icon/New").gameObject.SetActive (isNew);
        GetScript<RectTransform> ("Icon/Clear").gameObject.SetActive (isClear);
        GetScript<RectTransform> ("Icon/Complete").gameObject.SetActive (m_Complete);

        GetScript<TextMeshProUGUI> ("txtp_StageNumComplete").gameObject.SetActive (m_Complete);
        GetScript<TextMeshProUGUI> ("txtp_StageNum").gameObject.SetActive (!m_Complete);
    }

    public void Show()
    {
        if (!m_IsOpen) {
            ChangeButtonSprite (!m_IsOpen);
            StageDropListItem.Show ();
        }
    }
    public void Hide()
    {
        if (m_IsOpen) {
            ChangeButtonSprite (!m_IsOpen);
            StageDropListItem.HideTouchRoot ();
        }
    }

    void ChangeCompleteBatch()
    {
        // 表示
        var grid = GetScript<HorizontalLayoutGroup> ("StageMission/MissionGrid");

        // 一旦全部落とす
        foreach (var spriteChange in grid.GetComponentsInChildren<uGUISprite>(true)) {
            spriteChange.gameObject.SetActive (false);
        }

        // バッチの表示
        for (int batchCount = 1; batchCount <= m_StageDetails.Length; ++batchCount) {
            var sprite = GetScript<uGUISprite> (string.Format("MissionGrid/img_StageMissionClearIcon_{0}", batchCount));
            sprite.gameObject.SetActive (true);
            bool isComplite = m_QuestAchievements != null && m_QuestAchievements[batchCount - 1] != null && m_QuestAchievements[batchCount - 1].ReceivedMissionRewardCount >= 3;
            if(isComplite) {
                sprite.ChangeSprite ("img_StageMissionClearIconOn");
            } else {
                sprite.ChangeSprite ("img_StageMissionClearIconOff");
            }
        }
        grid.SetLayoutHorizontal ();
    }

    void ChangeButtonSprite(bool open)
    {
        m_IsOpen = open;
        var uGuiSprite = GetScript<uGUISprite> ("bt_Stage");
        if (uGuiSprite != null) {
            if (open) {
                if (m_Complete) {
                    uGuiSprite.ChangeSprite ("bt_EventStageOpenComplete");
                } else {
                    uGuiSprite.ChangeSprite ("bt_EventStageOpen");
                }
            } else {
                if (m_Complete) {
                    uGuiSprite.ChangeSprite ("bt_EventStageCloseComplete");
                } else {
                    uGuiSprite.ChangeSprite ("bt_EventStageClose");
                }
            }
        }
    }

    void ChangeButtonLockSprite()
    {
        var uGuiSprite = GetScript<uGUISprite> ("bt_Stage");
        if (uGuiSprite != null) {
            uGuiSprite.ChangeSprite ("bt_EventStageClose_disable");
        }
    }

    // ボタン : ステージ選択.
    void DidTapStage()
    {
        ChangeButtonSprite (!m_IsOpen);
        if (m_DidTapStage != null) {
            m_DidTapStage (this, m_IsOpen);
        }
    }

    void DidTapStageClose()
    {
        // 開かない
    }

    void ValueChange(int index)
    {
        if (m_IsOpen) {
            if (m_TapAction != null) {
                m_TapAction (m_ReleaseStageDetails [index]);
            }
        }
    }

    Action<EventQuestStageDetails> m_TapAction;
    Action<ListItem_EventStage, bool> m_DidTapStage;

    bool m_Complete;
    bool m_IsOpen;
    bool m_Released;
    MainQuestListInDropDown StageDropListItem;
    EventQuestStageDetails[] m_StageDetails;
    List<EventQuestStageDetails> m_ReleaseStageDetails;
    EventQuestAchievement[] m_QuestAchievements;
}
