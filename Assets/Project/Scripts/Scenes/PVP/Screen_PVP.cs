using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class Screen_PVP : ViewBase {
    enum ScreenMode {
        Main = 0,
        PlayerSelect,
    }
    public void Init(ReceivePvpGetOpponentList apiResponse, bool isPlayerSelect)
    {
        AwsModule.UserData.UserData = apiResponse.UserData;
        m_PvpUserData = apiResponse.PvpUserData;
        m_UpdateStartupTime = Time.realtimeSinceStartup;

        SoundManager.SharedInstance.PlayBGM (SoundClipName.bgm043, true);

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidBackButton;

        var viewMain = GameObjectEx.LoadAndCreateObject("PVP/View_PVPMain", this.gameObject);
        m_ViewMain = viewMain.GetOrAddComponent<View_PVPMain> ();

        m_ViewMain.Init (apiResponse.PvpUserData, DidTapPlayerSelect, OpenBpRecovery);
        m_IsListEmpty = apiResponse.OpponentPvpTeamDataList.Length <= 0;

        var viewSelect = GameObjectEx.LoadAndCreateObject("PVP/View_PVPSelectPlayer", this.gameObject);
        m_ViewSelectPlayer = viewSelect.GetOrAddComponent<View_PVPSelectPlayer> ();
        m_ViewSelectPlayer.Init (apiResponse.PvpUserData, apiResponse.PvpTeamData, apiResponse.OpponentPvpTeamDataList, OpenBpRecovery);

        if (isPlayerSelect && !AwsModule.PartyData.PvPTeam.IsEmpty) {
            m_Mode = ScreenMode.PlayerSelect;
            viewSelect.SetActive (true);
            viewMain.SetActive (false);
            m_ViewSelectPlayer.LoadBG (() => View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black));
        } else {
            m_Mode = ScreenMode.Main;
            viewMain.SetActive (true);
            viewSelect.SetActive (false);
            m_ViewMain.LoadBG (() => View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black));
        }
    }

    void UpdatePvpUser(PvpUserData pvpUserData)
    {
        m_PvpUserData = pvpUserData;
        m_UpdateStartupTime = Time.realtimeSinceStartup;
        m_ViewMain.UpdatePvpUser (pvpUserData);
        m_ViewSelectPlayer.UpdatePvpUser (pvpUserData);
    }

    void OpenBpRecovery()
    {
        View_BPRecovery.Create (UpdatePvpUser);
    }

    void DidBackButton()
    {
        if (m_Mode == ScreenMode.PlayerSelect) {
            // MainViewへ
            m_ViewMain.gameObject.SetActive (true);
            m_ViewSelectPlayer.gameObject.SetActive (false);
            m_Mode = ScreenMode.Main;
        } else {
            // Mypageへ
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage ());
        }
    }

    void DidTapPlayerSelect ()
    {
        if (m_IsListEmpty) {
            PopupManager.OpenPopupOK ("対戦相手が見つかりませんでした。\n再度、時間をおいてお試しください。", () => {
                // MainViewへ
                m_ViewMain.gameObject.SetActive (true);
                m_ViewSelectPlayer.gameObject.SetActive (false);
                m_Mode = ScreenMode.Main;
            });
        }
        m_Mode = ScreenMode.PlayerSelect;
        m_ViewMain.gameObject.SetActive (false);
        m_ViewSelectPlayer.gameObject.SetActive (true);
    }

    ScreenMode m_Mode;
    bool m_IsListEmpty;
    View_PVPMain m_ViewMain;
    View_PVPSelectPlayer m_ViewSelectPlayer;

    PvpUserData m_PvpUserData;
    float m_UpdateStartupTime;
}
