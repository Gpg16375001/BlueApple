using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ListItem : メインクエスト画面で使用される汎用リストアイテム
/// </summary>
public class ListItem_MainQuest : ViewBase
{
	

    #region Country proc.
    /// <summary>
    /// 国用のリストとして初期化.
    /// </summary>
	public void InitCountry(Belonging belonging, bool bOpen, bool bNew, bool bCleared, Action<Belonging> didTap)
    {
        Belonging = belonging;
        m_didTapCountry = didTap;
		this.SetCanvasCustomButtonMsg("bt_CountrySelect", DidTapCountry);

		// TODO : バッジ.
		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		var listInCountry = MasterDataTable.quest_main.GetQuestList(belonging);
		if(bCleared){
			this.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(true);
			this.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(false);
		}else{
			this.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(false);
			this.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(bOpen && bNew);
		}      
    }

    // ボタン : 国タップ.
    void DidTapCountry()
    {
        if (m_didTapCountry != null) {
            m_didTapCountry(Belonging);
        }
    }

	public Belonging Belonging { get; private set; }    // 国情報.
    private Action<Belonging> m_didTapCountry;  // 国タップ.
    #endregion

    #region Chapter proc.
	/// <summary>
    /// チャプターリストとして初期化.
    /// </summary>
	public void InitChapter(MainQuestChapterInfo chapter, Action<MainQuestChapterInfo> didTap)
    {
		m_chapter = chapter;
		m_didTapChapter = didTap;
		this.GetScript<TextMeshProUGUI>("txtp_ChapterNum").text = chapter.chapter.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_Other").text = chapter.chapter_name;
        this.SetCanvasCustomButtonMsg("btn_Other", DidTapChapter);

        // バッジ.
		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(chapter.country);
		var listInChapter = MasterDataTable.quest_main.GetQuestList(chapter.country, chapter.chapter);
		var bCleared = true;
		foreach (var q in listInChapter) {
			bCleared = achiveQuests.Exists(a => a.QuestId == q.id);
			if (!bCleared) {
                break;
            }
        }
		var bCompleteMission = true;
		foreach(var q in listInChapter){
			bCompleteMission = AwsModule.ProgressData.IsCompletedBattleMission(q);
			if(!bCompleteMission){
				break;
			}
		}
		if(bCleared){
			if(bCompleteMission){
				this.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(false);
                this.GetScript<Image>("img_StoryIconComplete").gameObject.SetActive(true);
				this.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(false);
			}else{
				this.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(true);
				this.GetScript<Image>("img_StoryIconComplete").gameObject.SetActive(false);
				this.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(false);
			}         
		}else{
			this.GetScript<Image>("img_StoryIconClear").gameObject.SetActive(false);
            this.GetScript<Image>("img_StoryIconComplete").gameObject.SetActive(false);
			this.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(chapter.chapter == latest.ChapterNum);
		}
    }
    
    // ボタン : 章タップ.
    void DidTapChapter()
    {
        if (m_didTapChapter != null) {
			m_didTapChapter(m_chapter);
        }
    }
 
	private MainQuestChapterInfo m_chapter;
	private Action<MainQuestChapterInfo> m_didTapChapter;  // 章タップ.
    #endregion

    #region Stage proc.
    /// <summary>
    /// ステージに連なるクエストのドロップダウンリスト.
    /// </summary>
	public MainQuestListInDropDown StageDropListItem { get; private set; }

    /// <summary>
    /// ステージ情報.
    /// </summary>
	public MainQuestStageInfo StageInfo { get; private set; }

