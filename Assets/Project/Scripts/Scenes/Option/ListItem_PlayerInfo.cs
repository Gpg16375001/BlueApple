using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// ListItem : プレイヤー情報.
/// </summary>
public class ListItem_PlayerInfo : ViewBase
{
    #if false
    /// <summary>
    /// 初期化. (1.0.9以前)
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
        if(menu.Enum == OptionMenuEnum.Forllow) {
			this.GetScript<RectTransform>("Release").gameObject.SetActive(true);
		}
        if(menu.Enum == OptionMenuEnum.Forllower) {
			this.GetScript<RectTransform>("ReleaseFollower").gameObject.SetActive(true);	//フォロワー解除
			this.GetScript<RectTransform>("Release").gameObject.SetActive(false);
			if( this.GetScript<RectTransform>("Follow").gameObject.activeSelf )
				this.GetScript<RectTransform>("ShowSupport").gameObject.SetActive(false);
		}

        // アイコン.
        this.GetScript<Image>("img_FriendIcon").gameObject.SetActive(m_data.IsFollow && m_data.IsFollower);  // 相互フォロー.
		this.GetScript<Image>("img_FollowIcon").gameObject.SetActive(m_data.IsFollow && !m_data.IsFollower); // フォロー.
		this.GetScript<Image>("img_FollowerIcon").gameObject.SetActive(!m_data.IsFollow && m_data.IsFollower); // フォロワー.

