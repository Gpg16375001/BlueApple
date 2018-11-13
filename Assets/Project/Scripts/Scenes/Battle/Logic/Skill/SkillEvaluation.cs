using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleLogic {
    public static partial class SkillEvaluation 
    {
        static public float Evaluation(BattleAISetting setting, Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, int enemyTotalHp, int allyAverageDefense)
        {
            float ret = 1.0f;
            int effectCount = skill.Skill.SkillEffects.Length;

            for (int i = 0; i < effectCount; i++) {
                bool skip = true;
                var effect = skill.Skill.SkillEffects [i];

                if (setting.action_type == BattleAIActionTypeEnum.Attack) {
                    skip = !effect.skill_effect.IsDamageLogic ();
                } else if (setting.action_type == BattleAIActionTypeEnum.Heal) {
                    skip = !effect.skill_effect.IsHealLogic ();
                } else if (setting.action_type == BattleAIActionTypeEnum.Buff) {
                    skip = !effect.skill_effect.IsBuffLogic (setting);
                } else if (setting.action_type == BattleAIActionTypeEnum.Debuff) {
                    skip = !effect.skill_effect.IsDebuffLogic (setting);
                } else if (setting.action_type == BattleAIActionTypeEnum.CancellationBuff) {
                    skip = !effect.skill_effect.IsCancellatioBuffLogic (setting);
                } else if (setting.action_type == BattleAIActionTypeEnum.CancellationDebuff) {
                    skip = !effect.skill_effect.IsCancellatioDebuffLogic (setting);
                }

                // 該当しない効果は評価値に影響を与えない
                if (skip) {
                    continue;
                }

                // ロジックに対応する関数がなければ無視する
                if (skillEvaluationFuncs.ContainsKey (effect.skill_effect.effect)) {
                    ret *= skillEvaluationFuncs [effect.skill_effect.effect] (invoker, target, allyTarget, skill, effect, enemyTotalHp, allyAverageDefense);
                }
            }
            return ret;
        }

        static public float Evaluation(bool isEnemy, Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, int enemyTotalHp, int allyAverageDefense)
        {
            float ret = 1.0f;
            int effectCount = skill.Skill.SkillEffects.Length;

            for (int i = 0; i < effectCount; i++) {
                var effect = skill.Skill.SkillEffects [i];
                if (isEnemy && effect.range.IsAlly) {
                    continue;
                }
                if (!isEnemy && effect.range.IsEnemy) {
                    continue;
                }

                // ロジックに対応する関数がなければ無視する
                if (skillEvaluationFuncs.ContainsKey (effect.skill_effect.effect)) {
                    ret *= skillEvaluationFuncs [effect.skill_effect.effect] (invoker, target, allyTarget, skill, effect, enemyTotalHp, allyAverageDefense);
                }
            }
            return ret;
        }

        delegate float SkillEvaluationFunc(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense);

        // ロジックと効果の付き合わせをする
        private static Dictionary<SkillEffectLogicEnum, SkillEvaluationFunc> skillEvaluationFuncs =
            new Dictionary<SkillEffectLogicEnum, SkillEvaluationFunc>() {
            {SkillEffectLogicEnum.damage, DamageEvaluationCore},
            {SkillEffectLogicEnum.special_damage, SpecialDamageEvaluationCore},
            {SkillEffectLogicEnum.guard, GuardEvaluationCore},
            {SkillEffectLogicEnum.hp_heal, HpHealEvaluationCore},
            {SkillEffectLogicEnum.condition_granting, ConditionGrantingEvaluationCore},
            {SkillEffectLogicEnum.sp_charge, SpChargeEvaluationCore},
            {SkillEffectLogicEnum.condition_remove, ConditionRemoveEvaluationCore},
        };
    }
}

