using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// ListItem : 横向き汎用文字列タブ.
/// </summary>
public class ListItem_HorizontalTextTab : ViewBase
{
	/// <summary>
    /// カテゴリー名.
    /// </summary>
    public string CategoryName { get; private set; }

	/// <summary>
    /// 選択中？.
    /// </summary>
    public bool IsSelected
    {
        get {
            return this.GetScript<Image>("bt_Tab").sprite == m_sptHeighlight;
        }
        set {
            this.GetScript<Image>("bt_Tab").sprite = value ? m_sptHeighlight : m_sptNormal;
            this.GetScript<uGUISprite>("TabCategoryImage").ChangeSprite(value ? "Common_On" : "Common_Off");
        }
    }

	/// <summary>
    /// 初期化.
    /// </summary>
    public void Init(string categoryName, Action<ListItem_HorizontalTextTab> didTapTab)
    {
        this.CategoryName = categoryName;
        m_didTapTab = didTapTab;
        m_sptNormal = this.GetScript<Image>("bt_Tab").sprite;
        m_sptHeighlight = this.GetScript<CustomButton>("bt_Tab").spriteState.highlightedSprite;

        // ラベル類.
        this.GetScript<TextMeshProUGUI>("txtp_Category").text = this.CategoryName;

        // ボタン設定.
        this.SetCanvasCustomButtonMsg("bt_Tab", DidTapTab);
    }

    /// <summary>
    /// 空文字指定で表示オフ.
    /// </summary>
	public void SetBadge(string badgeStr)
	{
		this.GetScript<RectTransform>("Badge").gameObject.SetActive(!string.IsNullOrEmpty(badgeStr));
        if (!string.IsNullOrEmpty(badgeStr)) {
            this.GetScript<TextMeshProUGUI>("Badge/txtp_Num").text = badgeStr;
        }
	}

	// ボタン : タブ選択.
    void DidTapTab()
    {
        if (m_didTapTab != null) {
            m_didTapTab(this);
        }
    }

	private Sprite m_sptHeighlight;
    private Sprite m_sptNormal;
	private Action<ListItem_HorizontalTextTab> m_didTapTab;
}
