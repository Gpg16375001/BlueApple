using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;


/// <summary>
/// AWS SDK "Cognito"を利用したローカルに保存したいデータ.
/// </summary>
public class AwsLocalSaveData : AwsCognitoDatasetBase
{
	/// <summary>AP回復時の通知が有効かどうか.</summary>
    public bool IsNotificateAP
    {
		get { return Get<bool>("IsNotificateAP"); }
	    set { Put("IsNotificateAP", value); }
    }
	/// <summary>BP回復時の通知が有効かどうか.</summary>
    public bool IsNotificateBP
    {
        get { return Get<bool>("IsNotificateBP"); }
        set { Put("IsNotificateBP", value); }
    }

    /// <summary>BGM音量.</summary>
    public float Volume_BGM
    {
        get { return Get<float>("Volume_BGM"); }
        set { Put("Volume_BGM", value); }
    }
    /// <summary>SE音量.</summary>
    public float Volume_SE
    {
        get { return Get<float>("Volume_SE"); }
        set { Put("Volume_SE", value); }
    }
    /// <summary>ボイス音量.</summary>
    public float Volume_Voice
    {
        get { return Get<float>("Volume_Voice"); }
        set { Put("Volume_Voice", value); }
    }
	/// <summary>シナリオオート</summary>
	public bool Scenario_Auto
	{
		get { return Get<bool> ("Scenario_Auto"); }
		set { Put ("Scenario_Auto", value); }
	}

    // コンストラクタ.
    public AwsLocalSaveData(CognitoSyncManager mng) : base(mng, "PlayerData") {}

    // 全値のリセット.
    protected override void ClearValues()
    {
		this.IsNotificateAP = true;
		this.IsNotificateBP = true;
        this.Volume_BGM = 1f;
        this.Volume_SE = 1f;
        this.Volume_Voice = 1f;
		this.Scenario_Auto = false;
    }
}
