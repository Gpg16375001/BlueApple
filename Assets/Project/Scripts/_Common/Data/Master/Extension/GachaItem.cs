using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using SmileLab;

public partial class GachaItem
{
	/// <summary>
    /// 優先順位
    /// </summary>
	public int Priority
    {
		get {
			if( (priority_start_date != null) || (priority_end_date != null) ) {
				var now = GameTime.SharedInstance.Now;
				if( (priority_start_date == null) || (priority_start_date < now) ) {
					if( (priority_end_date == null) || (priority_end_date > now) ) {
						return priority;
					}
				}
			}
			return 0;
		}
    }
}
