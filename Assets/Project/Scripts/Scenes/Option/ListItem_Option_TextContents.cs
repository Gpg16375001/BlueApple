using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// ListItem : .設定のDictionaryView用テキスト一覧
/// </summary>
public class ListItem_Option_TextContents : ViewBase
{
	/// <summary>
    /// 初期化.大項目リストとして
    /// </summary>
	public void InitMajorItem(string majorTitle, Action<string> didTap)
	{
		m_didTapMajorItem = didTap;
		this.GetScript<TextMeshProUGUI>("txtp_Title").text = majorTitle;
		this.SetCanvasCustomButtonMsg("img_SubTitle", () => m_didTapMajorItem(majorTitle));
	}

    /// <summary>
    /// 小項目リストとして初期化.
    /// </summary>
	public void InitSubItem(HelpInfo info, Action<HelpInfo> didTap)
	{
		m_didTapSubItem = didTap;
		this.GetScript<TextMeshProUGUI>("txtp_Title").text = info.subject_small;
		this.SetCanvasCustomButtonMsg("img_SubTitle", () => m_didTapSubItem(info));
	}
 
	private Action<string> m_didTapMajorItem;
	private Action<HelpInfo> m_didTapSubItem;
}
