using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class WeaponLevelTable
{
    public int GetMaxLevel(int table_id)
    {
        return DataList.Where (x => x.table_id == table_id).Max (x => x.level);
    }

    public int GetLevel(int table_id, int experience)
    {
        return DataList.Where (x => x.table_id == table_id && x.experience <= experience).Max (x => x.level);      
    }

	public int GetNextLevelExp(int table_id, int nowLevel, int experience)
    {
        int nextLevel = nowLevel + 1;
        var nextLevelData = DataList.FirstOrDefault(x => x.table_id == table_id && x.level == nextLevel);
        if (nextLevelData == null) {
            return 0;
        }
        var nowLevelData = DataList.FirstOrDefault(x => x.table_id == table_id && x.level == nowLevel);
        return nextLevelData.experience - nowLevelData.experience;
    }

    public int GetCurrentLevelExp(int table_id, int nowLevel, int experience)
    {
        var nowLevelData = DataList.FirstOrDefault(x => x.table_id == table_id && x.level == nowLevel);
        return experience - nowLevelData.experience;
    }
}
