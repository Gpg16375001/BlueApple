using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class ShopProductList
{
	public ItemType category_type1 { get { return MasterDataTable.item_type.DataList.Find(t => t.name == category_name1); } }
	public ItemType category_type2 { get { return MasterDataTable.item_type.DataList.Find(t => t.name == category_name2); } }
	public ItemType category_type3 { get { return MasterDataTable.item_type.DataList.Find(t => t.name == category_name3); } }
	public List<ItemType> category_list 
	{ 
		get {
			var list = new List<ItemType>(new ItemType[] { category_type1, category_type2, category_type3 });
			return list.Where(t => t != null).ToList(); 
		} 
	}
	
	/// <summary>
    /// 商品アイコンロード.
    /// </summary>
	public void LoadProductIcon(Action<Sprite> didLoad)
	{
        DLCManager.AssetBundleFromFileOrDownload(DLCManager.DLC_FOLDER.UI, "bundle_shop", bundleref => {
			var atlas = bundleref.assetbundle.LoadAsset<SpriteAtlas>("shop");
			var spt = atlas.GetSprite(id.ToString());
			didLoad(spt);
		});
	}

	private static List<AssetBundleRef> bundleRefList = new List<AssetBundleRef>();
}
