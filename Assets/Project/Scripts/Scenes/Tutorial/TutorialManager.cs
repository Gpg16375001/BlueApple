using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// チュートリアル管理.
/// </summary>
public class TutorialManager : MonoBehaviour
{   
	/// <summary>
	/// 非同期初期化.
	/// </summary>
	public static void InitAsync(Action didInit)
	{
		// チュートリアルで使用するシナリオのロード.
		UtageModule.SharedInstance.LoadUseChapter("Tutorial", () => {
            Debug.Log("[TutorialSController] Utage Load Success.");
			if(didInit != null){
				didInit();
			}         
        }, ACT_LIST);
	}
	private static readonly string[] ACT_LIST = { "tuto_1", "tuto_2", "tuto_3", "tuto_4", "tuto_5", "tuto_6", "tuto_7", "tuto_8", "tuto_9", "tuto_10", };

    /// <summary>
    /// シナリオ開始.
    /// </summary>
    public static void StartScenario(Action didScenarioEnd)
	{
		var label = GetScenarioLabel();
		UtageModule.SharedInstance.StartScenario(label, () => {
			Debug.Log("[TutorialManager] Utage Load Success : "+label);
			didScenarioEnd();
        }, false);
	}
    // TutorialStageNumに応じた現在のシナリオラベル.
	private static string GetScenarioLabel()
	{
		var step = AwsModule.ProgressData.TutorialStageNum;
		if(step <= 0){
			Debug.LogError("[TutorialManager] GetScenarioLabel Error!! : AwsModule.ProgressData.TutorialStageNum is less than 1. Already finish?");
			return "";
		}
		var label = "tuto_"+step.ToString();
		if(!Array.Exists(ACT_LIST, l => l == label)){
			Debug.LogError("[TutorialManager] GetScenarioLabel Error!! : "+label+" is not tutorial label.");
            return "";
		}
		return label;
	}   
}
