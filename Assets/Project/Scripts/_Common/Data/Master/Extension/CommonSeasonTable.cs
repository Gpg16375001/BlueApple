using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using SmileLab;

public partial class CommonSeasonTable
{
	public CommonSeasonEnum GetSeason( DateTime? dateTime=null )
	{
		if( dateTime == null )
			dateTime = GameTime.SharedInstance.Now;

		var type = CommonSeasonEnum.None;
		var season = DataList.Find( s => (s.is_default == false) && (s.start_date <= dateTime && s.end_date >= dateTime) );
		if( season == null )
			season = DataList.Find( s => (s.is_default == true) && (s.start_date <= dateTime && s.end_date >= dateTime) );
		if( season != null )
			type = season.type;
		return type;
	}
}

public partial class CommonSeason
{
	partial void InitExtension()
	{
	}
}
