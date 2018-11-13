using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Weapon
{
    public WeaponMotionType MotionType {
        get {
            return MasterDataTable.weapon_motion_type.DataList.FirstOrDefault(x => x.index == this.motion_type);
        }
    }
}
