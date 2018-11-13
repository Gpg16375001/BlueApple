using System.Collections;
using System.Collections.Generic;

using SmileLab.Net.API;

public partial class MaterialGrowthBoardItemCombination {
    public Dictionary<int, int> GetNeedMaterials(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role)
    {
        Dictionary<int, int> needMaterials = new Dictionary<int, int> ();
        if (material_type_1.HasValue) {
            var charaMaterial = GetMaterial1Data (cardId, element, belonging, role);
            needMaterials.Add (charaMaterial.id, count_1);
        }
        if (material_type_2.HasValue) {
            var charaMaterial = GetMaterial2Data (cardId, element, belonging, role);
            needMaterials.Add (charaMaterial.id, count_2);
        }
        if (material_type_3.HasValue) {
            var charaMaterial = GetMaterial3Data (cardId, element, belonging, role);
            needMaterials.Add (charaMaterial.id, count_3);
        }
        if (material_type_4.HasValue) {
            var charaMaterial = GetMaterial4Data (cardId, element, belonging, role);
            needMaterials.Add (charaMaterial.id, count_4);
        }
        return needMaterials;
    }

    public void Spending(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role)
    {
        List<MaterialData> spendData = new List<MaterialData> ();
        if (material_type_1.HasValue) {
            var charaMaterial = GetMaterial1Data (cardId, element, belonging, role);
            if (charaMaterial != null) {
                var materialData = MaterialData.CacheGet (charaMaterial.id);
                if (materialData != null) {
                    materialData.Count -= count_1;
                    spendData.Add(materialData);
                }
            }
        }

        if (material_type_2.HasValue) {
            var charaMaterial = GetMaterial2Data (cardId, element, belonging, role);
            if (charaMaterial != null) {
                var materialData = MaterialData.CacheGet (charaMaterial.id);
                if (materialData != null) {
                    materialData.Count -= count_2;
                    spendData.Add(materialData);
                }
            }
        }

        if (material_type_3.HasValue) {
            var charaMaterial = GetMaterial3Data (cardId, element, belonging, role);
            if (charaMaterial != null) {
                var materialData = MaterialData.CacheGet (charaMaterial.id);
                if (materialData != null) {
                    materialData.Count -= count_3;
                    spendData.Add(materialData);
                }
            }
        }

        if (material_type_4.HasValue) {
            var charaMaterial = GetMaterial4Data (cardId, element, belonging, role);
            if (charaMaterial != null) {
                var materialData = MaterialData.CacheGet (charaMaterial.id);
                if (materialData != null) {
                    materialData.Count -= count_4;
                    spendData.Add(materialData);
                }
            }
        }

        spendData.CacheSet ();
    }

    public bool IsRelease(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role, Dictionary<int, int>  selectedMaterials = null, int TotalCost = 0)
    {
        if (material_type_1.HasValue) {
            var charaMaterial = GetMaterial1Data (cardId, element, belonging, role);
            if (charaMaterial == null) {
                return false;
            }
            var materialData = MaterialData.CacheGet (charaMaterial.id);
            if (materialData == null) {
                return false;
            }

            int nowSelected = 0;
            if (selectedMaterials != null) {
                selectedMaterials.TryGetValue (charaMaterial.id, out nowSelected);
            }
            if (materialData.Count < count_1 + nowSelected) {
                return false;
            }
        }

        if (material_type_2.HasValue) {
            var charaMaterial = GetMaterial2Data (cardId, element, belonging, role);
            if (charaMaterial == null) {
                return false;
            }
            var materialData = MaterialData.CacheGet (charaMaterial.id);
            if (materialData == null) {
                return false;
            }

            int nowSelected = 0;
            if (selectedMaterials != null) {
                selectedMaterials.TryGetValue (charaMaterial.id, out nowSelected);
            }
            if (materialData.Count < count_2 + nowSelected) {
                return false;
            }
        }

        if (material_type_3.HasValue) {
            var charaMaterial = GetMaterial3Data (cardId, element, belonging, role);
            if (charaMaterial == null) {
                return false;
            }
            var materialData = MaterialData.CacheGet (charaMaterial.id);
            if (materialData == null) {
                return false;
            }

            int nowSelected = 0;
            if (selectedMaterials != null) {
                selectedMaterials.TryGetValue (charaMaterial.id, out nowSelected);
            }
            if (materialData.Count < count_3 + nowSelected) {
                return false;
            }
        }

        if (material_type_4.HasValue) {
            var charaMaterial = GetMaterial4Data (cardId, element, belonging, role);
            if (charaMaterial == null) {
                return false;
            }
            var materialData = MaterialData.CacheGet (charaMaterial.id);
            if (materialData == null) {
                return false;
            }

            int nowSelected = 0;
            if (selectedMaterials != null) {
                selectedMaterials.TryGetValue (charaMaterial.id, out nowSelected);
            }
            if (materialData.Count < count_4 + nowSelected) {
                return false;
            }
        }

        if (AwsModule.UserData.UserData.GoldCount < cost + TotalCost) {
            return false;
        }

        return true;
    }

    public CharaMaterial GetMaterial1Data(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role) {
        if (material_type_1.HasValue) {
            return GetMaterialData(material_type_1.Value, rarity_1, cardId, element, belonging, role);
        }
        return null;
    }

    public CharaMaterial GetMaterial2Data(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role) {
        if (material_type_2.HasValue) {
            return GetMaterialData(material_type_2.Value, rarity_2, cardId, element, belonging, role);
        }
        return null;
    }

    public CharaMaterial GetMaterial3Data(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role) {
        if (material_type_3.HasValue) {
            return GetMaterialData(material_type_3.Value, rarity_3, cardId, element, belonging, role);
        }
        return null;
    }

    public CharaMaterial GetMaterial4Data(int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role) {
        if (material_type_4.HasValue) {
            return GetMaterialData(material_type_4.Value, rarity_4, cardId, element, belonging, role);
        }
        return null;
    }

    private CharaMaterial GetMaterialData(CharaMaterialTypeEnum materialType, int rarity, int cardId, ElementEnum element, BelongingEnum belonging, CardRoleEnum role) {
        switch (materialType) {
        case CharaMaterialTypeEnum.card_based_material:
            return MasterDataTable.chara_material.GetCardBasedMaterial (cardId);
        case CharaMaterialTypeEnum.limit_break_material:
            return MasterDataTable.chara_material.GetLimitBreakMaterial (rarity);
        case CharaMaterialTypeEnum.role_based_material:
            return MasterDataTable.chara_material.GetGrowthMaterial (role, rarity);
        case CharaMaterialTypeEnum.element_based_growth_material:
            return MasterDataTable.chara_material.GetGrowthMaterial (element, rarity);
        case CharaMaterialTypeEnum.enemy_based_material:
            var roleDefine = MasterDataTable.material_growth_board_by_role[(int)role];
            if(rarity == 1) {
                return MasterDataTable.chara_material.DataList.Find(x => x.id == roleDefine.enemy_based_material_1);
            } else if(rarity == 2) {
                return MasterDataTable.chara_material.DataList.Find(x => x.id == roleDefine.enemy_based_material_2);
            } else if(rarity == 3) {
                return MasterDataTable.chara_material.DataList.Find(x => x.id == roleDefine.enemy_based_material_3);
            }
            break;
        }
        return null;
    }
}
