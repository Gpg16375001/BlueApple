using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// ScreenController : マイページ
/// </summary>
public class MyPageSController : ScreenControllerBase 
{
	public override void Dispose()
	{
		if(m_screen != null){
			m_screen.Dispose();
		}
		base.Dispose();
	}

	/// <summary>
	/// 初期化.
	/// </summary>
	public override void Init(System.Action<bool> didConnectEnd)
    {
        ScenarioProvider.CurrentSituation = ScenarioSituation.Other;
        ScenarioProvider.CurrentScenarioState = ScenarioProgressState.Other;
        AwsModule.ProgressData.CurrentScenarioSelectIdList.Clear();
        
        loginbonusDatas = null;

		// 受け取り可能なログインボーナスがある。(チュートリアル中はやらない)
		if(AwsModule.ProgressData.TutorialStageNum < 0){
			if (AwsModule.UserData.UserData.ReceivableLoginbonusIdList != null && AwsModule.UserData.UserData.ReceivableLoginbonusIdList.Length > 0) {
				LoginBonusReceive(didConnectEnd);
                return;
            }
		}
		this.OwnUserGetReceive(didConnectEnd);
    }
    private void LoginBonusReceive(System.Action<bool> didConnectEnd)
    {
        var localData = AwsModule.UserData;
        SendAPI.LoginbonusReceiveItem(localData.UserData.ReceivableLoginbonusIdList,
            (succeus, loginbonus) => {
			    if(AwsModule.ProgressData.TutorialStageNum < 0 && succeus && loginbonus.LoginbonusDataList.Length > 0) {
                    loginbonusDatas = loginbonus.LoginbonusDataList;
                } else {
                    loginbonusDatas = null;
                }
				this.OwnUserGetReceive(didConnectEnd);
            }
        );
        return;
    }
	// ミッション達成確認などの必要に応じて自身のユーザーデータは都度取り直す
	private void OwnUserGetReceive(System.Action<bool> didConnectEnd)
	{
		var localData = AwsModule.UserData;
		SendAPI.UsersGetUserData(new int[] { localData.UserData.UserId }, (bSuccess, res) => {
            if(bSuccess) {
    			var own = res.UserDataList.First(u => u.UserId == AwsModule.UserData.UserData.UserId);
    			AwsModule.UserData.UserData = own;
    			//localData.Sync((bSuccsess, sender, e) => didConnectEnd(bSuccsess));
            }

            if(AwsModule.UserData.MainCard != null) {
                didConnectEnd(bSuccess);
            } else {
                // レアリティが高いものをセット
                AwsModule.UserData.MainCardID = CardData.CacheGetAll().OrderBy(x => x.Rarity).Last().CardId;
                // メインカードを必ず一枚サポートカードとして設定する。
                AwsModule.UserData.SetSupportCardList(AwsModule.UserData.MainCard.Card.element.Enum, AwsModule.UserData.MainCard);
                AwsModule.PartyData.CurrentTeam.Init();
                AwsModule.UserData.Sync((bSuccess1, sender1, e1) => { 
                    AwsModule.PartyData.Sync((bSuccess2, sender2, e2) => { 
                        didConnectEnd(bSuccess);
                    });
                });
            }
        });
	}

    /// <summary>
    /// 起動画面の生成.
    /// </summary>
    public override void CreateBootScreen()
    {
        // BGM再生テスト.
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm009, true);

        // グローバルメニュー作成
		View_GlobalMenu.CreateIfMissing().IsEnableButton = true;
        var playerMenu = View_PlayerMenu.CreateIfMissing();
        playerMenu.UpdateView (AwsModule.UserData.UserData);
		playerMenu.IsEnableButton = true;      

        var go = GameObjectEx.LoadAndCreateObject("MyPage/Screen_MyPage", this.gameObject);
		m_screen = go.GetOrAddComponent<Screen_MyPage>();
		m_screen.Init(loginbonusDatas);
    }

    /// <summary>
    /// グローバルメニューより上に表示したいものなど表示レイヤーを変えたい場合ようにここでルートを作成.
    /// 自身の子階層のViewが呼ぶ想定.Delegateのネストが深くなるのが嫌なので横着してstaticにしてる.
    /// </summary>
	public static GameObject CreateRootObjects(Transform firstChild, int sortOrder, string rootName = null)
    {
        if(string.IsNullOrEmpty(rootName)) {
            rootName = firstChild.gameObject.GetInstanceID().ToString();
        }
        var go = new GameObject("Root_" + rootName);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        canvas.sortingOrder = sortOrder;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1136f, 640f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
        go.AddComponent<GraphicRaycaster>();
        var sc = firstChild.localScale;
        firstChild.SetParent(go.transform, false);
        firstChild.localScale = sc;
		return go;
    }

    private LoginbonusData[] loginbonusDatas;
	private Screen_MyPage m_screen;
}
