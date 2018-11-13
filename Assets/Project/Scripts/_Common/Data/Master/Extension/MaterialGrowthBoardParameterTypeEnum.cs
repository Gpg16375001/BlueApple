using System.Collections;
using System.Collections.Generic;


public static class MaterialGrowthBoardParameterTypeEnumExtension {
    public static string GetSlotDisplay(this MaterialGrowthBoardParameterTypeEnum self)
    {
        return MasterDataTable.material_growth_board_parameter_type [self].slot_display;
    }
}
