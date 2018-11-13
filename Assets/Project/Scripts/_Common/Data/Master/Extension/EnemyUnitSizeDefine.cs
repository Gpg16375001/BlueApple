using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EnemyUnitSizeDefine
{
    public EnemyUnitSize[] SizeList {
        get {
            return MasterDataTable.enemy_unit_size [id];
        }
    }
}
