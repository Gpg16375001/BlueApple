using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// ListItem : キャラ進化素材リスト.
/// </summary>
public class ListItem_UnitEvolutionMaterial : ViewBase
{
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(MaterialData data, CharaMaterial material, int needCnt)
	{
		m_data = data;
		m_material = material;
		m_needCnt = needCnt;

		this.GetScript<TextMeshProUGUI>("txtp_EvolutionMaterialName").text = m_material.name;

		var rootName = isEnough ? "Full" : "Lack";
		this.GetScript<RectTransform>("Full").gameObject.SetActive(isEnough);
		this.GetScript<RectTransform>("Lack").gameObject.SetActive(!isEnough);
		this.GetScript<TextMeshProUGUI>(string.Format("{0}/txtp_PossessionMaterial{1}", rootName, rootName)).text = m_data != null ? m_data.Count.ToString(): "0";
		this.GetScript<TextMeshProUGUI>(string.Format("{0}/txtp_MaterialNum", rootName)).SetText(m_needCnt);

        this.GetScript<Image> ("EvolutionMaterialIcon").overrideSprite = null;
        IconLoader.LoadMaterial (m_material.id, LoadedIconSprite);
	}

    void LoadedIconSprite(IconLoadSetting loadInfo, Sprite sprite)
    {
        if(loadInfo.type == ItemTypeEnum.material && loadInfo.id == m_material.id) {
            this.GetScript<Image> ("EvolutionMaterialIcon").overrideSprite = sprite;
        }
    }
    void OnDestroy()
    {
        IconLoader.RemoveLoadedEvent (ItemTypeEnum.material, m_material.id, LoadedIconSprite);
    }

    /// <summary>
    /// 進化反映.
    /// </summary>
    public void ApplyEvolution()
	{
		if(!isEnough){
			return;
		}
		m_data.Count -= m_needCnt;

		var rootName = isEnough ? "Full" : "Lack";
        this.GetScript<RectTransform>("Full").gameObject.SetActive(isEnough);
        this.GetScript<RectTransform>("Lack").gameObject.SetActive(!isEnough);
        this.GetScript<TextMeshProUGUI>(string.Format("{0}/txtp_PossessionMaterial{1}", rootName, rootName)).text = m_data != null ? m_data.Count.ToString() : "0";

		m_data.CacheSet();
	}

	private bool isEnough { get { return m_data != null && m_data.Count >= m_needCnt; } }

	private int m_needCnt;
	private MaterialData m_data;
	private CharaMaterial m_material;
}
