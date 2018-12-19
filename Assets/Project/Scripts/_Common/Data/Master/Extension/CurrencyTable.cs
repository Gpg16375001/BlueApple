using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class CurrencyTable 
{
    public Currency Get(ItemTypeEnum itemType)
    {
        Currency data = null;

        switch (itemType) {
        case ItemTypeEnum.friend_point:
            data = DataList.SingleOrDefault (x => x.id == 2);
            break;
        }

        return data;
    }

    public Currency Get(int itemID)
    {
        return DataList.SingleOrDefault (x => x.id == itemID);
    }
}
