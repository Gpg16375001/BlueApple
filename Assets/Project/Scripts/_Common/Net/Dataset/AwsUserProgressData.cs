using System;
using System.Linq;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// AWS SDK "Cognito"を利用したユーザーのゲーム進行データ.
/// ※バトル関連のデータはBattleProgressに任せている点に注意！
/// </summary>
public class AwsUserProgressData : AwsCognitoDatasetBase
{
	/// <summary>
    /// 初回起動かどうか.
    /// </summary>
	public bool IsFirstBoot
	{
		get { return Get<bool>("IsFirstBoot"); }
	    set { Put("IsFirstBoot", value); }
	}
	
	/// <summary>
	/// チュートリアル進行状況値.-1で終了.
	/// </summary>
	public int TutorialStageNum
	{
		get { return Get<int>("TutorialStageNum"); }
		private set { Put("TutorialStageNum", value); }
	}

    /// <summary>
    /// 0は未選択、1は最小限、2は全部
    /// </summary>
    public int SelectDLCType
    {
        get { return Get<int>("SelectDLCType"); }
        private set { Put("SelectDLCType", value); }
    }
    
	/// <summary>
    /// 既に見ている章解放演出のリスト.
    /// </summar
	public List<MainQuestChapterInfo> SeenChapterReleaseEffectList
    {
        get {
			var json = Get<string>("SeenChapterReleaseEffectList");
            if (string.IsNullOrEmpty(json)) {
				var listEmpty = new List<MainQuestChapterInfo>();
				Put("SeenChapterReleaseEffectList", new Serialization<MainQuestChapterInfo>(listEmpty));
                return listEmpty;
            }
		    return UnityEngine.JsonUtility.FromJson<Serialization<MainQuestChapterInfo>>(json).ToList();
        }
    }

	/// <summary>
    /// 既に見ている国クリア演出のリスト.
    /// </summar
	public List<BelongingEnum> SeenCountryClearEffectList
    {
		get {
			var json = Get<string>("SeenCountryClearEffectList");
            if (string.IsNullOrEmpty(json)) {
                var listEmpty = new List<BelongingEnum>();
				Put("SeenCountryClearEffectList", new Serialization<int>(listEmpty.Select(b => (int)b).ToList()));
                return listEmpty;
            }
            return UnityEngine.JsonUtility.FromJson<Serialization<int>>(json).ToList().Select(num => (BelongingEnum)num).ToList();
        }
    }

	/// <summary>
    /// 既に見ている国解放演出の国リスト.
    /// </summar
	public List<BelongingEnum> SeenCountryReleaseEffectList
	{
		get {
			var json = Get<string>("SeenCountryReleaseEffectList");
            if (string.IsNullOrEmpty(json)) {
                var listEmpty = new List<BelongingEnum>();
				Put("SeenCountryReleaseEffectList", new Serialization<int>(listEmpty.Select(b => (int)b).ToList()));
                return listEmpty;
            }
            return UnityEngine.JsonUtility.FromJson<Serialization<int>>(json).ToList().Select(num => (BelongingEnum)num).ToList();
        }
	}

	/// <summary>
	/// 既に見ているシナリオ演出のクエストIDリスト.
	/// </summary>
	public List<int> SeenScenarioEffectIdList
	{
		get {
			var json = Get<string>("SeenScenarioEffectIdList");
			if (string.IsNullOrEmpty(json)) {
				var listEmpty = new List<int>();
				Put("SeenScenarioEffectIdList", new Serialization<int>(listEmpty));
				return listEmpty;
			}
			return UnityEngine.JsonUtility.FromJson<Serialization<int>>(json).ToList();
		}
	}

	/// <summary>
	/// TODO : メインクエストの解放数.ミッション情報も内包する必要があるので単純なintのリストではダメかも.
	/// </summary>
	public List<int> ReleaseMainQuestIdList
	{
		get {
			var json = Get<string>("ReleaseMainQuestIdList");
			if (string.IsNullOrEmpty(json)) {
				var listEmpty = new List<int>();
				Put("ReleaseMainQuestIdList", new Serialization<int>(listEmpty));
				return listEmpty;
			}
			return UnityEngine.JsonUtility.FromJson<Serialization<int>>(json).ToList();
		}
	}

