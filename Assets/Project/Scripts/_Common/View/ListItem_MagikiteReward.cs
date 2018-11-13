using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// ListItem : マギカイトを予定報酬として表示する.
/// </summary>
public class ListItem_MagikiteReward : ViewBase
{   
    public void Init(int magikiteId, bool bIconOnly = false)
	{
		m_magikiteId = magikiteId;      
		IconLoader.LoadMagikite(m_magikiteId, IconSet);
		this.GetScript<GridLayoutGroup>("StarGrid").gameObject.SetActive(false);    // サイズ的に表示できないのでレアリティ表示はオフに.
		this.GetScript<Image>("WhitePanenl").gameObject.SetActive(false);
		this.GetScript<Image>("ListIconBg").gameObject.SetActive(!bIconOnly);
	}

	void IconSet(IconLoadSetting data, Sprite icon)
    {
		if (data.type == ItemTypeEnum.magikite && m_magikiteId == data.id) {
            GetScript<Image>("Icon").overrideSprite = icon;
        }
    }

	private int m_magikiteId;

	private const int MaxRarity = 5;
}
