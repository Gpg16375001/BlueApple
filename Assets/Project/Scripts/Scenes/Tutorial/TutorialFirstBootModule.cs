using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using TMPro;


/// <summary>
/// チュートリアル：マイページの初回起動からのチュートリアス.
/// </summary>
public class TutorialFirstBootModule : ViewBase
{
	public enum ViewMode
	{
		Blank,          // 無指定.これを指定してCreateIfMissingをする場合は後から個別でここのViewを作ること.
		StorySelect,    // マイページでストーリを押す.
        CountrySelect,  // メインクエスト選択画面でヴェルムを押す.
		ChapterSelect,  // メインクエスト選択画面でヴェルム1章を押す.
		StageSelect,    // メインクエスト選択画面でヴェルム1章1ステージを押す.
		QuestSelect,    // メインクエスト選択画面でヴェルム1章1ステージ1クエストを押す.
		FriendSelect,    // 出撃前サポート選択画面で一番上のサポートを押す.
        Sortie,         // 出撃
	}
   
    /// <summary>
    /// なければ生成.
    /// </summary>
	public static TutorialFirstBootModule CreateIfMissing(ViewMode mode, params ViewBase[] views)
	{
		if(instance == null){
			var go = new GameObject("TutorialFirstBootModule");
			instance = go.GetOrAddComponent<TutorialFirstBootModule>();
		}
		instance.InitInternal(mode, views);
		return instance;
	}
	private void InitInternal(ViewMode mode, params ViewBase[] views)
	{
		m_controllViewList = views;
  
		switch(mode){
			case ViewMode.StorySelect:
				this.CreateStorySelect();
				break;
			case ViewMode.CountrySelect:
				this.CreateCountrySelect();
				break;
			case ViewMode.ChapterSelect:
				this.CreateChapterSelect();
				break;
			case ViewMode.StageSelect:
				this.CreateStageSelect();
                break;
			case ViewMode.QuestSelect:
				this.CreateQuestSelect();
                break;
			case ViewMode.FriendSelect:
				this.CreateFriendSelect();
                break;
			case ViewMode.Sortie:
				this.CreateSortie();
                break;
		}
	}

    /// <summary>
    /// インスタンスの破棄.
    /// </summary>
    public static void DestroyInstance()
	{
		if(instance != null){
			instance.Dispose();
		}      
	}   
	public override void Dispose()
	{
		if (m_controllViewList != null && m_controllViewList.Length > 0) {
            foreach (var view in m_controllViewList) {
                view.IsEnableButton = true;
				foreach (var inputField in view.GetComponentsInChildren<InputField>(true)) {
                    inputField.interactable = true;
                }
            }
        }
		base.Dispose();
	}

	/// <summary>
	/// トップで再生するシナリオロード&スタート.
	/// </summary>
	public void LoadAndStartFirstScenario()
	{
		UtageModule.SharedInstance.SetActiveCore(true);
        UtageModule.SharedInstance.LoadUseChapter("Tutorial", () => {
            Debug.Log("[TutorialFirstBootModule] Utage Load Success.");         
			UtageModule.SharedInstance.StartScenario("tuto_7", () => {
				Debug.Log("[TutorialFirstBootModule] tuto_7 scenario end.");            
				UtageModule.SharedInstance.SetActiveCore(false);
			});         
		}, "tuto_1", "tuto_7");
	}

	#region CreateView

	/// ストーリー選択
	public void CreateStorySelect()
	{
		if(m_currentViewObj != null){
			GameObject.Destroy(m_currentViewObj);         
		}
		m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialMyPage_Story");

        // ボタンの入力制御.
		foreach(var view in m_controllViewList){
			view.IsEnableButton = false;
		}
		foreach (var view in m_controllViewList) {
            if (view.Exist<CustomButton>("bt_GlobalMenuStory")) {
                view.GetScript<CustomButton>("bt_GlobalMenuStory").interactable = true;
            }
        }
	}
    /// メインクエストの国選択
	public void CreateCountrySelect()
	{
		if (m_currentViewObj != null) {
            GameObject.Destroy(m_currentViewObj);
        }
        m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialMainQuest_Country");

		// ボタンの入力制御.
        foreach (var view in m_controllViewList) {
            view.IsEnableButton = false;
        }
		foreach (var view in m_controllViewList) {
			if (view.Exist<CustomButton>("Country4/bt_CountrySelect")) {
				view.GetScript<CustomButton>("Country4/bt_CountrySelect").interactable = true;
				break;
            }
        }
	}
	/// メインクエストの章選択
	public void CreateChapterSelect()
    {
        if (m_currentViewObj != null) {
            GameObject.Destroy(m_currentViewObj);
        }
        m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialMainQuest_Chapter");
  
        foreach (var view in m_controllViewList) {
            view.IsEnableButton = false;
        }
		foreach(var view in m_controllViewList){
			if(view.Exist<CustomButton>("btn_Other")){
				view.GetScript<CustomButton>("btn_Other").interactable = true;
				break;
			}
		}
    }

