using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_GemTrainingBundleOKPop : PopupViewBase {

    public static View_GemTrainingBundleOKPop Create(CardData card, MaterialGrowthBoardSlot[] slots, bool isAll, System.Action tapOK)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_GemTrainingBundleOKPop");
        var c = go.GetOrAddComponent<View_GemTrainingBundleOKPop>();
        c.InitInternal(card, slots, isAll, tapOK);
        return c;
    }

    private void InitInternal(CardData card, MaterialGrowthBoardSlot[] slots, bool isAll, System.Action tapOK)
    {
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

        int gem = slots.Sum (x => x.item_combination.gem_quantity);

        if (!isAll) {
            GetScript<TextMeshProUGUI> ("txtp_PopTitle").SetText ("開放");
            GetScript<TextMeshProUGUI> ("txtp_PopText").SetText ("ジェムでスロットを開放します");
        }

        GetScript<TextMeshProUGUI> ("txtp_UseGem").SetText (gem);
        GetScript<TextMeshProUGUI> ("txtp_TotalGem").SetText (AwsModule.UserData.UserData.GemCount - gem);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapCancel();
    }

    void DidTapOK()
    {
        if(IsClosed) {
            return;
        }

        if(m_TapOK != null) {
            m_TapOK();
        }
        PlayOpenCloseAnimation (false, () => {
            Dispose ();
        });
    }

    void DidTapCancel()
    {
        if(IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => Dispose ());
    }

    System.Action m_TapOK;
}
