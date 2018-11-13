using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FormationLevelTable {
    /// <summary>
    /// 最大レベル取得
    /// </summary>
    /// <returns>The max level.</returns>
    /// <param name="table_id">Table identifier.</param>
    public int GetMaxLevel(int table_id)
    {
        return DataList.Where (x => x.level_table_id == table_id).Max (x => x.level);
    }
}
