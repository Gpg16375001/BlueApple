using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

/// <summary>
/// 曜日クエストの曜日ボタンコントロールクラス
/// </summary>
public class WeekSelectItemControll : ViewBase {
    public void Init(int questType, int dayOfWeek, bool isFree, bool isUnlock, int restTime, float initSinceStartupTime, Action TimeLimited)
    {
        m_TimeLimited = TimeLimited;
        m_QuestType = questType;
        m_DayOfWeek = dayOfWeek;
        m_IsLock = !isFree && !isUnlock;
        m_RestTime = null;
        if (isUnlock) {
            m_RestTime = restTime;
        }

        var freeObj = GetScript<RectTransform>("bt_WeekSelect").gameObject;
        var unlockObj = GetScript<RectTransform>("Unlock").gameObject;
        var disableObj = GetScript<RectTransform>("Disable").gameObject;

        freeObj.SetActive (isFree);
        unlockObj.SetActive (!isFree && isUnlock);
        disableObj.SetActive (!isFree && !isUnlock);

        if (isUnlock && m_RestTime.HasValue) {
            m_TxtpTimeLimit = GetScript<TextMeshProUGUI>("txtp_TimeLimit");
            TimeSpan sub = new TimeSpan(0, 0, m_RestTime.Value);
            m_TxtpTimeLimit.SetTextFormat("{0:D2}:{1:D2}", sub.Minutes, sub.Seconds);
            m_InitSinceStartupTime = initSinceStartupTime;
        }

        SetCanvasCustomButtonMsg("bt_WeekSelect", DidTapWeekSelect);
        SetCanvasCustomButtonMsg("bt_WeekSelectUnlock", DidTapWeekSelect);
        SetCanvasCustomButtonMsg("bt_WeekSelectDisable", DidTapWeekSelect);
    }

    void DidTapWeekSelect()
    {
        // 曜日クエストの取得物情報をだす、解放ボタンを非アクティブ
        View_WeeklyQuestDropItemPop.Create(m_QuestType, m_DayOfWeek, m_IsLock);
    }

    // 時間の表示更新をする
    void Update()
    {
        if (m_RestTime.HasValue && m_TxtpTimeLimit != null) {
            var pastTime = Time.realtimeSinceStartup - m_InitSinceStartupTime;
            TimeSpan sub = new TimeSpan(0, 0, m_RestTime.Value - (int)pastTime);
            if (sub.TotalSeconds <= 0) {
                m_TxtpTimeLimit.SetText("00:00");
                if (m_TimeLimited != null) {
                    m_TimeLimited ();
                }
                m_RestTime = null;
            } else {
                m_TxtpTimeLimit.SetTextFormat ("{0:D2}:{1:D2}", sub.Minutes, sub.Seconds);
            }
        }
    }

    TextMeshProUGUI m_TxtpTimeLimit;
    Action m_TimeLimited;
    int? m_RestTime;
    float m_InitSinceStartupTime;
    bool m_IsLock;
    int m_QuestType;
    int m_DayOfWeek;
}
