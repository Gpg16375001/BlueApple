using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using TMPro;


/// <summary>
/// ListItem : 横向き武器タブ.
/// </summary>
public class ListItem_HorizontalWeaponTab : ViewBase
{
	/// <summary>
    /// ID.
    /// </summary>
	public int WeaponID { get; private set; }

	/// <summary>
    /// 武器名.
    /// </summary>
    public string WeaponName { get; private set; }

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
            this.GetScript<uGUISprite> ("TabCategoryImage").ChangeSprite (value ? "Common_On" : "Common_Off");
        }
    }

	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(WeaponType weaponType, Action<ListItem_HorizontalWeaponTab> didTapTab)
    {
		this.WeaponID = weaponType.index;
		this.WeaponName = weaponType.name;
        m_didTapTab = didTapTab;
        m_sptNormal = this.GetScript<Image>("bt_Tab").sprite;
        m_sptHeighlight = this.GetScript<CustomButton>("bt_Tab").spriteState.highlightedSprite;

        // ラベル類.
		this.GetScript<TextMeshProUGUI>("txtp_Category").text = this.WeaponName;

        // ボタン設定.
        this.SetCanvasCustomButtonMsg("bt_Tab", DidTapTab);
    }

	// ボタン : タブ選択.
    void DidTapTab()
    {
        Debug.Log("DidTapTab : name=" + this.GetScript<TextMeshProUGUI>("txtp_Category").text);
        if (m_didTapTab != null) {
            m_didTapTab(this);
        }
    }

	private Sprite m_sptHeighlight;
    private Sprite m_sptNormal;
	private Action<ListItem_HorizontalWeaponTab> m_didTapTab;
}