	// メインクエストのステージ選択
	public void CreateStageSelect(ListItem_MainQuest forcusTarget = null)
    {
        if (m_currentViewObj != null) {
            GameObject.Destroy(m_currentViewObj);
        }
        m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialMainQuest_Stage");

		if(forcusTarget != null){
			var v = m_currentViewObj.GetOrAddComponent<ViewBase>();
			var com = v.GetScript<RectTransform>("ListItem_MainQuest_Stage").gameObject.GetOrAddComponent<ListItem_MainQuest>();
            // 情報の整合を手打ちしていく.
			com.GetScript<TextMeshProUGUI>("txtp_Stage").text = forcusTarget.StageInfo.stage_name;
			com.GetScript<TextMeshProUGUI>("txtp_StageNum").text = forcusTarget.StageInfo.stage.ToString();
            // ミッション
			var allQuestList = MasterDataTable.quest_main.GetQuestList(forcusTarget.StageInfo.chapter_info.country, forcusTarget.StageInfo.chapter_info.chapter, forcusTarget.StageInfo.stage);
			for (var i = 1; i <= STAGE_MISSION_MAX; ++i) { 
				var spt = this.GetScript<uGUISprite>("img_StageMissionClearIcon_" + i);
                spt.gameObject.SetActive(i <= allQuestList.Count);
			}
		}

		foreach (var view in m_controllViewList) {
            view.IsEnableButton = false;
        }      
        // ※ステージ選択はボタンじゃないのでほっておいてもステージ選択しか押せなくなる.
    }
	private const int STAGE_MISSION_MAX = 6;    // デザイナーの方で暫定配置しているミッションの数.

	/// メインクエストのクエスト選択
	public void CreateQuestSelect(IQuestData questData = null)
    {
        if (m_currentViewObj != null) {
            GameObject.Destroy(m_currentViewObj);
        }
        m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialMainQuest_Quest");

		if(questData != null){
			var v = m_currentViewObj.GetOrAddComponent<ViewBase>();
            var com = v.GetScript<RectTransform>("ListItem_MainQuest_Quest").gameObject.GetOrAddComponent<ListItem_MainQuest>();
			com.GetScript<TextMeshProUGUI>("txtp_Quest").text = "クエスト "+questData.QuestNum;
			com.GetScript<TextMeshProUGUI>("txtp_QuestNum").text = questData.QuestNum.ToString();
			com.GetScript<TextMeshProUGUI>("txtp_OpenStageNum").text = questData.StageNum.ToString();
			com.GetScript<TextMeshProUGUI>("txtp_APNum").text = questData.NeedAP.ToString();
			com.GetScript<TextMeshProUGUI>("txtp_APNum").text = questData.NeedAP.ToString();
			// ミッション.
			var mission = MasterDataTable.battle_mission_setting.DataList.Find(s => s.stage_id == questData.BattleStageID);
            com.GetScript<Transform>("MissionGrid").gameObject.SetActive(mission != null);
			if (mission != null) { 
				var missionList = new List<int>();
                missionList.Add(mission.condition_1 ?? -1);
                missionList.Add(mission.condition_2 ?? -1);
                missionList.Add(mission.condition_3 ?? -1);
				com.GetScript<Transform>("MissionGrid").gameObject.SetActive(missionList.Count > 0);
				for (var i = 1; i <= missionList.Count; ++i) { 
					com.GetScript<uGUISprite>("img_MissionClearIcon_" + i).ChangeSprite("img_MissionClearIconOff");
				}
			}
			// 初回報酬.
			var mainQuest = MasterDataTable.quest_main[questData.ID];
            var iconInfo = mainQuest.reward_item_type.Enum.GetIconInfo(mainQuest.reward_item_id);
            var sprite = com.GetScript<uGUISprite>("ItemIcon");
            com.GetScript<Image>("img_RewardGet").gameObject.SetActive(false);
            com.GetScript<Transform>("Item").gameObject.SetActive(iconInfo.IsEnableSprite);
            if (iconInfo.IsEnableSprite) {
                sprite.LoadAtlasFromResources(iconInfo.AtlasName, iconInfo.SpriteName);
            } else if (iconInfo.IconObject != null) {
                iconInfo.IconObject.transform.SetParent(com.GetScript<Transform>("UnitWeaponRoot"));
            } 
		}

		foreach (var view in m_controllViewList) {
            view.IsEnableButton = false;
        }
		foreach (var view in m_controllViewList) {
			if(view.Exist<Image>("bt_Stage")){
				view.GetScript<Image>("bt_Stage").raycastTarget = false;    // ステージボタンを押せないように.
				break;
			}
        }
		foreach (var view in m_controllViewList) {
            if (view.Exist<Toggle>("Item 0: クエスト1")) {
                view.GetScript<Toggle>("Item 0: クエスト1").interactable = view.GetScript<Toggle>("Item 0: クエスト1").enabled = true;
				break;
            }
        }
    }
 
