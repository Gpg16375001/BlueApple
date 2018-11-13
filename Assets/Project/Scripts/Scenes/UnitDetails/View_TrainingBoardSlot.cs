using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class View_TrainingBoardSlot : ViewBase {
    public int BoardIndex {
        get {
            return m_BoardData.Index;
        }
    }

    public BoardData BoardData {
        get {
            return m_BoardData;
        }
    }

    public List<MaterialGrowthBoardSlot> SlotDataList
    {
        get {
            return m_SlotDataList;
        }
    }


    public void Init(CardData card, BoardData boardData, bool isLimitBreaked, MaterialGrowthBoardDefinition definition, List<MaterialGrowthBoardSlot> slots,
        System.Action<MaterialGrowthBoardSlot, bool, bool, bool> tapCallback,
        System.Action<MaterialGrowthBoardItemCombination> tapLimitBreak)
    {
        m_DidTapSlot = tapCallback;
        m_BoardData = boardData;
        m_SlotDataList = slots;
        m_BoardDefinition = definition;
        m_DidTapLimitBreak = tapLimitBreak;

        var limitBreak = GetScript<Transform> ("LimitBreak").gameObject;
        if (definition.limit_break_item_combination_id.HasValue && !isLimitBreaked) {
            SetCanvasCustomButtonMsg ("bt_TrainingBoardLimitBreak", DidTapLimitBreak);
            limitBreak.SetActive (true);
        } else {
            limitBreak.SetActive (false);
        }

        var maxCount = slots.Count;
        for (int i = 0; i < maxCount; ++i) {
            var slot = slots [i];
            var slotObject = GetScript<Transform> (string.Format ("TrainingBoardSlot{0}", slot.slot_index)).gameObject;
            // 解放されているか？
            bool released =  boardData.UnlockedSlotList.Contains(slot.slot_index);
            // スロットの解放条件を満たしているか？
            bool meetCondition = true;
            if (!released) {
                if (slot.release_conditoin1.HasValue || slot.release_conditoin2.HasValue || slot.release_conditoin3.HasValue || slot.release_conditoin4.HasValue) {
                    meetCondition = false;
                    if (slot.release_conditoin1.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin1.Value);
                    }
                    if (slot.release_conditoin2.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin2.Value);
                    }
                    if (slot.release_conditoin3.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin3.Value);
                    }
                    if (slot.release_conditoin4.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin4.Value);
                    }
                }
            }
            var slotScript = slotObject.GetOrAddComponent<ListItem_TrainingBoardSlot> ();
            slotScript.Init (card, slot, released, meetCondition, DidSlotTap);
            m_SlotObjectList.Add (slotScript);
        }
        GetScript<TextMeshProUGUI> ("BoardNo/txtp_Num").SetText (definition.board_index);
    }

    public void UpdateSlot(CardData card, BoardData boardData, bool isLimitBreaked)
    {
        var limitBreak = GetScript<Transform> ("LimitBreak").gameObject;
        if (m_BoardDefinition.limit_break_item_combination_id.HasValue && !isLimitBreaked) {
            limitBreak.SetActive (true);
        } else {
            limitBreak.SetActive (false);
        }

        foreach (var slotObj in m_SlotObjectList) {
            var slot = slotObj.SlotInfo;
            bool released =  boardData.UnlockedSlotList.Contains(slot.slot_index);
            // スロットの解放条件を満たしているか？
            bool meetCondition = true;
            if (!released) {
                if (slot.release_conditoin1.HasValue || slot.release_conditoin2.HasValue || slot.release_conditoin3.HasValue || slot.release_conditoin4.HasValue) {
                    meetCondition = false;
                    if (slot.release_conditoin1.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin1.Value);
                    }
                    if (slot.release_conditoin2.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin2.Value);
                    }
                    if (slot.release_conditoin3.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin3.Value);
                    }
                    if (slot.release_conditoin4.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin4.Value);
                    }
                }
            }
            slotObj.UpdateSlot (card, released, meetCondition, true);
        }
    }

    public void UpdateSlotInteractable(CardData card, Dictionary<int, int> selectedMaterials, int totalCost)
    {
        m_SlotObjectList.ForEach (x => x.UpdateInteractable(card, selectedMaterials, totalCost));
    }

    public void Deselect()
    {
        m_SlotObjectList.ForEach (x => x.Selected = false);
    }

    private void DidSlotTap(ListItem_TrainingBoardSlot item, MaterialGrowthBoardSlot slot, bool released, bool meetCondtion)
    {
        var prevSelect = item.Selected;
        m_SlotObjectList.ForEach (x => x.Selected = false);
        item.Selected = !prevSelect;

        if (m_DidTapSlot != null) {
            m_DidTapSlot (slot, item.Selected, released, meetCondtion);
        }
    }

    void DidTapLimitBreak()
    {
        var limitBreakItems = MasterDataTable.material_growth_board_item_combination [m_BoardDefinition.limit_break_item_combination_id.Value];
        if(m_DidTapLimitBreak != null) {
            m_DidTapLimitBreak (limitBreakItems);
        }
    }

    private BoardData m_BoardData;
    private bool m_PrevSelecterItemReleased;
    private MaterialGrowthBoardDefinition m_BoardDefinition;
    private List<ListItem_TrainingBoardSlot> m_SlotObjectList = new List<ListItem_TrainingBoardSlot>();
    private List<MaterialGrowthBoardSlot> m_SlotDataList = new List<MaterialGrowthBoardSlot>();
    private System.Action<MaterialGrowthBoardSlot, bool, bool, bool> m_DidTapSlot;
    private System.Action<MaterialGrowthBoardItemCombination> m_DidTapLimitBreak;
}
