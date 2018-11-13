using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// Screen : シナリオトップ画面.
/// </summary>
public class Screen_Scenario_Top : ViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(bool bLatestQuestClear = false)
    {
		m_bLatestQuestClear = bLatestQuestClear;

        // フェードを開ける.
        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, () => {
            // あればタイトル演出.
			this.StartTittleEffect(this.StartScenario);
        });
    }

    // タイトル演出開始.
    private void StartTittleEffect(Action didEnd)
    {
        var info = ScenarioProvider.GetScenarioEffectIfNeedEffect();
        if (info == null) {
            didEnd();
            return;
        }
        if(info.eff_type == ScenarioEffectTypeEnum.None){
            didEnd();
            return;
        }
        m_viewTitleEff = View_Scenario_TitleEffect.Create(info, () => {
            m_viewTitleEff.Dispose();
            didEnd();
        });
    }

    // シナリオ再生開始.
    private void StartScenario()
    {
        var scenario = ScenarioProvider.GetScenario();
        // シナリオが終わった時の処理.
        Action didEndScenario = () => {
            // バトル前なら出撃編成へ.
            if (ScenarioProvider.CurrentScenarioState == ScenarioProgressState.PrevBattle) {
				View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
					ScreenChanger.SharedInstance.GoToFriendSelect();
                });
                return;
            }
            // バトル後ならクエスト選択画面へ戻る.
            if (ScenarioProvider.CurrentScenarioState == ScenarioProgressState.AfterBattle) {
				GoToQuestSelect();
                return;
            }
            // TODO : それ以外はとりあえずマイページに戻る.
            this.IsEnableButton = false;
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToMyPage();
            });
        };

        // シナリオなし.
        if(string.IsNullOrEmpty(scenario)){
			// フェードを正しく挙動させるために次のフレームで実行する
			Observable.Return(Unit.Default)
					  .DelayFrame(1)
					  .Subscribe(_ => didEndScenario());
            return;
        }
        // シナリオ再生.
        UtageModule.SharedInstance.StartScenario(scenario, didEndScenario);
    }

    // クエスト選択シーンに移動.
    private void GoToQuestSelect()
    {
        this.IsEnableButton = false;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            if (AwsModule.ProgressData.CurrentQuest == null) { 
				AwsModule.ProgressData.PrevSelectedQuest = null;
				ScreenChanger.SharedInstance.GoToMainQuestSelect();
                return;
            }
            var current = AwsModule.ProgressData.CurrentQuest;
            // TODO :サブシーンは更新も移動しない.なんらか考えておいた方が良さそう.
            if(current.QuestType != 1){
                ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.SubScene, false);
                return;
            }         
			var next = MasterDataTable.quest_main.GetNextQuestInfo(current.ID);         
			// 次のクエストが見つからない=その国のクエスト全クリア.スキップする起動シーンも次の国の最初のクエストに切り替えておく.
			if(next == null){
				AwsModule.ProgressData.PrevSelectedQuest = null;
				ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Country, false);
                return;
            }
			Debug.Log("current= "+AwsModule.ProgressData.PrevSelectedQuest.ChapterNum + "-" + AwsModule.ProgressData.PrevSelectedQuest.StageNum + "-" + AwsModule.ProgressData.PrevSelectedQuest.QuestNum);
            Debug.Log("next= "+next.ChapterNum + "-" + next.StageNum + "-" + next.QuestNum);
			var prevChapterNum = AwsModule.ProgressData.CurrentQuest.ChapterNum;
			var prevStageNum = AwsModule.ProgressData.CurrentQuest.StageNum;
			var prevQuestNum = AwsModule.ProgressData.CurrentQuest.QuestNum;
			if(!next.is_force_lock){
				if(m_bLatestQuestClear){
					// 最新クエストを初回クリア.フォーカスクエスト更新用に次のクエストを差しておく
					Debug.Log("最新クエスト");
					AwsModule.ProgressData.PrevSelectedQuest = AwsModule.ProgressData.CurrentQuest = next;
				}else{
					// 過去クエスト.
					Debug.Log("過去クエスト");
					ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Stage, false);
					return;
				}            
			}else{
				// 次のクエストが一つ目のクエストの場合はステージ選択へ.
				if(next.QuestNum <= 1){
					// ステージも一つ目なら章選択へ.
					if(next.StageNum <= 1){
						ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Chapter, false);
						return;
					}
				}
				ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Stage, false);
                return;
			}         
   
            // 章更新.
			if(current.ChapterNum > prevChapterNum){
                ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Chapter, false, true);
            }
			// ステージ or クエスト更新.
			else {
				ScreenChanger.SharedInstance.GoToMainQuestSelect(MainQuestBootEnum.Stage, false, true);
			}
        });
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private bool m_bLatestQuestClear;
    private View_Scenario_TitleEffect m_viewTitleEff;
}
