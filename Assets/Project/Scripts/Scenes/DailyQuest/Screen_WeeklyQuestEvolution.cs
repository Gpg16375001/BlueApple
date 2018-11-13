using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

using SmileLab;
using SmileLab.Net.API;
public class Screen_WeeklyQuestEvolution : ViewBase {
    private const int QuestType = 5;
    public void Init(DailyQuestAchievement[] dailyQuestAchievementList)
    {
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        m_InitRealtimeSinceStartup = Time.realtimeSinceStartup;
        m_DailyQuestAchievementList = dailyQuestAchievementList.Where (x => x.QuestType == QuestType).ToArray ();
        m_DailyQuestWeeklyHeaderSpriteAtlas = Resources.Load ("Atlases/DailyQuestWeeklyHeader") as SpriteAtlas;

        // 曜日ボタンの設定
        SetWeeklySelect();

        // 解放されているクエストのリストを出す
        SetWeeklyQuestList();

        SetCanvasCustomButtonMsg ("GoEnhance/bt_CommonS01", DidTapGoEnhance);

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    void SetWeeklySelect()
    {
        var weeklyElements = GetScript<RectTransform>("WeekSelect").GetComponentsInChildren<WeeklyElement>(true);
        foreach(var element in weeklyElements)
        {
            var isFree = m_DailyQuestAchievementList.Any (x => x.DayOfWeek == element.WeekIndex && x.LockStatus == 0);
            var isUnlock = m_DailyQuestAchievementList.Any (x => x.DayOfWeek == element.WeekIndex && x.LockStatus == 2);

            int restTime = 0;
            if (isUnlock) {
                restTime = m_DailyQuestAchievementList.Where (x => x.DayOfWeek == element.WeekIndex).Max (x => x.TimeToLock);
            }
            var behaviour = element.gameObject.GetOrAddComponent<WeekSelectItemControll> ();
            behaviour.Init (QuestType, element.WeekIndex, isFree, isUnlock, restTime, m_InitRealtimeSinceStartup, TimeLimited);
        }
    }
    void TimeLimited()
    {
        if (m_IsTimeLimitedOpend) {
            return;
        }

        m_IsTimeLimitedOpend = true;
        PopupManager.OpenPopupSystemOK ("制限時間を迎えたクエストがあります。画面の更新を行います。",
            () => {
                LockInputManager.SharedInstance.IsLock = true;
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    ScreenChanger.SharedInstance.GoToDailyQuest(2);
                    LockInputManager.SharedInstance.IsLock = false;
                });
            }
        );
    }

    void SetWeeklyQuestList()
    {
        var scrollArea = GetScript<ScrollRect> ("ScrollAreaQuest");
        var rootObj = scrollArea.content.gameObject;

        rootObj.DestroyChildren ();

        var layourGruop = rootObj.GetComponent<InfiniteGridLayoutGroup>();
        var prot = Resources.Load ("DailyQuest/ListItem_WeeklyQuest") as GameObject;

        //　解放クエストを取得する。
        // 難易度で且Unlockしたクエストが優先表示されるように並び替え
        m_UnlockDailyQuests = m_DailyQuestAchievementList.Where(x => x.LockStatus == 0 ||
            (x.LockStatus == 2 && (!x.Info.release_condition.HasValue || m_DailyQuestAchievementList.Any(quset => quset.QuestId == x.Info.release_condition.Value && quset.IsAchieved)))).
            OrderByDescending(x => x.Info.difficulty).ThenByDescending(x => x.LockStatus).ToArray();

        layourGruop.OnUpdateItemEvent.AddListener (UpdateQuestList);
        layourGruop.Initialize(prot, 7,  m_UnlockDailyQuests.Length, false);
    }

    void UpdateQuestList(int index, GameObject obj)
    {
        var behaviour = obj.GetOrAddComponent<ListItem_WeeklyQuest> ();

        bool isOpen = !m_UnlockDailyQuests [index].Info.release_condition.HasValue ||  m_DailyQuestAchievementList.Any(x => x.QuestId == m_UnlockDailyQuests [index].Info.release_condition.Value && x.IsAchieved);
        behaviour.Init (m_UnlockDailyQuests [index], m_DailyQuestWeeklyHeaderSpriteAtlas, isOpen, m_InitRealtimeSinceStartup);
    }

    void DidTapGoEnhance()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToDailyQuest(1);
            LockInputManager.SharedInstance.IsLock = false;
        });
    }

    void DidTapBack()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToEvent());
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    SpriteAtlas m_DailyQuestWeeklyHeaderSpriteAtlas;

    DailyQuestAchievement[] m_DailyQuestAchievementList;
    DailyQuestAchievement[] m_UnlockDailyQuests;
    DailyQuest[] m_QusetList;
    float m_InitRealtimeSinceStartup;

    bool m_IsTimeLimitedOpend = false;
}