	/// <summary>
	/// TODO : サブクエストの解放数.ミッション情報も内包する必要があるので単純なintのリストではダメかも.
	/// </summary>
	public List<int> ReleaseSubQuestIdList
	{
		get {
			var json = Get<string>("ReleaseSubQuestIdList");
			if (string.IsNullOrEmpty(json)) {
				var listEmpty = new List<int>();
				Put("ReleaseSubQuestIdList", new Serialization<int>(listEmpty));
				return listEmpty;
			}
			return UnityEngine.JsonUtility.FromJson<Serialization<int>>(json).ToList();
		}
	}

    public EventQuestSaveData EventQuestSaveData {
        get { return Get<EventQuestSaveData>("EventQuestSaveData"); }
        set { Put("EventQuestSaveData", value); }
    }

	/// <summary>
	/// 現在選択しているクエスト.
	/// </summary>
	public IQuestData CurrentQuest { get; set; }

    /// <summary>
    /// 現在選択しているクエストのミッションクリア状況
    /// </summary>
    public int[] CurrentQuestAchievedMissionIdList { get; set; }

	/// <summary>
	/// 現在のシナリオで選択した選択肢IDリスト.
	/// </summary>
	public List<int> CurrentScenarioSelectIdList { get; set; }

	/// <summary>
	/// 直前に選択したクエスト.まだなにも選択していなければnull.
	/// </summary>
	public IQuestData PrevSelectedQuest
	{
		get {
			IQuestData rtn;
			rtn = MasterDataTable.quest_main[prevSelectedQuestID];
			if (rtn == null) {
				rtn = MasterDataTable.quest_sub[prevSelectedQuestID];
			}
			return rtn;
		}
		set {
			prevSelectedQuestID = value != null ? value.ID : -1;
		}
	}
	private int prevSelectedQuestID
	{
		get { return Get<int>("prevSelectedQuestID"); }
		set { Put("prevSelectedQuestID", value); }
	}
    
    /// <summary>
	/// メインクエストのクエストクリア時の報酬アイテム.なければnull.
	/// </summary>
	public ItemData RewardItemForClearMainQuestQuest { get; set; }
	/// <summary>
	/// メインクエストのステージクリア時の報酬アイテム.なければnull.
	/// </summary>
    public ItemData RewardItemForClearMainQuestStage { get; set; }
	/// <summary>
    /// メインクエストの章クリア時の報酬アイテム.なければnull.
    /// </summary>
    public ItemData RewardItemForClearMainQuestChapter { get; set; }

    
	/// <summary>
	/// チュートリアルポイント更新.
	/// </summary>
    public void UpdateTutorialPoint(int stgNum, bool sync = true)
	{
		this.TutorialStageNum = stgNum;
        if (sync) {
            this.Sync ();
        }
	}

    /// <summary>
    /// 最小限ダウンロードか全部ダウンロードどちらを選択したか保存する
    /// </summary>
    /// <param name="type">Type.</param>
    public void SetSelectDLCType(int type)
    {
        this.SelectDLCType = type;
        this.Sync ();
    }

	/// <summary>
    /// 既に見ている章解放演出リストの更新.
    /// </summary>
	public void UpdateSeenChapterReleaseEffectList(MainQuestChapterInfo chapterInfo)
    {
		var list = SeenChapterReleaseEffectList;
		if (list.Exists(c => c.country.Enum == chapterInfo.country.Enum && c.chapter == chapterInfo.chapter)) {
            return;
        }
		list.Add(chapterInfo);
		Put("SeenChapterReleaseEffectList", new Serialization<MainQuestChapterInfo>(list));
    }

	/// <summary>
    /// 既に見ている国解放演出リストの更新.
    /// </summary>
    public void UpdateSeenCountryClearEffectList(BelongingEnum belonging)
    {
		var list = SeenCountryClearEffectList;
        if (list.Contains(belonging)) {
            return;
        }
        list.Add(belonging);
		Put("SeenCountryClearEffectList", new Serialization<int>(list.Select(b => (int)b).ToList()));
    }

    /// <summary>
	/// 既に見ている国解放演出リストの更新.
    /// </summary>
	public void UpdateSeenCountryReleaseEffectList(BelongingEnum belonging)
	{
		var list = SeenCountryReleaseEffectList;
        if (list.Contains(belonging)) {
            return;
        }
        list.Add(belonging);
		Put("SeenCountryReleaseEffectList", new Serialization<int>(list.Select(b => (int) b).ToList()));
    }

