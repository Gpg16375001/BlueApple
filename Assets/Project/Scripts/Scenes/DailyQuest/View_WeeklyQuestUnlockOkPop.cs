using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_WeeklyQuestUnlockOkPop : PopupViewBase {

    private static View_WeeklyQuestUnlockOkPop instance;
    public static View_WeeklyQuestUnlockOkPop Create(int questType, int dayOfWeek)
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("DailyQuest/View_WeeklyQuestUnlockOkPop");
        instance = go.GetOrAddComponent<View_WeeklyQuestUnlockOkPop>();
        instance.InitInternal(questType, dayOfWeek);
        return instance;
    }

    private void InitInternal(int questType, int dayOfWeek)
    {
        m_QuestType = questType;
        m_DayOfWeek = dayOfWeek;

        GetScript<TextMeshProUGUI> ("txtp_SelectWeek").SetText (MasterDataTable.day_of_week[dayOfWeek].name);

        var item = ConsumerData.CacheGet (50001);
        if (item != null && item.Count > 0) {
            GetScript<TextMeshProUGUI> ("txtp_OpenItemStockNum").SetText (item.Count);
            // MEMO: 複数個で開くようなクエストのことは考えていない
            GetScript<TextMeshProUGUI> ("txtp_OpenItemNum").SetText (item.Count - 1);
            GetScript<CustomButton> ("Yes/bt_CommonS02").interactable = true;
        } else {
            GetScript<TextMeshProUGUI> ("txtp_OpenItemStockNum").SetText (0);
            // MEMO: 複数個で開くようなクエストのことは考えていない
            GetScript<TextMeshProUGUI> ("txtp_OpenItemNum").SetText (0);
            GetScript<CustomButton> ("Yes/bt_CommonS02").interactable = false;
        }

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("Cancel/bt_CommonS01", DidTapClose);
        SetCanvasCustomButtonMsg ("Yes/bt_CommonS02", DidTapYes);
        SetBackButton ();

    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    void DidTapYes()
    {
        if (IsClosed) {
            return;
        }
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.QuestsUnlockDailyQuest (m_QuestType, m_DayOfWeek,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                PlayOpenCloseAnimation (false, () => {
                    if(!success){

                        if(response.ResultCode == (int)ServerResultCodeEnum.QUEST_ALREADY_UNLOCKED) {
                            PopupManager.OpenPopupSystemOK(TextData.GetText(ServerResultCodeEnum.QUEST_ALREADY_UNLOCKED.ToString()),
                                () => {
                                    LockInputManager.SharedInstance.IsLock = true;
                                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                                        ScreenChanger.SharedInstance.GoToDailyQuest(m_QuestType == 4 ? 1 : 2);
                                        LockInputManager.SharedInstance.IsLock = false;
                                    });
                                }
                            );
                        } else if(response.ResultCode == (int)ServerResultCodeEnum.QUEST_NOT_LOCKED) {
                            PopupManager.OpenPopupSystemOK(TextData.GetText(ServerResultCodeEnum.QUEST_ALREADY_UNLOCKED.ToString()),
                                () => {
                                    LockInputManager.SharedInstance.IsLock = true;
                                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                                        ScreenChanger.SharedInstance.GoToDailyQuest(m_QuestType == 4 ? 1 : 2);
                                        LockInputManager.SharedInstance.IsLock = false;
                                    });
                                }
                            );
                        }
                        return;
                    }

                    var item = ConsumerData.CacheGet (50001);
                    item.Count -= 1;
                    item.CacheSet();

                    LockInputManager.SharedInstance.IsLock = true;
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToDailyQuest(m_QuestType == 4 ? 1 : 2);
                        LockInputManager.SharedInstance.IsLock = false;
                    });
                    Dispose();
                });
            }
        );
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            Dispose();
        });
    }

    int m_QuestType;
    int m_DayOfWeek;
}
