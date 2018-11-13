using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;
public class View_PVPStart : ViewBase {

    public void Init()
    {
        var pvpEnrtyData = AwsModule.BattleData.PVPBattleEntryData;

        // 自分
        var unitIocnRRoot = GetScript<RectTransform>("UnitIconR").gameObject;
        var unitIconR = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", unitIocnRRoot).GetOrAddComponent<ListItem_UnitIcon>();
        unitIconR.Init(AwsModule.UserData.MainCard);
        GetScript<Text> ("txt_PlayerNameR").text = pvpEnrtyData.PvpTeamData.Nickname;
        GetScript<TextMeshProUGUI> ("txtp_PVPWinNumR").SetText(pvpEnrtyData.PvpTeamData.WinCount);
        GetScript<TextMeshProUGUI> ("txtp_PVPLoseNumR").SetText(pvpEnrtyData.PvpTeamData.LoseCount);
        GetScript<TextMeshProUGUI> ("txtp_PVPWinPointNumR").SetText(pvpEnrtyData.PvpTeamData.TotalOverallIndex);

        // 相手
        var unitIocnLRoot = GetScript<RectTransform>("UnitIconL").gameObject;
        var unitIconL = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", unitIocnLRoot).GetOrAddComponent<ListItem_UnitIcon>();
        unitIconL.Init(pvpEnrtyData.OpponentPvpTeamData.MainCardData);
        GetScript<Text> ("txt_PlayerNameL").text = pvpEnrtyData.OpponentPvpTeamData.Nickname;
        GetScript<TextMeshProUGUI> ("txtp_PVPWinNumL").SetText(pvpEnrtyData.OpponentPvpTeamData.WinCount);
        GetScript<TextMeshProUGUI> ("txtp_PVPLoseNumL").SetText(pvpEnrtyData.OpponentPvpTeamData.LoseCount);
        GetScript<TextMeshProUGUI> ("txtp_PVPWinPointNumL").SetText(pvpEnrtyData.OpponentPvpTeamData.TotalOverallIndex);
    }
}
