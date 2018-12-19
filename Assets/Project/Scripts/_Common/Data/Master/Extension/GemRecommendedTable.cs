using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using SmileLab;

public partial class GemRecommendedTable
{
	public GemRecommended GetRecommended()
	{
		var now = GameTime.SharedInstance.Now;

		List<GemRecommended> list = new List<GemRecommended>();
		DataList.ForEach( gr => {
			if ( !gr.start_date.HasValue && !gr.end_date.HasValue ) {
				list.Add( gr );
			}else{
				if ( gr.start_date <= now && gr.start_date >= now ) {
					list.Add( gr );
				}
			}
		} );
		return list.OrderBy( x => x.priority ).First();
	}
}
