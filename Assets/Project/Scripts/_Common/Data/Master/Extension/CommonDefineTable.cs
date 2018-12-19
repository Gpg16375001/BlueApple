using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public partial class CommonDefineTable : ScriptableObject
{
	public int GetValue( string key, int defaultValue=0 ) {
		return _dataDict.ContainsKey( key ) ? _dataDict[key].define_value : defaultValue;
	}
}
