using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;


/// <summary>
/// MiniView : 設定のテキスト記載のみの記述.
/// </summary>
public class MiniView_OptionNotice : OptionMiniViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(string text)
    {
		this.GetScript<ScrollRect>("ScrollAreaTextOnlyView").verticalNormalizedPosition = 1f;
        this.GetScript<TextMeshProUGUI>("txtp_TextOnlyText").text = text;
    }
}
