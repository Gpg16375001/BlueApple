using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


public class ListItem_SaleBundleEffect : ViewBase
{
    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init()
    {
        this.GetScript<RectTransform>("R4_Effect").gameObject.SetActive(true);
    }
}
