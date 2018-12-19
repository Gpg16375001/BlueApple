using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;


/// <summary>
/// ListItem : マイページのお知らせ.
/// </summary>
public class ListItem_MyPageNotes : ViewBase
{   
    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(CommonNotice info)
	{
		m_info = info;

		var txtp_Heading = this.GetScript<Text> ("txtp_Heading");
		txtp_Heading.gameObject.SetActive (!string.IsNullOrEmpty (info.sun_title));	//テキストが空の場合非表示
		txtp_Heading.text = info.sun_title;

		var txtp_InfoTitle = this.GetScript<Text> ("txtp_InfoTitle");
		txtp_InfoTitle.gameObject.SetActive (!string.IsNullOrEmpty (info.title));	//テキストが空の場合非表示
		txtp_InfoTitle.text = info.title;

		this.GetScript<TextMeshProUGUI>("txtp_InfoDate").text = info.start_date.ToString("MM/dd");
		this.GetScript<Image>("img_TabNew").gameObject.SetActive(AwsModule.NotesModifiedData.IsNew(m_info));

		this.SetCanvasCustomButtonMsg("bt_InfoFrame", DidTapItem);
	}

	// ボタン : タップ
    void DidTapItem()
	{
		View_WebView.Open(DLCManager.AddUrlMasterVersion(m_info.url));
		AwsModule.NotesModifiedData.ConrirmedData(m_info);
	}
	
	public static View_WebView DisplayItem( CommonNotice info )
	{
		var view = View_WebView.Open( DLCManager.AddUrlMasterVersion( info.url ) );
		AwsModule.NotesModifiedData.ConrirmedData( info );
		return view;
	}

	private CommonNotice m_info;
	private Action<CommonNotice> m_didTap;   
}
