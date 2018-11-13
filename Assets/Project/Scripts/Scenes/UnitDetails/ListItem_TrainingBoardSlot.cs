using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class ListItem_TrainingBoardSlot : ViewBase {
    private GameObject m_SelectedObj;
    private bool m_Selected;
    public bool Selected {
        get {
            return m_Selected;
        }
        set {
            if (m_Selected != value) {
                m_Selected = value;
                if(m_SelectedObj != null) {
                    m_SelectedObj.SetActive(m_Selected);
                }
            }
        }
    }


    public MaterialGrowthBoardSlot SlotInfo {
        get {
            return m_SlotInfo;
        }
    }

    public void Init(CardData card, MaterialGrowthBoardSlot slot, bool released, bool meetCondtion, System.Action<ListItem_TrainingBoardSlot, MaterialGrowthBoardSlot, bool, bool> tapCallback)
    {
        m_SlotInfo = slot;
        m_Released = released;
        m_MeetCondition = meetCondtion;
        m_TapCallback = tapCallback;

        m_SelectedObj = GetScript<Transform> ("img_TrainingBoardSlotSelect").gameObject;
        var slotButton = GetScript<CustomButton> ("bt_TrainingBoardSlot");

        slotButton.onClick.AddListener (DidTap);

        UpdateSlot (card, released, meetCondtion, false);
    }

    public void UpdateSlot(CardData card, bool released, bool meetCondtion, bool playReleasedEffect)
    {

        if (m_Released != released && released && playReleasedEffect) {
            m_SlotReleaseEffect = GameObjectEx.LoadAndCreateObject ("UnitDetails/eff_TrainingBoardSlot", gameObject);
        }
        m_Released = released;
        m_MeetCondition = meetCondtion;

        var doneImage = GetScript<Transform> ("img_TrainingBoardSlotDone").gameObject;
        var offImage = GetScript<Transform> ("img_TrainingBoardSlotOff").gameObject;
        var slotButton = GetScript<CustomButton> ("bt_TrainingBoardSlot");

        offImage.gameObject.SetActive (!released &&
            (!meetCondtion || !m_SlotInfo.item_combination.IsRelease(card.CardId, card.Card.element.Enum, card.Card.character.belonging.Enum, card.Card.role)));
        doneImage.SetActive (released);

        SetSprite (m_SlotInfo.parameter_type);
        SetText (m_SlotInfo.parameter_type, m_SlotInfo.parameter_value);

    }

    public void UpdateInteractable(CardData card, Dictionary<int, int> selectedMaterials, int totalCost)
    {
    }

    private void SetSprite(MaterialGrowthBoardParameterTypeEnum type)
    {
        var spriteName = "";
        var uguiSpriteChange = GetScript<uGUISprite> ("bt_TrainingBoardSlot");
        switch (type) {
        case MaterialGrowthBoardParameterTypeEnum.hp:
            spriteName = "bt_TrainingBoardSlotHP";
            break;
        case MaterialGrowthBoardParameterTypeEnum.attack:
            spriteName = "bt_TrainingBoardSlotATK";
            break;
        case MaterialGrowthBoardParameterTypeEnum.defence:
            spriteName = "bt_TrainingBoardSlotDEF";
            break;
        case MaterialGrowthBoardParameterTypeEnum.agility:
            spriteName = "bt_TrainingBoardSlotSPD";
            break;
        case MaterialGrowthBoardParameterTypeEnum.action_skill_level:
        case MaterialGrowthBoardParameterTypeEnum.special_skill_level:
        case MaterialGrowthBoardParameterTypeEnum.passive_skill_level:
            spriteName = "bt_TrainingBoardSlotSkill";
            break;
        }
        uguiSpriteChange.ChangeSprite (spriteName);
    }

    private void SetText(MaterialGrowthBoardParameterTypeEnum type, int value)
    {
        var statusValueObj = GetScript<Transform> ("StatusValue").gameObject;
        var parameterValueTxt = GetScript<TextMeshProUGUI> ("txtp_Num");
        var freeTextObj = GetScript<Transform> ("FreeText").gameObject;
        var txtpFreeText = GetScript<TextMeshProUGUI> ("txtp_Text");

        switch (type) {
        case MaterialGrowthBoardParameterTypeEnum.hp:
        case MaterialGrowthBoardParameterTypeEnum.attack:
        case MaterialGrowthBoardParameterTypeEnum.defence:
        case MaterialGrowthBoardParameterTypeEnum.agility:
            statusValueObj.SetActive (true);
            freeTextObj.SetActive (false);
            parameterValueTxt.SetTextFormat ("+{0}", value);
            break;

        case MaterialGrowthBoardParameterTypeEnum.action_skill_level:
        case MaterialGrowthBoardParameterTypeEnum.special_skill_level:
        case MaterialGrowthBoardParameterTypeEnum.passive_skill_level:
            statusValueObj.SetActive (false);
            freeTextObj.SetActive (true);
            txtpFreeText.SetText ("LvUP");
            break;
        }
    }

    private void DidTap()
    {
        if (m_TapCallback != null) {
            m_TapCallback (this, m_SlotInfo, m_Released, m_MeetCondition);
        }
    }

    void OnDisable()
    {
        if (m_SlotReleaseEffect != null) {
            GameObject.Destroy (m_SlotReleaseEffect);
            m_SlotReleaseEffect = null;
        }
    }

    private MaterialGrowthBoardSlot m_SlotInfo;
    private bool m_Released;
    private bool m_MeetCondition;
    private System.Action<ListItem_TrainingBoardSlot, MaterialGrowthBoardSlot, bool, bool> m_TapCallback;

    private GameObject m_SlotReleaseEffect;
}
