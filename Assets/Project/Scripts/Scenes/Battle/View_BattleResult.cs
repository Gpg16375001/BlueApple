using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// View : バトル結果画面.
/// </summary>
public class View_BattleResult : ViewBase
{   
	/// <summary>
	/// 生成メソッド.クエスト情報から.
	/// </summary>
	public static View_BattleResult CreateForQuestResult(ReceiveQuestsCloseQuest questResult, bool bLatestQuestClear)
	{      
		var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleResult");
		go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
		var instance = go.GetOrAddComponent<View_BattleResult>();
		instance.InitInternal(questResult, bLatestQuestClear);
		return instance;
	}

	/// <summary>
	/// 生成メソッド.チュートリアルから.
	/// </summary>
	public static View_BattleResult CreateForTutorial(List<CardCard> allys, Action didEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleResult");
		go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
		var instance = go.GetOrAddComponent<View_BattleResult>();
		instance.InitInternal(allys, didEnd);
		return instance;
	}
 
	// 開く.
	private void Open()
	{
		if (m_bOpen && gameObject.activeSelf) {
			return;
		}
		// オープン時はアクティブをあげる
		gameObject.SetActive(true);
		this.StartCoroutine(this.PlayOpenClose());
	}
 
	// 内部初期化.クエストリザルトとして.
	private void InitInternal(ReceiveQuestsCloseQuest questResult, bool bLatestQuestClear)
	{
        m_bClose = false;
		m_bLatestQuestClear = bLatestQuestClear;

        int count = 0;
		// 経験値.
		var rootObj = GetScript<CanvasGroup>("Contents").gameObject;
		var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleExpResult", rootObj);
		var viewExp = go.GetOrAddComponent<MiniView_BattleExpResult>();
        viewExp.Init(count++, questResult);
		go.SetActive(false);
		pages.Add(viewExp);
        // リワード系.
		go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleRewardResult", rootObj);
		var viewReward = go.GetOrAddComponent<MiniView_BattleRewardResult>();
        viewReward.Init(count++, questResult);
		go.SetActive(false);
		pages.Add(viewReward);

        // ミッション報酬がある場合のみ追加
        if (questResult.MissionRewardItemList != null && questResult.MissionRewardItemList.Length > 0) {
            go = GameObjectEx.LoadAndCreateObject ("Battle/View_BattleMissionRewardPop", rootObj);
            var viewMisionReward = go.GetOrAddComponent<MiniView_BattleMissionRewardPop> ();
            viewMisionReward.Init (count++, questResult);
            go.SetActive (false);
            pages.Add (viewMisionReward);
        }

        if (questResult.GainEventPoint > 0) {
			// eventIdの取得
			var current = AwsModule.ProgressData.CurrentQuest;
			int eventId = 1;
			if(current.QuestType == 6){
				eventId = MasterDataTable.event_quest_stage_details [current.ID].EventQuestData.id;
			}
            go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleEventRewardPop", rootObj);
            var viewEvent = go.GetOrAddComponent<MiniView_BattleEventResult>();
			viewEvent.Init(count++, eventId, questResult);
            go.SetActive(false);
            pages.Add(viewEvent);
        }

		// ページ設定.
		pages.Sort((x, y) => x.Index - y.Index);
		maxPage = pages.Count;
        page = -1;

        touchBlock = false;

		// ボタン設定
        this.SetCanvasButtonMsg("btn_Next", DidTapNext);
        this.SetCanvasButtonMsg("OK/bt_Common", DidTapNext);
        this.SetCanvasButtonMsg("bt_Close", DidTapNext);

		NextResult(bExistNext => Open());

        currentTitle = ResultTitle.Clear;
	}

    private void BattleClearAnim(ResultTitle current, ResultTitle? next = null)
    {
        var anim = GetScript<Animation> ("eff_BattleClear");
        if (current == ResultTitle.Clear) {
            anim.Play ("BattleResultClearOut");
        } else if (current == ResultTitle.Mission) {
            anim.Play ("BattleResultMissionOut");
        } else if (current == ResultTitle.Event) {
            anim.Play ("BattleResultEvent01Out");
        }

        if (next.HasValue) {
            if (waitAndPlay != null) {
                StopCoroutine (waitAndPlay);
            }
            waitAndPlay = StartCoroutine (WaitAndPlay (anim, next.Value));
        }
    }

    Coroutine waitAndPlay;
    IEnumerator WaitAndPlay(Animation anim, ResultTitle next)
    {
        yield return new WaitUntil (() => !anim.isPlaying);
        if (next == ResultTitle.Clear) {
            anim.Play ("BattleResultClearIn");
        } else if (next == ResultTitle.Mission) {
            anim.Play ("BattleResultMissionIn");
        } else if (next == ResultTitle.Event) {
            anim.Play ("BattleResultEvent01In");
        }
        waitAndPlay = null;
    }

