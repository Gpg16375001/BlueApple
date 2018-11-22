using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using SmileLab;
using SmileLab.UI;


/// <summary>
/// ListItem : キャラクターガチャのカテゴリータブ.
/// </summary>
public class ListItem_GachaPage : ViewBase
{
	/// <summary>
    /// ガチャ.
    /// </summary>
	public Gacha Gacha { get; private set; }
 
    /// <summary>
    /// リンクしているガチャViewのオブジェクト.
    /// </summary>
	public GameObject LinkViewObj {
		get { return linkViewObj; }
		set {
			linkViewObj = value;
		}
	}
	[SerializeField] GameObject linkViewObj;

	/// <summary>
    /// 強制ハイライト.
    /// </summary>
	public bool ForceHighlight
	{ 
		get {
			return this.GetScript<CustomButton>("txtp_Page").ForceHighlight;
		}
		set {
			this.GetScript<CustomButton>("txtp_Page").ForceHighlight = value;
		}
	}
	
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(Gacha gacha, Action<GameObject, ListItem_GachaPage> didTap, bool bLast = false)
	{
		Gacha = gacha;
		m_didTap = didTap;

		this.GetScript<Image>("img_Separation").gameObject.SetActive(!bLast);
		GetScript<TextMeshProUGUI>("txtp_Page").text = Gacha.name;

		this.SetCanvasCustomButtonMsg("txtp_Page", DidTapCategory);
	}

	// ボタン : タップ.
    void DidTapCategory()
	{
		if(m_didTap != null){
			m_didTap(LinkViewObj, this);         
		}
	}
       
	private Action<GameObject, ListItem_GachaPage> m_didTap;   
}
