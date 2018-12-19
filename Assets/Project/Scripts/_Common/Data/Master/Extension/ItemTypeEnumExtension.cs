using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab.Net.API;

public static class ItemTypeEnumExtension {

    public static string GetName(this ItemTypeEnum type, int id)
    {
		try {
	        switch (type) {
	        case ItemTypeEnum.card:
				return MasterDataTable.card[id].nickname;
	        case ItemTypeEnum.money:
			case ItemTypeEnum.pvp_medal:
	        case ItemTypeEnum.free_gem:
	        case ItemTypeEnum.paid_gem:
	        case ItemTypeEnum.weapon_exp:
	        case ItemTypeEnum.card_exp:
	            return MasterDataTable.item_type.DataList.Find(i => i.Enum == type).display_name;
			case ItemTypeEnum.event_point:
                if(MasterDataTable.event_quest_point_name != null) {
				    return MasterDataTable.event_quest_point_name [id].point_name;
                }
                return MasterDataTable.item_type.DataList.Find(i => i.Enum == type).display_name;
	        case ItemTypeEnum.formation:
	            return MasterDataTable.formation[id].name;
	        case ItemTypeEnum.weapon:
	            return MasterDataTable.weapon[id].name;
	        case ItemTypeEnum.magikite:
	            return MasterDataTable.magikite[id].name;
			case ItemTypeEnum.consumer:
	            return MasterDataTable.consumer_item[id].name;    // TODO
	        case ItemTypeEnum.material:
	            return MasterDataTable.chara_material[id].name;
	        }
		} catch (System.Exception e) {
			// 通知だけして例外は無視する
			Debug.LogException (e);
		}
        return string.Empty;
    }

    public static string GetNameAndQuantity(this ItemTypeEnum type, int id, int quantity)
    {
		var itemName = type.GetName(id);
        switch (type) {
        case ItemTypeEnum.money:
		case ItemTypeEnum.pvp_medal:
        case ItemTypeEnum.card_exp:
        case ItemTypeEnum.weapon_exp:
        case ItemTypeEnum.event_point:
			return !string.IsNullOrEmpty(itemName) ? string.Format("{1}{0}", itemName, quantity) : string.Empty;    // 100クレド
		default:
			return !string.IsNullOrEmpty(itemName) ? string.Format("{0}x{1}", itemName, quantity) : string.Empty;   // エクスカリバーx10
        }
    }

	/// <summary>
    /// 通貨系のタイプであればアイコン名を返す.無効なタイプであればnull.
    /// </summary>
    public static string GetCurrencyIconName(this ItemTypeEnum self, int itemId = 0)
    {
        var iconName = "Icon";
        switch (self) {
            case ItemTypeEnum.free_gem:
            case ItemTypeEnum.paid_gem:
                return iconName+"Gem";
            case ItemTypeEnum.consumer: 
				{
                    if (itemId <= 0) {
                        return null;
                    }
                    // ガチャチケット.
                    var item = MasterDataTable.consumer_item.DataList.Find(c => c.index == itemId);
                    if (item.name.Contains("ガチャ") && item.name.Contains("チケット")) {
                        return iconName+"GachaTicket";
                    }
                    // TODO : スキップチケット系.
                    return null;
                }
            case ItemTypeEnum.money:
                return iconName+"Coin";
            case ItemTypeEnum.gacha_coin:
				{
					var size = "S";
					if(itemId == 2000002){
						size = "M";
					}else if(itemId == 2000003){
						size = "L";
					}
					return iconName+"Resoul"+size;
				}
            case ItemTypeEnum.pvp_medal:
                return iconName+"PVPMedal";
            case ItemTypeEnum.event_point:
                return iconName+"EventPoint";
            case ItemTypeEnum.friend_point:
                return iconName+"FriendPoint";
        }
        return null;
    }

	/// <summary>
    /// アイコン情報取得.
    /// </summary>
    public static ItemIconInfo GetIconInfo(this ItemTypeEnum type, int id, bool bIconOnly = false)
    {
		switch (type) {
            case ItemTypeEnum.card: {
					ItemIconInfo info = new ItemIconInfo();
                    info.IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon");
                    info.IconObject.GetOrAddComponent<ListItem_UnitIcon>().Init(new CardData(MasterDataTable.card[id]));
                    return info;
                }
            case ItemTypeEnum.money: // ゲーム内通貨
				return new ItemIconInfo { AtlasName = "CurrencyIcon", SpriteName = "IconCoin" };
            case ItemTypeEnum.pvp_medal:
                return new ItemIconInfo { AtlasName = "CurrencyIcon", SpriteName = "IconPVPMedal" };
            case ItemTypeEnum.weapon: {
					ItemIconInfo info = new ItemIconInfo();
                    info.IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon");
					info.IconObject.GetOrAddComponent<ListItem_WeaponIcon>().Init(new WeaponData(MasterDataTable.weapon[id]), !bIconOnly ? ListItem_WeaponIcon.DispStatusType.Default: ListItem_WeaponIcon.DispStatusType.IconOnly);
                    return info;
                }
            case ItemTypeEnum.paid_gem:
            case ItemTypeEnum.free_gem:
				return new ItemIconInfo { AtlasName = "CurrencyIcon", SpriteName = "IconGem" };
            case ItemTypeEnum.event_point:
				{
					ItemIconInfo info = new ItemIconInfo();
					info.IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_EventQuestPointIcon");
					info.IconObject.GetOrAddComponent<ListItem_EventQuestPointIcon>().Init(id);
					return info;
				}
            case ItemTypeEnum.material: // 素材
                {
					ItemIconInfo info = new ItemIconInfo();
                    info.IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_MaterialIcon");
					if(!bIconOnly){
						info.IconObject.GetOrAddComponent<ListItem_MaterialIcon>().Init(id);                  
					}else{
						info.IconObject.GetOrAddComponent<ListItem_MaterialIcon>().InitIconOnly(id);
					}
                    return info;
                }
			case ItemTypeEnum.magikite:
				{
                    ItemIconInfo info = new ItemIconInfo();
                    info.IconObject = GameObjectEx.LoadAndCreateObject("UnitDetails/ListItem_MagiIcon");
					info.IconObject.GetOrAddComponent<ListItem_MagikiteReward>().Init(id, bIconOnly);
                    return info;
                }
            case ItemTypeEnum.consumer:
                {
                    ItemIconInfo info = new ItemIconInfo();
                    info.IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_ConsumerIcon");
                    info.IconObject.GetOrAddComponent<ListItem_ConsumerIcon>().Init(id);
                    return info;
                }
            case ItemTypeEnum.friend_point:
                {
                    ItemIconInfo info = new ItemIconInfo { AtlasName = "CurrencyIcon", SpriteName = "IconFriendPoint" };
                    var loadAtlas = Resources.Load("Atlases/"+ info.AtlasName) as UnityEngine.U2D.SpriteAtlas;
                    info.IconSprite = loadAtlas.GetSprite(info.SpriteName);
                    return info;
                }
        }
		return new ItemIconInfo();
    }
}

/// struct : アイテムアイコン情報.
public struct ItemIconInfo
{
    public bool IsEnableSprite { get { return !string.IsNullOrEmpty(AtlasName) && !string.IsNullOrEmpty(SpriteName); } }

    public string AtlasName;
    public string SpriteName;

	public Sprite IconSprite;

    /// <summary>スプライト情報があればnull.アイコンGameObject.</summary>
    public GameObject IconObject;
}
