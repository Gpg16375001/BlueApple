using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public partial class BattleWeaponExchangeSettingTable
{
    public BattleWeaponExchangeSetting[] GetSettings(int motionType)
    {
        return DataList.Where (x => x.motion_type == motionType).ToArray ();
    }
}