	/// 出撃前のフレンド選択
	public void CreateFriendSelect(ListItem_Friend forcusTarget = null)
    {
        if (m_currentViewObj != null) {
            GameObject.Destroy(m_currentViewObj);
        }
        m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialFriendSelect_Friend");
        
		if(forcusTarget != null){
			var v = m_currentViewObj.GetOrAddComponent<ViewBase>();
            var com = v.GetScript<RectTransform>("ListItem_Friend").gameObject.GetOrAddComponent<ListItem_Friend>();
            com.Init(forcusTarget.UserData, forcusTarget.CardData);
		}

		foreach (var view in m_controllViewList) {
            view.IsEnableButton = false;
        }
		foreach (var view in m_controllViewList) {
			if (view is ListItem_Friend) {
				if(view.transform.GetSiblingIndex() <= 0){
					view.IsEnableButton = true;
                    break;
				}            
            }
			var item = view.GetComponentsInChildren<ListItem_Friend>(true).FirstOrDefault(i => i.transform.GetSiblingIndex() <= 0);
			if(item != null){
				Debug.Log(item.CardData.Card.nickname);
				item.IsEnableButton = true;
				break;
			}
        }
    }
	/// 出撃
	public void CreateSortie()
    {
        if (m_currentViewObj != null) {
            GameObject.Destroy(m_currentViewObj);
        }
		m_currentViewObj = this.CreateViewObject("Tutorial/View_TutorialPartyEdit_BattleStart");

		foreach (var view in m_controllViewList) {
            view.IsEnableButton = false;                  
        }
		// 上記だけだと効くボタンがあったので更に再帰的に封じる.
		foreach (var view in m_controllViewList) {
			foreach (var btn in view.GetComponentsInChildren<CustomButton>(true)) {
				btn.interactable = false;
			}
			foreach (var inputField in view.GetComponentsInChildren<InputField>(true)) {
                inputField.interactable = false;
            }
		}
		foreach(var view in m_controllViewList){
			if(view.Exist<CustomButton>("bt_BattleStart")){
                // 保存されていないくてもこの場合は出撃できるようにする。
                view.GetScript<CustomButton> ("bt_PartySave").gameObject.SetActive (false);
                view.GetScript<CustomButton> ("bt_BattleStart").gameObject.SetActive (true);
				view.GetScript<CustomButton>("bt_BattleStart").interactable = true;
				break;
			}
		}
    }

    #endregion
 
	private GameObject CreateViewObject(string name)
	{
		var go = GameObjectEx.LoadAndCreateObject(name);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        go.transform.SetParent(this.transform);
		return go;
	}   

	private GameObject m_currentViewObj;
	private ViewBase[] m_controllViewList;

	private static TutorialFirstBootModule instance;
}
