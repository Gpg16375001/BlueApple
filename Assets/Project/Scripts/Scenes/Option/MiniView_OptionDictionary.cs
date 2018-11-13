using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// MiniView : 項目とその詳細内容が1セットになったView
/// </summary>
public class MiniView_OptionDictionary : OptionMiniViewBase
{   
    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(List<HelpInfo> infoList)
	{
		this.SwitchViewMode(ViewMode.List);
		
		// リスト.      
		var root = this.GetScript<ScrollRect>("ScrollAreaDictionaryList").content.gameObject;
		root.DestroyChildren();
		foreach(var info in infoList){
			var go = GameObjectEx.LoadAndCreateObject("Option/ListItem_Option_TextContents", root);
			var c = go.GetOrAddComponent<ListItem_Option_TextContents>();
			c.InitSubItem(info, DidTapContents);
		}

		// ボタン.
		this.GetScript<CustomButton>("BackListTop/bt_BackS").onClick.RemoveAllListeners();
        this.GetScript<CustomButton>("BackListBottom/bt_BackS").onClick.RemoveAllListeners();
        this.GetScript<CustomButton>("BackCategoryListTop/bt_BackS").onClick.RemoveAllListeners();
		this.SetCanvasCustomButtonMsg("BackListTop/bt_BackS", DidTapBack);
		this.SetCanvasCustomButtonMsg("BackListBottom/bt_BackS", DidTapBack);
        this.GetScript<RectTransform>("BackCategoryListTop").gameObject.SetActive(false);
	}

    // 表示モード切り替え.
	private void SwitchViewMode(ViewMode mode)
	{
		this.GetScript<RectTransform>("DictionaryList").gameObject.SetActive(mode == ViewMode.List);
		this.GetScript<RectTransform>("DictionaryContents").gameObject.SetActive(mode == ViewMode.Detail);
		m_mode = mode;
	}

	// ボタン : 項目を押した.
	void DidTapContents(HelpInfo info)
	{
		this.GetScript<TextMeshProUGUI>("txtp_Title").text = info.subject_small;
		this.GetScript<Text>("txtp_Main").text = info.text_detail;
		
		this.SwitchViewMode(ViewMode.Detail);      
	}   

	// ボタン : リストに戻る.
    void DidTapBack()
	{
		if(m_mode == ViewMode.List){
			return;
		}
		this.SwitchViewMode(ViewMode.List);
	}

	private ViewMode m_mode;
	// enum : 表示モード.
    private enum ViewMode
	{
		List,
        Detail,
	}
}
