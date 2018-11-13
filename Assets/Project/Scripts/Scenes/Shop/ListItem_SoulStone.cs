using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_SoulStone : ViewBase {
    public void SetInfo(CardBasedMaterial info)
    {
        m_Info = info;
        var rowMaterialData = MaterialData.CacheGet (info.card_based_row_material_id);
        m_RowMaterialDataCount = rowMaterialData == null ? 0 : rowMaterialData.Count;

        if (m_RowMaterialDataCount >= m_Info.card_based_row_material_need_number) {
            GetScript<RectTransform> ("Lack").gameObject.SetActive (false);
            GetScript<RectTransform> ("Full").gameObject.SetActive(true);

            GetScript<TextMeshProUGUI> ("Full/txtp_RequiredSoulPiece").SetText (m_Info.card_based_row_material_need_number);
            GetScript<TextMeshProUGUI> ("Full/txtp_SoulPieceNum").SetText (m_RowMaterialDataCount);
        } else {
            GetScript<RectTransform> ("Lack").gameObject.SetActive (true);
            GetScript<RectTransform> ("Full").gameObject.SetActive(false);

            GetScript<TextMeshProUGUI> ("Lack/txtp_RequiredSoulPiece").SetText (m_Info.card_based_row_material_need_number);
            GetScript<TextMeshProUGUI> ("Lack/txtp_SoulPieceNum").SetText (m_RowMaterialDataCount);
        }

        GetScript<Image> ("Icon").overrideSprite = null;
        IconLoader.LoadMaterial (m_Info.card_based_material_id, LoadedIcon);

        GetScript<Image> ("Full/IconSoulPiece").overrideSprite = null;
        GetScript<Image> ("Lack/IconSoulPiece").overrideSprite = null;
        IconLoader.LoadMaterial (m_Info.card_based_row_material_id, LoadedIconSoulPiece);

        if (!m_IsInit) {
            SetCanvasCustomButtonMsg ("ListIconBg", DidTap);
            m_IsInit = true;
        }
    }

    void DidTap()
    {
        View_ShopSoulStonePurchasePop.Create (m_Info, m_RowMaterialDataCount, UpdateIcon);
    }

    void LoadedIcon(IconLoadSetting data, Sprite icon)
    {
        if (data.id == m_Info.card_based_material_id && data.type == ItemTypeEnum.material) {
            GetScript<Image> ("Icon").overrideSprite = icon;
        }
    }

    void LoadedIconSoulPiece(IconLoadSetting data, Sprite icon)
    {
        if (data.id == m_Info.card_based_row_material_id && data.type == ItemTypeEnum.material) {
            GetScript<Image> ("Full/IconSoulPiece").overrideSprite = icon;
            GetScript<Image> ("Lack/IconSoulPiece").overrideSprite = icon;
        }
    }

    void UpdateIcon(int resultNum)
    {
        m_RowMaterialDataCount = resultNum;

        if (m_RowMaterialDataCount >= m_Info.card_based_row_material_need_number) {
            GetScript<RectTransform> ("Lack").gameObject.SetActive (false);
            GetScript<RectTransform> ("Full").gameObject.SetActive(true);

            GetScript<TextMeshProUGUI> ("Full/txtp_RequiredSoulPiece").SetText (m_Info.card_based_row_material_need_number);
            GetScript<TextMeshProUGUI> ("Full/txtp_SoulPieceNum").SetText (m_RowMaterialDataCount);
        } else {
            GetScript<RectTransform> ("Lack").gameObject.SetActive (true);
            GetScript<RectTransform> ("Full").gameObject.SetActive(false);

            GetScript<TextMeshProUGUI> ("Lack/txtp_RequiredSoulPiece").SetText (m_Info.card_based_row_material_need_number);
            GetScript<TextMeshProUGUI> ("Lack/txtp_SoulPieceNum").SetText (m_RowMaterialDataCount);
        }
    }

    bool m_IsInit = false;
    CardBasedMaterial m_Info;
    int m_RowMaterialDataCount;
}
