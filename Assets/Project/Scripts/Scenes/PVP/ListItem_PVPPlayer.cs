using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_PVPPlayer : ViewBase {

    public bool Selected {
        get {
            return m_Selected;
        }
        set {
            if (m_Selected != value) {
                if (m_BtPVPPlayerInfoFrame != null) {
                    m_BtPVPPlayerInfoFrame.ForceHighlight = value;
                }
                m_Selected = value;
            }
        }
    }
    public void Init(PvpTeamData teamData, Action<ListItem_PVPPlayer, PvpTeamData> tapFrame)
    {
        m_TeamData = teamData;
        m_TapFrameCallback = tapFrame;

        var infoGo = GetScript<RectTransform> ("Info").gameObject;
        var noPlayerGo = GetScript<RectTransform> ("NoPlayer").gameObject;
        if (m_TeamData == null) {
            infoGo.SetActive (false);
            noPlayerGo.SetActive (true);
            return;
        }
        infoGo.SetActive (true);
        noPlayerGo.SetActive (false);
        // 総合値
        GetScript<TextMeshProUGUI>("txtp_OwnTotalPoint").SetText(teamData.TotalOverallIndex);

        Debug.Log (teamData.Nickname);
        // Player名
        GetScript<Text>("txt_PlayerName").text = teamData.Nickname;

        // ユニット画像表示
        LoadPartyImage(teamData.MainCardData);

        // Grade設定
        SetGrade(teamData.RivalStrength, teamData.GainWinningPoint);

        m_BtPVPPlayerInfoFrame = GetScript<CustomButton> ("bt_PVPPlayerInfoFrame");
        if (!m_IsInit) {
            SetCanvasCustomButtonMsg ("OtherPlayerDetails/bt_CommonS01", DidTapDetail);
            SetCanvasCustomButtonMsg ("bt_PVPPlayerInfoFrame", DidTapFrame);
        }
        m_IsInit = true;
    }

    void SetGrade(int rivalStrength, int GainWinningPoint)
    {
        var goPvpLower = GetScript<RectTransform> ("img_PVPLowerPlayer").gameObject;
        var goPvpSame = GetScript<RectTransform> ("img_PVPSamePlayer").gameObject;
        var goPvpUpper = GetScript<RectTransform> ("img_PVPUpperPlayer").gameObject;

        goPvpLower.SetActive (rivalStrength == 0);
        goPvpSame.SetActive (rivalStrength == 1);
        goPvpUpper.SetActive (rivalStrength == 2);

        var txtpPvpLower = GetScript<TextMeshProUGUI> ("txtp_GetWinPointLower");
        var txtpPvpSame = GetScript<TextMeshProUGUI> ("txtp_GetWinPointSame");
        var txtpPvpUpper = GetScript<TextMeshProUGUI> ("txtp_GetWinPointUpper");
        txtpPvpLower.gameObject.SetActive (rivalStrength == 0);
        txtpPvpSame.gameObject.SetActive (rivalStrength == 1);
        txtpPvpUpper.gameObject.SetActive (rivalStrength == 2);

        txtpPvpLower.SetText (GainWinningPoint);
        txtpPvpSame.SetText (GainWinningPoint);
        txtpPvpUpper.SetText (GainWinningPoint);
    }

    private void LoadPartyImage (CardData card)
    {
        var partyCardImage = this.GetScript<Image> ("PartyEditUnitCard");
        partyCardImage.overrideSprite = null;

        UnitResourceLoader loader = new UnitResourceLoader (card);
        loader.LoadFlagReset ();
        loader.IsLoadPartyCardImage = true;

        loader.LoadResource ((resource) => {
            partyCardImage.overrideSprite = resource.GetPartyCardImage(card.Rarity);
        });
    }

    // 詳細押下
    void DidTapDetail()
    {
        View_PVPSelectPlayerDetailsPop.Create (m_TeamData);
    }

    // Frame押下
    void DidTapFrame()
    {
        if (m_TapFrameCallback != null) {
            m_TapFrameCallback (this, m_TeamData);
        }
    }

    private PvpTeamData m_TeamData;
    private Action<ListItem_PVPPlayer, PvpTeamData> m_TapFrameCallback;
    private bool m_Selected;
    private CustomButton m_BtPVPPlayerInfoFrame;

    private bool m_IsInit = false;
}
