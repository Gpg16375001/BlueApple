using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// static class : シナリオ情報提供クラス.システムが保存している進捗状況などを加味して提供する.
/// </summary>
public static class ScenarioProvider
{
    /// <summary>
    /// シチュエーション.
    /// </summary>
    public static ScenarioSituation CurrentSituation { get; set; }
    
    /// <summary>
    /// シナリオ進捗状況.
    /// </summary>
    public static ScenarioProgressState CurrentScenarioState { get; set; }


    /// <summary>
    /// 様々な状況を加味して現在ロードすべき宴プロジェクト名を算出する..
    /// </summary>
    public static string GetUtageProject()
    {
        // TODO : 現在の選択中がイベントクエストなどであればそちらから取得する処理を追加.
        switch (CurrentSituation) {
            case ScenarioSituation.MainQuest:
                return GetUtageProjectFromMainQuest();
            case ScenarioSituation.Event:
                return GetUtageProjectFromEventQuest ();
        }
        return null;
    }
    // メインクエスト情報から現在表示すべき宴プロジェクト名を取得.なければnull.
    private static string GetUtageProjectFromMainQuest()
    {
        if (AwsModule.ProgressData.CurrentQuest == null) { 
            return null;
        }
        var item = MasterDataTable.scenario_setting.DataList.Find(s => s.id == AwsModule.ProgressData.CurrentQuest.ID);
        if (item == null) {
            return null;
        }
        return item.adv_project_name;
    }
    private static string GetUtageProjectFromEventQuest()
    {
        if (AwsModule.ProgressData.CurrentQuest == null) { 
            return null;
        }
        var item = MasterDataTable.event_quest_stage_scenario_setting.DataList.Find(s => s.id == AwsModule.ProgressData.CurrentQuest.ID);
        if (item == null) {
            return null;
        }
        return item.adv_project_name;
    }

    /// <summary>
    /// 様々な状況を加味して現在ロードすべき宴の幕リストを算出する.
    /// </summary>
    public static List<string> GetActList()
    {
        // TODO : 現在の選択中がイベントクエストなどであればそちらから取得する処理を追加.
        var rtn = new List<string>();
        switch (CurrentSituation) {
            case ScenarioSituation.MainQuest:
                rtn = GetActListFromMainQuest();
                break;
            case ScenarioSituation.Event:
                rtn = GetActListFromEventQuest();
                break;
        }
        return rtn;
    }
    private static List<string> GetActListFromMainQuest()
    {
        var rtn = new List<string>();
        var progressData = AwsModule.ProgressData;
        if (progressData.CurrentQuest == null) {
            return rtn;
        }

        // 最新話の幕.
        var info = MasterDataTable.scenario_setting.DataList.Find(s => s.id == progressData.CurrentQuest.ID);
		// TODO : 同じadv_project_nameのものは全て読み込んでおく.最終的に未クリアのものは省いた方がいい.
		var allScenarios = MasterDataTable.scenario_setting.DataList.FindAll(s => s.adv_project_name == info.adv_project_name);
		foreach(var scenario in allScenarios){
			if (!string.IsNullOrEmpty(scenario.scenario_pre_battle)) {
				rtn.Add(scenario.scenario_pre_battle);
            }
			if (!string.IsNullOrEmpty(scenario.scenario_in_battle)) {
				rtn.Add(scenario.scenario_in_battle);
            }
			if (!string.IsNullOrEmpty(scenario.scenario_out_battle)) {
				rtn.Add(scenario.scenario_out_battle);
            }
			if (!string.IsNullOrEmpty(scenario.scenario_aft_battle)) {
				rtn.Add(scenario.scenario_aft_battle);
            }
		}
        return rtn;
    }

