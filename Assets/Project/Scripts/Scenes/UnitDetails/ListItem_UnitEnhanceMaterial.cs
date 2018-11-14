using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// ListItem : キャラ強化素材.
/// </summary>
public class ListItem_UnitEnhanceMaterial : ViewBase
{   
	/// <summary>
	/// 識別用のID.MaterialId.
    /// </summary>
    public int ID { get { return m_material.id; } }

	/// <summary>
    /// 素材残数.
    /// </summary>
    public int RemainCount { get; private set; }

    /// <summary>
    /// 有効か無効か.
    /// </summary>
	public bool IsEnable 
	{ 
		get {
			return this.GetScript<CustomButton>("bt_EnhanceMaterial").interactable;
		}
		set {
			this.GetScript<CustomButton>("bt_EnhanceMaterial").interactable = value;
			this.GetScript<Image>("EnhanceMaterialIcon_disable").gameObject.SetActive(!value);
		}
	}


    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(CharaMaterial material, MaterialData data, Func<MaterialData,bool> didTap)
	{
        m_material = material;
		m_data = data;
		m_didTap = didTap;
		this.RemainCount = m_data != null ? m_data.Count: 0;

        // ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_MaterialNum").text = RemainCount.ToString();
		this.GetScript<RectTransform>("MaterialSelectNum").gameObject.SetActive(false); // 何個選んでるかルート.
		IconLoader.LoadMaterial(m_material.id, SetIcon);
        
		// ボタン.      
		this.GetScript<CustomButton>("bt_EnhanceMaterial").onClick.AddListener(DidTapItem);

		this.IsEnable = RemainCount > 0;
	}

    public void SetIcon(IconLoadSetting data, Sprite icon)
    {
		if(data.id == m_material.id){
			this.GetScript<Image>("EnhanceMaterialIcon").sprite = icon;
			this.GetScript<Image>("EnhanceMaterialIcon_disable").sprite = icon;
		}
    }

    /// <summary>
    /// 選択解除.
    /// </summary>
    public void ResetSelect()
	{
		this.RemainCount = m_data != null ? m_data.Count: 0;
		this.GetScript<TextMeshProUGUI>("txtp_MaterialNum").text = RemainCount.ToString();      
		this.GetScript<RectTransform>("MaterialSelectNum").gameObject.SetActive(false);
		this.IsEnable = this.RemainCount > 0;
	}

    /// <summary>
    /// 一つ選択状態を戻す.
    /// </summary>
    public void ReturnOneSelect()
	{
		if(this.RemainCount >= m_data.Count){
			return;
		}
		++this.RemainCount;
		this.GetScript<TextMeshProUGUI>("txtp_MaterialNum").text = this.RemainCount.ToString();
		this.GetScript<RectTransform>("MaterialSelectNum").gameObject.SetActive(this.chooseCount > 0);
		this.GetScript<TextMeshProUGUI>("txtp_SelectMaterialNum").text = this.chooseCount.ToString();
		this.IsEnable = true;
	}

    /// <summary>
    /// 消費反映.
    /// </summary>
	public void Apply()
	{
        if (m_data == null) {
            return;
        }
		m_data.Count = this.RemainCount;
		this.GetScript<RectTransform>("MaterialSelectNum").gameObject.SetActive(false);
		m_data.CacheSet();
	}

	// ボタン : タップ
    void DidTapItem()
	{
		if(this.RemainCount <= 0){
			return;
		}

		if(m_didTap != null){
			if(m_didTap(m_data)){
				this.RemainCount--;          
				this.GetScript<TextMeshProUGUI>("txtp_MaterialNum").text = this.RemainCount.ToString();
				this.GetScript<RectTransform>("MaterialSelectNum").gameObject.SetActive(true);
                this.GetScript<TextMeshProUGUI>("txtp_SelectMaterialNum").text = this.chooseCount.ToString();
			}
		}
		this.IsEnable &= this.RemainCount > 0;
	}

    void OnDestroy()
    {
        if (m_material != null) {
			IconLoader.RemoveLoadedEvent (ItemTypeEnum.material, m_material.id, SetIcon);
        }
    }

	private Func<MaterialData,bool> m_didTap;   // 選択上限に達していた場合はタップできないのでfalse.
	private MaterialData m_data;
    private CharaMaterial m_material;

    // 素材選択数.
	private int chooseCount { get { return m_data.Count - this.RemainCount; } }
}
