using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_TrainingBundleOKPop : PopupViewBase {
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_TrainingBundleOKPop Create(CardData card, BoardData boardData, MaterialGrowthBoardSlot[] slots, System.Action<MaterialGrowthBoardSlot[]> tapOK)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_TrainingBundleOKPop");
        var c = go.GetOrAddComponent<View_TrainingBundleOKPop>();
        c.InitInternal(card, boardData, slots, tapOK);
        return c;
    }

    private void InitInternal(CardData card, BoardData boardData, MaterialGrowthBoardSlot[] slots, System.Action<MaterialGrowthBoardSlot[]> tapOK)
    {
        
        int totalCost = 0;
        if (slots.Any (x => x.item_combination.IsRelease (card.CardId, card.Parameter.Element.Enum, card.Parameter.belonging.Enum, card.Card.role))) {
            // 開けられるものがある場合は開けられるものだけ選択する。
            List<MaterialGrowthBoardSlot> releasebleSlot = new List<MaterialGrowthBoardSlot>();
            Dictionary<int, int> selectedMaterials = new Dictionary<int, int>();

            List<MaterialGrowthBoardSlot> copySlots = new List<MaterialGrowthBoardSlot> (slots);
            List<MaterialGrowthBoardSlot> sortedSlots = new List<MaterialGrowthBoardSlot> ();

            int count = 0;
            // 解放の順序のための並び替え
            do {
                foreach (var slot in copySlots.OrderBy(x => x.slot_index).ToArray()) {
                    if (slot.release_conditoin1.HasValue || slot.release_conditoin2.HasValue || slot.release_conditoin3.HasValue || slot.release_conditoin4.HasValue) {
                        if(slot.release_conditoin1.HasValue && (sortedSlots.Any(x => x.slot_index == slot.release_conditoin1.Value) || boardData.UnlockedSlotList.Contains(slot.release_conditoin1.Value))) {
                            copySlots.Remove (slot);
                            sortedSlots.Add(slot);
                        } else if(slot.release_conditoin2.HasValue && (sortedSlots.Any(x => x.slot_index == slot.release_conditoin2.Value) || boardData.UnlockedSlotList.Contains(slot.release_conditoin2.Value))) {
                            copySlots.Remove (slot);
                            sortedSlots.Add(slot);
                        } else if(slot.release_conditoin3.HasValue && (sortedSlots.Any(x => x.slot_index == slot.release_conditoin3.Value) || boardData.UnlockedSlotList.Contains(slot.release_conditoin3.Value))) {
                            copySlots.Remove (slot);
                            sortedSlots.Add(slot);
                        } else if(slot.release_conditoin4.HasValue && (sortedSlots.Any(x => x.slot_index == slot.release_conditoin4.Value) || boardData.UnlockedSlotList.Contains(slot.release_conditoin4.Value))) {
                            copySlots.Remove (slot);
                            sortedSlots.Add(slot);
                        }
                    } else {
                        copySlots.Remove (slot);
                        sortedSlots.Add(slot);
                    }
                }
                // 無限ループ回避処理
                count++;
            } while(copySlots.Count > 0 && count < 10000);

            foreach (var slot in sortedSlots)
            {
                if (!slot.item_combination.IsRelease (card.CardId, card.Parameter.Element.Enum, card.Parameter.belonging.Enum, card.Card.role, selectedMaterials, totalCost)) {
                    continue;
                }

                if (slot.release_conditoin1.HasValue || slot.release_conditoin2.HasValue || slot.release_conditoin3.HasValue || slot.release_conditoin4.HasValue)
                {
                    bool meetCondition = false;
                    if (slot.release_conditoin1.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin1.Value) ||
                            releasebleSlot.Any(x => x.slot_index == slot.release_conditoin1.Value);
                    }
                    if (slot.release_conditoin2.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin2.Value) ||
                            releasebleSlot.Any(x => x.slot_index == slot.release_conditoin2.Value);
                    }
                    if (slot.release_conditoin3.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin3.Value) ||
                            releasebleSlot.Any(x => x.slot_index == slot.release_conditoin3.Value);
                    }
                    if (slot.release_conditoin4.HasValue) {
                        meetCondition |= boardData.UnlockedSlotList.Contains (slot.release_conditoin4.Value) ||
                            releasebleSlot.Any(x => x.slot_index == slot.release_conditoin4.Value);
                    }
                    if (!meetCondition) {
                        continue;
                    }
                }

                releasebleSlot.Add (slot);
                var need = slot.item_combination.GetNeedMaterials (card.CardId, card.Parameter.Element.Enum, card.Parameter.belonging.Enum, card.Card.role);
                foreach (var kv in need) {
                    if (selectedMaterials.ContainsKey (kv.Key)) {
                        selectedMaterials [kv.Key] += kv.Value;
                    } else {
                        selectedMaterials [kv.Key] = kv.Value;
                    }
                }
                totalCost += slot.item_combination.cost;
            }
            slots = releasebleSlot.ToArray ();
        }

        m_ReleasebleSlots = slots;
        m_TapOK = tapOK;
        GetScript<TextMeshProUGUI> ("txtp_UnitHP").SetText (card.Parameter.MaxHp);
        GetScript<TextMeshProUGUI> ("txtp_UnitATK").SetText (card.Parameter.Attack);
        GetScript<TextMeshProUGUI> ("txtp_UnitDEF").SetText (card.Parameter.Defense);
        GetScript<TextMeshProUGUI> ("txtp_UnitSPD").SetText (card.Parameter.Agility);

        var hp = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.hp);
        var atk = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.attack);
        var def = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.defence);
        var agi = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.agility);
        var action_level = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.action_skill_level);
        var passive_level = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.passive_skill_level);
        var special_level = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.special_skill_level);

        var UnitStatus = GetScript<RectTransform> ("UnitStatus").gameObject;
        UnitStatus.SetActive (hp > 0 || atk > 0 || def > 0 || agi > 0);
        if (UnitStatus.activeSelf) {
            // 上がってないパラメータは非表示
            var afterHp = GetScript<RectTransform> ("HP/After").gameObject;
            if (hp > 0) {
                afterHp.SetActive (true);
                GetScript<TextMeshProUGUI> ("txtp_AfterHP").SetText (card.Parameter.MaxHp + hp);
            } else {
                afterHp.SetActive (false);
            }

            var afterAtk = GetScript<RectTransform> ("ATK/After").gameObject;
            if (atk > 0) {
                afterAtk.SetActive (true);
                GetScript<TextMeshProUGUI> ("txtp_AfterATK").SetText (card.Parameter.Attack + atk);
            } else {
                afterAtk.SetActive (false);
            }

            var afterDef = GetScript<RectTransform> ("DEF/After").gameObject;
            if (def > 0) {
                afterDef.SetActive (true);
                GetScript<TextMeshProUGUI> ("txtp_AfterDEF").SetText (card.Parameter.Defense + def);
            } else {
                afterDef.SetActive (false);
            }

            var afterSpd = GetScript<RectTransform> ("SPD/After").gameObject;
            if (agi > 0) {
                afterSpd.SetActive (true);
                GetScript<TextMeshProUGUI> ("txtp_AfterSPD").SetText (card.Parameter.Agility + agi);
            } else {
                afterSpd.SetActive (false);
            }
        }

        int skillCount = 1;
        if (action_level > 0) {
            var skill = GetScript<RectTransform> (string.Format("Skill{0}", skillCount)).gameObject;
            var actionSkill = card.Parameter.UnitActionSkillList.FirstOrDefault (x => !x.IsNormalAction);
            if (actionSkill != null) {
                skill.SetActive (true);
                GetScript<TextMeshProUGUI> (string.Format("txtp_SkillName{0}", skillCount)).SetText (actionSkill.Skill.display_name);
                GetScript<TextMeshProUGUI> (string.Format("txtp_SkillLv{0}", skillCount)).SetText (actionSkill.Level);
                GetScript<TextMeshProUGUI> (string.Format("txtp_AfterSkillLv{0}", skillCount)).SetText (actionSkill.Level + 1);
            } else {
                skill.SetActive (false);
            }
            skillCount++;
        }
        if (passive_level > 0) {

            var skill = GetScript<RectTransform> (string.Format("Skill{0}", skillCount)).gameObject;
            var passiveSkill = card.Parameter.PassiveSkillList.FirstOrDefault ();
            if (passiveSkill != null) {
                skill.SetActive (true);
                GetScript<TextMeshProUGUI> (string.Format("txtp_SkillName{0}", skillCount)).SetText (passiveSkill.Skill.display_name);
                GetScript<TextMeshProUGUI> (string.Format("txtp_SkillLv{0}", skillCount)).SetText (passiveSkill.Level);
                GetScript<TextMeshProUGUI> (string.Format("txtp_AfterSkillLv{0}", skillCount)).SetText (passiveSkill.Level + 1);

            } else {
                skill.SetActive (false);
            }
            skillCount++;
        } 
        if (special_level > 0) {
            var skill = GetScript<RectTransform> (string.Format("Skill{0}", skillCount)).gameObject;
            var specialSkill = card.Parameter.SpecialSkill;
            if (specialSkill != null) {
                skill.SetActive (true);
                GetScript<TextMeshProUGUI> (string.Format("txtp_SkillName{0}", skillCount)).SetText (specialSkill.Skill.display_name);
                GetScript<TextMeshProUGUI> (string.Format("txtp_SkillLv{0}", skillCount)).SetText (specialSkill.Level);
                GetScript<TextMeshProUGUI> (string.Format("txtp_AfterSkillLv{0}", skillCount)).SetText (specialSkill.Level + 1);
            } else {
                skill.SetActive (false);
            }
            skillCount++;
        }

        for (; skillCount <= 3; skillCount++) {
            GetScript<RectTransform> (string.Format ("Skill{0}", skillCount)).gameObject.SetActive (false);
        }

        SetCanvasCustomButtonMsg ("OK/bt_CommonS02", DidTapOK);
        SetCanvasCustomButtonMsg ("Cancel/bt_CommonS01", DidTapCancel);
        SetCanvasCustomButtonMsg ("bt_Close", DidTapCancel);

        bool isNeedMaterial = true;
        totalCost = 0;
        if (slots.Length > 0) {
            Dictionary<int, int> selectedMaterials = new Dictionary<int, int>();
            foreach (var slot in slots) {
                var need = slot.item_combination.GetNeedMaterials (card.CardId, card.Parameter.Element.Enum, card.Parameter.belonging.Enum, card.Card.role);
                foreach (var kv in need) {
                    if (selectedMaterials.ContainsKey (kv.Key)) {
                        selectedMaterials [kv.Key] += kv.Value;
                    } else {
                        selectedMaterials [kv.Key] = kv.Value;
                    }
                }
                totalCost += slot.item_combination.cost;
            }
            var materialGridObj = GetScript<Transform> ("MaterialGrid").gameObject;
            foreach (var needMaterial in selectedMaterials) {
                var go =GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_TrainingMaterial", materialGridObj);
                go.GetOrAddComponent<ListItem_TrainingMaterial> ().Init (needMaterial.Key, needMaterial.Value);
                var materialData = MaterialData.CacheGet (needMaterial.Key);
                int count = materialData != null ? materialData.Count : 0;
                // 必要な素材が足りているか判断
                isNeedMaterial &= count >= needMaterial.Value;
            }
        }
        isNeedMaterial &= AwsModule.UserData.UserData.GoldCount >= totalCost;

        GetScript<TextMeshProUGUI> ("Cost/txtp_Coin").SetTextFormat ("{0:#,0}", totalCost);

        GetScript<CustomButton> ("OK/bt_CommonS02").interactable = isNeedMaterial;
        GetScript<RectTransform> ("txtp_PopText").gameObject.SetActive (isNeedMaterial);
        GetScript<RectTransform> ("txtp_PopAlert").gameObject.SetActive (!isNeedMaterial);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

    void DidTapOK()
    {
        if (IsClosed) {
            return;
        }

        if(m_TapOK != null) {
            m_TapOK(m_ReleasebleSlots);
        }
        PlayOpenCloseAnimation (false, () => {
            Dispose ();
        });
    }

    void DidTapCancel()
    {
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, () => Dispose ());
    }

    System.Action<MaterialGrowthBoardSlot[]> m_TapOK;
    MaterialGrowthBoardSlot[] m_ReleasebleSlots;
}
