using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// ListItem : 戦闘前フレンドリスト.
/// </summary>
public class ListItem_Friend : ViewBase
{
	public UserData UserData { get; private set; }
    public SupporterCardData CardData { get; private set; }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(UserData user, SupporterCardData card, bool buttonInteractable=true)
    {
		UserData = user;

        // 各種ユーザー情報.
        GetScript<Text>("txt_FriendName").text = UserData.Nickname;
        GetScript<TextMeshProUGUI>("txtp_FriendLv").SetText(UserData.Level);
        GetScript<Text>("txt_Message").text = UserData.Comment;

		// キャラ設定.
        SetCardData(card);

        var lastLoginDate = DateTime.Parse(user.LastLoginDate, null, DateTimeStyles.RoundtripKind);      
        var lastLogin = "";
        var days = GameTime.SharedInstance.Now.GetDays(lastLoginDate);
        if (days > 0) {
            lastLogin = days.ToString() + "日前";
        } else {
			var span = GameTime.SharedInstance.Now - lastLoginDate;
            if (span.Hours > 0) {
                lastLogin = span.Hours + "時間前";
            } else {
                if (span.Minutes > 0) {
                    lastLogin = span.Minutes + "分前";
                } else {
                    lastLogin = span.Seconds + "秒前";
                }
            }
        }
        GetScript<TextMeshProUGUI>("txtp_LoginDate").SetText(lastLogin);
		if( Exist<RectTransform>("Group_FriendPoint") ) {
			GetScript<RectTransform>("Group_FriendPoint").gameObject.SetActive( user.GainFriendPointOnSupport != 0 );
			GetScript<TextMeshProUGUI>("Group_FriendPoint/txtp_Num").text = string.Format( "{0}", user.GainFriendPointOnSupport );
		}

		GetScript<Image>("img_FriendIcon").gameObject.SetActive(UserData.IsFollow && UserData.IsFollower);
		GetScript<Image>("img_FollowIcon").gameObject.SetActive(UserData.IsFollow && !UserData.IsFollower);
		GetScript<Image>("img_FollowerIcon").gameObject.SetActive(!UserData.IsFollow && UserData.IsFollower);

        SetCanvasButtonMsg("img_PlayerList", DidTapFrined);
        GetScript<Button> ("img_PlayerList").interactable = buttonInteractable;
    }

    /// <summary>
	/// 表示するカード情報設定.
    /// </summary>
    public void SetCardData(SupporterCardData card)
	{
        CardData = card;

        var UnitIconRoot = GetScript<Transform> ("UnitIcon");
        var unitIcon = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", UnitIconRoot.gameObject);
        unitIcon.GetOrAddComponent<ListItem_UnitIcon> ().Init (card.ConvertCardData());

        GetScript<TextMeshProUGUI>("txtp_PlayerUnitName").SetText(CardData.Card.nickname);
        GetScript<TextMeshProUGUI>("txtp_PlayerUnitLv").SetText(CardData.Level);
        GetScript<TextMeshProUGUI>("txtp_UnitHP").SetText(CardData.Parameter.MaxHp);
        GetScript<TextMeshProUGUI>("txtp_UnitATK").SetText(CardData.Parameter.Attack);
        GetScript<TextMeshProUGUI>("txtp_UnitDEF").SetText(CardData.Parameter.Defense);
        GetScript<TextMeshProUGUI>("txtp_UnitSPD").SetText(CardData.Parameter.Agility);

        SetWeaponData (card.EquippedWeaponData);
	}
	// アイコン設定.
    void SetIcon(IconLoadSetting data, Sprite icon)
    {
        // 更新状態によっては内部データが変わっている可能性があるので判定が必要
		if (CardData.CardId == data.id) {
            var iconImg = GetScript<Image>("IconCh");
            iconImg.sprite = icon;
        }
    }

    private void SetWeaponData(WeaponData weapon)
    {
        var root = GetScript<RectTransform> ("WeaponIcon").gameObject;
        if (weapon == null || weapon.BagId <= 0) {
            // 
            root.DestroyChildren();
            return;
        }
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", root);
		go.GetOrAddComponent<ListItem_WeaponIcon>().Init(weapon, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);

    }
    // ボタン : タップ.
    void DidTapFrined()
    {
		// 初回起動
        if (AwsModule.ProgressData.IsFirstBoot) {
			TutorialFirstBootModule.DestroyInstance();
        }
		
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            AwsModule.BattleData.SupportUserId = UserData.UserId;
			ScreenChanger.SharedInstance.GoToQuestPreparation(CardData);
        });
    }   
}
