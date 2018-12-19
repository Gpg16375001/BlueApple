using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

	/// <summary>
    /// 強制表示期間中かどうか
    /// </summary>
	public bool IsPopupEnable
    {
		get {
			var now = GameTime.SharedInstance.Now;
            if ( !popup_start_date.HasValue || popup_start_date > now) {
                return false;
            }
            if ( !popup_end_date.HasValue || popup_end_date < now) {
                return false;
            }
            return true;
		}
    }

	partial void InitExtension ()
	{
	}
}
