using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// チュートリアルコントローラ.
/// </summary>
public class TutorialSController : ScreenControllerBase
{
	/// <summary>
	/// 初期化.スクリーン展開前の通信処理がある場合はここで.
	/// </summary>
	/// <param name="didConnectEnd">通信終了時の処理</param>
	public override void Init(Action<bool> didConnectEnd)
	{
		CoroutineAgent.Execute(InitAsync(didConnectEnd));
	}   
	private IEnumerator InitAsync(Action<bool> didConnectEnd)
	{
		do {
			yield return null;
		} while (!TutorialResourceDownloader.IsLoadEnd);
		yield return new WaitForSeconds(0.3f);
		didConnectEnd(true);
	}

	/// <summary>
	/// コントローラが最初に管理するスクリーンの生成
	/// </summary>
	public override void CreateBootScreen()
	{
		UtageModule.SharedInstance.IsHideSkipButton = true; // チュートリアル中はスキップ禁止にしておく.

		if (AwsModule.ProgressData.TutorialStageNum != 0) { 
			UtageModule.SharedInstance.SetActiveCore(true);
			var startAct = this.GetStartProc();
            startAct();
			return;
		}

		// 初回起動.利用規約に同意したらスタート.いきなりポップが出てくる印象があるのでフェードが開けるのを待っておく.
		this.LoadAsyncGachaModel();
		View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, () => {
            View_Popup_Terms.Create(() => {
                UtageModule.SharedInstance.SetActiveCore(true);
                this.Proc1();
            });
		});      
	}

    // 開始処理を取得.
    private Action GetStartProc()
	{
		if(AwsModule.ProgressData.TutorialStageNum <= 2){
			this.LoadAsyncGachaModel();
		}
		
		switch(AwsModule.ProgressData.TutorialStageNum){
			case 1:
				return Proc1;
			case 2:
				return Proc2;
			case 3:
				return Proc3;
			case 4:
				return Proc4;
			case 5:
				return Proc5;
			case 6:
                return Proc6;
			default:
				Debug.LogError("[TutorialManager] GetStartProc Error!! : Not regist proc. TutorialStageNum="+AwsModule.ProgressData.TutorialStageNum);
				return Proc1;
		}
	}

	#region TutorialProcs.

	// 処理1. 【司書パート】最初
	private void Proc1()
	{
		// 冒頭会話シーンスタート.
		AwsModule.ProgressData.UpdateTutorialPoint(1);
		UtageModule.SharedInstance.StartScenario("tuto_1", () => {
			Debug.Log("[TutorialManager] Utage Load Success : tuto_1.");
			View_FadePanel.SharedInstance.SetProgress(0);
			View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => {
				this.Proc2();
			});
		}, false);
	}

	// 処理2. 【司書パート】ガチャチュートリアル前
    private void Proc2()
	{
		AwsModule.ProgressData.UpdateTutorialPoint(2);

		// ガチャ開始.      
		this.RequestGacha((objDict, res) => {
		    // 演出画面表示.
			View_GachaMovie.Create(res, objDict, () => {
				// 結果画面.
                View_GachaResult.Create(res, Proc3);
				TutorialResourceDownloader.DisposeLive2dModels();
			});

            // シナリオスタート.      
            UtageModule.SharedInstance.StartScenario("tuto_2", () => {
                Debug.Log("[TutorialManager] Utage Load Success : tuto_2.");
                UtageModule.SharedInstance.SetActiveCore(false);
            });
            
            View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
        });
	}

	// 処理3. 【司書パート】最後
	private void Proc3()
	{
		//SoundManager.SharedInstance.StopBGM();
		AwsModule.ProgressData.UpdateTutorialPoint(3);
		UtageModule.SharedInstance.SetActiveCore(true);
        // シナリオスタート
        UtageModule.SharedInstance.StartScenario("tuto_3", () => {
            Debug.Log("[TutorialManager] Utage Load Success : tuto_3.");
			View_FadePanel.SharedInstance.FadeOut(View_FadePanel.FadeColor.Black, () => {
				this.Proc4();
			});
        }, true);
	}
    
	// 処理4. 【M7会議パート】前半
    private void Proc4()
    {
		AwsModule.ProgressData.UpdateTutorialPoint(4);
		TutorialResourceDownloader.LoadUtageChapters();
        StartDLC();
		View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
    }

    private void StartDLC()
    {
        long dlcSize = DLCManager.DownloadMinimumContentsFilesSize ();
        long maxDlcSize = DLCManager.DownloadAllFilesSize ();
        if (dlcSize <= 0 && maxDlcSize <= 0) {
            Proc4Core ();
            return;
        }

        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm045);

        UtageModule.SharedInstance.SetActiveCore (false);
        var dlcType = AwsModule.ProgressData.SelectDLCType;
        // 選択済みか確認する。
        if (dlcType == 0) {
            View_DLSelectPop.Create (StartDownload);
        } else {
            // dlcのタイプが最小限で最小限のダウンロードがなければ次にすすむ
            if (dlcType == 1 && dlcSize <= 0) {
				WaitLoadScenario ();
                return;
            }
            StartDownload (AwsModule.ProgressData.SelectDLCType == 2);
        }
    }

    void StartDownload(bool all)
    {
        var dlView = View_DL.CreateIfMissing();
        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black,
            () => {
                if (all) {
                    // 全体選択を設定
                    AwsModule.ProgressData.SetSelectDLCType(2);
                    DLCManager.DownloadAllFiles ((ret) => {
					    View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black,
                            () => {
                                View_DL.DisposeProc();
						        PopupManager.OpenPopupSystemOK("データのダウンロードが\n完了しました。", WaitLoadScenario);
                            }
                        );
                    }, null, dlView.SetProgress);
                } else {
                    // 最小限選択を設定
                    AwsModule.ProgressData.SetSelectDLCType(1);
                    DLCManager.DownloadMinimumContentsFiles ((ret) => {
					    View_FadePanel.SharedInstance.FadeOut (View_FadePanel.FadeColor.Black,
                            () => {
                                View_DL.DisposeProc();
						        PopupManager.OpenPopupSystemOK("データのダウンロードが\n完了しました。", WaitLoadScenario);
                            }
                        );
                    }, null, dlView.SetProgress);
                }
            }
        );
    }
	void WaitLoadScenario()
	{
		this.StartCoroutine(this.WaitLoadLaterScenario());
	}
	IEnumerator WaitLoadLaterScenario()
	{
		while(!TutorialResourceDownloader.IsDownloadEndUtage){
			yield return null;
		}
		Proc4Core();
	}

    private void Proc4Core()
    {
		SoundManager.SharedInstance.StopBGM();
        UtageModule.SharedInstance.SetActiveCore(true);
         
        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);            
        // シナリオスタート
        UtageModule.SharedInstance.StartScenario ("tuto_4", () => {
            Debug.Log ("[TutorialManager] Utage Load Success : tuto_4.");
            this.Proc5 ();
        }, false);
    }

	// 処理5. 【バトルチュートリアル】通常攻撃の説明
    private void Proc5()
	{      
		UtageModule.SharedInstance.SetActiveCore(false);
        AwsModule.ProgressData.UpdateTutorialPoint(5);
		// バトルスタート
        AwsModule.BattleData.SetStage(-1);
        SoundManager.SharedInstance.DownloadResource(SoundClipName.bgm015, () => SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm015));
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => { 
			TutorialBattleManager.CreateBattleView(1, instance => {
				//View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
                m_battleManager = instance;
				m_battleManager.StartBattle(() => {
                    // シナリオスタート
                    UtageModule.SharedInstance.SetActiveCore(true);
                    UtageModule.SharedInstance.StartScenario("tuto_5", () => {
                        Debug.Log("[TutorialManager] Utage Load Success : tuto_5.");
						// ダウンロードとヴェルムバトルBGMを再生する
                        UtageModule.SharedInstance.SetActiveCore(false);
                        // 初回行動.
						View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.Attack);
                        m_battleManager.StartNextAction(() => {
                            this.Proc5_1();
                        });
                    }, false);
                });
            });
		});
	}
	// 処理5_1. 【バトルチュートリアル】行動順の説明
	private void Proc5_1()
	{
		// シナリオスタート
		UtageModule.SharedInstance.SetActiveCore(true);
		UtageModule.SharedInstance.StartScenario("tuto_5_1", () => {
			Debug.Log("[TutorialManager] Utage Load Success : tuto_5_1.");         
			UtageModule.SharedInstance.SetActiveCore(false);
			this.Proc5_2_pre();
		}, false);
    }
    private void Proc5_2_pre()
	{
		this.StartCoroutine(WaitDisp5_2_pre());
	}
	IEnumerator WaitDisp5_2_pre()
	{
		// TODO : タイムラインアイコン上に矢印表示.
		View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.TimeLine);
		yield return new WaitForSeconds(3f);
		m_battleManager.StartNextAction(() => {
			this.Proc5_2();
		});
    }
	// 処理5_2. 【バトルチュートリアル】スキルの説明　前半
    private void Proc5_2()
	{
		// シナリオスタート
        UtageModule.SharedInstance.SetActiveCore(true);
        UtageModule.SharedInstance.StartScenario("tuto_5_2", () => {
            Debug.Log("[TutorialManager] Utage Load Success : tuto_5_2.");
            UtageModule.SharedInstance.SetActiveCore(false);
			View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.Target);
			m_battleManager.StartNextAction(() => {
				this.Proc5_3();
            });
        }, false);
	}
	// 処理5_3. 【バトルチュートリアル】スキルの説明　後半
    private void Proc5_3()
	{
        // シナリオスタート
        UtageModule.SharedInstance.SetActiveCore(true);
        UtageModule.SharedInstance.StartScenario("tuto_5_3", () => {
            Debug.Log("[TutorialManager] Utage Load Success : tuto_5_3.");
            UtageModule.SharedInstance.SetActiveCore(false);         
            m_battleManager.StartNextAction(() => {
				this.Proc5_4();
            });
        }, false);
	}
	// 処理5_4. 【バトルチュートリアル】必殺技の説明
	private void Proc5_4()
    {
        // シナリオスタート
        UtageModule.SharedInstance.SetActiveCore(true);
        UtageModule.SharedInstance.StartScenario("tuto_5_4", () => {
            Debug.Log("[TutorialManager] Utage Load Success : tuto_5_4.");
            UtageModule.SharedInstance.SetActiveCore(false);
			View_TutorialBattleFade.CreateIfMissing(View_TutorialBattleFade.ViewMode.SPSkill);
            m_battleManager.StartNextAction(() => {
                m_battleManager.Dispose();  // バトル終了
                this.Proc6();
            });
        }, false);
    }

	// 処理6 : 【M7会議パート】後半
    private void Proc6()
	{
		SoundManager.SharedInstance.StopBGM();
		AwsModule.ProgressData.UpdateTutorialPoint(6);
		// シナリオスタート
        UtageModule.SharedInstance.SetActiveCore(true);
        UtageModule.SharedInstance.StartScenario("tuto_6", () => {
            Debug.Log("[TutorialManager] Utage Load Success : tuto_6.");
            UtageModule.SharedInstance.SetActiveCore(false);
			DLCManager.Mp4FromDownloadOrCache("op.mp4", savePath => {
				View_OpMovie.Create(savePath, ProcEnd);
			});
        }, false);
	}   
       
    // 終了.マイページへ遷移
    private void ProcEnd()
	{
		AwsModule.ProgressData.UpdateTutorialPoint(-1);      
		AwsModule.ProgressData.UpdateSeenCountryReleaseEffectList(BelongingEnum.Varm);  // ヴェルム解放演出は見た判定に.手前でやると復帰できなくなるのでここで.
		UtageModule.SharedInstance.SetActiveCore(false);
		View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => {
			UtageModule.SharedInstance.DestroyCore();
			SendAPI.HomeLogin((success, res) => {
				if (res == null || !success) {
					Debug.LogError("SendUsersGetUserData Error");
					return; // エラー.
				}

				AwsModule.UserData.UserData = res.UserData;
				res.CardDataList.CacheSet();
				res.FormationDataList.CacheSet();
				res.WeaponDataList.CacheSet();
				res.MagikiteDataList.CacheSet();
                res.MaterialDataList.CacheSet();
                res.ConsumerDataList.CacheSet();

                // レアリティが高いものをセット
                AwsModule.UserData.MainCardID = CardData.CacheGetAll().OrderBy(x => x.Rarity).Last().CardId;
                // メインカードを必ず一枚サポートカードとして設定する。
                AwsModule.UserData.SetSupportCardList(AwsModule.UserData.MainCard.Card.element.Enum, AwsModule.UserData.MainCard);
				AwsModule.PartyData.CurrentTeam.Init();
                AwsModule.UserData.Sync((bSuccess1, sender1, e1) => { 
                    AwsModule.PartyData.SyncAndForceCommit((bSuccess2, sender2, e2) => {
                        ScreenChanger.SharedInstance.GoToMyPage();
                    });
                });
			});
		});
	}

	#endregion

	// 擬似マイページ作成
	private void CreateDummyMyPageScreen()
	{
		if(dummyMyPageCtrl != null){
			dummyMyPageCtrl.Dispose();
		}      
		dummyMyPageCtrl = ScreenControllerBase.Create<MyPageSController>();
		dummyMyPageCtrl.Init(bSuccess => {
			dummyMyPageCtrl.CreateBootScreen();
			SoundManager.SharedInstance.StopBGM();
			View_PlayerMenu.IsVisible = false;  // TODO : ヘッダーはまだログインしてないのでとりあえず表示しない
		});      
	}

	private void RequestGacha(Action<List<KeyValuePair<int, GameObject>>, ReceiveGachaExecuteTutorialGacha> didLoad)
	{
		this.StartCoroutine(this.WaitRequestGacha(didLoad));
	}
	IEnumerator WaitRequestGacha(Action<List<KeyValuePair<int, GameObject>>, ReceiveGachaExecuteTutorialGacha> didLoad)
	{
		while(!TutorialResourceDownloader.IsDownloadEndForGacha) {
            yield return null;
        }
		while(TutorialResourceDownloader.GachaObjectList == null){
			yield return null;
		}
		while (m_gachaModelList == null) {
			yield return null;
		}
		while(m_gachaModelList.Count < TutorialResourceDownloader.GachaObjectList.Count){
			yield return null;
		}
		didLoad(m_gachaModelList, TutorialResourceDownloader.GachaResponse);
	}

	// チュートリアルガチャモデルロード.
	private void LoadAsyncGachaModel()
	{
		this.StartCoroutine(this.LoadWaitGacha());
	}
	private IEnumerator LoadWaitGacha()
	{
		do {
			yield return null;
		} while (!TutorialResourceDownloader.IsDownloadEndForGacha);

		m_gachaModelList = new List<KeyValuePair<int, GameObject>>();
		foreach(var kvp in TutorialResourceDownloader.GachaObjectList){
			m_gachaModelList.Add(new KeyValuePair<int, GameObject>(kvp.Key, kvp.Value.Target as GameObject));
		}
	}

	private bool m_bEndLive2dLoad = false;
	private List<KeyValuePair<int, GameObject>> m_gachaModelList;
	private static MyPageSController dummyMyPageCtrl;
	private TutorialBattleManager m_battleManager;
}
