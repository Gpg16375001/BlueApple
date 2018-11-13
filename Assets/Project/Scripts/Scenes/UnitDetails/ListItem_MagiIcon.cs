using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_MagiIcon : ViewBase {
    private const int MaxRarity = 5;

    private bool m_Selected;
    public bool Selected {
        get {
            return m_Selected;
        }
        set {
            if (m_Selected != value) {
                m_Selected = value;
                GetScript<RectTransform> ("img_IconSelectFrame").gameObject.SetActive (m_Selected);
            }
        }
    }

    public void UpdateItem(MagikiteData magikite, Action<ListItem_MagiIcon, MagikiteData> didTap = null, Action<MagikiteData> didLongTap = null, bool isRemove = false, bool isLock = false, bool isEquiped = false, bool isGrayOut = false, bool selected = false)
    {
        var button = GetScript<CustomButton> ("ListIconBg");
        if (!m_IsInit) {
            button.onLongPress.AddListener (DidLongTap);
            button.onClick.AddListener (DidTap);

            m_IsInit = true;
        }

        // アイコンの中身を更新するときにロード処理などを止めたりする
        var oldMagikiteData = m_MagikiteData;
        m_MagikiteData = magikite;
        m_Magikite = magikite.Magikite;
        if (oldMagikiteData != null && oldMagikiteData.BagId != m_MagikiteData.BagId) {
            IconLoader.RemoveLoadedEvent (ItemTypeEnum.magikite, oldMagikiteData.MagikiteId, IconSet);
        }

        if (m_MagikiteData == null) {
            GetScript<Image> ("Icon").overrideSprite = null;
            return;
        }
        IconLoader.LoadMagikite (m_MagikiteData.MagikiteId, IconSet);
        m_DidTap = didTap;
        m_DidLongTap = didLongTap;
        if (m_DidLongTap != null) {
            // LongTapの設定がある時は有効にする。
            button.m_EnableLongPress = true;
        }

        SetRarity ();

        Selected = selected;
        GetScript<RectTransform> ("img_MagiRemove").gameObject.SetActive (isRemove);
        GetScript<RectTransform> ("img_Lock").gameObject.SetActive (isLock);
        GetScript<RectTransform> ("img_MagiEquip").gameObject.SetActive (isEquiped);
        GetScript<RectTransform> ("img_Grayout").gameObject.SetActive (isGrayOut);
    }

    private void IconSet(IconLoadSetting data, Sprite icon)
    {
        if(data.type == ItemTypeEnum.magikite && m_MagikiteData.MagikiteId == data.id) {
            GetScript<Image> ("Icon").overrideSprite = icon;
        }
    }

    private void SetRarity()
    {
        for (int i = 1; i <= MaxRarity; ++i) {
            GetScript<RectTransform>(string.Format("Star{0}", i)).gameObject.SetActive(i <= m_Magikite.rarity);
        }
    }

    private void DidTap()
    {
        if (m_DidTap != null) {
            m_DidTap (this, m_MagikiteData);
        }
    }

    private void DidLongTap()
    {
        if (m_DidLongTap != null) {
            m_DidLongTap (m_MagikiteData);
        }
    }

    private void Destory()
    {
        IconLoader.RemoveLoadedEvent (ItemTypeEnum.magikite, m_MagikiteData.MagikiteId, IconSet);
    }

    bool m_IsInit = false;
    Action<ListItem_MagiIcon, MagikiteData> m_DidTap;
    Action<MagikiteData> m_DidLongTap;
    Magikite m_Magikite;
    MagikiteData m_MagikiteData;
}
