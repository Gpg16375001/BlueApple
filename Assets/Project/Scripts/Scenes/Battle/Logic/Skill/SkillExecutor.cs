using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleLogic {
    // スキル効果の計算を行う。
    public static partial class SkillExecutor
    {
        /// <summary>
        /// スキルの発動
        /// </summary>
        /// <param name="invoker">Invoker.</param>
        /// <param name="target">Target.</param>
        /// <param name="skill">Skill.</param>
        public static SkillExecutionResult Execute(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill)
        {
            SkillExecutionResult ret = new SkillExecutionResult();
            int effectCount = skill.Skill.SkillEffects.Length;
            for (int i = 0; i < effectCount; i++) {
                var effect = skill.Skill.SkillEffects [i];
                // ロジックに対応する関数がなければ無視する
                if (skillExecuteFuncs.ContainsKey (effect.skill_effect.effect)) {
                    var func = skillExecuteFuncs [effect.skill_effect.effect];

                    // ターゲット情報を取得する。
                    IEnumerable<Parameter> receivers = Calculation.GetReceivers(invoker, target, allyTarget, effect.range);
                    var uniqReceivers = receivers.GroupBy(x => x);

                    foreach(var uniqReceiver in uniqReceivers) {
                        var loopCount = uniqReceiver.Count ();
                        var receiver = uniqReceiver.Key;
                        for (int exeCount = 0; exeCount < loopCount; ++exeCount) {
                            // 条件で判定が変わる場合の処理
                            invoker.CalcSituationParameterVariation(true, skill, receiver);
                            receiver.CalcSituationParameterVariation(false, skill, invoker);

                            ret.AddResult(receiver, func (invoker, receiver, skill, effect, null));
                        }
                    }
                }
            }
            return ret;
        }

        delegate SkillEffectResultBase SkillExexuteFunc(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element);

        // ロジックと効果の付き合わせをする
        private static Dictionary<SkillEffectLogicEnum, SkillExexuteFunc> skillExecuteFuncs =
            new Dictionary<SkillEffectLogicEnum, SkillExexuteFunc>() {
            {SkillEffectLogicEnum.damage, DamageExecuteCore},
            {SkillEffectLogicEnum.special_damage, SpecialDamageExecuteCore},
            {SkillEffectLogicEnum.guard, GuardExecuteCore},
            {SkillEffectLogicEnum.hp_heal, HpHealExecuteCore},
            {SkillEffectLogicEnum.condition_granting, ConditionGrantingExecuteCore},
            {SkillEffectLogicEnum.sp_charge, SpChargeExecuteCore},
            {SkillEffectLogicEnum.condition_remove, ConditionRemoveExecuteCore},
        };

        /// <summary>
        /// 状態からのスキルロジック利用
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <param name="target">Target.</param>
        /// <param name="skill">Skill.</param>
        public static SkillExecutionResult Execute(Condition condition, Parameter target, SkillParameter skill)
        {
            SkillExecutionResult ret = new SkillExecutionResult();
            int effectCount = skill.Skill.SkillEffects.Length;
            for (int i = 0; i < effectCount; i++) {
                var effect = skill.Skill.SkillEffects [i];
                // ロジックに対応する関数がなければ無視する
                var changeElemet = target.StateEffectChange(condition);
                if (skillExecuteFuncs.ContainsKey (effect.skill_effect.effect)) {
                    var func = skillExecuteFuncs [effect.skill_effect.effect];
                    //ターゲットは必ずtargetのみ
                    target.CalcSituationParameterVariation (false, skill, null);
                    ret.AddResult(target, func (null, target, skill, effect, changeElemet));
                }
            }
            return ret;
        }
    }
}
