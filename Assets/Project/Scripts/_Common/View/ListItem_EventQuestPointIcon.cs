using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;

public class ListItem_EventQuestPointIcon : ViewBase {

    public void Init(int id)
    {
        m_ConsumerId = id;

        GetScript<Image> ("Icon").overrideSprite = null;
		IconLoader.LoadEventPoint (id, LoadedIcon);
    }

    void LoadedIcon(IconLoadSetting loadSetting, Sprite spt)
    {
        if (loadSetting.type == ItemTypeEnum.event_point && m_ConsumerId == loadSetting.id) {
            GetScript<Image> ("Icon").overrideSprite = spt;
        }
    }

    void OnDestroy()
    {
        IconLoader.RemoveLoadedEvent (ItemTypeEnum.consumer, m_ConsumerId, LoadedIcon);
    }

    int m_ConsumerId;
}
