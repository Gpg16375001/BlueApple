using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class DailyQuestSController : ScreenControllerBase {
    public int Type { get; set; }
    private DailyQuestAchievement[] DailyQuestAchievementList;

    public override void Init (System.Action<bool> didConnectEnd)
    {
        SendAPI.QuestsGetDailyAchievement (
            (success, response) => {
                if(!success) {
                    didConnectEnd(false);
                    return;
                }
                DailyQuestAchievementList = response.DailyQuestAchievementList;
                didConnectEnd(true);
            }
        );
    }

    public override void CreateBootScreen ()
    {
        // シナリオのパラメータの指定
        ScenarioProvider.CurrentScenarioState = ScenarioProgressState.PrevBattle;
        ScenarioProvider.CurrentSituation = ScenarioSituation.Other;

        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm009, true);
        if (Type == 2) {
            var go = GameObjectEx.LoadAndCreateObject("DailyQuest/Screen_WeeklyQuestEvolution", gameObject);
            go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
            var behaviour = go.GetOrAddComponent<Screen_WeeklyQuestEvolution> ();
            behaviour.Init (DailyQuestAchievementList);
        } else {
            var go = GameObjectEx.LoadAndCreateObject("DailyQuest/Screen_WeeklyQuestEnhance", gameObject);
            go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
            var behaviour = go.GetOrAddComponent<Screen_WeeklyQuestEnhance> ();
            behaviour.Init (DailyQuestAchievementList);
        }
    }
}
