using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// partial class : キャラ強化素材

public partial class CharaMaterial
{

    /// <summary>
    /// キャラ強化素材か？
    /// </summary>
    public bool IsCharaEnhanceMaterial
	{
		get {
			var enhanceMatType = MasterDataTable.chara_material_type.DataList.Find(m => m.Enum == CharaMaterialTypeEnum.reinforcement_material);
			return type == enhanceMatType.name;
		}
	}

    /// <summary>
    /// キャラ進化素材か？
    /// </summary>
    public bool IsCharaEvolutionMaterial
	{
		get {
			var evolutionMatTypeList = MasterDataTable.chara_material_type.DataList.FindAll(m => m.Enum == CharaMaterialTypeEnum.element_based_evolution_material ||
                                                                                                 m.Enum == CharaMaterialTypeEnum.role_based_material || 
			                                                                                     m.Enum == CharaMaterialTypeEnum.enemy_based_material);
			return evolutionMatTypeList.Exists(m => m.name == type);
		}
	}

    /// <summary>
    /// キャラ育成素材か？
    /// </summary>
    public bool IsCharaGrowthMaterial
    {
        get {
            var growthMatTypeList = MasterDataTable.chara_material_type.DataList.FindAll(
                m => m.Enum == CharaMaterialTypeEnum.element_based_growth_material ||
                m.Enum == CharaMaterialTypeEnum.role_based_material || 
                m.Enum == CharaMaterialTypeEnum.enemy_based_material
            );
            return growthMatTypeList.Exists(m => m.name == type);
        }
    }

    /// <summary>
    /// キャラ限界突破素材か？
    /// </summary>
    public bool IsCharaLimitBreakMaterial
    {
        get {
            var limitBreakMatType = MasterDataTable.chara_material_type.DataList.Find(
                m => m.Enum == CharaMaterialTypeEnum.limit_break_material
            );
            return type == limitBreakMatType.name;
        }
    }

    /// <summary>
    /// キャラ限界突破素材か？
    /// </summary>
    public bool IsCharaBasedMaterial
    {
        get {
            var charaBasedMatType = MasterDataTable.chara_material_type.DataList.Find(
                m => m.Enum == CharaMaterialTypeEnum.card_based_material
            );
            return type == charaBasedMatType.name;
        }
    }

    /// <summary>
    /// 指定したタイプを持つ素材か？
    /// </summary>
    /// <param name="typeEnums">タイプ指定</param>
    public bool IsSelectTypeMaterial(params CharaMaterialTypeEnum[] typeEnums) {
        var typeList = MasterDataTable.chara_material_type.DataList.FindAll (m => typeEnums.Any(x => x == m.Enum));
        return typeList.Exists(m => m.name == type);
    }
}
