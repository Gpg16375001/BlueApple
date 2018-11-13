using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 属性.
/// </summary>
public partial class ElementTable
{
	/// <summary>
	/// タブなどでカテゴリーとして表示する属性名リスト.
	/// </summary>
	public List<string> GetCategoryNameList()
	{
		var rtn = new List<string>();
		rtn.Add("ALL");
		rtn.AddRange(DataList.Where(x => !INVALID_CATEGORY_LIST.Contains(x.Enum)).Select(x => x.name));
		return rtn;
	}

    /// <summary>
    /// カテゴリー名からElement情報を取得する.
    /// </summary>
	public Element GetInfoFromCategoryName(string categoryName)
	{
		if (!GetCategoryNameList().Contains(categoryName)) {
            Debug.LogError("[ElementTable] GetElementFromCategoryName Error!! : " + categoryName + " is invalid category.");
            return null;
        }
		return DataList.FirstOrDefault(x => x.name == categoryName);
	}	

    // 運営上カテゴリーとして無視したい属性リスト.
	private static readonly ElementEnum[] INVALID_CATEGORY_LIST = { ElementEnum.naught, ElementEnum.rainbow, };
}
