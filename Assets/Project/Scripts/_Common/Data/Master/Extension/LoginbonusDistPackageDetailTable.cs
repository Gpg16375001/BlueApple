using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class LoginbonusDistPackageDetailTable
{
    public List<LoginbonusDistPackageDetail> Get(int distpackageID)
    {
        return DataList.FindAll(x => x.dist_package_id == distpackageID);
    }

    public LoginbonusDistPackageDetail Get(int distpackageID, int packageIndex)
    {
        return DataList.SingleOrDefault (x => x.dist_package_id == distpackageID && x.package_index == packageIndex);
    }
}
