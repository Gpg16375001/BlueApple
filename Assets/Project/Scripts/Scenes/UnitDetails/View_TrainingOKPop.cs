using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_TrainingOKPop : PopupViewBase {
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_TrainingOKPop Create(CardData card, MaterialGrowthBoardSlot slot, System.Action tapOK)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_TrainingOKPop");
        var c = go.GetOrAddComponent<View_TrainingOKPop>();
        c.InitInternal(card, slot, tapOK);
        return c;
    }

    private void InitInternal(CardData card, MaterialGrowthBoardSlot slot, System.Action tapOK)
    {
        m_TapOK = tapOK;
        GetScript<TextMeshProUGUI> ("txtp_UnitHP").SetText (card.Parameter.MaxHp);
        GetScript<TextMeshProUGUI> ("txtp_UnitATK").SetText (card.Parameter.Attack);
        GetScript<TextMeshProUGUI> ("txtp_UnitDEF").SetText (card.Parameter.Defense);
        GetScript<TextMeshProUGUI> ("txtp_UnitSPD").SetText (card.Parameter.Agility);

        MaterialGrowthBoardSlot[] slots = new MaterialGrowthBoardSlot[1] { slot };
        var hp = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.hp);
        var atk = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.attack);
        var def = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.defence);
        var agi = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.agility);
        var action_level = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.action_skill_level);
        var passive_level = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.passive_skill_level);
        var special_level = BattleLogic.Parameter.CalcGrowthBoardSlot (slots, MaterialGrowthBoardParameterTypeEnum.special_skill_level);

        if (hp > 0 || atk > 0 || def > 0 || agi > 0) { 
            GetScript<RectTransform> ("UnitStatus").gameObject.SetActive (true);
            GetScript<RectTransform> ("Skill").gameObject.SetActive (false);
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
        else if (action_level > 0 || passive_level > 0 || special_level > 0) {
            GetScript<RectTransform> ("UnitStatus").gameObject.SetActive (false);
            GetScript<RectTransform> ("Skill").gameObject.SetActive (true);

            var skill = GetScript<RectTransform> ("Skill").gameObject;
            if (action_level > 0) {
                var actionSkill = card.Parameter.UnitActionSkillList.FirstOrDefault (x => !x.IsNormalAction);
                if (actionSkill != null) {
                    skill.SetActive (true);
                    GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText (actionSkill.Skill.display_name);
                    GetScript<TextMeshProUGUI> ("txtp_SkillLv").SetText (actionSkill.Level);
                    GetScript<TextMeshProUGUI> ("txtp_AfterSkillLv").SetText (actionSkill.Level + 1);
                } else {
                    skill.SetActive (false);
                }
            } else if (passive_level > 0) {
                var passiveSkill = card.Parameter.PassiveSkillList.FirstOrDefault ();
                if (passiveSkill != null) {
                    skill.SetActive (true);
                    GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText (passiveSkill.Skill.display_name);
                    GetScript<TextMeshProUGUI> ("txtp_SkillLv").SetText (passiveSkill.Level);
                    GetScript<TextMeshProUGUI> ("txtp_AfterSkillLv").SetText (passiveSkill.Level + 1);
                } else {
                    skill.SetActive (false);
                }
            } else if (special_level > 0) {
                var specialSkill = card.Parameter.SpecialSkill;
                if (specialSkill != null) {
                    skill.SetActive (true);
                    GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText (specialSkill.Skill.display_name);
                    GetScript<TextMeshProUGUI> ("txtp_SkillLv").SetText (specialSkill.Level);
                    GetScript<TextMeshProUGUI> ("txtp_AfterSkillLv").SetText (specialSkill.Level + 1);
                } else {
                    skill.SetActive (false);
                }
            } else {
                skill.SetActive (false);
            }
        }

        SetCanvasCustomButtonMsg ("OK/bt_CommonS02", DidTapOK);
        SetCanvasCustomButtonMsg ("Cancel/bt_CommonS01", DidTapCancel);
        SetCanvasCustomButtonMsg ("bt_Close", DidTapCancel);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapCancel();
    }

    void DidTapOK()
    {
        if (IsClosed) {
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
        if (IsClosed) {
            return;
        }

        PlayOpenCloseAnimation (false, () => Dispose ());
    }
        

    System.Action m_TapOK;
}
