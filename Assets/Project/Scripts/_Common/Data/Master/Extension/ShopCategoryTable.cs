using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class ShopCategoryTable
{
	/// <summary>
    /// アイテムショップとして並ぶカテゴリーリストを返す.有効な販売アイテムがあるタブのみ表示.
    /// </summary>
	public List<ShopCategory> GetItemShopCategories()
	{      
		// ジェムとクレドで購入するショップは定常前提.
		return DataList.Where(c => !c.name.Contains("を購入"))
            .Where(c => c.name.Contains("ジェムで") || c.name.Contains("クレドで") ||
                        MasterDataTable.shop_list.DataList.Exists(s => s.shop_category.name == c.name) ||
                        (c.name.Contains("ソウルと交換") && MasterDataTable.card_based_material.EnableData.Length > 0))
			           .ToList();
	}
}
