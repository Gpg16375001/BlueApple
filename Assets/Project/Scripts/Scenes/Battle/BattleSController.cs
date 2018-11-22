using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class BattleSController : ScreenControllerBase
{
    public bool IsReversion;
    /// <summary>
    /// 破棄.
    /// </summary>
    public override void Dispose()
    {
        // 復帰時の場合はグルーバルメニューなどを作り直す
        if(IsReversion) {
            // グローバルメニュー作成
            View_GlobalMenu.CreateIfMissing();
            var playerMenu = View_PlayerMenu.CreateIfMissing();
            playerMenu.UpdateView (AwsModule.UserData.UserData);
        }

		if(m_screenUI == null){
			Debug.LogError("[BattleSController] Dispose Error!! : Screen_BattleUI is null");
		}
		if (m_screenField == null) {
            Debug.LogError("[BattleSController] Dispose Error!! : Screen_BattleField is null");
        }
		
        ScenarioProvider.CurrentScenarioState = ScenarioProgressState.AfterBattle;
        //CameraHelper.SharedInstance.IsEnableBattleCameraInput = false;      
		if(m_screenUI != null){
			m_screenUI.Dispose();
		}      
		if (m_screenField != null) { 
			m_screenField.Dispose();
		}

        BattleProgressManager.Shared.Dispose ();

        // バトルで使用したリソースを削除
        BattleResourceManager.Shared.Dispose ();

        base.Dispose();
    }

    public override void Init(System.Action<bool> didConnectEnd)
    {
        if (!IsReversion) {
            ScenarioProvider.CurrentScenarioState = ScenarioProgressState.InBattle;

            // シナリオがあればロード.
            var utageProjectName = ScenarioProvider.GetUtageProject ();
            var actList = ScenarioProvider.GetActList ();
            if (!string.IsNullOrEmpty (utageProjectName) && actList != null && actList.Count > 0) {
                UtageModule.SharedInstance.LoadUseChapter (utageProjectName, () => {
                    UtageModule.SharedInstance.SetActiveCore (false);
                    BattleDataDownload (didConnectEnd);
                }, actList.ToArray ());
            } else {
                BattleDataDownload (didConnectEnd);
            }
        } else {
            // バトル情報の復帰
            AwsModule.BattleData.Reversion ((ret) => {
                if(!ret) {
                    didConnectEnd(ret);
                }
                // クエスト周りのデータの復帰
                if(AwsModule.BattleData.QuestType == 1) {
                    ScenarioProvider.CurrentSituation = ScenarioSituation.MainQuest;
                    ScenarioProvider.CurrentScenarioState = ScenarioProgressState.InBattle;
                    AwsModule.ProgressData.CurrentQuest = MasterDataTable.quest_main [AwsModule.BattleData.QuestID] as IQuestData;
                } else if(AwsModule.BattleData.QuestType == 2) {
                    ScenarioProvider.CurrentSituation = ScenarioSituation.SubQuest;
                    ScenarioProvider.CurrentScenarioState = ScenarioProgressState.InBattle;
                    AwsModule.ProgressData.CurrentQuest = MasterDataTable.quest_sub [AwsModule.BattleData.QuestID] as IQuestData;
                } else if(AwsModule.BattleData.QuestType == 3) {
                    ScenarioProvider.CurrentSituation = ScenarioSituation.Other;
                    ScenarioProvider.CurrentScenarioState = ScenarioProgressState.InBattle;
                    AwsModule.ProgressData.CurrentQuest = MasterDataTable.quest_unit [AwsModule.BattleData.QuestID] as IQuestData;
                } else if(AwsModule.BattleData.QuestType == 4 || AwsModule.BattleData.QuestType == 5) {
                    ScenarioProvider.CurrentSituation = ScenarioSituation.Other;
                    ScenarioProvider.CurrentScenarioState = ScenarioProgressState.InBattle;
                    AwsModule.ProgressData.CurrentQuest = MasterDataTable.quest_daily [AwsModule.BattleData.QuestID] as IQuestData;
                } else if(AwsModule.BattleData.QuestType == 6) {
                    ScenarioProvider.CurrentSituation = ScenarioSituation.Event;
                    ScenarioProvider.CurrentScenarioState = ScenarioProgressState.InBattle;
                    AwsModule.ProgressData.CurrentQuest = MasterDataTable.event_quest_stage_details [AwsModule.BattleData.QuestID] as IQuestData;
                }

                var utageProjectName = ScenarioProvider.GetUtageProject ();
                var actList = ScenarioProvider.GetActList ();
                if (!string.IsNullOrEmpty (utageProjectName) && actList != null && actList.Count > 0) {
                    UtageModule.SharedInstance.LoadUseChapter (utageProjectName, () => {
                        UtageModule.SharedInstance.SetActiveCore (false);
                        BattleResourceManager.Shared.BattleLoadResource(() => didConnectEnd(true));
                    }, actList.ToArray ());
                } else {
                    BattleResourceManager.Shared.BattleLoadResource(() => didConnectEnd(true));
                }
            });

        }
    }

    private void BattleDataDownload(System.Action<bool> didConnectEnd)
    {
        // バトルのマスターデータの取得とバトル用のデータを構築
        AwsModule.BattleData.StartProc( 
            (ret) => {
                // 必要なリソースのダウンロード
                BattleResourceManager.Shared.BattleLoadResource(() => didConnectEnd(true));
            }
        );
    }

    /// <summary>
    /// 起動時展開画面生成.
    /// </summary>
    public override void CreateBootScreen()
    {
        
        var waveSetting = AwsModule.BattleData.StageWaveSettings.FirstOrDefault(x => x.wave_count == AwsModule.BattleData.WaveCount);
        if (waveSetting != null && !string.IsNullOrEmpty (waveSetting.wave_bgm)) {
            SoundManager.SharedInstance.PlayBGM (waveSetting.wave_bgm, true);
        } else {
            SoundManager.SharedInstance.PlayBGM (AwsModule.BattleData.Stage.bgm_clip_name, true);
        }

        BattleProgressManager.Shared.Init();

        // バトル関連マネージャーを生成する。
        GameObjectEx.LoadAndCreateObject("Battle/BattleManagers");

        m_screenUI = this.CreateBattleUIScreen();
        m_screenField = this.CreateBattleFieldScreen();

        BattleProgressManager.Shared.BattleStart();
    }

    // UIスクリーンの作成.
    private Screen_BattleUI CreateBattleUIScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleUI");
        DontDestroyOnLoad (go);
		go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var c = go.GetOrAddComponent<Screen_BattleUI>();
        c.Init();
        return c;
    }
    // フィールドスクリーンの作成.
    private Screen_BattleField CreateBattleFieldScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleField");
        DontDestroyOnLoad (go);
        var c = go.GetOrAddComponent<Screen_BattleField>();
        c.Init();
        return c;
    }

    //private Screen_Battle m_screen;
    private Screen_BattleUI m_screenUI;
    private Screen_BattleField m_screenField;
}
