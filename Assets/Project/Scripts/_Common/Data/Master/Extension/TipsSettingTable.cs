using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public partial class TipsSettingTable
{
    public TipsSetting GetRandom()
    {
        var list = DataList.OrderByDescending (x => x.frequency);
        var sumFrequency = list.Sum (x => x.frequency);

        var randomValue = Random.Range (0, sumFrequency);
        int value = 0;
        foreach (var tips in list) {
            if (value >= randomValue) {
                return tips;
            }
            value += tips.frequency;
        }
        return list.First ();
    }
}