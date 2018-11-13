using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_PVPSelectPlayerDetailsPop : PopupViewBase {

    private static View_PVPSelectPlayerDetailsPop instance;
    public static View_PVPSelectPlayerDetailsPop Create(PvpTeamData teamData)
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("PVP/View_PVPSelectPlayerDetailsPop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        instance = go.GetOrAddComponent<View_PVPSelectPlayerDetailsPop>();
        instance.InitInternal(teamData);
        return instance;
    }

    private void InitInternal(PvpTeamData teamData)
    {
        GetScript<Text> ("txt_PlayerName").text = teamData.Nickname;

        GetScript<TextMeshProUGUI> ("txtp_OwnTotalPoint").SetText(teamData.TotalOverallIndex);

        var txtpPvpLower = GetScript<TextMeshProUGUI> ("txtp_GetWinPointLower");
        var txtpPvpSame = GetScript<TextMeshProUGUI> ("txtp_GetWinPointSame");
        var txtpPvpUpper = GetScript<TextMeshProUGUI> ("txtp_GetWinPointUpper");
        txtpPvpLower.gameObject.SetActive (teamData.RivalStrength == 0);
        txtpPvpSame.gameObject.SetActive (teamData.RivalStrength == 1);
        txtpPvpUpper.gameObject.SetActive (teamData.RivalStrength == 2);
        txtpPvpLower.SetText (teamData.GainWinningPoint);
        txtpPvpSame.SetText (teamData.GainWinningPoint);
        txtpPvpUpper.SetText (teamData.GainWinningPoint);

        GetScript<TextMeshProUGUI> ("txtp_ActiveFormationLv").SetText(teamData.FormationLevel);
        GetScript<TextMeshProUGUI> ("txtp_ActiveFormationName").SetText(teamData.Formation.name);
        InitFormation (teamData);

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    private void InitFormation(PvpTeamData teamData)
    {
        var formation = teamData.Formation;
        for (int i = 0; i < 9; ++i) {
            PvpCardData card = null;
            int index = formation.GetPositionIndex(i / 3, i % 3);
            if (index - 1 < teamData.MemberPvpCardDataList.Length) {
                card = teamData.MemberPvpCardDataList [index - 1];
            }
            GetScript<RectTransform> (string.Format("ListItem_PVPFormationSquare{0}", i + 1)).gameObject.GetOrAddComponent<ListItem_PVPFormationSquare> ().Init (formation, index, card);
        }
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => {
            this.Dispose();
        });
    }
}
