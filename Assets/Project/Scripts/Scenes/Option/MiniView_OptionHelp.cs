using System.Linq;
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
public class MiniView_OptionHelp : OptionMiniViewBase
{   
    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(List<HelpInfo> infoList)
    {
		m_allList = new List<HelpInfo>(infoList);
		foreach (var i in infoList){
			Debug.Log(i.subject_large);
		}
		m_majorList = infoList.Select(i => i.subject_large).Distinct().ToList();

		// ボタン.
		this.GetScript<CustomButton>("BackListTop/bt_BackS").onClick.RemoveAllListeners();
		this.GetScript<CustomButton>("BackListBottom/bt_BackS").onClick.RemoveAllListeners();
		this.GetScript<CustomButton>("BackCategoryListTop/bt_BackS").onClick.RemoveAllListeners();
        this.SetCanvasCustomButtonMsg("BackListTop/bt_BackS", DidTapBack);
        this.SetCanvasCustomButtonMsg("BackListBottom/bt_BackS", DidTapBack);
        this.SetCanvasCustomButtonMsg("BackCategoryListTop/bt_BackS", DidTapBack);
        
        this.SwitchViewMode(ViewMode.MajorList);
    }

    // 表示モード切り替え.
    private void SwitchViewMode(ViewMode mode)
    {
        this.GetScript<RectTransform>("DictionaryList").gameObject.SetActive(mode == ViewMode.MajorList || mode == ViewMode.SubList);
        this.GetScript<RectTransform>("DictionaryContents").gameObject.SetActive(mode == ViewMode.Detail);
        if(mode != ViewMode.Detail){
            //this.GetScript<RectTransform>("BackListBottom").gameObject.SetActive(mode == ViewMode.MajorList);
            this.GetScript<RectTransform>("BackCategoryListTop").gameObject.SetActive(mode == ViewMode.SubList);
            this.UpdateList(mode);
        }      
        m_mode = mode;
    }

    // リスト更新
    private void UpdateList(ViewMode mode)
    {
        if(mode == ViewMode.Detail){
            return;
        }
        
		var root = this.GetScript<ScrollRect>("ScrollAreaDictionaryList").content.gameObject;
        root.DestroyChildren();
		switch(mode){
			case ViewMode.MajorList:
				{
					foreach (var title in m_majorList) {
						var go = GameObjectEx.LoadAndCreateObject("Option/ListItem_Option_TextCategory", root);
                        var c = go.GetOrAddComponent<ListItem_Option_TextContents>();
						c.InitMajorItem(title, DidTapMajorContents);
                    }
				}
				break;
			case ViewMode.SubList:
				{
					foreach (var title in m_subList) {
						var go = GameObjectEx.LoadAndCreateObject("Option/ListItem_Option_TextContents", root);
                        var c = go.GetOrAddComponent<ListItem_Option_TextContents>();
						c.InitSubItem(title, DidTapSubContents);
                    }
				}
				break;
		}
    }

#region ButtonDelegate.
	// ボタン : 大項目を押した
	void DidTapMajorContents(string title)
	{
		m_subList = m_allList.FindAll(i => i.subject_large == title);
		this.GetScript<TextMeshProUGUI>("txtp_Title").text = title;      
        this.SwitchViewMode(ViewMode.SubList);
	}

    // ボタン : 小項目を押した.
    void DidTapSubContents(HelpInfo info)
    {
		this.GetScript<TextMeshProUGUI>("txtp_Title").text = info.subject_small;
		this.GetScript<Text>("txtp_Main").text = info.text_detail;
        this.SwitchViewMode(ViewMode.Detail);
    }   

    // ボタン : リストに戻る.
    void DidTapBack()
    {
        switch (m_mode) {
            case ViewMode.MajorList:
                return;
            case ViewMode.SubList:
                this.SwitchViewMode(ViewMode.MajorList);
                break;
            case ViewMode.Detail:
                this.SwitchViewMode(ViewMode.SubList);
                break;
        }
    }
	#endregion.

	private List<HelpInfo> m_allList;
	private List<string> m_majorList; // 大項目リスト
	private List<HelpInfo> m_subList;   // 小項目リスト
    private ViewMode m_mode;
    // enum : 表示モード.
    private enum ViewMode
    {
		MajorList,  // 大項目リスト
        SubList,    // 小項目リスト
        Detail,
    }
}
