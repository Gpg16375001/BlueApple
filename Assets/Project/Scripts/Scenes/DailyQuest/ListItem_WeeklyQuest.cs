using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_WeeklyQuest : ViewBase {
    public void Init(DailyQuestAchievement questAchievement, SpriteAtlas headerSpriteAtlas, bool isOpen, float initSinceStartupTime)
    {
        m_QuestAchievement = questAchievement;
        m_InitSinceStartupTime = initSinceStartupTime;

        var quest = questAchievement.Info;
        GetScript<Image> ("img_WeeklyQuestHeader").overrideSprite = 
            headerSpriteAtlas.GetSprite(quest.day_of_week.ToString());
        GetScript<TextMeshProUGUI> ("txtp_APNum").SetText (quest.cost_ap);
        GetScript<TextMeshProUGUI> ("txtp_QuestTitle").SetText (quest.quest_name);

        bool isFree = questAchievement.LockStatus == 0;
        m_IsUnlock = questAchievement.LockStatus == 2;

        GetScript<RectTransform> ("bt_WeeklyQuest").gameObject.SetActive(isFree && isOpen);
        GetScript<RectTransform> ("Disable").gameObject.SetActive(!isOpen);
        GetScript<RectTransform> ("Unlock").gameObject.SetActive(m_IsUnlock);
        if (m_IsUnlock) {
            m_TxtpTimeLimit = GetScript<TextMeshProUGUI> ("txtp_TimeLimit");
            TimeSpan sub = new TimeSpan(0, 0, questAchievement.TimeToLock);
            m_TxtpTimeLimit.SetTextFormat("{0:D2}:{1:D2}", sub.Minutes, sub.Seconds);
            m_InitSinceStartupTime = initSinceStartupTime;
        }

        SetCanvasCustomButtonMsg ("bt_WeeklyQuest", DidTapQuest);
        SetCanvasCustomButtonMsg ("bt_WeeklyQuestUnlock", DidTapQuest);
    }

    void DidTapQuest()
    {
        AwsModule.ProgressData.CurrentQuest = m_QuestAchievement.Info;
        AwsModule.ProgressData.CurrentQuestAchievedMissionIdList = m_QuestAchievement.AchievedMissionIdList;
        AwsModule.BattleData.SetStage(AwsModule.ProgressData.CurrentQuest.BattleStageID);
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToFriendSelect();
            }
        );
    }

    // 時間の表示更新をする
    void Update()
    {
        if (m_IsUnlock && m_TxtpTimeLimit != null) {
            var pastTime = Time.realtimeSinceStartup - m_InitSinceStartupTime;
            TimeSpan sub = new TimeSpan(0, 0, m_QuestAchievement.TimeToLock - (int)pastTime);
            if (sub.TotalSeconds <= 0) {
                m_TxtpTimeLimit.SetText("00:00");
                m_IsUnlock = false;
            } else {
                m_TxtpTimeLimit.SetTextFormat ("{0:D2}:{1:D2}", sub.Minutes, sub.Seconds);
            }
        }
    }

    bool m_IsUnlock;
    TextMeshProUGUI m_TxtpTimeLimit;
    DailyQuestAchievement m_QuestAchievement;
    float m_InitSinceStartupTime;
}
