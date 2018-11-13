using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_PopupInheriting : PopupViewBase {

    private static View_PopupInheriting instance;
    public static View_PopupInheriting Create()
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Title/View_PopupInheriting");
        instance = go.GetOrAddComponent<View_PopupInheriting>();
        instance.InitInternal();
        return instance;
    }


    private void InitInternal()
    {
        SetCanvasButtonMsg ("Takeover/bt_Common", TapTakeover);
        SetCanvasButtonMsg ("Inherting/bt_Common", TapInherting);
        if (AwsModule.ProgressData.TutorialStageNum >= 0) {
            // チュートリアル中のユーザーは連携できない。
            GetScript<Button>("Inherting/bt_Common").interactable = false;
        }
        SetCanvasButtonMsg ("bt_Close", TapClose);

        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        TapClose ();
    }

    void TapTakeover()
    {
        if (IsClosed) {
            return;
        }

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.TakeoverGetFgidLoginInfo (
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(success) {
                    takeover = true;
                    m_GameToken = response.GameToken;
#if UNITY_IOS && !UNITY_EDITOR
                    SafariView.Init (gameObject);
                    SafariView.LaunchURL (response.LoginUrl);
                    StartCoroutine ("WaitLunchCustomURL");
#else
                    Application.OpenURL(response.LoginUrl);
#endif
                }
            }
        );
    }

    void TapInherting()
    {
        if (IsClosed) {
            return;
        }

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.FgidGetLoginInfo (
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(success) {
                    if(response.IsAssociated) {
                        PopupManager.OpenPopupSystemOK(
                            string.Format("名前：{0}\nRank：{1}\nお客様番号：{2}\nこちらで連携済みです。",
                                response.Nickname,
                                MasterDataTable.user_level.GetLevel(response.Exp),
                                response.CustomerId
                            )
                        );
                    } else {
                        takeover = false;
                        m_GameToken = response.GameToken;
#if UNITY_IOS && !UNITY_EDITOR
                        SafariView.Init (gameObject);
                        SafariView.LaunchURL (response.LoginUrl);
                        StartCoroutine ("WaitLunchCustomURL");
#else
                        Application.OpenURL(response.LoginUrl);
#endif
                    }
                }
            }
        );
    }

    void TapClose()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => {
            Dispose();
        });
    }

    IEnumerator WaitLunchCustomURL()
    {
        var waitFor05S = new WaitForSeconds (0.5f);
        var uriInfo = GetURLSchemePlugin.GetUriInfo ();
        while (!uriInfo.isUri) {
            yield return waitFor05S;
            uriInfo = GetURLSchemePlugin.GetUriInfo ();
        }
        SafariView.Close ();
        ProcTakeoverOrResume (uriInfo);
    }

    void DidFinish()
    {
        StopCoroutine ("WaitLunchCustomURL");
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause) {
            var uriInfo = GetURLSchemePlugin.GetUriInfo ();
            ProcTakeoverOrResume (uriInfo);
        }
    }

    void ProcTakeoverOrResume(UriInfo uriInfo)
    {
        if (!uriInfo.isUri) {
            Debug.LogError ("not url launch");
            return;
        }

        if (!takeover && uriInfo.hostName != "associate") {
            Debug.LogErrorFormat ("Mode Associate Not Host associate : uri host = {0}", uriInfo.hostName);
            return;
        }
        if (takeover && uriInfo.hostName != "takeover") {
            Debug.LogErrorFormat ("Mode Takeover Not Host takeover : uri host = {0}", uriInfo.hostName);
            return;
        }

        var token = uriInfo.GetParameter ("g");
        // tokenのMD5を比較する
        if (m_GameToken.ToHashMD5() != token) {
            Debug.LogErrorFormat ("Not Equle Tokes api token = {0} : uri token = {1}", m_GameToken, token);
            return;
        }

        // 引き継ぎ処理とかする。
        if (takeover) {
            ProcTakeOver ();
        } else {
            ProcLogin ();
        }
    }


    void ProcTakeOver()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        // ユーザー確認用
        SendAPI.TakeoverConfirmUser (m_GameToken,
            (success, response) => {
                if(success) {
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    LockInputManager.SharedInstance.IsLock = false;
                    View_PopupInheritingOK.Create(
                        response.Nickname,
                        MasterDataTable.user_level.GetLevel(response.Exp).ToString(),
                        response.CustomerId,
                        () => {
                            ProcResumeUser();
                        }, 
                        () => {
                        }
                    );
                } else {
                    LockInputManager.SharedInstance.IsLock = false;
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                }
            }
        );
    }

    void ProcResumeUser()
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;

        SendAPI.TakeoverResumeUser(m_GameToken, 
            (success, response) => {
                if(success) {
					AwsModule.UserData.SetAuthUserInfo(response.AuthUsername, response.AuthPassword);            
                    AwsModule.Request.RequestUserAuth(
                        (ret) => {
                            AwsModule.ResetDataSet( () => {
                                View_FadePanel.SharedInstance.IsLightLoading = false;
                                LockInputManager.SharedInstance.IsLock = false;
                                TapClose();

                                PopupManager.OpenPopupOK("引き継ぎ完了", () => {
                                    ScreenChanger.SharedInstance.Reboot();
                                });
                            });
                        }
                    );


                } else {
                    LockInputManager.SharedInstance.IsLock = false;
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                }
            }
        );
    }

    void ProcLogin()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.FgidAssociateUser (m_GameToken,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(success) {
                    PlayOpenCloseAnimation(false, () => {
                        Dispose();
                        PopupManager.OpenPopupOK("連携完了");
                    });
                }
            }
        );
    }

    string m_GameToken;
    bool takeover = false;
}
