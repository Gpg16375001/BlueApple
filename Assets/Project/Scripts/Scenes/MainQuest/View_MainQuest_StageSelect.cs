using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// View : メインクエスト画面の幕選択.
/// </summary>
public class View_MainQuest_StageSelect : ViewBase, IViewMainQuest
{
    /// <summary>
    /// 起動画面.
    /// </summary>
    public MainQuestBootEnum Boot { get { return m_boot; } }
    private MainQuestBootEnum m_boot;

    /// <summary>
    /// 幕番号.
    /// </summary>
    public int StageNum { get; private set; }


    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(MainQuestBootEnum boot, Belonging belogining, int chapter, int stage, Action<MainQuestStageInfo> didChangeStage, Action<MainQuestBootEnum> procChangeView)
    {
        m_boot = boot;
        m_belonging = belogining;
        m_chapterNum = chapter;
		m_stageNum = stage;
		m_didChangeStage = didChangeStage;
        m_procChangeView = procChangeView;
        this.CreateList();

		// 初回起動
		this.StartCoroutine(this.PlayAndWaitSlideInForTutorial());

		// ラベル
		var chapterInfo = MasterDataTable.quest_main_chapter_info.GetListThisCountry(belogining).Find(q => q.chapter == chapter);
		this.GetScript<TextMeshProUGUI>("txtp_SelectCountry").text = chapterInfo.country.name;
		this.GetScript<TextMeshProUGUI>("txtp_SelectChapterTitle").text = chapterInfo.chapter_name;
		this.GetScript<TextMeshProUGUI>("txtp_SelectChapter").text = chapterInfo.chapter.ToString()+"章";

		// ティッカー
		var ticker = this.GetScript<Transform>("Ticker").gameObject.GetOrAddComponent<View_MainQuestTicker>();
		ticker.Init();

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_BackCountrySelect", DidTapClose);
        this.SetCanvasButtonMsg("bt_BackList", DidTapBack);
		View_PlayerMenu.DidTapBackButton += DidTapBack;
    }

