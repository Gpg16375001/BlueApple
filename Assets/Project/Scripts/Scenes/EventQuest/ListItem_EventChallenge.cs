using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_EventChallenge : ViewBase {

    public void Init(EventQuestStage stage, Action<EventQuestStageDetails> tapAction, EventQuestAchievement[] questAchievements)
    {
        m_StageDetailsData = MasterDataTable.event_quest_stage_details.DataList.FirstOrDefault (x => x.stage_id == stage.id);

        if (m_StageDetailsData == null) {
            Dispose ();
            return;
        }
        m_TapAction = tapAction;

        GetScript<TextMeshProUGUI> ("txtp_APNum").SetText (m_StageDetailsData.cost_ap);
        GetScript<TextMeshProUGUI> ("Lock/txtp_QuestTitle").SetText (stage.name);
        GetScript<TextMeshProUGUI> ("Unlock/txtp_QuestTitle").SetText (stage.name);
        if (m_StageDetailsData.recommended_level > 0) {
            GetScript<TextMeshProUGUI> ("txtp_RecommendLv").SetText (m_StageDetailsData.recommended_level);
        } else {
            GetScript<TextMeshProUGUI> ("txtp_RecommendLv").SetText ("-");
        }


        var isUnlock = !m_StageDetailsData.release_condition.HasValue || (questAchievements.Any(x => x.StageDetailId == m_StageDetailsData.id && x.IsOpen));
            
        SetCanvasCustomButtonMsg ("bt_EventChallenge", DidTap);
        SetCanvasCustomButtonMsg ("bt_EventChallengeLock", DidLockTap);
        // Lock状態の設定
        SetLock(!isUnlock);
    }

    void SetLock(bool isLock)
    {
        GetScript<RectTransform> ("Lock").gameObject.SetActive(isLock);
        GetScript<RectTransform> ("Unlock").gameObject.SetActive(!isLock);
    }

    void DidTap()
    {
        if (m_TapAction != null) {
            m_TapAction (m_StageDetailsData);
        }
    }

    void DidLockTap()
    {
        if (m_StageDetailsData.release_condition.HasValue) {
            var detals = MasterDataTable.event_quest_stage_details [m_StageDetailsData.release_condition.Value];
            PopupManager.OpenPopupOK (string.Format ("{0}の\nクエスト{1}をクリアで開放", detals.EventQuestStageData.name, detals.QuestNum));
        }
    }

    Action<EventQuestStageDetails> m_TapAction;
    EventQuestStageDetails m_StageDetailsData;
}
