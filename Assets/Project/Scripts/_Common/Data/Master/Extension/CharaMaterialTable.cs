using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// キャラ素材.
/// </summary>
public partial class CharaMaterialTable
{
    /// <summary>
    /// 指定属性の進化素材リストを取得する.
    /// </summary>
	public List<CharaMaterial> GetEvolutionMaterialList(ElementEnum element)
	{
		return DataList.FindAll(m => m.element != null && m.element == element && m.IsCharaEvolutionMaterial);
	}
	/// <summary>
    /// 指定ロール固有の進化素材リストを取得する.
    /// </summary>
    public List<CharaMaterial> GetEvolutionMaterialList(CardRoleEnum role)
    {
        return DataList.FindAll(m => m.role != null && m.role == role && m.IsCharaEvolutionMaterial);      
    }

    /// <summary>
    /// 指定属性の育成素材リストを取得する.
    /// </summary>
    public List<CharaMaterial> GetGrowthMaterialList(ElementEnum element)
    {
        return DataList.FindAll(m => m.element.HasValue && m.element == element && m.IsCharaGrowthMaterial);
    }
    /// <summary>
    /// 指定ロール固有の育成素材リストを取得する.
    /// </summary>
    public List<CharaMaterial> GetGrowthMaterialList(CardRoleEnum role)
    {
        return DataList.FindAll(m => m.role.HasValue && m.role == role && m.IsCharaGrowthMaterial);      
    }

    /// <summary>
    /// 指定属性の育成素材を取得する.
    /// </summary>
    public CharaMaterial GetGrowthMaterial(ElementEnum element, int rarity)
    {
        return DataList.Find(m => m.element.HasValue && m.element == element && m.rarity == rarity && m.IsCharaGrowthMaterial);
    }
    /// <summary>
    /// 指定ロール固有の育成素材を取得する.
    /// </summary>
    public CharaMaterial GetGrowthMaterial(CardRoleEnum role, int rarity)
    {
        return DataList.Find(m => m.role.HasValue && m.role == role && m.rarity == rarity && m.IsCharaGrowthMaterial);      
    }
    /// <summary>
    /// 指定国固有の育成素材を取得する.
    /// </summary>
    public CharaMaterial GetLimitBreakMaterial(int rarity)
    {
        return DataList.Find(m => m.IsCharaLimitBreakMaterial && m.rarity == rarity);
    }

    /// <summary>
    /// カード固有の育成素材を取得する.
    /// </summary>
    public CharaMaterial GetCardBasedMaterial(int cardId)
    {
        var cardBasedMaterial = MasterDataTable.card_based_material [cardId];
        return DataList.Find(m => m.IsCharaBasedMaterial && m.id == cardBasedMaterial.card_based_material_id);
    }
}
