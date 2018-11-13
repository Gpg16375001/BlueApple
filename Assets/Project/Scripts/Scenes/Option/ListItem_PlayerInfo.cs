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
/// ListItem : プレイヤー情報.
/// </summary>
public class ListItem_PlayerInfo : ViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void InitFromOption(UserData data, OptionMenu menu, Action didRequest = null)
    {
        m_data = data;
        m_didRequest = didRequest;

		// ユニットアイコン.
		var icon = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", this.GetScript<RectTransform>("UnitIcon").gameObject);
		var c = icon.GetOrAddComponent<ListItem_UnitIcon>();
		c.Init(data.MainCardData);

        // フレーム.
        this.GetScript<Image>("img_FollowList").gameObject.SetActive(menu.Enum == OptionMenuEnum.Forllow);      // フォロー.
        this.GetScript<Image>("img_FollowerList").gameObject.SetActive(menu.Enum == OptionMenuEnum.Forllower);  // フォロワー.
        this.GetScript<Image>("img_PlayerList").gameObject.SetActive(menu.Enum == OptionMenuEnum.Search);       // 検索.   

        // ラベル類.
		this.GetScript<Text>("txt_FriendName").text = m_data.Nickname;
		this.GetScript<TextMeshProUGUI>("txtp_FriendLv").text = m_data.Level.ToString();
		this.GetScript<Text>("txt_Message").text = m_data.Comment;

        var lastLoginDate = DateTime.Parse(m_data.LastLoginDate, null, DateTimeStyles.RoundtripKind);
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
        this.GetScript<TextMeshProUGUI>("txtp_LoginDate").text = lastLogin;

        // ボタンルート.
        this.GetScript<RectTransform>("Release").gameObject.SetActive(m_data.IsFollow);  // フォロー解除.
        this.GetScript<RectTransform>("Follow").gameObject.SetActive(!m_data.IsFollow);  // フォローする.

        // アイコン.
        this.GetScript<Image>("img_FriendIcon").gameObject.SetActive(m_data.IsFollow && m_data.IsFollower);  // 相互フォロー.
		this.GetScript<Image>("img_FollowIcon").gameObject.SetActive(m_data.IsFollow && !m_data.IsFollower); // フォロー.
		this.GetScript<Image>("img_FollowerIcon").gameObject.SetActive(!m_data.IsFollow && m_data.IsFollower); // フォロワー.

        if (!m_isBoot){
            // ボタンイベント登録.
            this.SetCanvasCustomButtonMsg("Follow/bt_CommonS", DidTapFollow);
            this.SetCanvasCustomButtonMsg("ShowSupport/bt_CommonS", DidTapSupportConfirm);
            this.SetCanvasCustomButtonMsg("Release/bt_CommonS", DidTapUnfollow);
            m_isBoot = true;
        }
    }

    public void InitFromBattle(UserData support)
    {
        m_data = support;

        // ラベル類.
        this.GetScript<TextMeshProUGUI>("txtp_FriendName").text = m_data.Nickname;
        this.GetScript<TextMeshProUGUI>("txtp_FriendLv").text = "Lv." + m_data.Level;
        this.GetScript<TextMeshProUGUI>("txtp_Message").text = m_data.Comment;

        var lastLoginDate = DateTime.Parse(m_data.LastLoginDate, null, DateTimeStyles.RoundtripKind);
        var lastLogin = "";
        var days = GameTime.SharedInstance.Now.GetDays(lastLoginDate);
        if (days > 0) {
            lastLogin = days.ToString() + "日前";
        } else {
            var span = lastLoginDate - GameTime.SharedInstance.Now;
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
        this.GetScript<TextMeshProUGUI>("txtp_LoginDate").text = "最終ログイン : " + lastLogin;
        this.GetScript<Image>("img_FriendIcon").gameObject.SetActive(!m_data.IsFollow && m_data.IsFollower); // フォロワー.
    }

    #region ButtonDelegate.

    // ボタン : フォロー.
    void DidTapFollow()
    {
        if(m_data.IsFollow){
            return; // すでにフォロー中.
        }
		var max = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follower_max;
		if(m_data.FollowerCount >= max){
			PopupManager.OpenPopupOK("相手の"+TextData.GetText("FOLLOWER_USER_FULL"));
			return;
		}
		max = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follow_max;
		if(AwsModule.UserData.UserData.FollowCount >= max){
            PopupManager.OpenPopupOK("自分の"+TextData.GetText("FOLLOW_USER_FULL"));
            return;
        }
        PopupManager.OpenPopupYN(m_data.Nickname+"さんをフォローしますか？", () => {
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.UsersFollowUser(m_data.UserId, (bSuccess, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!bSuccess || res == null){
                    return;
                }
				AwsModule.UserData.UserData = res.UserData;
                PopupManager.OpenPopupOK(m_data.Nickname + "さんをフォローしました。");
                if(m_didRequest != null){
                    m_didRequest();
                }
            }); 
        });
    }

    // ボタン : フォロー解除.
    void DidTapUnfollow()
    {
        if (!m_data.IsFollow) {
            return; // フォローしてない.
        }
        PopupManager.OpenPopupYN(m_data.Nickname + "さんをフォロー解除しますか？", () => {
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.UsersUnfollowUser(m_data.UserId, (bSuccess, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!bSuccess || res == null) {
                    return;
                }
				AwsModule.UserData.UserData = res.UserData;
                PopupManager.OpenPopupOK(m_data.Nickname + "さんをフォロー解除しました。");
                if (m_didRequest != null) {
                    m_didRequest();
                }
            });
        });
    }

	// ボタン : サポート確認.
    void DidTapSupportConfirm()
	{
		this.IsEnableButton = false;
		View_OptionSupportInfoPop.Create(m_data, () => this.IsEnableButton = true);
	}

    #endregion

    private UserData m_data;
    private Action m_didRequest;
    private bool m_isBoot;
}
