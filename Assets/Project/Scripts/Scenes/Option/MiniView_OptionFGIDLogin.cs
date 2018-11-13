using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class MiniView_OptionFGIDLogin : OptionMiniViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
    {

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;

        GetScript<RectTransform> ("Associated").gameObject.SetActive (false);
        GetScript<RectTransform> ("Login").gameObject.SetActive (false);

        SendAPI.FgidGetLoginInfo (
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(success) {
                    m_GameToken = response.GameToken;
                    m_LoginUrl = response.LoginUrl;
                    if(response.IsAssociated) {
                        InitAssociated();
                    } else {
                        InitLogin();
                    }
                } else {
                }
            }
        );
    }

    void InitAssociated()
    {
        GetScript<RectTransform> ("Associated").gameObject.SetActive (true);
        SetCanvasButtonMsg ("AssociatedLogin/bt_Common", DidTapLogin);
    }

    void InitLogin()
    {
        GetScript<RectTransform> ("Login").gameObject.SetActive (true);
        SetCanvasButtonMsg ("FGLogin/bt_Common", DidTapLogin);
    }

    void DidTapLogin()
    {
#if UNITY_IOS && !UNITY_EDITOR
        SafariView.Init (gameObject);
        SafariView.LaunchURL (m_LoginUrl);
        StartCoroutine ("WaitLunchCustomURL");
#else
        Application.OpenURL(m_LoginUrl);
#endif
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
        ProcResume (uriInfo);
    }

    void DidFinish()
    {
        StopCoroutine ("WaitLunchCustomURL");
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause) {
            var uriInfo = GetURLSchemePlugin.GetUriInfo ();
            ProcResume (uriInfo);
        }
    }

    void ProcResume(UriInfo uriInfo)
    {
        if (!uriInfo.isUri) {
            Debug.LogError ("not url launch");
            return;
        }
        if (uriInfo.hostName != "associate") {
            Debug.LogErrorFormat ("Mode Associate Not Host associate : uri host = {0}", uriInfo.hostName);
            return;
        }
        var token = uriInfo.GetParameter ("g");
        if (m_GameToken.ToHashMD5() != token) {
            Debug.LogErrorFormat ("Not Equle Tokes api token = {0} : uri token = {1}", m_GameToken, token);
            return;
        }

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.FgidAssociateUser (m_GameToken,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(success) {
                    PopupManager.OpenPopupOK("連携完了");
                }
            }
        );
    }

    string m_GameToken;
    string m_LoginUrl;
}
