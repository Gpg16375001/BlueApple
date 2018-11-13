using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using SmileLab;

public partial class EventInfoTable {
    public EventInfo[] EnableEventInfo {
        get {
            return DataList.Where (x => x.Enable()).OrderBy(x => x.priority).ToArray ();
        }
    }
}

public partial class EventInfo
{
    public bool Enable()
    {
        var now = GameTime.SharedInstance.Now;
        if (start_at.HasValue) {
            if (start_at.Value > now) {
                return false;
            }
        }

        if (end_at.HasValue) {
            if (end_at.Value < now) {
                return false;
            }
        }

        return true;
    }
}
