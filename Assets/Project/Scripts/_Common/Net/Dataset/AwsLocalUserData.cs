using System;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

using System.Linq;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// AWS SDK "Cognito"を利用したローカルに保存したいユーザーデータ.
/// </summary>
public class AwsLocalUserData : AwsCognitoDatasetBase
{
    /// <summary>ユーザーID.0ならまだ作られていない.内部処理用で外には基本的に見せない.</summary>
    public uint UserID
    {
        get { return Get<uint> ("UserID"); }
        set { Put("UserID", value); }
    }

    /// <summary>お問い合わせ用.対外用で外部露出させる値.</summary>
    public string CustomerID
    {
        get { { return Get<string>("CustomerID"); } }
        set { Put("CustomerID", value); }
    }

    /// <summary>ユーザー名.</summary>
    public string UserName
    {
        get { { return Get<string>("UserName"); } }
        set { Put("UserName", value); }
    }
    public string EditUserName {
        get;
        set;
    }

	public CardData MainCard
	{
		get { return CardData.CacheGet(MainCardID); }
	}
    /// <summary>お気に入りにしているカードのID.</summary>
	public int MainCardID
	{
		get { { return Get<int>("MainCardID"); } }
		set { { Put("MainCardID", value); }}
	}

    private bool _IsSupportCardListModfiy;
    public bool IsSupportCardListModfiy
    {
        get {
            return _IsSupportCardListModfiy;
        }
    }
    private int[] _SupportCardIDList;

    /// <summary>プロフィール表示の一言.</summary>
	public string ProfileMessage
	{
		get { { return Get<string>("ProfileMessage"); } }
		set { { Put("ProfileMessage", value); } }
	}
    public string EditProfileMessage {
        get;
        set;
    }

	private static readonly string AuthUserFileName = "authusernameandpass";
    private static string AuthUserInfoPath { get { return System.IO.Path.Combine(SmileLab.GameSystem.LocalSaveDirectoryPath, AuthUserFileName.ToHashMD5()); } }

    private static readonly string BirthInfoFileName = "birthinfo";
    public string BirthInfoPath {
        get {
            return System.IO.Path.Combine (SmileLab.GameSystem.LocalSaveDirectoryPath, BirthInfoFileName.ToHashMD5 ());
        }
    }   

    /// <summary>
    /// ユーザー生成済み？.
    /// </summary>
    public bool IsExistAcount 
	{ 
		get {
			return System.IO.File.Exists(AuthUserInfoPath);
		} 
	}

    public bool IsExistBirthInfo {
        get {
            return System.IO.File.Exists (BirthInfoPath);
        }
    }
    private UserData _UserData; 
    private float updateStartupFromTime;
    public event Action<UserData> UpdateUserData;
    public UserData UserData {
        get{
            return _UserData;
        }
        set{
            _UserData = value;
            updateStartupFromTime = Time.realtimeSinceStartup;
            if (UpdateUserData != null) {
                UpdateUserData (_UserData);
            }
        }
    }

    public float UpdateStartupFromTime
    {
        get {
            return updateStartupFromTime;
        }
    }

    public int ActionPointTimeToFull {
        get {
            if (_UserData != null) {
                return Mathf.Max(0, _UserData.ActionPointTimeToFull - (int)(Time.realtimeSinceStartup - updateStartupFromTime));
            }
            return 0;
        }
    }


    public int ActionPoint {
        get {
            if (_UserData != null) {
                if (_UserData.ActionPointTimeToFull > 0) {
                    int recoveryTime = MasterDataTable.userpoint_recovery_time [UserPointTypeEnum.AP].recovery_time;
                    var restTime = _UserData.ActionPointTimeToFull % recoveryTime;
                    var elasped = (int)(Time.realtimeSinceStartup - updateStartupFromTime);
                    if (restTime > elasped) {
                        return _UserData.ActionPoint;
                    }
                    int addAp = 1 + (elasped - restTime) / recoveryTime;

                    return Mathf.Min (MasterDataTable.user_level [_UserData.Level].ap_max, _UserData.ActionPoint + addAp);
                } 
                return _UserData.ActionPoint;
            }
            return 0;
        }
    }

    public int BattlePointTimeToFull {
        get {
            if (_UserData != null) {
                return Mathf.Max(0, _UserData.BattlePointTimeToFull - (int)(Time.realtimeSinceStartup - updateStartupFromTime));
            }
            return 0;
        }
    }

    public int BattlePoint {
        get {
            if (_UserData != null) {
                if (_UserData.BattlePointTimeToFull > 0) {
                    int recoveryTime = MasterDataTable.userpoint_recovery_time [UserPointTypeEnum.BP].recovery_time;
                    var restTime = _UserData.BattlePointTimeToFull % recoveryTime;
                    var elasped = (int)(Time.realtimeSinceStartup - updateStartupFromTime);
                    if (restTime > elasped) {
                        return _UserData.BattlePoint;
                    }
                    int addBp = 1 + (elasped - restTime) / recoveryTime;
                    return Mathf.Min (5, _UserData.BattlePoint + addBp);
                }
                return _UserData.BattlePoint;
            }
            return 0;
        }
    }

