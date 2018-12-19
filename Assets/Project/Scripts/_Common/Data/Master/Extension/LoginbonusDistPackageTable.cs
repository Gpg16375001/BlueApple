using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class LoginbonusDistPackageTable 
{
    public LoginbonusDistPackage Get(int loginbonusID, int day) {
        return DataList.SingleOrDefault (x => x.loginbonus_id == loginbonusID && x.day_count == day);
    }
}
