using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_BattleSupportFollowPop : PopupViewBase {

    public static View_BattleSupportFollowPop Create(UserData support, bool bLatestClearQuest = false)
    {
        var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleSupportFollowPop");
        var c = go.GetOrAddComponent<View_BattleSupportFollowPop>();
		c.InitInternal(support, bLatestClearQuest);
        return c; 
    }

	private void InitInternal(UserData support, bool bLatestClearQuest)
    {
        m_Suppoter = support;
        m_SuppoterUserID = support.UserId;
		m_bLatestClearQuest = bLatestClearQuest;

        GetScript<Text> ("txt_SupportPlayerName").text = support.Nickname;

        m_OptionTabs.Clear ();
        var TabMenuGo = this.GetScript<Transform>("TabMenu").gameObject;
        List<string> ElementStringList = MasterDataTable.element.GetCategoryNameList();
        ElementStringList.Remove ("ALL");
        ElementEnum? firstElement = null;
        foreach (var elementString in ElementStringList) {
            var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_HorizontalTextTab", TabMenuGo);
            var c = go.GetOrAddComponent<ListItem_HorizontalElementTab> ();
            c.Init (elementString, DidTapTab);
            var cardData = GetCardData (c.ElementType);
            c.SetInteractable (cardData != null);
            if (cardData != null && !firstElement.HasValue) {
                c.IsSelected = true;
                firstElement = c.ElementType;
            }
            m_OptionTabs.Add (c);
        }

        var supportInfoRoot = GetScript<RectTransform>("SupportPlayerInfoAnchor").gameObject;
        var ListItemFriend = GameObjectEx.LoadAndCreateObject ("FriendSelect/ListItem_Friend", supportInfoRoot);
        m_ListItemFriend = ListItemFriend.GetOrAddComponent<ListItem_Friend> ();
        m_ListItemFriend.Init (m_Suppoter, GetCardData(firstElement), false);

        SetCanvasCustomButtonMsg ("Unfollow/bt_Common", DidTapUnfollow);
        SetCanvasCustomButtonMsg ("Follow/bt_Common", DidTapFollow);

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapUnfollow();
    }

    void DidTapTab(ListItem_HorizontalElementTab tabItem)
    {
        if (IsClosed) {
            return;
        }

        m_OptionTabs.ForEach (x => x.IsSelected = false); 
        tabItem.IsSelected = true;

        if (m_ListItemFriend != null) {
            m_ListItemFriend.SetCardData (GetCardData(tabItem.ElementType));
        }
    }

    SupporterCardData GetCardData(ElementEnum? element)
    {
        SupporterCardData cardData = null;
        if (element.HasValue) {
            if (m_Suppoter.SupporterCardList != null && m_Suppoter.SupporterCardList.Length > 0) {
                cardData = System.Array.Find(m_Suppoter.SupporterCardList, x => x.Card != null && x.Card.element.Enum == element);
            }
        }
        return cardData;
    }

    void DidTapUnfollow()
    {
        if (IsClosed) {
            return;
        }

        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, NextScene);
    }

    void DidTapFollow()
    {
        if (IsClosed) {
            return;
        }

        var max = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follower_max;
        if(m_Suppoter.FollowerCount >= max){
            PopupManager.OpenPopupOK("相手の"+TextData.GetText("FOLLOWER_USER_FULL"));
            return;
        }
        max = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follow_max;
        if(AwsModule.UserData.UserData.FollowCount >= max){
            PopupManager.OpenPopupOK("自分の"+TextData.GetText("FOLLOW_USER_FULL"));
            return;
        }

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.UsersFollowUser (m_SuppoterUserID,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, NextScene);
            }
        );
    }

    void NextScene()
    {
        // 次に遷移させる
        AwsModule.BattleData.GameOverProc();
        if(AwsModule.ProgressData.CurrentQuest.QuestType == 4) {
            ScreenChanger.SharedInstance.GoToDailyQuest(1); 
        } else if(AwsModule.ProgressData.CurrentQuest.QuestType == 5) {
            ScreenChanger.SharedInstance.GoToDailyQuest(2);
        } else {
            ScreenChanger.SharedInstance.GoToScenario(m_bLatestClearQuest);
        }
        this.Dispose();
    }

    private int m_SuppoterUserID;
	private bool m_bLatestClearQuest;
    private UserData m_Suppoter;
    private List<ListItem_HorizontalElementTab> m_OptionTabs = new List<ListItem_HorizontalElementTab>();
    private ListItem_Friend m_ListItemFriend;
}
