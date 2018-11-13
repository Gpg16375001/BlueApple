using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// メインクエスト章情報.
/// </summary>
public partial class MainQuestChapterInfoTable
{

	/// <summary>
	/// 指定国のチャプターリスト一覧.
	/// </summary>
	public List<MainQuestChapterInfo> GetListThisCountry(Belonging belonging)
	{
		return DataList.FindAll(i => i.country.Enum == belonging.Enum);
	}

    /// <summary>
    /// 指定国の最初の章.
    /// </summary>
	public MainQuestChapterInfo GetFirstChapter(Belonging belonging)
	{
		var minId = GetListThisCountry(belonging).Select(i => i.id).Min();
		return DataList[minId];
	}

	/// <summary>
    /// 指定章の次の章.なければnull.
    /// </summary>
	public MainQuestChapterInfo GetNextChapter(Belonging belonging, MainQuestChapterInfo info)
    {
		return GetListThisCountry(belonging).Find(i => i.chapter == info.chapter+1);
    }
}
