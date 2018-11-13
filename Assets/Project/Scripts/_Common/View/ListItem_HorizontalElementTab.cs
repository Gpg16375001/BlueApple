using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// ListItem : 横向き属性タブ.
/// </summary>
public class ListItem_HorizontalElementTab : ViewBase
{   
    /// <summary>
    /// カテゴリー名.
    /// </summary>
	public string CategoryName { get; private set; }

    /// <summary>
    /// 属性タイプ.
    /// </summary>
	public ElementEnum? ElementType { get; private set; }

    /// <summary>
    /// 選択中？.
    /// </summary>
    public bool IsSelected 
    {
        get {
            return this.GetScript<Image>("bt_Tab").sprite == m_sptHeighlight;
        }
        set {
            this.GetScript<Image>("bt_Tab").sprite = value ? m_sptHeighlight: m_sptNormal;
            this.GetScript<uGUISprite> ("TabCategoryImage").ChangeSprite (GetTabImageName(value));
        }
    }

	/// <summary>
	/// 初期化.
	/// </summary>
    public void Init(string categoryName, Action<ListItem_HorizontalElementTab> didTapTab)
    {
		this.CategoryName = categoryName;
		var info = MasterDataTable.element.GetInfoFromCategoryName(categoryName);
		if(info != null){
			this.ElementType = info.Enum;
		}
        m_didTapTab = didTapTab;
        m_sptNormal = this.GetScript<Image>("bt_Tab").sprite;
        this.GetScript<uGUISprite> ("TabCategoryImage").ChangeSprite (GetTabImageName(IsSelected));
        m_sptHeighlight = this.GetScript<CustomButton>("bt_Tab").spriteState.highlightedSprite;

        // ラベル類.
		this.GetScript<TextMeshProUGUI>("txtp_Category").text = this.CategoryName;
        
        // ボタン設定.
        this.SetCanvasCustomButtonMsg("bt_Tab", DidTapTab);
    }

    public void SetInteractable(bool interactable)
    {
        this.GetScript<CustomButton>("bt_Tab").interactable = interactable;
    }


    // ボタン : タブ選択.
    void DidTapTab()
    {
        Debug.Log("DidTapTab : name="+this.GetScript<TextMeshProUGUI>("txtp_Category").text);
        if(m_didTapTab != null){
            m_didTapTab(this);
        }
    }

    string GetTabImageName(bool selected)
    {
        if (ElementType.HasValue) {
            return string.Format ("{0}_{1}", (int)ElementType.Value, selected ? "On" : "Off");
        }
        return selected ? "Common_On" : "Common_Off";
    }
    private Sprite m_sptHeighlight;
    private Sprite m_sptNormal;
	private Action<ListItem_HorizontalElementTab> m_didTapTab;
}