    public void SetAuthUserInfo(string name, string pass)
	{
		FileUtility.WriteToFileWith3DES(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", name, pass)), AuthUserInfoPath, true);
	}

    public void SetAge(int year, int month)
    {
        FileUtility.WriteToFileWith3DES(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", year, month)), AwsModule.UserData.BirthInfoPath, true);
    }

    public static void DeleteUserInfoFile()
	{
        FileUtility.Delete(AwsLocalUserData.AuthUserInfoPath);
	}
    public void DeleteAgeFile()
    {
        FileUtility.Delete (AwsModule.UserData.BirthInfoPath);
    }

    public string GetAuthUserName()
	{
        var data = FileUtility.ReadFromFileWith3DES(AwsLocalUserData.AuthUserInfoPath, true);
		var userString = System.Text.Encoding.UTF8.GetString(data);
		return userString.Split(':')[0];
	}
	public string GetAuthPass()
    {
        var data = FileUtility.ReadFromFileWith3DES(AwsLocalUserData.AuthUserInfoPath, true);
        var userString = System.Text.Encoding.UTF8.GetString(data);
        return userString.Split(':')[1];
    }
    public int GetAge()
    {
        int age = -1;
        var data = FileUtility.ReadFromFileWith3DES(AwsModule.UserData.BirthInfoPath, true);
        var birthString = System.Text.Encoding.UTF8.GetString (data);
        var birthSplits = birthString.Split (':');
        if (birthSplits.Length >= 2) {
            int year = 0;
            int month = 0;
            bool ret = true;
            ret &= int.TryParse (birthSplits [0], out year);
            ret &= int.TryParse (birthSplits [1], out month);
            if (ret && year >= 1900 && month >= 1 && month <= 12) {
                DateTime birthMonth = new DateTime (year, month, 1);
                age = GameTime.SharedInstance.Now.Year - birthMonth.Year;
                if (GameTime.SharedInstance.Now > birthMonth.AddYears (age)) {
                    age--;
                }
            }
        }
        return age;
    }

    public void SetSupportCardList(ElementEnum element, CardData card, bool isOverride=false)
    {
        int cardId = card != null ? card.CardId : 0;
        if (isOverride || this._SupportCardIDList [(int)element - 1] != cardId) {
            this._SupportCardIDList [(int)element - 1] = cardId;
            this._IsSupportCardListModfiy = true;
        } else {
            this._SupportCardIDList [(int)element - 1] = 0;
            this._IsSupportCardListModfiy = true;
        }
    }
    public CardData GetSupportCardList(ElementEnum element)
    {
        var cardID = this._SupportCardIDList [(int)element - 1];
        return CardData.CacheGet(cardID);
    }

    public void SupportUnitReset()
    {
        // データから取得して元に戻す
        var SupportCardIDListString = Get<string>("SupportCardIDList");
        if (!string.IsNullOrEmpty (SupportCardIDListString)) {
            // 解析
            this._SupportCardIDList = SupportCardIDListString.TrimStart ('[').TrimEnd (']').Split (',').Select (x => int.Parse (x)).ToArray ();
            this._IsSupportCardListModfiy = false;
        } else {
            this._SupportCardIDList = new int[6];
            for (int i = 0; i < 6; ++i) {
                this._SupportCardIDList[i] = 0;
            }
        }
        this._IsSupportCardListModfiy = false;
    }

    // コンストラクタ.
    public AwsLocalUserData(CognitoSyncManager mng) : base(mng, "UserData")
    {
        // サポートカードIDの取得
        var SupportCardIDListString = Get<string>("SupportCardIDList");
        if (!string.IsNullOrEmpty (SupportCardIDListString)) {
            // 解析
            this._SupportCardIDList = SupportCardIDListString.TrimStart ('[').TrimEnd (']').Split (',').Select (x => int.Parse (x)).ToArray ();
            this._IsSupportCardListModfiy = false;
        } else {
            this._SupportCardIDList = new int[6];
            for (int i = 0; i < 6; ++i) {
                this._SupportCardIDList[i] = 0;
            }
            this._IsSupportCardListModfiy = true;
        }      

        EditUserName = null;
        EditProfileMessage = null;
    }

    // 全値のリセット.
    protected override void ClearValues()
    {
        this.UserID = 0;
        this.CustomerID = string.Empty;
		this.UserName = string.Empty;
		this.MainCardID = 0;
		this.ProfileMessage = "よろしくお願いします。";
        this._SupportCardIDList = new int[6];
        // サポートカードIDの取得
        for (int i = 0; i < 6; ++i) {
            this._SupportCardIDList[i] = 0;
        }
        this._IsSupportCardListModfiy = true;
    }

    public override void Sync (DidSyncDelegate didCallback)
    {
        // サポートカードIDの保存
        if (_IsSupportCardListModfiy) {
            Put ("SupportCardIDList", string.Format ("[{0}]", string.Join (",", this._SupportCardIDList.Select (x => x.ToString ()).ToArray ())));
            this._IsSupportCardListModfiy = false;
        }
        base.Sync (didCallback);
    }

    public void OriginalSync (DidSyncDelegate didCallback)
    {
        base.Sync (didCallback);
    }
}
