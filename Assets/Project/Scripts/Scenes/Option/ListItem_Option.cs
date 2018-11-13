using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// ListItem : オプションのメニュー項目.
/// </summary>
public class ListItem_Option : ViewBase
{
    /// <summary>
    /// 自身のメニュー.
    /// </summary>
    public OptionMenu Menu { get; private set; }


    /// <summary>
    /// 選択中？.
    /// </summary>
    public bool IsSelected 
    {
        get {
			return this.GetScript<Image>("bt_OptionMenu").sprite == m_sptHeighlight;
        }
        set {
			this.GetScript<Image>("bt_OptionMenu").sprite = value ? m_sptHeighlight: m_sptNormal;
			this.GetScript<TextMeshProUGUI>("txtp_Menu").gameObject.SetActive(!value);
			this.GetScript<TextMeshProUGUI>("txtp_Menu_highlight").gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(OptionMenu menu, Action<OptionMenu> didTapMenu)
    {
		m_sptNormal = this.GetScript<Image>("bt_OptionMenu").sprite;
		m_sptHeighlight = this.GetScript<CustomButton>("bt_OptionMenu").spriteState.highlightedSprite;

        this.Menu = menu;
        m_didTapMenu = didTapMenu;

        // ラベル設定
		this.GetScript<TextMeshProUGUI>("txtp_Menu").text = this.GetScript<TextMeshProUGUI>("txtp_Menu_highlight").text = this.Menu.name;

        // ボタン設定
		this.SetCanvasCustomButtonMsg("bt_OptionMenu", DidTapMenu);
    }

    // ボタン : 自身をタップ.
    void DidTapMenu()
    {
        if(m_didTapMenu != null){
            m_didTapMenu(this.Menu);
        }
    }

    private Action<OptionMenu> m_didTapMenu;
    private Sprite m_sptHeighlight;
    private Sprite m_sptNormal;
}