    /// <summary>
    /// ステージリストとして初期化.
    /// </summary>
	public void InitStage(MainQuestStageInfo stageInfo, List<IQuestData> questList, Action<MainQuestStageInfo> didTapStage, Action<IQuestData> didTapQuest, Action didCreateAndOpenList = null, bool bDefaultOpen = false)
    {
		StageInfo = stageInfo;
		m_questList = questList;
		m_didTapStage = didTapStage;
		m_didTapQuest = didTapQuest;
		m_bOpenStageList = bDefaultOpen;
		this.GetScript<TextMeshProUGUI>("txtp_StageNum").text = StageInfo.stage.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_Stage").text = StageInfo.stage_name;

		// リスト準備. 
		StageDropListItem = this.GetComponent<MainQuestListInDropDown>();
		StageDropListItem.options.Clear();
		StageDropListItem.AddOptions(m_questList, didCreateAndOpenList);      
        // コールバックイベント類.
		StageDropListItem.RootButtonName = "bt_Stage";
		StageDropListItem.onClickRoot = DidTapStage;
		StageDropListItem.onValueChanged.AddListener(DidTapQuest);

		// クエスト情報まで設定があれば展開済みの状態にしておく.
        if (m_bOpenStageList) {
            StageDropListItem.Show();
        }

		// ステージ単位でのミッション達成状況.
		m_bCompletedMissionsInStage = true;
		var allQuestList = MasterDataTable.quest_main.GetQuestList(stageInfo.chapter_info.country, stageInfo.chapter_info.chapter, StageInfo.stage);
		for (var i = 1; i <= STAGE_MISSION_MAX; ++i){
			var spt = this.GetScript<uGUISprite>("img_StageMissionClearIcon_"+i);
			spt.gameObject.SetActive(i <= allQuestList.Count);
			if(i > allQuestList.Count){
				continue;	
			}
			var quest = allQuestList[i-1];
			var masterData = MasterDataTable.battle_mission_setting.DataList.Find(s => s.stage_id == quest.BattleStageID);
			if(masterData == null){
				spt.gameObject.SetActive(false);
				if (m_bCompletedMissionsInStage) {
                    m_bCompletedMissionsInStage = false;
                }
				continue;
			}
			var bClearedMission = AwsModule.ProgressData.IsCompletedBattleMission(quest);
			if (m_bCompletedMissionsInStage && !bClearedMission) {
                m_bCompletedMissionsInStage = false;
            }
			var sptName = bClearedMission ? "img_StageMissionClearIconOn" : "img_StageMissionClearIconOff";
            spt.ChangeSprite(sptName);
		}

        // バッジ.
		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		var listInStage = MasterDataTable.quest_main.GetQuestList(stageInfo.chapter_info.country, stageInfo.chapter_info.chapter, stageInfo.stage);      
		var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(stageInfo.chapter_info.country);
		m_bClearedStage = true;
		foreach(var q in listInStage){
			m_bClearedStage = achiveQuests.Exists(a => a.QuestId == q.id);
			if(!m_bClearedStage){
				Debug.Log(q.id + " : ステージ" + q.StageNum + " クエスト" + q.QuestNum + " is not clear yet.");
				break;
			}
		}
		if(m_bClearedStage){
			if(m_bCompletedMissionsInStage){
				this.GetScript<Image>("Icon/Clear").gameObject.SetActive(false);
                this.GetScript<Image>("Icon/Complete").gameObject.SetActive(true);         
				this.GetScript<Image>("Icon/New").gameObject.SetActive(false);
				this.GetScript<uGUISprite>("bt_Stage").ChangeSprite(m_bOpenStageList ? "bt_StageOpenComplete": "bt_StageCloseComplete");
			}else{
				this.GetScript<Image>("Icon/Clear").gameObject.SetActive(true);
                this.GetScript<Image>("Icon/Complete").gameObject.SetActive(false);
				this.GetScript<Image>("Icon/New").gameObject.SetActive(false);
				this.GetScript<uGUISprite>("bt_Stage").ChangeSprite(m_bOpenStageList ? "bt_StageOpen" :"bt_StageClose");
			}
		}else{
			this.GetScript<Image>("Icon/Clear").gameObject.SetActive(false);
            this.GetScript<Image>("Icon/Complete").gameObject.SetActive(false);         
			this.GetScript<Image>("Icon/New").gameObject.SetActive(latest.StageNum == stageInfo.stage);
			this.GetScript<uGUISprite>("bt_Stage").ChangeSprite(m_bOpenStageList ? "bt_StageOpen" : "bt_StageClose");
		}      
    }
	private const int STAGE_MISSION_MAX = 6;    // デザイナーの方で暫定配置しているミッションの数.

    // ボタン : ステージタップ.
	void DidTapStage()
    {
		if(!this.GetComponent<MainQuestListInDropDown>().interactable){
			return;
		}
		
		m_bOpenStageList = !m_bOpenStageList;
		if (m_bClearedStage) {
            if (m_bCompletedMissionsInStage) {
                this.GetScript<uGUISprite>("bt_Stage").ChangeSprite(m_bOpenStageList ? "bt_StageOpenComplete" : "bt_StageCloseComplete");
            } else {
				this.GetScript<uGUISprite>("bt_Stage").ChangeSprite(m_bOpenStageList ? "bt_StageOpen" : "bt_StageClose");
            }
        } else {
			this.GetScript<uGUISprite>("bt_Stage").ChangeSprite(m_bOpenStageList ? "bt_StageOpen" : "bt_StageClose");
        }

		if(m_didTapStage != null){
			m_didTapStage(StageInfo);
		}
    }
    
	// ボタン : クエストタップ.
    void DidTapQuest(int questIdx)
	{
		if(m_didTapQuest != null){
			m_didTapQuest(m_questList[questIdx]);
		}
	}
    
	private List<IQuestData> m_questList;
	private Action<MainQuestStageInfo> m_didTapStage;
	private Action<IQuestData> m_didTapQuest;
	private bool m_bOpenStageList = false;
	private bool m_bClearedStage = false;
	private bool m_bCompletedMissionsInStage = false;
    #endregion

    #region SubActProc
    /// <summary>
    /// サブクエスト幕リストとして初期化.
    /// </summary>
    public void InitSubAct(int questIndex, Action<int> didTap)
    {
        m_questIndex = questIndex;
        m_didTapSubAct = didTap;
        this.GetScript<TextMeshProUGUI>("txtp_Scene").text = MasterDataTable.quest_sub_name[m_questIndex].name;
        this.SetCanvasButtonMsg("bt_Scene", DidTapSubAct);
    }

    // ボタン : サブクエスト幕タップ.
    void DidTapSubAct()
    {
        if (m_didTapSubAct != null) {
            m_didTapSubAct(m_questIndex);
        }
    }

    private int m_questIndex;
    private Action<int> m_didTapSubAct;  // シーンタップ.
    #endregion

    #region SubActProc
    /// <summary>
    /// サブクエストシーンリストとして初期化.
    /// </summary>
    public void InitSubScene(SubQuest quest, string stageName, Action<SubQuest> didTap)
    {
        m_subQuest = quest;
        m_didTapSubScene = didTap;
        this.GetScript<TextMeshProUGUI>("txtp_Scene").text = stageName;
        this.GetScript<TextMeshProUGUI>("txtp_SceneNum").text = quest.scene.ToString();
        this.SetCanvasButtonMsg("bt_Scene", DidTapSubScene);
    }

    // ボタン : サブクエスト幕タップ.
    void DidTapSubScene()
    {
        if (m_didTapSubScene != null) {
            m_didTapSubScene(m_subQuest);
        }
    }

    private SubQuest m_subQuest;
    private Action<SubQuest> m_didTapSubScene;  // シーンタップ.
    #endregion
}