	/// <summary>
	/// 既に見ているシナリオ演出のクエストIDリストの更新.
	/// </summary>
	public void UpdateSeenScenarioEffectID(int questId)
	{
		var list = SeenScenarioEffectIdList;
		if (list.Contains(questId)) {
			return;
		}
		list.Add(questId);
		Put("SeenScenarioEffectIdList", new Serialization<int>(list));
		this.Sync();
	}

	/// <summary>
	/// クエストの解放数更新.
	/// </summary>
	public void UpdateReleaseQuest(IQuestData questInfo)
	{
        List<int> list = null;
        if (questInfo.QuestType == 1) {
            list = ReleaseMainQuestIdList;
        } else if (questInfo.QuestType == 2) {
            list = ReleaseSubQuestIdList;
        }

        if (list == null) {
            return;
        }

		if (list.Contains(questInfo.ID)) {
			return;
		}
		list.Add(questInfo.ID);
        string cacheName = string.Empty;
        if (questInfo.QuestType == 1) {
            cacheName = "ReleaseMainQuestIdList";
        } else if (questInfo.QuestType == 2) {
            cacheName = "ReleaseSubQuestIdList";
        }
		Put(cacheName, new Serialization<int>(list));
	}
	public void UpdateReleaseQuest(List<IQuestData> questList)
	{
		if (questList.Count <= 0) {
			return;
		}

		var mainList = ReleaseMainQuestIdList;
		var subList = ReleaseSubQuestIdList;
		foreach (var info in questList) {
            List<int> list = null;
            if (info.QuestType == 1) {
                list = mainList;
            } else if (info.QuestType == 2) {
                list = subList;
            } else {
                continue;
            }

            if (list.Contains(info.ID)) {
				continue;
			}
			list.Add(info.ID);
		}
		Put("ReleaseMainQuestIdList", new Serialization<int>(mainList));
		Put("ReleaseSubQuestIdList", new Serialization<int>(subList));
	}

	/// <summary>
	/// メインクエストの解放数上書き.
	/// </summary>
	public void OverwriteReleaseMainQuest(List<int> questIdList)
	{
		Put("ReleaseMainQuestIdList", new Serialization<int>(questIdList));
	}

	/// <summary>
	/// サブクエストの解放数上書き.
	/// </summary>
	public void OverwriteReleaseSubQuest(List<int> questIdList)
	{
		Put("ReleaseSubQuestIdList", new Serialization<int>(questIdList));
	}
 
    /// <summary>
    /// バトルミッションコンプリートかどうか.
    /// </summary>
	public bool IsCompletedBattleMission(IQuestData quest)
	{
		var achive = QuestAchievement.CacheGet(quest.ID);
		var masterData = MasterDataTable.battle_mission_setting.DataList.Find(s => s.stage_id == quest.BattleStageID);
		if(masterData == null){
			return false;    // 一つも設定されていないのであればコンプリートにはならないとのこと.
		}
		var missionIdList = new List<int?>();
        missionIdList.Add(masterData.condition_1);
        missionIdList.Add(masterData.condition_2);
        missionIdList.Add(masterData.condition_3);
        missionIdList = missionIdList.Where(id => id != null).ToList();      
		return achive.AchievedMissionIdList != null && achive.AchievedMissionIdList.Length >= missionIdList.Count;  // 各クエストは3つミッションが設定されている前提.
	}

    /// <summary>
    /// 指定国の最新クリア済みクエスト取得.なければnull.
    /// </summary>
	public MainQuest GetLatestClearMainQuest(string countryName)
	{
		// TODO : このオーバーライドはマスター側をrelation設定することで消したい.
		var belonging = MasterDataTable.belonging.DataList.Find(b => b.name == countryName);
		return this.GetLatestClearMainQuest(belonging);
	}
	public MainQuest GetLatestClearMainQuest(Belonging belonging)
	{
		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		if (achiveQuests.Count < 1) {
            return null;
        }      
        var qList = MasterDataTable.quest_main.GetQuestList(belonging);
		var achiveList = achiveQuests.FindAll(a => qList.Exists(q => q.id == a.QuestId));
        if (achiveList.Count < 1) {
            return null;
        }
		return MasterDataTable.quest_main[achiveList.Select(a => a.QuestId).Max()];
	}

