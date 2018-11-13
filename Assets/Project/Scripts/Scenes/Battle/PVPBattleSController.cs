using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class PVPBattleSController : ScreenControllerBase {
    public PvpBattleEntryData enrtyData;

    public override void Dispose()
    {
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
        BattleDataDownload (didConnectEnd);
    }

    private void BattleDataDownload(System.Action<bool> didConnectEnd)
    {
        // バトルのマスターデータの取得とバトル用のデータを構築
        AwsModule.BattleData.PvpStartProc(enrtyData);
        BattleResourceManager.Shared.PVPBattleLoadResource(() => didConnectEnd(true));
    }

    /// <summary>
    /// 起動時展開画面生成.
    /// </summary>
    public override void CreateBootScreen()
    {
        SoundManager.SharedInstance.PlayBGM (AwsModule.BattleData.Stage.bgm_clip_name, true);

        BattleProgressManager.Shared.Init();

        // バトルスケジューラーを生成する。
        GameObjectEx.LoadAndCreateObject("Battle/BattleManagers");

        m_screenField = this.CreateBattleFieldScreen();
        m_screenUI = this.CreateBattleUIScreen();

        BattleProgressManager.Shared.BattleStart();
    }

    // UIスクリーンの作成.
    private Screen_PVPBattleUI CreateBattleUIScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("Battle/View_PVPBattleUI");
        DontDestroyOnLoad (go);
		go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var c = go.GetOrAddComponent<Screen_PVPBattleUI>();
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
    private Screen_PVPBattleUI m_screenUI;
    private Screen_BattleField m_screenField;

}