	// 終了時の処理だけを登録した単調な初期化.クリア演出だけ見たいときなど.
	private void InitInternal(List<CardCard> allys, Action didEnd)
	{
		m_didEnd = didEnd;

		// ボタン設定
        this.SetCanvasButtonMsg("btn_Next", DidTapNext);
        this.SetCanvasButtonMsg("OK/bt_Common", DidTapNext);
        this.SetCanvasButtonMsg("bt_Close", DidTapNext);
	}

	private void NextResult(Action<bool/*bNextExist*/> didNext)
	{
		LockInputManager.SharedInstance.IsLock = true;
		var current = pages.Find(p => p.Index == page);
		if(current != null && current.IsEffecting){
			current.ForceImmediateEndAnimation();
			if(didNext != null){
				didNext(true);
			}
			LockInputManager.SharedInstance.IsLock = false;
			return;
		}
		++page;

		Action nextOpen = () => { 
			var next = pages.Find(p => p.Index == page);
			if (next == null) {
                BattleClearAnim(currentTitle);
                if (didNext != null) {
                    didNext(false);
                }
				LockInputManager.SharedInstance.IsLock = false;
                return;
            }

            var nextTitle = next.GetResultTitle();

            if(nextTitle != currentTitle) {
                BattleClearAnim(currentTitle, nextTitle);
                currentTitle = nextTitle;
            }
            next.Open();
            if (didNext != null) {
                didNext(true);
            }
			LockInputManager.SharedInstance.IsLock = false;
		};
  
		if(current != null){
			current.Close(nextOpen);
		}else{
			nextOpen();
		}
	}

    private void NextScene()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            // 次に遷移させる
            AwsModule.BattleData.GameOverProc(); // 表示崩れを防ぐためここでゲームオーバー処理.
            if(AwsModule.ProgressData.CurrentQuest.QuestType == 4) {
                ScreenChanger.SharedInstance.GoToDailyQuest(1); 
            } else if(AwsModule.ProgressData.CurrentQuest.QuestType == 5) {
                ScreenChanger.SharedInstance.GoToDailyQuest(2);
            } else {
				ScreenChanger.SharedInstance.GoToScenario(m_bLatestQuestClear);
            }
        });
    }
    
    private void FollowPopup()
    {
        LockInputManager.SharedInstance.IsLock = true;
        SendAPI.UsersGetUserData( new int[1] { AwsModule.BattleData.BattleEntryData.SupporterUserId }, 
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                if(!success) {
                    NextScene();
                } else {
                    // すでにフォロー済み
                    if(response.UserDataList[0].IsFollow) {
                        NextScene();
                    } else {
                        // 閉じてフォロー確認ポップアップを開く
                        PlayOpenClose();
					    View_BattleSupportFollowPop.Create(response.UserDataList[0], m_bLatestQuestClear);
                    }
                }
            }
        );
    }

	#region ButtonDelegate.

    bool touchBlock;
	// ボタン：Touch Screen
	void DidTapNext()
	{
        if (m_bClose) {
            return;
        }

        if (touchBlock) {
            return;
        }

        touchBlock = true;
		NextResult(bExistNext => {
            touchBlock = false;
			if(bExistNext){
				return;
			}

            m_bClose = true;
			if (m_didEnd != null) {
                m_didEnd();
            } else {                
                FollowPopup();
            }
		});
	}

	#endregion

	// アニメーション開閉処理.
	private IEnumerator PlayOpenClose()
	{
		LockInputManager.SharedInstance.IsLock = true;

		if (m_anim == null) {
			m_anim = this.GetComponent<IgnoreTimeScaleAnimation>();
		}

		if (m_bOpen) {
			m_anim.Play("PopupClose");
		} else {
			m_anim.Play("PopupOpen");
		}
		yield return null;

		m_bOpen = !m_bOpen;

		// アニメーション終了待ち
		while (m_anim.isPlaying) {
			yield return null;
		}
		if (!m_bOpen) {
			// クローズ時はアクティブを落とす
			gameObject.SetActive(m_bOpen);
		}

		LockInputManager.SharedInstance.IsLock = false;
	}

	private Action m_didEnd = null;
	private bool m_bOpen = true;        // 開閉状態.
	private IgnoreTimeScaleAnimation m_anim;
	private int page = 0;
	private List<IBattleResultPage> pages = new List<IBattleResultPage>();
	private int maxPage = 0;

    bool m_bClose;
	bool m_bLatestQuestClear;

    ResultTitle currentTitle = ResultTitle.Clear;
}
