using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;

/// <summary>
/// MiniView : 設定のフレンドサポート画面.
/// </summary>
public class MiniView_OptionSupport : OptionMiniViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(OptionMenu menu)
    {
        m_menu = menu;

		// ヘッダー設定
		var bFollowOrFollower = menu.Enum == OptionMenuEnum.Forllow || menu.Enum == OptionMenuEnum.Forllower;      
		this.GetScript<RectTransform>("HeaderFollow").gameObject.SetActive(bFollowOrFollower);
		if(bFollowOrFollower){
			this.GetScript<TextMeshProUGUI>("Num/txtp_Title").text = MasterDataTable.option_menu.DataList.Find(o => o.Enum == menu.Enum).name;
		}      
        this.GetScript<RectTransform>("HeaderSearchSupport").gameObject.SetActive(menu.Enum == OptionMenuEnum.Search);
		if(menu.Enum == OptionMenuEnum.Forllow){
			this.GetScript<TextMeshProUGUI>("Num/txtp_Num").text = AwsModule.UserData.UserData.FollowCount.ToString();
			this.GetScript<TextMeshProUGUI>("Num/txtp_LimitNum").text = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follow_max.ToString();
		}else if(menu.Enum == OptionMenuEnum.Forllower){
			this.GetScript<TextMeshProUGUI>("Num/txtp_Num").text = AwsModule.UserData.UserData.FollowerCount.ToString();
            this.GetScript<TextMeshProUGUI>("Num/txtp_LimitNum").text = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follower_max.ToString();
		}else if(menu.Enum == OptionMenuEnum.Search){
			this.GetScript<TextMeshProUGUI>("NumSearchSupport/txtp_Num").text = AwsModule.UserData.UserData.FollowCount.ToString();
			this.GetScript<TextMeshProUGUI>("NumSearchSupport/txtp_LimitNum").text = MasterDataTable.user_level[AwsModule.UserData.UserData.Level].follow_max.ToString();
		}
		this.GetScript<TextMeshProUGUI>("txtp_NoItem").SetText("ユーザーが一人もいません。");

        // リスト初期化.
		this.RequestList(currentSort, isDescend);

		// 検索
		this.GetScript<TextMeshProUGUI>("SearchType/txtp_SerchType").text = m_searchMode == SearchMode.UserID ? "プレイヤーID" : "プレイヤー名";
		this.GetScript<Text>("InputID/Placeholder").text = m_searchMode == SearchMode.UserID ? "プレイヤーIDで検索" : "プレイヤー名で検索";
		this.SetCanvasCustomButtonMsg("SearchType/bt_Change", DidTapSwitchSearchMode);
		this.GetScript<Text>("txt_InputID").text = this.GetScript<InputField>("InputID").text = "";

        // ソート.
		var dropdown = GetScript<TMP_Dropdown>("Sort/bt_PullDown");
        dropdown.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(SortDropdownValueChange));      

        // ボタン.
		this.SetCanvasCustomButtonMsg("DoSearch/bt_CommonS", DidTapSearch);
		this.SetCanvasCustomButtonMsg("Order/bt_Ascentd", DidTapOrder);
		this.SetCanvasCustomButtonMsg("Order/bt_Descend", DidTapOrder);
		this.GetScript<CustomButton>("bt_Ascentd").gameObject.SetActive(!isDescend);
        this.GetScript<CustomButton>("bt_Descend").gameObject.SetActive(isDescend);
    }

    // 通信リクエスト : リスト初期化.
	private void RequestList(SortType type = SortType.Login, bool bDescend = false, Action didLoad = null)
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;

		currentSort = type;
		isDescend = bDescend;

        // リスト生成.通信終了で共通で呼ばれる.
        Action<UserData[]> didLoadEx = (list) => {
            View_FadePanel.SharedInstance.IsLightLoading = false;
            LockInputManager.SharedInstance.IsLock = false;
            this.GetScript<TextMeshProUGUI>("txtp_NoItem").gameObject.SetActive(list == null || list.Length <= 0);  
			var root = this.GetScript<InfiniteGridLayoutGroup>("PlayerList");
			root.gameObject.DestroyChildren();         
			if(list == null || list.Length <= 0){
                return;
            }         
            m_userList = new UserData[list.Length];
            Array.Copy(list, m_userList, list.Length);
            var prefab = Resources.Load("Option/ListItem_PlayerInfo") as GameObject;
            root.OnUpdateItemEvent.AddListener(DidCreateInfiniteListItem);
            root.Initialize(prefab, MIN_CNT_LIST, list.Length, false);
			root.ResetScrollPosition();
            if(didLoad != null){
                didLoad();
            }
        };
  
		var reqSortType = RequestSortType.LoginEarly;
		if(currentSort == SortType.Login){
			reqSortType = bDescend ? RequestSortType.LoginLate : RequestSortType.LoginEarly;
		}else if(currentSort == SortType.Rank){
			reqSortType = bDescend ? RequestSortType.RankHigh : RequestSortType.RankLow;
		}else if(currentSort == SortType.Follow){
			reqSortType = bDescend ? RequestSortType.FollowLate : RequestSortType.FollowEarly;
		}
		Debug.Log("=> reqSortType="+reqSortType);

        // フォロー
        if(m_menu.Enum == OptionMenuEnum.Forllow){
			SendAPI.UsersGetFollowList(0, AwsModule.UserData.UserData.FollowCount, (int)reqSortType, (bSuccess, res) => {
                if(!bSuccess || res == null){
                    LockInputManager.SharedInstance.IsLock = false;
                    Debug.LogError("[MiniView_OptionSupport] FollowRequestList Error!! : Response error. res="+res);
                    return;
                }
				this.GetScript<TextMeshProUGUI>("Num/txtp_Num").text = AwsModule.UserData.UserData.FollowCount.ToString();
                didLoadEx(res.UserDataList);
            });
			var dd = this.GetScript<TMP_Dropdown>("Sort/bt_PullDown");  
			foreach(var opt in dd.options){
				if(opt.text.Contains("フォロワー")){
					opt.text = "フォロー";
					break;
				}
			}
			if (dd.captionText.text.Contains("フォロワー")) {
				dd.captionText.text = "フォロー";
            }
        }
        // フォロワー
        else if(m_menu.Enum == OptionMenuEnum.Forllower){
			SendAPI.UsersGetFollowerList(0, AwsModule.UserData.UserData.FollowerCount, (int)reqSortType, (bSuccess, res) => {
                if(!bSuccess || res == null){
                    LockInputManager.SharedInstance.IsLock = false;
                    Debug.LogError("[MiniView_OptionSupport] FollowerRequestList Error!! : Response error. res=" + res);
                    return;
                }
				this.GetScript<TextMeshProUGUI>("Num/txtp_Num").text = AwsModule.UserData.UserData.FollowerCount.ToString();
                didLoadEx(res.UserDataList);
            });
			var dd = this.GetScript<TMP_Dropdown>("Sort/bt_PullDown");         
			foreach (var opt in dd.options) {
				if (opt.text.Contains("フォロー")) {
					opt.text = "フォロワー";
                    break;
                }
            }
			if (dd.captionText.text.Contains("フォロー")){
				dd.captionText.text = "フォロワー";
			}
        }
        // 検索
        else if(m_menu.Enum == OptionMenuEnum.Search){
            SendAPI.UsersGetFriendCandidateList((bSuccess, res) => { 
                if (!bSuccess || res == null) {
                    LockInputManager.SharedInstance.IsLock = false;
                    Debug.LogError("[MiniView_OptionSupport] UsersGetFriendCandidateList Error!! : Response error. res=" + res);
                    return;
                }
				this.GetScript<TextMeshProUGUI>("NumSearchSupport/txtp_Num").text = AwsModule.UserData.UserData.FollowCount.ToString();
                didLoadEx(res.UserDataList);
            });
        }
    }
    // 無限スクロール常にリストアイテムを生成した際のコールバック.
    void DidCreateInfiniteListItem(int index, GameObject createObj)
    {
        if(m_userList == null || m_userList.Length <= 0){
            return;
        }
        var c = createObj.GetOrAddComponent<ListItem_PlayerInfo>();
        Debug.Log("index="+index+"/length="+m_userList.Length);
		c.InitFromOption(m_userList[index], m_menu, () => {
			this.GetScript<Text>("txt_InputID").text = this.GetScript<InputField>("InputID").text = "";
			RequestList();
		});
    }

    // ドロップダウンからのソート.   
	private void SortDropdownValueChange(int sortVal)
	{
		currentSort = (SortType)sortVal;
		this.RequestList(currentSort, isDescend);
	}
    #region ButtonDelegate.

    // ボタン : 検索.
    void DidTapSearch()
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;

        // リスト生成.通信終了で共通で呼ばれる.
        Action<UserData[]> didLoad = (list) => {
            View_FadePanel.SharedInstance.IsLightLoading = false;         
			var root = this.GetScript<InfiniteGridLayoutGroup>("PlayerList");         
			root.gameObject.DestroyChildren();
			this.GetScript<TextMeshProUGUI>("txtp_NoItem").gameObject.SetActive(list == null || list.Length <= 0);  
            if(list == null || list.Length <= 0){
                return;
            }
            m_userList = new UserData[list.Length];
            Array.Copy(list, m_userList, list.Length);
            var prefab = Resources.Load("Option/ListItem_PlayerInfo") as GameObject;
            root.OnUpdateItemEvent.AddListener(DidCreateInfiniteListItem);
            root.Initialize(prefab, MIN_CNT_LIST, list.Length, false); 
        };

		// 検索モードに応じて検索方法を変更.
		var searchInput = this.GetScript<Text>("txt_InputID").text;
		if (m_searchMode == SearchMode.UserID) {
			// ID検索         
			if (!string.IsNullOrEmpty(searchInput)) { 
				SendAPI.UsersSearchByCustomerId(searchInput, (bSuccess, res) => {
                    if (!bSuccess && res == null) {
                        Debug.LogError("UsersSearchByCustomerId Error!!");
                        return;
                    }
					if (res.UserDataList.Length <= 0) {
                        PopupManager.OpenPopupOK("該当ユーザーが見つかりませんでした。");
                    }
                    didLoad(res.UserDataList);
                });
			}else{
				didLoad(m_userList);
			}
		}else if(m_searchMode == SearchMode.UserName) { 
			// ニックネーム検索.
			if (!string.IsNullOrEmpty(searchInput)) { 
				SendAPI.UsersSearchByNickname(searchInput, (bSuccess, res) => {
                    if (!bSuccess && res == null) {
                        Debug.LogError("UsersSearchByCustomerId Error!!");
                        return;
                    }
					if (res.UserDataList.Length <= 0) {
                        PopupManager.OpenPopupOK("該当ユーザーが見つかりませんでした。");
                    }
                    didLoad(res.UserDataList);
                });
			}else{
                didLoad(m_userList);
            }
		}
    }

	// ボタン : 検索モード切り替え.
    void DidTapSwitchSearchMode()
	{
		m_searchMode = m_searchMode == SearchMode.UserID ? SearchMode.UserName : SearchMode.UserID;
		this.GetScript<Text>("txt_InputID").text = this.GetScript<InputField>("InputID").text = "";
		this.GetScript<TextMeshProUGUI>("SearchType/txtp_SerchType").text = m_searchMode == SearchMode.UserID ? "プレイヤーID" : "プレイヤー名";
        this.GetScript<Text>("InputID/Placeholder").text = m_searchMode == SearchMode.UserID ? "プレイヤーIDで検索" : "プレイヤー名で検索";      
	}

	// ボタン : 昇順と降順の切り替え.
    void DidTapOrder()
	{
		isDescend = !isDescend;
		this.GetScript<CustomButton>("bt_Ascentd").gameObject.SetActive(!isDescend);
		this.GetScript<CustomButton>("bt_Descend").gameObject.SetActive(isDescend);
		this.RequestList(currentSort, isDescend);
	}
    #endregion

    // 破棄処理.インプット内容やリストをクリアする.
    protected override void WillDetachProc(Action didProcEnd)
    {
        this.GetScript<InfiniteGridLayoutGroup>("PlayerList").gameObject.DestroyChildren();
        this.GetScript<Text>("txt_InputID").text = "";
        m_userList = null;
        m_menu = null;
        base.WillDetachProc(didProcEnd);
    }

	private SearchMode m_searchMode = SearchMode.UserID;
    private UserData[] m_userList;
	private OptionMenu m_menu;
	private static SortType currentSort = SortType.Login;
	private static bool isDescend = false;
    private static readonly int MIN_CNT_LIST = 5;


    // enum : 並び順タイプ.
    private enum SortType
    {
		Login = 0,
        Rank,
        Follow,
    }
    private enum RequestSortType
	{      
		LoginEarly = 0,  // ログイン早い順
        LoginLate,       // ログイン遅い順
		RankLow,         // ランク低い順
        RankHigh,        // ランク高い順
        FollowEarly,     // フォロー早い順
        FollowLate,      // フォロー遅い順
	}
	// enum : 検索モード.
    private enum SearchMode
	{
		UserID,
        UserName,
	}
}