using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;

public class ListItem_MaterialIcon : ViewBase {

	public void InitIconOnly(int materialId, int? number = null)
	{
		Init(materialId, number);
		GetScript<Image>("img_Material").gameObject.SetActive(false);
	}
    public void Init(int materialId, int? number = null)
    {
        m_MaterialId = materialId;

        var txtpPossesion = GetScript<TextMeshProUGUI> ("txtp_PossessionMaterial");
        if (number.HasValue) {
            txtpPossesion.gameObject.SetActive (true);
            txtpPossesion.SetText (number.Value);
        } else {
            txtpPossesion.gameObject.SetActive (false);
        }
        GetScript<Image> ("MaterialIcon").overrideSprite = null;
        IconLoader.LoadMaterial (materialId, LoadedMaterialIcon);
    }

    void LoadedMaterialIcon(IconLoadSetting loadSetting, Sprite spt)
    {
        if (loadSetting.type == ItemTypeEnum.material && m_MaterialId == loadSetting.id) {
            GetScript<Image> ("MaterialIcon").overrideSprite = spt;
        }
    }

    void OnDestroy()
    {
        IconLoader.RemoveLoadedEvent (ItemTypeEnum.material, m_MaterialId, LoadedMaterialIcon);
    }

    int m_MaterialId;
}
