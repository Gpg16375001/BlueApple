using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// Common notice table.
/// </summary>
public partial class CommonNoticeTable
{
	/// <summary>
    /// 有効リスト.
    /// </summary>
	public List<CommonNotice> EnableList { get { return DataList.Where(d => d.IsEnable).ToList(); } }

    /// <summary>
    /// 指定のストアタイプで表示すべきリストを取得する.
    /// </summary>
	public List<CommonNotice> GetListThisPlatform(CommonNoticeCategoryEnum categoryEnum)
	{
		var stores = Enum.GetValues(typeof(StoreTypeEnum)) as StoreTypeEnum[];
		var storeType = Array.Find(stores, s => s.ToString() == GameSystem.GetPlatformName());
		var shift = ((int)storeType)-1; // 0ビット目も使用するので0始まりに変換する必要.
		var check = 1 << shift;
		var rtnList = DataList.FindAll(d => d.IsEnable && d.category == categoryEnum && (d.view_target_bit & check) > 0);
		rtnList.Sort((x, y) => x.priority -y.priority);
		return rtnList;
	}
}
