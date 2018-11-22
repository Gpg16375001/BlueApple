using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public partial class EventQuestSchedule {

    public bool Enable()
    {
        var now = GameTime.SharedInstance.Now;

        return start_at <= now && now <= end_at;
    }
}
