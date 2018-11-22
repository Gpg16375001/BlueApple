using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// Screen : メインクエスト.
/// </summary>
public class Screen_MainQuest_Top : ViewBase
{   
	public override void Dispose()
	{
		m_map.Dispose();
		base.Dispose();
	}

	/// <summary>
	/// 初期化.
	/// </summary>
	public void Init(MainQuestCountryData[] countryDatas, bool bBoot = true, MainQuestBootEnum boot = MainQuestBootEnum.Country, bool bCheckNewRoot = false)
	{
		m_countryInfoList = new List<MainQuestCountryData>(countryDatas);
		m_bBoot = bBoot;
		m_bCheckNewRoot = bCheckNewRoot;
		m_bLandingThisView = false;

		// マップ設定.
		var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_QuestMap", this.transform.parent.gameObject);
		go.GetComponent<Canvas>().worldCamera = CameraHelper.SharedInstance.CameraQuestMap;
		go.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
		m_map = go.GetOrAddComponent<View_QuestMap>();
		m_map.Init();

		// Screenに連なるViewの操作をする必要があるので専用コントローラをアタッチ.
		m_viewCtrl = this.gameObject.GetOrAddComponent<MainQuestViewController>();
		m_viewCtrl.Init(this.GetScript<RectTransform>("QuestListRoot").gameObject, countryDatas, CallbackInfoDecided, CallbackWillDecideInfo);

		View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;

		// CurrentQuest == nullは起動して初回遷移.
		var fixBoot = boot;
		if (AwsModule.ProgressData.IsFirstBoot) {
			fixBoot = MainQuestBootEnum.Country;
		} else {
			if (m_bBoot) {
				fixBoot = this.FixBoot();
			} else {
				m_viewCtrl.ForceUpdateDecideInfo(fixBoot, AwsModule.ProgressData.CurrentQuest);
			}
		}

		// Debug.
		this.GetScript<RectTransform>("DebugContents").gameObject.SetActive(DefinePlayerSettings.PRODUCT_NAME == "DevSeven");
		this.SetCanvasCustomButtonMsg("bt_EffectConfirm", DidTaoEffectConfirm);
		this.SetCanvasCustomButtonMsg("bt_FujiReview", DidTapFujiReview);

		// TODO : フェードを開ける.ローンチ段階ではサブクエストは入らない.
        m_viewCtrl.ChangeView(fixBoot, () => {
            if(m_bBoot) {
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, CheckBootMainQuestFromRoot);
            } else {
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, CheckBackToMainQuest);
            }
        });
	}
	// 起動Viewのモードを修正.
	private MainQuestBootEnum FixBoot()
	{
		// 前回選択したクエスト基準で途中のViewをスキップする仕様.
		var current = AwsModule.ProgressData.PrevSelectedQuest;
		if (current == null) {
			return MainQuestBootEnum.Country;
		}
		Debug.Log("FixBoot : " + current.Country.name+" "+current.ChapterNum+"章 ステージ"+current.StageNum+" クエスト"+current.QuestNum);
		// TODO :サブシーンは更新も移動しない.なんらか考えておいた方が良さそう.
		if (current.QuestType != 1) {
			return MainQuestBootEnum.SubScene;
		}
		// 国クリア.
		var info = m_countryInfoList.Find(i => i.MainQuestCountry == (int)current.Country.Enum);
		var next = MasterDataTable.quest_main.GetNextQuestInfo(current.ID);
		if (next == null && !info.IsClear) {
			return MainQuestBootEnum.Country;
		}
		m_viewCtrl.ForceUpdateDecideInfo(MainQuestBootEnum.Stage, current);

		// ステージ or クエスト更新.
		return MainQuestBootEnum.Stage;
	}

	// メインクエスト画面をルートから起動した際のチェック.
	private void CheckBootMainQuestFromRoot()
	{
		if (AwsModule.ProgressData.IsFirstBoot) {
			return;
		}
		var userProgress = AwsModule.ProgressData;
		if (userProgress.PrevSelectedQuest == null) {
			return;
		}

		// あらすじチェック.
		if (userProgress.CheckViewMainQuestSummary()) {
			View_PlayerMenu.CreateIfMissing().IsEnableButton = false;
			View_GlobalMenu.CreateIfMissing().IsEnableButton = false;
			View_MainQuestSummary.Create(() => {
				View_PlayerMenu.CreateIfMissing().IsEnableButton = true;
				View_GlobalMenu.CreateIfMissing().IsEnableButton = true;
			});
		}
	}
	// この画面にシナリオなどから戻って来た際に呼び出される処理.
	private void CheckBackToMainQuest()
	{
		var userProgress = AwsModule.ProgressData;
		if (userProgress.CurrentQuest == null) {
			return;
		}

		// 1. 各種達成報酬.
		View_PlayerMenu.CreateIfMissing().IsEnableButton = false;
		View_GlobalMenu.CreateIfMissing().IsEnableButton = false;
		this.CheckRewardItem(() => {

			// 2. 次章出現演出 or 国クリア演出    
			var currenQuest = userProgress.CurrentQuest as MainQuest;
			if (currenQuest != null) {
				var latestClearedQuest = userProgress.GetLatestClearMainQuest(currenQuest.Country);            
				var prevClearedQuest = MasterDataTable.quest_main.IsLastQuestInCountry(currenQuest) ? currenQuest : MasterDataTable.quest_main.GetPrevQuest(currenQuest);
                var bLatestClear = prevClearedQuest != null && latestClearedQuest != null && latestClearedQuest.id == prevClearedQuest.id;
				if (!bLatestClear) {
					if(m_map.DetailMap != null){
						m_map.DetailMap.ForcusCamera(currenQuest, false, () => m_map.DetailMap.ChangeMarkerPostion(currenQuest));   // 過去クエストプレイなのでルート解放しない.
					}               
					AwsModule.ProgressData.RewardItemForClearMainQuestQuest = null;
                    AwsModule.ProgressData.RewardItemForClearMainQuestStage = null;
                    AwsModule.ProgressData.RewardItemForClearMainQuestChapter = null;
					View_PlayerMenu.CreateIfMissing().IsEnableButton = true;
					View_GlobalMenu.CreateIfMissing().IsEnableButton = true;
					return;
				}

				// ※3. 国出現.(コールバック)
				Debug.Log("m_bCheckNewRoot=" + m_bCheckNewRoot);
				Action openCountry = () => {
					if (latestClearedQuest.release_quest_id <= 0) {
						if (m_map.DetailMap != null) {
							m_map.DetailMap.ForcusCamera(currenQuest, m_bCheckNewRoot);
						}
						m_bCheckNewRoot = false;
						View_PlayerMenu.CreateIfMissing().IsEnableButton = true;
						View_GlobalMenu.CreateIfMissing().IsEnableButton = true;
						return;
					}
					var openQuest = MasterDataTable.quest_main[latestClearedQuest.release_quest_id];
					if (AwsModule.ProgressData.SeenCountryReleaseEffectList.Contains(openQuest.Country.Enum)) {
						if (m_map.DetailMap != null) {
							m_map.DetailMap.ForcusCamera(currenQuest, m_bCheckNewRoot);
						}
						m_bCheckNewRoot = false;
						View_PlayerMenu.CreateIfMissing().IsEnableButton = true;
						View_GlobalMenu.CreateIfMissing().IsEnableButton = true;
						return;
					}
					var data = MasterDataTable.quest_main_release_country_info.DataList.Find(i => i.country.Enum == openQuest.Country.Enum);
					var unlockEff = View_MainQuestUnlock.Create(data);
					unlockEff.PlayInOutAnimation(true, null, () => {
						unlockEff.PlayInOutAnimation(false, () => {
							unlockEff.Dispose();
							View_CountryOpenPop.Create(openQuest.Country, () => {
								if (m_map.DetailMap != null) {
                                    m_map.DetailMap.ForcusCamera(currenQuest, m_bCheckNewRoot);
                                }
								m_bCheckNewRoot = false;
								View_PlayerMenu.CreateIfMissing().IsEnableButton = true;
                                View_GlobalMenu.CreateIfMissing().IsEnableButton = true;          
							});
						});
					});
					AwsModule.ProgressData.UpdateSeenCountryReleaseEffectList(openQuest.Country.Enum);
				};

				// 次章出現演出.(国内部で)            
				if (latestClearedQuest != null) {
					var nextChap = MasterDataTable.quest_main_chapter_info.GetNextChapter(latestClearedQuest.Country, latestClearedQuest.stage_info.chapter_info);
					if (nextChap != null) {
						var bSeenChapterRelease = AwsModule.ProgressData.SeenChapterReleaseEffectList.Exists(c => c.country.Enum == nextChap.country.Enum && c.chapter == nextChap.chapter);
						if (MasterDataTable.quest_main.IsLastQuestInChapter(latestClearedQuest) && !bSeenChapterRelease) {
							View_ChapterOpen.Create(nextChap, () => {
								openCountry();
								AwsModule.ProgressData.RewardItemForClearMainQuestQuest = null;
								AwsModule.ProgressData.RewardItemForClearMainQuestStage = null;
								AwsModule.ProgressData.RewardItemForClearMainQuestChapter = null;
							});
							return;
						}
					}

					// 国クリア演出.
					var bSeenCountryClear = AwsModule.ProgressData.SeenCountryClearEffectList.Contains(latestClearedQuest.Country.Enum);
					if (MasterDataTable.quest_main.IsLastQuestInCountry(latestClearedQuest) && !bSeenCountryClear) {
						View_CountryClear.Create(currenQuest.Country, () => {
							openCountry();
							AwsModule.ProgressData.RewardItemForClearMainQuestQuest = null;
							AwsModule.ProgressData.RewardItemForClearMainQuestStage = null;
							AwsModule.ProgressData.RewardItemForClearMainQuestChapter = null;
						});
						return;
					}

                    // 初回クリア報酬がある時だけ出す
                    if(latestClearedQuest != null && AwsModule.ProgressData.RewardItemForClearMainQuestQuest != null) {
                        if(latestClearedQuest.Country.Enum == BelongingEnum.Varm && latestClearedQuest.ChapterNum == 2 && latestClearedQuest.StageNum == 3 && latestClearedQuest.QuestNum == 1) {
                            View_Popup_Review.Create(() => {
                                openCountry();
                                AwsModule.ProgressData.RewardItemForClearMainQuestQuest = null;
                                AwsModule.ProgressData.RewardItemForClearMainQuestStage = null;
                                AwsModule.ProgressData.RewardItemForClearMainQuestChapter = null;
                            });
                            return;
                        }
                    }
				}
				openCountry();
			}

			AwsModule.ProgressData.RewardItemForClearMainQuestQuest = null;
			AwsModule.ProgressData.RewardItemForClearMainQuestStage = null;
			AwsModule.ProgressData.RewardItemForClearMainQuestChapter = null;

			// TODO : サブシナリオ解放チェック.
			//this.CheckReleaseSubScenario();
		});
	}
	// クリア報酬チェック
	private void CheckRewardItem(Action didCheck)
	{
		var userProgress = AwsModule.ProgressData;
		var current = userProgress.CurrentQuest;
		if (current == null) {
			didCheck();
			return;
		}
		var clearQuest = userProgress.GetLatestClearMainQuest(current.Country);
		if (clearQuest == null) {
			didCheck();
			return;
		}

		// 1. クエスト達成報酬.      
		if (userProgress.RewardItemForClearMainQuestQuest != null) {
			Debug.Log("クエスト達成報酬");
			View_QuestRewardItemPop.Create(QuestRewardType.Quest, clearQuest, userProgress.RewardItemForClearMainQuestQuest, () => {
				// 2. ステージ達成報酬.
				if (userProgress.RewardItemForClearMainQuestStage != null) {
					Debug.Log("ステージ達成報酬");
					View_QuestRewardItemPop.Create(QuestRewardType.Stage, clearQuest, userProgress.RewardItemForClearMainQuestStage, () => {
						// 3. クエスト達成報酬.
						if (userProgress.RewardItemForClearMainQuestChapter != null) {
							Debug.Log("章クリア達成報酬");
							View_QuestRewardItemPop.Create(QuestRewardType.Chapter, clearQuest, userProgress.RewardItemForClearMainQuestChapter, () => {
								didCheck();
							});
						} else {
							didCheck();
						}
					});
				} else {
					didCheck();
				}
			});
		} else {
			didCheck();
		}
	}

	// TODO : サブシナリオ解放チェック.UserProgressでやるようにする.ローンチではサブクエストは入らない.
	private void CheckReleaseSubScenario()
	{
		var list = AwsModule.ProgressData.GetReleaseSubQuestList();
		if (list == null || list.Count <= 0) {
			return;
		}
		View_SubScenarioOpenPop.Create(list, subQuest => {
			m_viewCtrl.ForceUpdateDecideInfo(MainQuestBootEnum.SubScene, subQuest);
			m_viewCtrl.ChangeView(MainQuestBootEnum.SubScene);
		});
	}

	#region Callbacks.

	// コールバック : 情報が決まる直前.
	void CallbackWillDecideInfo(QuestDecideInfo info, Action didProcEnd)
	{
		// 国情報があれば国ごとのBGMを鳴らす.
		if (info.Belonging != null) {
			var clip = GetSoundClip(info.Belonging);
			SoundManager.SharedInstance.DownloadResource(clip, () => SoundManager.SharedInstance.PlayBGM(clip, true));
		} else if (AwsModule.ProgressData.CurrentQuest != null) {
			var clip = GetSoundClip(AwsModule.ProgressData.CurrentQuest.Country);
			SoundManager.SharedInstance.DownloadResource(clip, () => SoundManager.SharedInstance.PlayBGM(clip, true));
		}

		// 適宜フォーカス.
		if (this.DrawDetailMap(info, didProcEnd)) {
			return;
		}
		// 国選択へ一気に戻った際.
		if (m_map.IsForcusDetail && info.Belonging == null) {
			m_map.ResetZoomAndForcus();
		}
		didProcEnd();
	}
	private SoundClipName GetSoundClip(Belonging belonging)
	{
		if (belonging == null) {
			return SoundClipName.bgm009;
		}
		switch (belonging.Enum) {
			case BelongingEnum.Varm:
				return SoundClipName.bgm004;
			case BelongingEnum.Aldis:
				return SoundClipName.bgm002;
			case BelongingEnum.Jerida:
				return SoundClipName.bgm005;
			case BelongingEnum.Palladia:
				return SoundClipName.bgm007;
			case BelongingEnum.Ars:
				return SoundClipName.bgm003;
			case BelongingEnum.Liber:
				return SoundClipName.bgm006;
			case BelongingEnum.Amatu:
				return SoundClipName.bgm008;
		}
		return SoundClipName.bgm001;
	}

	// コールバック : 情報が決まるごと呼ばれる.
	void CallbackInfoDecided(QuestDecideInfo info)
	{
		if (info.IsMain) {
			CallbackMainQuestDecided(info);
		} else {
			CallbackSubQuestDecided(info);
		}
	}
	void CallbackMainQuestDecided(QuestDecideInfo info)
	{
		// 国選択時.ズーム演出.
		if (!m_map.IsZoom && info.Belonging != null) {
			m_map.PlayZoomAnimation(true, info.Belonging);
		} else if (m_map.IsZoom && info.Belonging == null) {
			m_map.PlayZoomAnimation(false);
		}

		// 章設定があれば詳細マップに切り替え.
		if (!m_map.IsForcusDetail && info.Belonging != null && info.ChapterNum > 0) {
			var bLatest = AwsModule.ProgressData.IsLatestMainQuest(info.Belonging, info.ChapterNum);
			var quest = bLatest ? AwsModule.ProgressData.GetPlayableLatestMainQuest(info.Belonging) : MasterDataTable.quest_main_stage_info.GetDefaultMainQuest(info);
			m_map.DrawAndForcusDetailMap(quest);
		} else if (m_map.IsZoom && m_map.IsForcusDetail && info.Belonging != null && info.ChapterNum <= 0) {
			m_map.ReleaseDetailMap(info.Belonging);
		}
		// フォーカス状態でのステージ切り替え.
		if (m_bLandingThisView && info.StageNum > 0) {
			var bLatest = AwsModule.ProgressData.IsLatestMainQuest(info.Belonging, info.ChapterNum, info.StageNum);
			var quest = bLatest ? AwsModule.ProgressData.GetPlayableLatestMainQuest(info.Belonging) : MasterDataTable.quest_main_stage_info.GetDefaultMainQuest(info, info.StageNum);
			m_map.DetailMap.ForcusCamera(quest, false, () => m_map.DetailMap.ChangeMarkerPostion(quest));
		}
		m_bLandingThisView = true;
	}
	void CallbackSubQuestDecided(QuestDecideInfo info)
	{
		if (m_map.IsZoom) {
			m_map.PlayZoomAnimation(false);
		}

		// TODO : var subQuest = MasterDataTable.quest_sub.DataList.Find(q => q.act == info.ActNum && q.scene == info.SceneNum);
		this.GetScript<TextMeshProUGUI>("txtp_SelectCountry").text = "サブクエスト";
		this.GetScript<TextMeshProUGUI>("txtp_SelectChapter").text = ""; // TODO : info.ActNum > 0 && subQuest != null ? MasterDataTable.quest_sub_name[subQuest.index].name: "";
		this.GetScript<TextMeshProUGUI>("txtp_SelectAct").text = "";
	}

	void DidTaoEffectConfirm()
	{
		this.gameObject.SetActive(false);
		m_map.gameObject.SetActive(false);
		View_MainQuest_Debug.CreateEffectCheck(() => {
			this.gameObject.SetActive(true);
			m_map.gameObject.SetActive(true);
		});
	}
	void DidTapFujiReview()
	{
		this.gameObject.SetActive(false);
		m_map.gameObject.SetActive(false);
		View_MainQuest_Debug.CreateFujiReview(() => {
			this.gameObject.SetActive(true);
			m_map.gameObject.SetActive(true);
		});
	}
	#endregion

	// カメラ : 詳細マップ描画.
	private bool DrawDetailMap(QuestDecideInfo info, Action didDrawMap = null)
	{
		// クエスト選択済みの状態であればフォーカスしている状態で遷移.クエストクリア後の自動遷移では地点描画はしない.解放演出側で行う.   
		if (!m_map.IsForcusDetail && info.Belonging != null && info.ChapterNum > 0 && info.IsMain && info.StageNum > 0 && info.QuestNum > 0) {
			var quest = MasterDataTable.quest_main.DataList.Find(q => q.Country.Enum == info.Belonging.Enum &&
																 q.ChapterNum == info.ChapterNum &&
																 q.StageNum == info.StageNum &&
																 q.QuestNum == info.QuestNum);
			Debug.Log(quest.id+" : " + quest.Country.name + " " + quest.ChapterNum + "章 " + quest.StageNum + "ステージ" + quest.QuestNum + "クエスト");
			var bDrawAllRoot = m_bBoot || !AwsModule.ProgressData.IsLatestMainQuest(quest.ID);
			m_map.DrawAndForcusDetailMap(quest, m_bCheckNewRoot, didDrawMap);
			return true;
		}
		// 起動直後 or 過去クエストプレイの場合以外は地点描画まで行わない.別シナリオ再生後、戻ってきて再選択した場合はDidのコールバック側でステージ切り替えとして処理しそちらで地点描画がなされる.
		if (!m_map.IsForcusDetail && info.Belonging != null && info.ChapterNum > 0 && info.IsMain && info.StageNum > 0) {
			Debug.Log(info.Belonging.name + " " + info.ChapterNum + "章 " + info.StageNum + "ステージ");
			var bLatest = AwsModule.ProgressData.IsLatestMainQuest(info.Belonging, info.ChapterNum, info.StageNum);
			var quest = bLatest ? AwsModule.ProgressData.GetPlayableLatestMainQuest(info.Belonging) : MasterDataTable.quest_main_stage_info.GetDefaultMainQuest(info);
			m_map.DrawAndForcusDetailMap(quest, m_bCheckNewRoot, didDrawMap);
			return true;
		}
		// MEMO : 起動直後以外という前提で、クエストクリア後章だけ選択している状態=新章解放 他章だけ選択している状況=クエスト選択後戻ってきて再選択した場合
		if (!m_map.IsForcusDetail && info.Belonging != null && info.ChapterNum > 0 && info.IsMain) {
			Debug.Log(info.Belonging.name + " " + info.ChapterNum + "章 ");
			var bLatest = AwsModule.ProgressData.IsLatestMainQuest(info.Belonging, info.ChapterNum);
			var quest = bLatest ? AwsModule.ProgressData.GetPlayableLatestMainQuest(info.Belonging) : MasterDataTable.quest_main_stage_info.GetDefaultMainQuest(info);
			m_map.DrawAndForcusDetailMap(quest, m_bCheckNewRoot, didDrawMap);
			return true;
		}
		return false;
	}

	void Awake()
	{
		var canvas = this.gameObject.GetOrAddComponent<Canvas>();
		if (canvas != null) {
			canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
		}
	}

	private bool m_bBoot;
	private bool m_bCheckNewRoot;
	private bool m_bLandingThisView = false;     // この選択画面に着地したかどうか.
	private List<MainQuestCountryData> m_countryInfoList;
	private View_QuestMap m_map;
	private Animation m_animCountryDetail;
	private IViewMainQuest m_currentView;
	private MainQuestViewController m_viewCtrl;
}