    private static List<string> GetActListFromEventQuest()
    {
        var rtn = new List<string>();
        var progressData = AwsModule.ProgressData;
        if (progressData.CurrentQuest == null) {
            return rtn;
        }

        // 最新話の幕.
        var info = MasterDataTable.event_quest_stage_scenario_setting.DataList.Find(s => s.id == progressData.CurrentQuest.ID);
        if (info == null) {
            return rtn;
        }
		if (!string.IsNullOrEmpty(info.scenario_pre_battle)) {
			rtn.Add(info.scenario_pre_battle);
        }
		if (!string.IsNullOrEmpty(info.scenario_in_battle)) {
			rtn.Add(info.scenario_in_battle);
        }
		if (!string.IsNullOrEmpty(info.scenario_out_battle)) {
			rtn.Add(info.scenario_out_battle);
        }
		if (!string.IsNullOrEmpty(info.scenario_aft_battle)) {
			rtn.Add(info.scenario_aft_battle);
        }
        return rtn;
    }

    /// <summary>
    /// 様々な状況を加味して現在再生すべきシナリオ名を返す.
    /// </summary>
    public static string GetScenario()
    {
        switch (CurrentSituation) {
            case ScenarioSituation.MainQuest:
                return GetMainQuestScenario();
            case ScenarioSituation.Event:
            return GetEventQuestScenario();
        }
        return null;
    }
    // メインクエストに付随するシナリオ取得.
    private static string GetMainQuestScenario()
    {
        var progressData = AwsModule.ProgressData;
        if(progressData.CurrentQuest == null){
            return null;
        }
        var info = MasterDataTable.scenario_setting.DataList.Find(s => s.id == progressData.CurrentQuest.ID);
        if(info == null){
            return null;
        }
        if(CurrentScenarioState == ScenarioProgressState.PrevBattle){
            return info.scenario_pre_battle;
        }
        if (CurrentScenarioState == ScenarioProgressState.InBattle) {
            return info.scenario_in_battle;
        }
		if (CurrentScenarioState == ScenarioProgressState.OutBattle) {
			return info.scenario_out_battle;
        }
        if (CurrentScenarioState == ScenarioProgressState.AfterBattle) {
            return info.scenario_aft_battle;
        }
        return null;
    }

    private static string GetEventQuestScenario()
    {
        var progressData = AwsModule.ProgressData;
        if(progressData.CurrentQuest == null){
            return null;
        }
        if (progressData.CurrentQuest.QuestType != 6) {
            return null;
        }

        var info = MasterDataTable.event_quest_stage_scenario_setting.DataList.Find(s => s.id == progressData.CurrentQuest.ID);
        if(info == null){
            return null;
        }
        if(CurrentScenarioState == ScenarioProgressState.PrevBattle){
            return info.scenario_pre_battle;
        }
        if (CurrentScenarioState == ScenarioProgressState.InBattle) {
            return info.scenario_in_battle;
        }
        if (CurrentScenarioState == ScenarioProgressState.OutBattle) {
            return info.scenario_out_battle;
        }
        if (CurrentScenarioState == ScenarioProgressState.AfterBattle) {
            return info.scenario_aft_battle;
        }
        return null;
    }

    /// <summary>
    /// シナリオ演出情報を取得.現状が該当しない場合はnull.
    /// </summary>
    public static ScenarioSetting GetScenarioEffectIfNeedEffect()
    {
        if(CurrentScenarioState != ScenarioProgressState.PrevBattle){
            return null;
        }
        // メインクエスト.
        if(CurrentSituation == ScenarioSituation.MainQuest){
            if(AwsModule.ProgressData.CurrentQuest == null){
                return null;
            }
            var info = MasterDataTable.scenario_setting[AwsModule.ProgressData.CurrentQuest.ID];
            if(info == null || info.eff_type == ScenarioEffectTypeEnum.None){
                return null;
            }
            if(AwsModule.ProgressData.SeenScenarioEffectIdList.Exists(id => id == info.id)){
                return null;    // もう見た.
            }
            AwsModule.ProgressData.UpdateSeenScenarioEffectID(info.id);
            return info;
        }
        return null;
    }
}

/// <summary>
/// enum : シナリオ再生のシチュエーション.
/// </summary>
public enum ScenarioSituation
{
    MainQuest,
    SubQuest,
    Event,
    Other,
}

/// <summary>
/// enum : シナリオ進捗状況.
/// </summary>
public enum ScenarioProgressState
{
    Other,
    PrevBattle,
    InBattle,
	OutBattle,
    AfterBattle,
}