	private IEnumerator PlayAndWaitSlideInForTutorial()
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("MainQuestSlideIn");
        if (!AwsModule.ProgressData.IsFirstBoot) {
            yield break;
        }
		LockInputManager.SharedInstance.IsLock = true;
        do {
            yield return null;
        } while (anim.isPlaying);
		var module = TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.Blank, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
		module.CreateStageSelect(this.GetComponentsInChildren<ListItem_MainQuest>(true).FirstOrDefault(i => i.transform.GetSiblingIndex() <= 0));
        LockInputManager.SharedInstance.IsLock = false;
    }

    /// <summary>
    /// 破棄処理.
    /// </summary>
    public void Destroy()
    {
        this.Dispose();
    }
	public override void Dispose()
    {
        View_PlayerMenu.DidTapBackButton -= DidTapBack;
        base.Dispose();
    }

    // 幕一覧リスト作成.
    private void CreateList()
    {
		var achiveList = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
        var root = this.GetScript<ScrollRect>("StageScrollView").content.gameObject;
        root.DestroyChildren();
		var list = MasterDataTable.quest_main_stage_info.GetListThisCountryChapter(m_belonging, m_chapterNum);
		list.Sort((x, y) => y.id - x.id);
        var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(m_belonging);
		foreach (var info in list) {
			var bReleaseQuest = AwsModule.ProgressData.IsReleaseMainQuest(m_belonging, m_chapterNum, info.stage);
			if (!bReleaseQuest && info.stage > latest.StageNum) {
				continue;
            }         
			var qList = MasterDataTable.quest_main.DataList.FindAll(q => q.Country.Enum == m_belonging.Enum && q.ChapterNum == m_chapterNum && q.StageNum == info.stage);
			var firstQuestNum = qList.Select(i => i.QuestNum).Min();
			var firstQuest = qList.Find(i => i.QuestNum == firstQuestNum);
			var go = GameObjectEx.LoadAndCreateObject("MainQuest/ListItem_MainQuest_Stage", root);
            var c = go.GetOrAddComponent<ListItem_MainQuest>();         
			m_questList = MasterDataTable.quest_main
			                             .GetQuestList(m_belonging, m_chapterNum, info.stage)
                                         .Where(q => achiveList.Exists(a => a.QuestType == 1 && a.QuestId == q.id) || MasterDataTable.quest_main.GetFirstSceneIdList().Contains(q.id) || q.id == latest.id)
			                             .Select(q => q as IQuestData)
			                             .ToList();
			m_questList.Sort((x, y) => y.ID - x.ID);
			c.InitStage(info, m_questList, DidTapStage, DidTapQuest, DidCreateAndOpenList, info.stage == m_stageNum);
			c.GetComponentsInChildren<MainQuestListInDropDown>(true)[0].interactable = !firstQuest.is_force_lock;
            c.GetScript<TextMeshProUGUI>("txtp_Stage").gameObject.SetActive(!firstQuest.is_force_lock);
			c.GetScript<Image>("Icon/New").gameObject.SetActive(c.GetScript<Image>("Icon/New").gameObject.activeSelf && !firstQuest.is_force_lock);
			c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockStageNote").gameObject.SetActive(firstQuest.is_force_lock);
			c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockStageNote").enabled = c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockStageNote").interactable = firstQuest.is_force_lock;
			c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockStageNote").onClick.RemoveAllListeners();
			if (firstQuest.is_force_lock) {
				c.SetCanvasCustomButtonMsg("bt_ForceLockStageNote", () => PopupManager.OpenPopupOK("このステージはまだ解放されておりません。\nアップデートされるまで\nお待ちください。"));
            }
        }
    }

    #region ButtonDelegate.

    // ボタン : 閉じるボタン.
    void DidTapClose()
    {
		this.PlaySlideOut(() => m_procChangeView(MainQuestBootEnum.Country));
    }

    // ボタン : 戻るボタン.
    void DidTapBack()
    {
		this.PlaySlideOut(() => m_procChangeView(MainQuestBootEnum.Chapter));
    }
    
	// ボタン : ステージ選択.
	void DidTapStage(MainQuestStageInfo stageInfo)
	{
		this.StageNum = stageInfo.stage;
		foreach(var item in this.GetComponentsInChildren<ListItem_MainQuest>()){
			if(item.StageInfo.id != stageInfo.id){
				item.StageDropListItem.HideTouchRoot();
			}         
		}
		if(m_didChangeStage != null){
			m_didChangeStage(stageInfo);
		}      
	}
	void DidCreateAndOpenList()
	{
		if (AwsModule.ProgressData.IsFirstBoot) {
			var module = TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.Blank, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
			module.CreateQuestSelect(m_questList.FirstOrDefault());
        }
	}

    // ボタン : クエスト選択.
	void DidTapQuest(IQuestData quest)
    {
		if (AwsModule.ProgressData.IsFirstBoot) {
			TutorialFirstBootModule.DestroyInstance();
		}

		// その国のキャラクターの情報更新確認用データを取得しておく.クエストで解放される要因があるため.
        foreach (var card in CardData.CacheGetAll().Where(c => c.Card.release_chapter_flavor2 != null && c.Card.release_chapter_flavor2.country.Enum == quest.Country.Enum)) {
			AwsModule.CardModifiedData.UpdateData(card);
        }

		this.PlaySlideOut(() => {
			AwsModule.ProgressData.CurrentQuest = AwsModule.ProgressData.PrevSelectedQuest = quest;

            var achiemnet = QuestAchievement.CacheGetAll().Where(a => a.QuestType == quest.QuestType && a.QuestId == quest.ID).FirstOrDefault();
            AwsModule.ProgressData.CurrentQuestAchievedMissionIdList =  achiemnet.AchievedMissionIdList;

			AwsModule.BattleData.SetStage(AwsModule.ProgressData.CurrentQuest.BattleStageID);
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToScenario();
            });
        });
    }

    #endregion

	private void PlaySlideOut(Action didEnd)
    {
        this.StartCoroutine(this.CoPlaySlideOut(didEnd));
    }
    private IEnumerator CoPlaySlideOut(Action didEnd)
    {
		LockInputManager.SharedInstance.IsLock = true;
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("MainQuestSlideOut");
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didEnd != null) {
            didEnd();
        }
		LockInputManager.SharedInstance.IsLock = false;
    }

    private Belonging m_belonging;
    private int m_chapterNum;
	private int m_stageNum;
	private List<IQuestData> m_questList;
	Action<MainQuestStageInfo> m_didChangeStage;
    Action<MainQuestBootEnum> m_procChangeView;
}
