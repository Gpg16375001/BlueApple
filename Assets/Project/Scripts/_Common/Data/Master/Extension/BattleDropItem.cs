using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleDropItem {
    public string GetName()
    {
        return reward_type.GetName (reward_id);
    }
}