        if (!m_isBoot){
            // ボタンイベント登録.
            this.SetCanvasCustomButtonMsg("Follow/bt_CommonS", DidTapFollow);
            this.SetCanvasCustomButtonMsg("ShowSupport/bt_CommonS", DidTapSupportConfirm);
            this.SetCanvasCustomButtonMsg("UnitIcon/bt_CommonS", DidTapSupportConfirm);
            this.SetCanvasCustomButtonMsg("Release/bt_CommonS", DidTapUnfollow);
            this.SetCanvasCustomButtonMsg("ReleaseFollower/bt_CommonS", DidTapDelete);
            m_isBoot = true;
        }
    }
    #endif

    /// <summary>
    /// 初期化.(1.0.9以降)
    /// </summary>
    public void InitFromUserData(UserData data, OptionMenu menu, Action didRequest = null)
    {
        m_UserId = data.UserId;
        m_MainCardData = data.MainCardData;
        m_Nickname = data.Nickname;
        m_Level = data.Level;
        m_Comment = data.Comment;
        m_LastLoginDate = data.LastLoginDate;
        m_FollowerCount = data.FollowerCount;
        m_IsFollow = data.IsFollow;
        m_IsFollower = data.IsFollower;
        m_SupporterCardData = data.SupporterCardList;
        m_HasSupportInfo = true;
        m_didRequest = didRequest;

        InitFormOption (menu);
    }

    /// <summary>
    /// 初期化.(1.0.9以降)
    /// </summary>
    public void InitFromFriendData(FriendData data, OptionMenu menu, Action didRequest = null)
    {
        m_UserId = data.UserId;
        m_MainCardData = data.MainCardData;
        m_Nickname = data.Nickname;
        m_Level = data.Level;
        m_Comment = data.Comment;
        m_LastLoginDate = data.LastLoginDate;
        m_FollowerCount = data.FollowerCount;
        m_IsFollow = data.IsFollow;
        m_IsFollower = data.IsFollower;
        m_SupporterCardData = null;
        m_HasSupportInfo = false;
        m_didRequest = didRequest;

        InitFormOption (menu);
    }

    void InitFormOption(OptionMenu menu)
    {
        // ユニットアイコン.
        var icon = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", this.GetScript<RectTransform>("UnitIcon").gameObject);
        var c = icon.GetOrAddComponent<ListItem_UnitIcon>();
        c.Init(m_MainCardData);

        // フレーム.
        this.GetScript<Image>("img_FollowList").gameObject.SetActive(menu.Enum == OptionMenuEnum.Forllow);      // フォロー.
        this.GetScript<Image>("img_FollowerList").gameObject.SetActive(menu.Enum == OptionMenuEnum.Forllower);  // フォロワー.
        this.GetScript<Image>("img_PlayerList").gameObject.SetActive(menu.Enum == OptionMenuEnum.Search);       // 検索.   

        // ラベル類.
        this.GetScript<Text>("txt_FriendName").text = m_Nickname;
        this.GetScript<TextMeshProUGUI>("txtp_FriendLv").text = m_Level.ToString();
        this.GetScript<Text>("txt_Message").text = m_Comment;

        var lastLoginDate = DateTime.Parse(m_LastLoginDate, null, DateTimeStyles.RoundtripKind);
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
        this.GetScript<RectTransform>("Release").gameObject.SetActive(m_IsFollow);  // フォロー解除.
        this.GetScript<RectTransform>("Follow").gameObject.SetActive(!m_IsFollow);  // フォローする.
        if(menu.Enum == OptionMenuEnum.Forllow) {
            this.GetScript<RectTransform>("Release").gameObject.SetActive(true);
        }
        if(menu.Enum == OptionMenuEnum.Forllower) {
            this.GetScript<RectTransform>("ReleaseFollower").gameObject.SetActive(true);    //フォロワー解除
            this.GetScript<RectTransform>("Release").gameObject.SetActive(false);
            this.GetScript<RectTransform> ("ShowSupport").gameObject.SetActive (m_IsFollow);
        }

        // アイコン.
        this.GetScript<Image>("img_FriendIcon").gameObject.SetActive(m_IsFollow && m_IsFollower);  // 相互フォロー.
        this.GetScript<Image>("img_FollowIcon").gameObject.SetActive(m_IsFollow && !m_IsFollower); // フォロー.
        this.GetScript<Image>("img_FollowerIcon").gameObject.SetActive(!m_IsFollow && m_IsFollower); // フォロワー.

        if (!m_isBoot){
            // ボタンイベント登録.
            this.SetCanvasCustomButtonMsg("Follow/bt_CommonS", DidTapFollow);
            this.SetCanvasCustomButtonMsg("ShowSupport/bt_CommonS", DidTapSupportConfirm);
            this.SetCanvasCustomButtonMsg("UnitIcon/bt_CommonS", DidTapSupportConfirm);
            this.SetCanvasCustomButtonMsg("Release/bt_CommonS", DidTapUnfollow);
            this.SetCanvasCustomButtonMsg("ReleaseFollower/bt_CommonS", DidTapDelete);
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
        if(IsFollow){
            return; // すでにフォロー中.
        }
		var max = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follower_max;
        if(FollowerCount >= max){
			PopupManager.OpenPopupOK("相手の"+TextData.GetText("FOLLOWER_USER_FULL"));
			return;
		}
		max = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follow_max;
		if(AwsModule.UserData.UserData.FollowCount >= max){
            PopupManager.OpenPopupOK("自分の"+TextData.GetText("FOLLOW_USER_FULL"));
            return;
        }
        PopupManager.OpenPopupYN(Nickname + "さんをフォローしますか？", () => {
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.UsersFollowUser(UserID, (bSuccess, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!bSuccess || res == null){
                    return;
                }
				AwsModule.UserData.UserData = res.UserData;
                PopupManager.OpenPopupOK(Nickname + "さんをフォローしました。");
                if(m_didRequest != null){
                    View_OptionSupport.IsFollowUser(UserID, res.FriendData);
                    m_didRequest();
                }
            }); 
        });
    }

    // ボタン : フォロー解除.
    void DidTapUnfollow()
    {
        if (!IsFollow) {
            return; // フォローしてない.
        }
        PopupManager.OpenPopupYN (Nickname + "さんをフォロー解除しますか？", () => {
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.UsersUnfollowUser (UserID, (bSuccess, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!bSuccess || res == null) {
                    return;
                }
                AwsModule.UserData.UserData = res.UserData;
                PopupManager.OpenPopupOK (Nickname + "さんをフォロー解除しました。");
                if (m_didRequest != null) {
                    View_OptionSupport.IsUnFollowUser (UserID);
                    m_didRequest ();
                }
            });
        });
    }

	// ボタン : サポート確認.
    void DidTapSupportConfirm()
	{
		this.IsEnableButton = false;
        if (m_data != null) {
            View_OptionSupportInfoPop.Create (m_data, () => this.IsEnableButton = true);
        } else if (View_OptionSupport.CacheUserDataList.ContainsKey (UserID)) {
            View_OptionSupportInfoPop.Create (View_OptionSupport.CacheUserDataList[UserID], () => this.IsEnableButton = true);
        } else {
            // ユーザーデータをリクエストしてデータを取得する
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.UsersGetUserData(new int[] { UserID } , (bSuccess, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!bSuccess || res == null){
                    return;
                }
                var _userData = res.UserDataList[0];
                if(!View_OptionSupport.CacheUserDataList.ContainsKey(_userData.UserId)) {
                    View_OptionSupport.CacheUserDataList.Add(_userData.UserId, _userData);
                }
                View_OptionSupportInfoPop.Create (View_OptionSupport.CacheUserDataList[UserID], () => this.IsEnableButton = true);
            }); 
        }
	}

	// ボタン : 削除
	void DidTapDelete()
	{
        if (!IsFollower) {
            return; // フォローされていない.
        }
        PopupManager.OpenPopupYN(Nickname + "さんをフォロワー解除しますか？", () => {
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.UsersRemoveFollower(UserID, (bSuccess, res) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!bSuccess || res == null){
                    return;
                }
                PopupManager.OpenPopupOK(Nickname + "さんをフォロワー解除しました。");
                if(m_didRequest != null){
                    View_OptionSupport.IsRemoveFollower (UserID);
                    m_didRequest();
                }
            }); 
        });
	}

    #endregion

    private UserData m_data;
    private Action m_didRequest;
    private bool m_isBoot;

    //TODO; 1.0.9以降
    int m_UserId;                               // ユーザーID
    int m_Level;                                // レベル
    string m_Nickname;                          // ニックネーム
    CardData m_MainCardData;                    // メインカードのデータ
    string m_Comment;                           // コメント
    string m_LastLoginDate;                     // 最終ログイン
    int m_FollowerCount;                        // フォロワー数
    bool m_IsFollow;                            // フォローフラグ
    bool m_IsFollower;                          // フォロワーフラグ
    bool m_HasSupportInfo;                      // サポートデータを持っている
    SupporterCardData[] m_SupporterCardData;    // サポートカードリスト
    OptionMenuEnum m_MenuEnum;                  // メニュータイプ

    int UserID {
        get {
            return m_data != null ? m_data.UserId : m_UserId;
        }
    }

    string Nickname {
        get {
            return m_data != null ? m_data.Nickname : m_Nickname; 
        }
    }

    bool IsFollow { 
        get {
            return m_data != null ? m_data.IsFollow : m_IsFollow;
        }
    }

    bool IsFollower {
        get {
            return m_data != null ? m_data.IsFollower : m_IsFollower;
        }
    }

    int FollowerCount {
        get {
            return m_data != null ? m_data.FollowerCount : m_FollowerCount;
        }
    }
}
