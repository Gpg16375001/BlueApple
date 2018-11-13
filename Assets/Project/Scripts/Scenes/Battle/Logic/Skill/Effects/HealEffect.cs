using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleLogic
{
    public class HealSkillEffectResult : SkillEffectResultBase
    {
        public int HealValue {
            get;
            private set;
        }

        public HealSkillEffectResult (bool success, SkillEffectLogicEnum logic, int healValue) : base(success, logic)
        {
            HealValue = healValue;
        }
    }

    public static partial class SkillExecutor
    {
        private static HealSkillEffectResult HpHealExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;
            int healValue = 0;
            int healHPRatio = 0;
            var skillEffect = effect.skill_effect;
            bool ignorePassiveEffect = false;
            // 固定値を持っている場合は固定値が優先
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.FixedValue)) {
                healValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.FixedValue);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupFixedValue)) {
                healValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupFixedValue) * (level - 1);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.HPRatio)) {
                healHPRatio += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.HPRatio);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupHPRatio)) {
                healHPRatio += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupHPRatio) * (level - 1);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.IgnorePassiveEffect)) {
                ignorePassiveEffect = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.IgnorePassiveEffect) != 0;
            }

            healValue = healValue + (receiver.MaxHp * healHPRatio / 100);
            if (element != null) {
                healValue = healValue + element;
            }
            healValue = Mathf.Max(0, invoker.HealValue(healValue, ignorePassiveEffect));

            receiver.Hp += healValue;

            return new HealSkillEffectResult(true, effect.skill_effect.effect, healValue);
        }
    }

    public static partial class SkillEvaluation
    {
        static float HpHealEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            float evaluation = 1.0f;
            int level = skill.Level;
            IEnumerable<Parameter> receivers = Calculation.GetReceivers(invoker, target, allyTarget, effect.range);

            int healValue = 0;
            var skillEffect = effect.skill_effect;
            int healHPRatio = 0;
            bool ignorePassiveEffect = false;
            // 固定値を持っている場合は固定値が優先
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.FixedValue)) {
                healValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.FixedValue);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupFixedValue)) {
                healValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupFixedValue) * (level - 1);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.HPRatio)) {
                healHPRatio += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.HPRatio);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupHPRatio)) {
                healHPRatio += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupHPRatio) * (level - 1);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.IgnorePassiveEffect)) {
                ignorePassiveEffect = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.IgnorePassiveEffect) != 0;
            }

            int totalHeal = 0;
            float minHpProgress = float.MaxValue;
            var count = receivers.Count ();
            for (int i = 0; i < count; ++i) {
                var receiver = receivers.ElementAt (i);

                if (minHpProgress > receiver.HpProgress) {
                    minHpProgress = receiver.HpProgress;
                }
                var heal = healValue + (receiver.MaxHp * healHPRatio / 100);
                heal = Mathf.Max(0, invoker.HealValue(heal, ignorePassiveEffect));
                totalHeal += Mathf.Min (receiver.MaxHp - receiver.Hp, heal);
            }

            // そもそも誰もHP減ってない。。。
            if (Mathf.Approximately(minHpProgress, 1.0f)) {
                return 0.0f;
            }

            if (minHpProgress >= 0.6f) {
                //evaluation *= 1.0f;
            } else if (minHpProgress >= 0.4f) {
                evaluation *= 1.2f;
            } else if (minHpProgress >= 0.3f) {
                evaluation *= 2.0f;
            } else {
                evaluation *= 3.0f;
            }
            evaluation *= Mathf.Pow (1.1f, (float)(count - 1));
            evaluation *= Mathf.Pow (1.1f, (float)(totalHeal / 1000));

            return evaluation;
        }
    }
}