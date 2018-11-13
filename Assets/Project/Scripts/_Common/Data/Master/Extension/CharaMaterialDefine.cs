using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab.Net.API;


// partial class : キャラ強化素材定義
public partial class CharaMaterialDefine
{
	/// <summary>
	/// 素材と強化対象の属性が一致した場合の上昇百分率のfloat.
    /// </summary>
	public float SameElementRatioPer { get { return ((float)same_element_ratio / 100f) + 1f; } }
 
	/// <summary>
    /// 対象の属性から強化ポイントを取得.
    /// </summary>
	public int GetEnhancePoint(ElementEnum? matElement, Element targetElement)
	{
		var rtn = base_point;
		if(matElement == null){
			return rtn;
		}
        // 虹素材は無条件で1.5ばい
		if(matElement == ElementEnum.rainbow || matElement == targetElement.Enum){
			rtn = Mathf.FloorToInt((float)rtn * SameElementRatioPer);   // TODO : 小数点以下の端数は切り捨てるようにしとく.
		}
		return rtn;
	}
}
