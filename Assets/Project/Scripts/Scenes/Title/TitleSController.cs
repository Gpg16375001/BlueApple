using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

using com.adjust.sdk;

/// <summary>
/// ScreenController : タイトル
/// </summary>
public class TitleSController : ScreenControllerBase
{
    public override void Init(Action<bool> didConnectEnd)
    {
        // TODO : ユーザー登録や認証をとりあえずタイトル前のここでざっとやってる.
        // タイトル前画面でやるようであればそちらに動かすなり、ログインロジックだけ浮かせるなり考える必要があり.
        this.RequestUserLogin(didConnectEnd);
    }

    //>通信リクエスト：ユーザーログイン.必要であれば新規登録.
    private void RequestUserLogin(Action<bool> didConnectEnd)
    {
        if(AwsModule.UserData.IsExistAcount) {
            // ユーザー認証
			AwsModule.Request.RequestUserAuth(bSuccess => {
				// チュートリアル中であればブート段階で必要アセットのDLを行う.
				if (AwsModule.ProgressData.TutorialStageNum >= 0 && AwsModule.ProgressData.TutorialStageNum <= 2) {
                    TutorialResourceDownloader.RequestGacha();
                }
				didConnectEnd(bSuccess);
			});
        } else {
            // 新規登録
            SendAPI.RegisterIndex(
                (sucess, res) => {
                    if(!sucess || res == null) {
                        didConnectEnd(false);
                        return; // エラー.
                    }

                    AwsModule.ClearDataset ();

                    // インストールイベントの通知
                    AdjustEvent adjustEvent = new AdjustEvent("cmmzeh");
                    adjustEvent.addCallbackParameter("uid", res.UserId.ToString());
                    Adjust.trackEvent(adjustEvent);

                    AwsModule.UserData.UserID = (uint)res.UserId;
                    AwsModule.UserData.CustomerID = res.CustomerId;
                    AwsModule.UserData.SetAuthUserInfo(res.AuthUsername, res.AuthPassword);
                    AwsModule.Request.RequestUserAuth(
                        (ret) => {
                            AwsModule.UserData.Sync(
                                (success, sender, e) => {                           
                                    // ユーザー認証へ.
                                    didConnectEnd(ret);
                                }
                            );

							// チュートリアル中であればブート段階で必要アセットのDLを行う.
                            Debug.Log(AwsModule.ProgressData.TutorialStageNum);
    						if (AwsModule.ProgressData.TutorialStageNum >= 0 && AwsModule.ProgressData.TutorialStageNum <= 2) {
                                TutorialResourceDownloader.RequestGacha();
                            }
                        }
                    );

                }
            );
        }
    }

    /// <summary>
    /// 起動画面の生成.
    /// </summary>
    public override void CreateBootScreen()
    {
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm045, true);

        var go = GameObjectEx.LoadAndCreateObject("Title/Screen_Title", this.gameObject);
        var c = go.GetOrAddComponent<Screen_Title>();
        c.Init();
    }
}
