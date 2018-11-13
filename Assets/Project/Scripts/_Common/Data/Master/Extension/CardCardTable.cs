using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

public partial class CardCardTable
{
    public CardCard this[int id]
    {
        get {
            return DataList.Where(x => x.id == id).OrderBy(x => x.rarity).FirstOrDefault();
        }
    }
}