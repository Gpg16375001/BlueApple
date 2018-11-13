using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_TrainingMaterial : ViewBase {

    public void Init(int materialId, int needCount)
    {
        m_material = MasterDataTable.chara_material.DataList.Find (x => x.id == materialId);
        var materialData = MaterialData.CacheGet (materialId); 
        int materialCount = materialData == null ? 0 : materialData.Count;

        IconLoader.LoadMaterial(materialId, LoadedMaterialIcon);

        if (needCount <= materialCount) {
            GetScript<Transform> ("Full").gameObject.SetActive (true);
            GetScript<Transform> ("Lack").gameObject.SetActive (false);
            GetScript<TextMeshProUGUI> ("txtp_MaterialNum").SetText (needCount);
            GetScript<TextMeshProUGUI> ("txtp_PossessionMaterialFull").SetText (materialCount);
        } else {
            GetScript<Transform> ("Full").gameObject.SetActive (false);
            GetScript<Transform> ("Lack").gameObject.SetActive (true);
            GetScript<TextMeshProUGUI> ("txtp_MaterialNumLack").SetText (needCount);
            GetScript<TextMeshProUGUI> ("txtp_PossessionMaterialLack").SetText (materialCount);
        }
    }

    void LoadedMaterialIcon(IconLoadSetting loadSetting, Sprite spt)
    {
        if (loadSetting.type == ItemTypeEnum.material && m_material.id == loadSetting.id) {
            GetScript<Image> ("MaterialIcon").overrideSprite = spt;
        }
    }

    void OnDestroy()
    {
        IconLoader.RemoveLoadedEvent (ItemTypeEnum.material, m_material.id, LoadedMaterialIcon);
    }

    CharaMaterial m_material;
}
