using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EventQuestExchangeSettingTable {
	public EventQuestExchangeSetting[] GetEnableList(int eventId) {
		var now = SmileLab.GameTime.SharedInstance.Now;
		return this[eventId].Where (x => x.is_enalbe && (!x.start_date.HasValue || x.start_date.Value <= now) && (!x.end_date.HasValue || x.end_date.Value >= now)).ToArray ();
	}
}