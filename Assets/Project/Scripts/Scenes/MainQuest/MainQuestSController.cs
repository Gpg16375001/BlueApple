using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ScreenController : メインのシナリオクエスト.
/// </summary>
public class MainQuestSController : ScreenControllerBase
{
    /// <summary>起動画面.</summary>
    public MainQuestBootEnum BootMode { get; set; }

	/// <summary>boot起動かどうか.クエスト画面に頭から遷移する場合はtrue、連続再生(シナリオ終了後に再遷移など)ではfalse.</summary>
	public bool IsBoot { get; set; }

    /// <summary>マップ表示時に新しいルート解放をチェックするかどうか.</summary>
	public bool IsCheckNewRoot { get; set; }

	/// <summary>クリア演出処理.あれば.</summary>
	public Action ClearEffectProc { get; set; }


	public override void Dispose()
	{
		if(m_topScreen != null){
			m_topScreen.Dispose();
		}      
		base.Dispose();
	}

	public override void Init(Action<bool> didConnectEnd)
    {
		ScenarioProvider.CurrentScenarioState = ScenarioProgressState.PrevBattle;
        ScenarioProvider.CurrentSituation = ScenarioSituation.MainQuest;
        
		SendAPI.QuestsGetMainCountryList((bSuccess, res) => { 
			if(!bSuccess || res == null){
				didConnectEnd(false);
				return;
			}
			m_arrayCountry = new MainQuestCountryData[res.MainQuestCountryDataList.Length];
			res.MainQuestCountryDataList.CopyTo(m_arrayCountry, 0);
            // 国選択状態であればその国の情報も持ってくる.
			if(AwsModule.ProgressData.PrevSelectedQuest != null){
				SendAPI.QuestsGetMainAchievementByCountry((int)AwsModule.ProgressData.PrevSelectedQuest.Country.Enum, (bSuccess2, res2) => {
					if(!bSuccess2 || res2 == null){
						didConnectEnd(false);
						return;
					}
					res2.QuestAchievementList.CacheSet();
					didConnectEnd(true);
				});
				return;
			}
			didConnectEnd(true);
		});
    }

    public override void CreateBootScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("MainQuest/Screen_MainQuest", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        m_topScreen = go.GetOrAddComponent<Screen_MainQuest_Top>();
		m_topScreen.Init(m_arrayCountry, IsBoot, BootMode, IsCheckNewRoot);
    }
 
	private Screen_MainQuest_Top m_topScreen;
	private MainQuestCountryData[] m_arrayCountry;
}
