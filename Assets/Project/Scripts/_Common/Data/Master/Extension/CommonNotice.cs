using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public partial class CommonNotice
{
	/// <summary>
    /// 有効かどうか
    /// </summary>
	public bool IsEnable
    {
		get {
			var now = GameTime.SharedInstance.Now;
            if (start_date > now) {
                return false;
            }
            if (end_date < now) {
                return false;
            }
            return true;
		}
    }
}
