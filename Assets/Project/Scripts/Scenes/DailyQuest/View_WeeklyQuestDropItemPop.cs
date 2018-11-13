using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_WeeklyQuestDropItemPop : PopupViewBase {

    private static View_WeeklyQuestDropItemPop instance;
    public static View_WeeklyQuestDropItemPop Create(int questType, int dayOfWeek, bool isLock)
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("DailyQuest/View_WeeklyQuestDropItemPop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        instance = go.GetOrAddComponent<View_WeeklyQuestDropItemPop>();
        instance.InitInternal(questType, dayOfWeek, isLock);
        return instance;
    }

    private void InitInternal(int questType, int dayOfWeek, bool isLock)
    {
        m_QuestType = questType;
        m_DayOfWeek = dayOfWeek;

        GetScript<TextMeshProUGUI> ("txtp_WeekTitle").SetText(MasterDataTable.day_of_week[dayOfWeek].name);

        var gridObj = GetScript<RectTransform> ("ItemGrid").gameObject;
        gridObj.DestroyChildren ();
        foreach(var item in MasterDataTable.quest_daily_reward_info[dayOfWeek, questType])
        {
            var go = GameObjectEx.LoadAndCreateObject ("DailyQuest/ListItem_EventQuestDropItem", gridObj);
            go.GetOrAddComponent<ListItem_EventQuestDropItem> ().Init (item);
        }

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("Unlock/bt_CommonS02", DidTapUnlock);
        GetScript<CustomButton> ("Unlock/bt_CommonS02").interactable = isLock;
        SetBackButton ();

        if (m_QuestType == 4) {
            GetScript<TextMeshProUGUI> ("txtp_MaterialCategory").SetText ("強化素材");
        } else {
            GetScript<TextMeshProUGUI> ("txtp_MaterialCategory").SetText ("進化素材");
        }

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    void DidTapUnlock()
    {
        PlayOpenCloseAnimation (false, () => {
            // 確認ポップアップを開く
            var item = ConsumerData.CacheGet (50001);
            if(item != null && item.Count > 0) {
                View_WeeklyQuestUnlockOkPop.Create(m_QuestType, m_DayOfWeek);
            } else {
                PopupManager.OpenPopupSystemOK("解放アイテムを所持していません。");
            }
            Dispose();
        });
    }

    void DidTapClose()
    {
        PlayOpenCloseAnimation (false, () => {
            Dispose();
        });
    }

    int m_QuestType;
    int m_DayOfWeek;
}
