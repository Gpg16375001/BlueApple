using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

/// <summary>
/// ScreenController : シナリオシーン.
/// </summary>
public class ScenarioSController : ScreenControllerBase
{
	/// <summary>最新クエストをクリアしたかどうか.</summary>
	public bool IsLatestQuestClear { get; set; }


    /// <summary>
    /// 初期化.ScreenChangerが呼ぶ.
    /// </summary>
    public override void Init(Action<bool> didConnectEnd)
    {
        var utageProjectName = ScenarioProvider.GetUtageProject();
        // 設定がないだけなのでスルー.
        if(string.IsNullOrEmpty(utageProjectName)){
            didConnectEnd(true);
            return;
        }
        var actList = ScenarioProvider.GetActList();
        if(actList.Count <= 0){
            Debug.LogError("[ScenarioSController] Init Error!! : Act not found.");
            didConnectEnd(false);
            return;
        }

        // BGM停止テスト.
        SoundManager.SharedInstance.StopBGM();

        // ロード.
        UtageModule.SharedInstance.LoadUseChapter(utageProjectName, () => {
            Debug.Log("Utage Load Success.");
            didConnectEnd(true);
        }, actList.ToArray());
    }

    /// <summary>
    /// 起動シーン作成.
    /// </summary>
	public override void CreateBootScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("Scenario/Screen_Scenario", this.gameObject);
        var c = go.GetOrAddComponent<Screen_Scenario_Top>();
		c.Init(IsLatestQuestClear);
    }
}
