using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// メインクエストのステージ情報.
/// </summary>
public partial class MainQuestStageInfoTable
{
 
    /// <summary>
    /// 国章指定でリスト取得.
    /// </summary>
	public List<MainQuestStageInfo> GetListThisCountryChapter(Belonging belonging, int chapter)
	{
		return DataList.FindAll(i => i.chapter_info.country.Enum == belonging.Enum && i.chapter_info.chapter == chapter);
	}

	/// <summary>
	/// 規定クエスト取得.ステージを指定しない場合は小児設定されているデフォルトステージのデフォルトクエスト.
	/// </summary>
	public MainQuest GetDefaultMainQuest(QuestDecideInfo decideInfo, int stage = -1)
	{
		if (!decideInfo.IsMain) {
            return null;
        }
		var item = DataList.FindAll(d => d.chapter_info.country.Enum == decideInfo.Belonging.Enum && d.chapter_info.chapter == decideInfo.ChapterNum)
		                   .Find(d => stage > 0 ? d.stage == stage : d.is_default_stage);
		var rtn = MasterDataTable.quest_main.DataList.Find(q => q.country == decideInfo.Belonging.name &&
		                                                   q.ChapterNum == decideInfo.ChapterNum &&
		                                                   q.StageNum == item.stage &&
		                                                   q.QuestNum == item.default_quest);
		return rtn;
	}
}
