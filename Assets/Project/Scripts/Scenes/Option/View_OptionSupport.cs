using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;


/// <summary>
/// View : オプション画面 サポート.
/// </summary>
public class View_OptionSupport : View_OptionListTypeBase
{
    private static bool SendFollow;

    // フォロー
    private static List<SmileLab.Net.API.FriendData> m_userFollowList;
    public static SmileLab.Net.API.FriendData[] UserFollowList { 
        get {
            if (m_userFollowList == null) {
                return null;
            }
            return m_userFollowList.ToArray ();
        }
    }
    // フォロワー
    private static List<SmileLab.Net.API.FriendData> m_userFollowerList;
    public static SmileLab.Net.API.FriendData[] UserFollowerList {
        get {
            if (m_userFollowerList == null) {
                return null;
            }
            return m_userFollowerList.ToArray ();
        }
    }
    // ユーザーデータのキャッシュ
    private static Dictionary<int, SmileLab.Net.API.UserData> m_cacheUserDataList;
    public static Dictionary<int, SmileLab.Net.API.UserData> CacheUserDataList {
        get {
            m_cacheUserDataList = m_cacheUserDataList ?? new Dictionary<int, SmileLab.Net.API.UserData> ();
            return m_cacheUserDataList;
        }
        set {
            m_cacheUserDataList = value;
        }
    }

    public static void ResFollowList(SmileLab.Net.API.FriendData[] dataArray)
    {
        m_userFollowList = m_userFollowList ?? new List<SmileLab.Net.API.FriendData> ();
        if (dataArray.Length > 0) {
            m_userFollowList = dataArray.ToList ();
        }
    }

    public static void ResFollowerList(SmileLab.Net.API.FriendData[] dataArray)
    {
        m_userFollowerList = m_userFollowerList ?? new List<SmileLab.Net.API.FriendData> ();
        if (dataArray.Length > 0) {
            m_userFollowerList = dataArray.ToList ();
        }
    }

    /// <summary>
    /// 特定ユーザーをフォロー状態にする
    /// </summary>
    /// <param name="userID">ユーザーID</param>
    /// <param name="friendData">フレンドデータ</param>
    public static void IsFollowUser(int userID, SmileLab.Net.API.FriendData friendData = null)
    {
        if (m_userFollowerList != null && m_userFollowerList.Any (x => x.UserId == userID)) {
            m_userFollowerList.Find (x => x.UserId == userID).IsFollow = true;
        }

        if (friendData != null && m_userFollowList != null) {
            if (!m_userFollowList.Any (x => x.UserId == friendData.UserId)) {
                m_userFollowList.Add (friendData);
            } else {
                int index = m_userFollowList.FindIndex (x => x.UserId == friendData.UserId);
                m_userFollowList [index] = friendData;
            }
        }
    }

    public static void IsUnFollowUser(int userID)
    {
        if (m_userFollowerList != null) {
            if (m_userFollowerList.Any (x => x.UserId == userID)) {
                m_userFollowerList.Find (x => x.UserId == userID).IsFollow = false;
            }
        }

        if (m_userFollowList.Any (x => x.UserId == userID)) {
            int index = m_userFollowList.FindIndex (x => x.UserId == userID);
            m_userFollowList.RemoveAt (index);
        }
    }

    public static void IsRemoveFollower(int userID)
    {
        if (m_userFollowerList.Any (x => x.UserId == userID)) {
            int index = m_userFollowerList.FindIndex (x => x.UserId == userID);
            m_userFollowerList.RemoveAt (index);
            AwsModule.UserData.UserData.FollowerCount = m_userFollowerList.Count;
        }

        if (m_userFollowList != null && m_userFollowList.Any (x => x.UserId == userID)) {
            int index = m_userFollowList.FindIndex (x => x.UserId == userID);
            m_userFollowList[index].IsFollower = false;
        }
    }

    /// <summary>
    /// データソート
    /// </summary>
    /// <param name="menuEnum">メニューのEnum</param>
    public static void DataSort(OptionMenuEnum menuEnum)
    {
        if (menuEnum == OptionMenuEnum.Forllow) {
            if (m_userFollowList != null && m_userFollowList.Count > 0) {
                SendFollow = true;
                m_userFollowList.Sort (Sort);
            }
        } else if (menuEnum == OptionMenuEnum.Forllower){
            if (m_userFollowerList != null && m_userFollowerList.Count > 0) {
                SendFollow = false;
                m_userFollowerList.Sort (Sort);
            }
        }
    }

    private static int Sort(SmileLab.Net.API.FriendData x, SmileLab.Net.API.FriendData y)
    {
        bool isDescend = AwsModule.LocalData.OptionSupportSortData.IsDescending;
        int res = 0;

        switch (AwsModule.LocalData.OptionSupportSortData.SortType) {
        case 0:
            // ログイン
            res = DateTime.Parse(y.LastLoginDate).CompareTo(DateTime.Parse(x.LastLoginDate));
            break;
        case 1:
            // ランク
            res = x.Level - y.Level;
            break;
        case 2:
            // フォロー / フォロワー
            if (SendFollow) {
                if (!(string.IsNullOrEmpty (x.FollowDate) && string.IsNullOrEmpty (y.FollowDate))) {
                    res = DateTime.Parse (x.FollowDate).CompareTo (DateTime.Parse (y.FollowDate));
                }
            } else {
                if (!(string.IsNullOrEmpty (x.FollowerDate) && string.IsNullOrEmpty (y.FollowerDate))) {
                    res = DateTime.Parse (x.FollowerDate).CompareTo (DateTime.Parse (y.FollowerDate));
                }
            }
            break;
        }

        if (res == 0) {
            res = DateTime.Parse(y.LastLoginDate).CompareTo(DateTime.Parse(x.LastLoginDate));
        }

        return isDescend ? res * -1 : res;
    }

    public static void DataClear()
    {
        if (m_userFollowList != null) {
            m_userFollowList.Clear ();
            m_userFollowList = null;
        }
        if (m_userFollowerList != null) {
            m_userFollowerList.Clear ();
            m_userFollowerList = null;
        }
        if (m_cacheUserDataList != null) {
            m_cacheUserDataList.Clear ();
            m_cacheUserDataList = null;
        }
    }

    /// メニュー表示.
    protected override void ChangeView(OptionMenu menu)
    {
        base.ChangeView(menu);
        CreateSupportView();
    }

    // サポート > サポートリスト View生成.
    private void CreateSupportView()
    {
        this.SwitchViewMode(ViewMode.Friend);
        var c = this.gameObject.AddComponent<MiniView_OptionSupport>();
        c.Init(m_currentMenu);
        m_currentMiniView = c;
    }
}