    /// <summary>
	/// 指定国の受託可能なクエストの中で最新のものを返す.
    /// </summary>
    public MainQuest GetPlayableLatestMainQuest(Belonging belonging)
    {
		var prev = this.GetLatestClearMainQuest(belonging);
		if(prev == null){
			return MasterDataTable.quest_main.GetFirstSceneQuest(belonging);
		}
		var next = MasterDataTable.quest_main.GetNextQuestInfo(prev.id);
		return next ?? prev;
    }

    /// <summary>
    /// 国章幕それぞれの設定に応じてクエストが解放されているかを確認する.
    /// </summary>
	public bool IsReleaseMainQuest(Belonging belonging, int chapter, int stage = -1, int quest = -1)
    {
		var releaseList = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();      
		var first = MasterDataTable.quest_main.GetFirstSceneQuest(belonging);
		if(releaseList.Count <= 0){
			if(stage > 0){
				if(quest > 0){
					return first.Country.Enum == belonging.Enum && first.ChapterNum == chapter && first.StageNum == stage && first.QuestNum == quest;
				}
				return first.Country.Enum == belonging.Enum && first.ChapterNum == chapter && first.StageNum == stage;
			}         
			return first.Country.Enum == belonging.Enum && first.ChapterNum == chapter;    // 解放クエストがないので最初のクエストであれば最新クエスト.
		}
        if(quest > 0 && stage > 0){
			var decide = MasterDataTable.quest_main.DataList.Find(q => q.Country.Enum == belonging.Enum && 
			                                                      q.ChapterNum == chapter && 
			                                                      q.StageNum == stage && 
			                                                      q.quest == quest);
			return decide.id == first.id || releaseList.Exists(a => a.QuestId == decide.id);
        }
        if(stage > 0){
			var stageList = MasterDataTable.quest_main.DataList.FindAll(q => q.Country.Enum == belonging.Enum &&
			                                                            q.ChapterNum == chapter &&
			                                                            q.StageNum == stage);
			return releaseList.Exists(a => stageList.Exists(q => q.id == a.QuestId));
        }
		var chapterList = MasterDataTable.quest_main.DataList.FindAll(q => q.Country.Enum == belonging.Enum &&
		                                                              q.ChapterNum == chapter);
		return releaseList.Exists(a => chapterList.Exists(q=> q.id == a.QuestId || q.id == first.id));
    }
    /// <summary>
    /// 最新のクエストかどうか.
    /// </summary>
    public bool IsLatestMainQuest(int questId)
    {
		var quest = MasterDataTable.quest_main.DataList.Find(q => q.id == questId);      
		var first = MasterDataTable.quest_main.GetFirstSceneQuest(quest.Country);

		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		if (achiveQuests.Count <= 0) {
			return first.id == quest.id;    // 解放クエストがないので最初のクエストであれば最新クエスト.
        }
  
		var countryQuests = MasterDataTable.quest_main.DataList.FindAll(q => q.country == quest.country);

		var releaseQuests = new List<QuestAchievement>();
		foreach(var a in achiveQuests){
			if(countryQuests.Exists(cq => cq.ID == a.QuestId)){
				releaseQuests.Add(a);
			}
		}
		releaseQuests.Add(QuestAchievement.CacheGet(first.id));

		var next = MasterDataTable.quest_main.GetNextQuestInfo(releaseQuests.Select(a => a.QuestId).Max());
		UnityEngine.Debug.Log("IsLatestMainQuest : next=" + (next != null ? next.id : questId));
		return next != null && questId == next.id;   // 次がない場合この国のクエストは全クリしているとみなしてfalse
    }
    /// <summary>
    /// 最新クエストのある国章かどうか.
    /// </summary>
	public bool IsLatestMainQuest(Belonging belonging, int chapter, int stage = -1)
	{
		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		if (achiveQuests.Count <= 0) {
            return true;    //解放クエストがないので全部最新.
        }
		var countryQuests = MasterDataTable.quest_main.GetQuestList(belonging);
		var releaseIds = achiveQuests.FindAll(a => countryQuests.Exists(q => q.id == a.QuestId));
        if (releaseIds.Count <= 0) {
            return true;
        }
		var next = MasterDataTable.quest_main.GetNextQuestInfo(releaseIds.Select(a => a.QuestId).Max());
		if(next == null){
			return false;
		}
		if(stage < 0){
			return next.country == belonging.name && next.ChapterNum == chapter;
		}      
		return next.country == belonging.name && next.ChapterNum == chapter && next.StageNum == stage;
	}

