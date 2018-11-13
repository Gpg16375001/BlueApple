using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainQuestTable
{
    /// <summary>
    /// クエスト一覧.
    /// </summary>
    public List<MainQuest> GetQuestList(Belonging belonging)
    {
        return DataList.FindAll(q => q.country == belonging.name);
    }

    /// <summary>
    /// クエスト一覧.
    /// </summary>
    public List<MainQuest> GetQuestList(Belonging belonging, int chapterNum)
    {
		return DataList.FindAll(q => q.country == belonging.name && q.ChapterNum == chapterNum);
    }
    
    /// <summary>
    /// クエスト一覧.
    /// </summary>
	public List<MainQuest> GetQuestList(Belonging belonging, int chapterNum, int stageNum)
    {
		return DataList.FindAll(q => q.country == belonging.name && q.ChapterNum == chapterNum && q.StageNum == stageNum);
    }

    /// <summary>
    /// 次のクエスト情報を取得する.次のシーンがない場合はnullを返す.
    /// </summary>
    public MainQuest GetNextQuestInfo(int questId)
    {
        var quest = DataList.Find(q => q.id == questId);
        if(quest == null){
            Debug.LogError("[MainQuestTable] GetNextQuestInfo Error!!: Invalid id. id="+questId);
            return null;
        }

        // 最終シーン.
        var allScene = DataList.FindAll(q => q.country == quest.country &&
		                                q.ChapterNum == quest.ChapterNum && 
		                                q.StageNum == quest.StageNum);
		var lastScene = allScene.Select(q => q.quest).Max();
		if(quest.quest < lastScene){
            return DataList.Find(q => q.country == quest.country &&
			                     q.ChapterNum == quest.ChapterNum &&
			                     q.StageNum == quest.StageNum &&
			                     q.quest == quest.quest+1);    
        }
        // 幕最後.
        var allAct = DataList.FindAll(q => q.country == quest.country &&
		                              q.ChapterNum == quest.ChapterNum);
		var lastAct = allAct.Select(q => q.StageNum).Max();
		if(quest.stage_info.stage < lastAct){
            return DataList.Find(q => q.country == quest.country &&
			                     q.ChapterNum == quest.ChapterNum &&
			                     q.StageNum == quest.StageNum+1 &&
			                     q.quest == 1);
        }
        // 章最後.
        var allChapter = DataList.FindAll(q => q.country == quest.country);
		var lastChapter = allChapter.Select(q => q.ChapterNum).Max();
		if (quest.ChapterNum < lastChapter) {
            return DataList.Find(q => q.country == quest.country &&
			                     q.ChapterNum == quest.ChapterNum+1 &&
			                     q.StageNum == 1 &&
			                     q.quest == 1);
        }
        return null;
    }

    /// <summary>
    /// クエスト情報からバトルステージ情報を取得する.
    /// </summary>
	public BattleStage GetBattleStageInfo(MainQuest quest)
    {
        return MasterDataTable.stage.DataList.Find(s => s.id == quest.stage_id);
    }

    /// <summary>
    /// 指定国の最初のシーンのクエスト情報.
    /// </summary>
    public MainQuest GetFirstSceneQuest(Belonging belonging)
    {
        var list = DataList.FindAll(q => q.country == belonging.name);
        list.Sort((x, y) => x.id - y.id);
        return list[0];
    }
    /// <summary>
    /// 各国の最初のシーンのクエストIDリスト.マルチシナリオのため各国の最初のシーンはデフォルトで選択できる状態になる.
    /// </summary>
    public List<int> GetFirstSceneIdList()
    {
        var rtn = new List<int>();
        foreach (var belongingEnum in Enum.GetValues(typeof(BelongingEnum)) as BelongingEnum[]) {
            if (belongingEnum == BelongingEnum.Unknown) {
                continue;
            }
            var questId = DataList.FindAll(q => q.country == MasterDataTable.belonging[belongingEnum].name)
                                  .Select(q => q.id)
                                  .Min();
            rtn.Add(questId);
        }
        return rtn;
    }

    /// <summary>
    /// 指定国の最後のクエスト.
    /// </summary>
	public MainQuest GetLastSceneQuest(Belonging belonging)
	{
		var list = DataList.FindAll(q => q.country == belonging.name);
        list.Sort((x, y) => x.id - y.id);
		return list[list.Count-1];
	}

	/// <summary>
    /// 国を超えて前のメインクエストを参照する.無ければnull
    /// </summary>
    public MainQuest GetPrevQuest(MainQuest quest)
    {
        if (IsFirstQuestInCountry(quest)) {
            var country = MasterDataTable.belonging[quest.Country.Enum];
            if (country.priority_view > 1) {
                return GetLastSceneQuest(MasterDataTable.belonging.DataList.Find(b => b.priority_view == (country.priority_view - 1)));
            }
            return null;
        }

        var idList = DataList.Select(d => d.id).ToList();
        idList.Sort();

        var index = idList.FindIndex(id => id == quest.id);
        return MasterDataTable.quest_main[idList[index - 1]];

    }

	/// <summary>
    /// 指定クエストは国最初のクエストか.
    /// </summary>
    public bool IsFirstQuestInCountry(MainQuest quest)
    {
		var first = GetFirstSceneQuest(quest.Country);
		return quest.id == first.id;
    }

	/// <summary>
    /// 指定クエストは国最後のクエストか.
    /// </summary>
    public bool IsLastQuestInCountry(MainQuest quest)
    {
		var list = GetQuestList(quest.Country);
		return quest.id == list.Max(q => q.id);;
    }

    /// <summary>
    /// 指定クエストは章最後のクエストか.
    /// </summary>
	public bool IsLastQuestInChapter(MainQuest quest)
	{
		var list = GetQuestList(quest.Country, quest.ChapterNum);
		return quest.id == list.Max(q => q.id);
	}

	/// <summary>
    /// 指定クエストはステージ最後のクエストか.
    /// </summary>
    public bool IsLastQuestInStage(MainQuest quest)
    {
		var list = GetQuestList(quest.Country, quest.ChapterNum, quest.StageNum);
		return quest.id == list.Max(q => q.id);
    }
}