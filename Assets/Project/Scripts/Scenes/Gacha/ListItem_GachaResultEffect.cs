using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// ListItem : ガチャ結果画面の演出.
/// </summary>
public class ListItem_GachaResultEffect : ViewBase
{   
    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(Mode mode)
	{
		this.GetScript<RectTransform>("R4_Effect").gameObject.SetActive((mode & Mode.R4) == Mode.R4);
		if(this.Exist<RectTransform>("SoulStone")){
			this.GetScript<RectTransform>("SoulStone").gameObject.SetActive((mode & Mode.Soul) == Mode.Soul);
		}      
		this.GetScript<RectTransform>("New").gameObject.SetActive((mode & Mode.New) == Mode.New);
	}

    /// <summary>
	/// enum : 表示モード.
    /// </summary>
	[System.Flags]
	public enum Mode
	{
		None = 0x0,
		R4 = 0x1,
		Soul = 0x2,
		New = 0x4,
	}
}