    /// <summary>
    /// 解放済みサブシナリオを取得する.
    /// </summary>
	public List<SubQuest> GetReleaseSubQuestList()
	{
		var questList = new List<SubQuest>();
		var achiveQuests = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		if (achiveQuests == null || achiveQuests.Count <= 0) {
			return questList;
        }
		var selectIdList = achiveQuests.SelectMany(a => a.AchievedSelectionIdList).ToList();
		foreach (var achive in achiveQuests) {
			var quest = MasterDataTable.quest_sub_release.GetReleaseQuest(achive.QuestId, selectIdList, this.ReleaseSubQuestIdList);
            if (quest == null) {
                continue;
            }
            questList.Add(quest);
        }
        if (questList != null && questList.Count > 0) {
            AwsModule.ProgressData.UpdateReleaseQuest(questList.Select(q => q as IQuestData).ToList());         
        }
		return questList;
	}

    /// <summary>
    /// メインクエストのあらすじを表示すべきかどうかのチェック.
    /// </summary>
	public bool CheckViewMainQuestSummary()
	{
		if(PrevSelectedQuest == null){
			return false;
		}
		if(IsForceIgnoreOnceCheckMainQuestSummary){
			IsForceIgnoreOnceCheckMainQuestSummary = false;
			return false;
		}
		// 初めてチェック.
		var now = GameTime.SharedInstance.Now;
		var timeStr = prevViewTimeStrMainQuestSummary;
		if(string.IsNullOrEmpty(timeStr)){
			prevViewTimeStrMainQuestSummary = now.ToString();
			return true;
		}      
		var prevTime = DateTime.Parse(timeStr);
        var nextDay = prevTime.AddDays(1);
        var limitTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 4, 0, 0);   // 4時リセットとのこと.
		var bCheck = now > limitTime;
		if(bCheck){
			prevViewTimeStrMainQuestSummary = now.ToString();
		}
		return bCheck;

	}
    // 前回メインクエストあらすじを表示した時間文字列
    private string prevViewTimeStrMainQuestSummary
    {
		get { return Get<string>("prevViewTimeStrMainQuestSummary"); }
		set { Put("prevViewTimeStrMainQuestSummary", value); }
    }

    /// <summary>
    /// あらすじ強制無視フラグ.次の1回のチェックだけ無効にする.
    /// </summary>
	public bool IsForceIgnoreOnceCheckMainQuestSummary { get; set; }

    /// <summary>
    /// お知らせを表示する華道家のチェック.0時リセット、1日1回.
    /// </summary>
    public bool CheckViewNotice()
	{
		// 初めてチェック.
        var now = GameTime.SharedInstance.Now;
		var timeStr = prevViewTimeStrNotice;
        if(string.IsNullOrEmpty(timeStr)){
			prevViewTimeStrNotice = now.ToString();
            return true;
        }
		var prevTime = DateTime.Parse(timeStr);
        var nextDay = prevTime.AddDays(1);
		var limitTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);   // 明示的に0時を指定しておく.
		var bCheck = now > limitTime;
		if (bCheck) {
			prevViewTimeStrNotice = now.ToString();
        }
		return bCheck;
	}
	// 前回お知らせを確認した時間.
    private string prevViewTimeStrNotice
    {
		get { return Get<string>("prevViewTimeStrNotice"); }
		set { Put("prevViewTimeStrNotice", value); }
    }

    public void SetEventQuestReadScenarioId(int eventQuestId, int scerarioSettingId)
    {
        var saveData = EventQuestSaveData;
        if(saveData == null) {
            saveData = new EventQuestSaveData();
        }
        saveData.SetReadedScenario(eventQuestId, scerarioSettingId);
        EventQuestSaveData = saveData;
        Sync();
    }

    public void ResetEventQuestReadScenarioId(int eventQuestId)
    {
        var saveData = EventQuestSaveData;
        if(saveData == null) {
            return;
        }
        saveData.ResetReadedScenario(eventQuestId);
        EventQuestSaveData = saveData;
        Sync();
    }

    // コンストラクタ.
    public AwsUserProgressData(CognitoSyncManager mng) : base(mng, "PlayerProgressData") 
    {
        CurrentScenarioSelectIdList = new List<int>();
    }

    // 全値のリセット.
    protected override void ClearValues()
    {
        this.TutorialStageNum = 0;
        this.SelectDLCType = 0;
		this.IsFirstBoot = true;
    }
}
