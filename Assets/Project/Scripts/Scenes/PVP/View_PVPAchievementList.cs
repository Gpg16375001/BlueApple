using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SmileLab;

public class View_PVPAchievementList : PopupViewBase
{
    public static View_PVPAchievementList Create(int contestId)
    {
        var go = GameObjectEx.LoadAndCreateObject("PVP/View_PVPAchievementList");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_PVPAchievementList>();
        c.InitInternal(contestId);
        return c;
    }


    private void InitInternal(int contestId)
    {
        var leagueId = MasterDataTable.pvp_contest [contestId].league_table_id;
        var leagueRewards = MasterDataTable.pvp_league [leagueId];

        var achievementGrid = GetScript<RectTransform> ("AchievementGrid").gameObject;
        foreach (var leagueReward in leagueRewards.Where(x => x.item_type.HasValue).OrderBy(x => x.winning_point)) {
            var go = GameObjectEx.LoadAndCreateObject ("PVP/ListItem_PVPAchievement", achievementGrid);
            var behaviour = go.GetOrAddComponent<ListItem_PVPAchievement> ();
            behaviour.Init (leagueReward);
        }
        SetCanvasCustomButtonMsg ("bt_Close", TapClose);
        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        TapClose();
    }

    void TapClose()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
    }

}
