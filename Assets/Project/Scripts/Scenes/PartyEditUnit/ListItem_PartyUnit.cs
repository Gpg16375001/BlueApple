using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_PartyUnit : ViewBase
{
    public void Init(CardData card, UnitListSortType sortOrder, bool isDispRemove = false, bool isDispOrganizing = false, Action<CardData> DidTap = null, Action<CardData> DidLongPress = null)
    {
        if (m_CardData != null) {
			IconLoader.RemoveLoadedEvent (ItemTypeEnum.card, m_CardData.CardId, SetIcon);
        }

        m_CardData = card;
        m_DidTap = DidTap;
        m_DidLongPress = DidLongPress;
        var txtpUnitStatus = this.GetScript<TextMeshProUGUI> ("txtp_UnitStatus");
        var elementIcon = GetScript<Image> ("ElementIcon");
        if (m_CardData == null) {
            txtpUnitStatus.gameObject.SetActive (false);
            elementIcon.gameObject.SetActive (false);
            return;
        }


        IconLoader.LoadCardIcon (m_CardData, SetIcon);

        txtpUnitStatus.gameObject.SetActive (true);
        switch (sortOrder) {
        case UnitListSortType.Level:
        case UnitListSortType.Rarity:
        case UnitListSortType.Create:
            txtpUnitStatus.SetText(string.Format("Lv.{0}", card.Level));
            break;
        case UnitListSortType.HP:
            txtpUnitStatus.SetText(string.Format("Hp.{0}", card.Parameter.Hp));
            break;
        case UnitListSortType.ATK:
            txtpUnitStatus.SetText(string.Format("Atk.{0}", card.Parameter.Attack));
            break;
        case UnitListSortType.DEF:
            txtpUnitStatus.SetText(string.Format("Def.{0}", card.Parameter.Defense));
            break;
        case UnitListSortType.SPD:
            txtpUnitStatus.SetText(string.Format("Spd.{0}", card.Parameter.Agility));
            break;
        }

        elementIcon.gameObject.SetActive (true);
        elementIcon.sprite = IconLoader.LoadElementIcon (m_CardData.Parameter.Element);

        // 外すボタンの表示
        this.GetScript<Transform>("img_UnitRemove").gameObject.SetActive(isDispRemove);

        // 編成中表示
        this.GetScript<Transform>("img_UnitOrganizing").gameObject.SetActive(isDispOrganizing);

        var iconButton = GetScript<CustomButton> ("img_IconChFrameSR");
        if (m_DidTap == null && DidLongPress == null) {
            // コールバックが設定されない場合はボタンとして機能しないようにする。
            iconButton.targetGraphic.raycastTarget = false;
            iconButton.enabled = false;
        } else {
            iconButton.targetGraphic.raycastTarget = true;
            iconButton.enabled = true;
            iconButton.onClick.RemoveListener (DidTapIcon);
            iconButton.onClick.AddListener (DidTapIcon);
            if (DidLongPress == null) {
                iconButton.m_EnableLongPress = false;
            } else {
                iconButton.m_EnableLongPress = true;
                iconButton.onLongPress.RemoveListener (DidLongPressIcon);
                iconButton.onLongPress.AddListener (DidLongPressIcon);
            }
        }
    }

    public void SetIcon(IconLoadSetting data, Sprite icon)
    {
        // 更新状態によっては内部データが変わっている可能性があるので判定が必要
        if(m_CardData.CardId == data.id) {
            var iconImg = this.GetScript<Image> ("IconChCDummy");
            iconImg.sprite = icon;
        }
    }

    private void DidTapIcon()
    {
        if (m_DidTap != null) {
            m_DidTap (m_CardData);
        }
    }

    private void DidLongPressIcon()
    {
        if (m_DidLongPress != null) {
            m_DidLongPress (m_CardData);
        }
    }

    void OnDestroy()
    {
        if (m_CardData != null) {
			IconLoader.RemoveLoadedEvent (ItemTypeEnum.card, m_CardData.CardId, SetIcon);
        }
    }

    private CardData m_CardData;
    private Action<CardData> m_DidTap;
    private Action<CardData> m_DidLongPress;
}
