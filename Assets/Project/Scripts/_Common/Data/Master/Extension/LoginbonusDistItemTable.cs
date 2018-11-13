using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class LoginbonusDistItemTable
{
    public LoginbonusDistItem Get(int loginbonusID, int dayCount)
    {
        return DataList.FirstOrDefault(x => x.loginbonus_id == loginbonusID && x.day_count == dayCount);
    }
}
