using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_FormationName : ViewBase
{
    public void Init(FormationData formation, bool isUse, System.Action<ListItem_FormationName, FormationData> DidTap)
    {
        m_FormationData = formation;
        m_DidTap = DidTap;

        // アイコンの作成
        if (m_FormationIcon == null) {
            var iconRoot = this.GetScript<Transform> ("img_FormationIconBase").gameObject;
            var go = GameObjectEx.LoadAndCreateObject ("PartyEdit/FormationIcon", iconRoot);
            m_FormationIcon = go.GetComponent<FormationIcon> ();
        }
        m_FormationIcon.Init (formation.Formation);

        // 使用中かの設定
        SetUse (isUse);

        // 名前の設定
        this.GetScript<TextMeshProUGUI> ("txtp_FormationName").SetText (formation.Formation.name);

        // ボタンの設定
        this.SetCanvasCustomButtonMsg("bt_SmallList", DidTapItem);
    }

    /// <summary>
    /// 設定中アイコン表示設定
    /// </summary>
    /// <param name="isUse">If set to <c>true</c> is use.</param>
    public void SetUse(bool isUse)
    {
        this.GetScript<Transform>("img_SettingFormationMark").gameObject.SetActive(isUse);
    }

    public void SetSelect(bool selected)
    {
        if (selected) {
            this.GetScript<CustomButton> ("bt_SmallList").ForceHighlight = true;
        } else {
            this.GetScript<CustomButton> ("bt_SmallList").ForceHighlight = false;
        }
    }

    void DidTapItem()
    {
        if (m_DidTap != null) {
            m_DidTap (this, m_FormationData);
        }
    }

    private FormationIcon m_FormationIcon;
    private FormationData m_FormationData;
    private System.Action<ListItem_FormationName, FormationData> m_DidTap;
}
