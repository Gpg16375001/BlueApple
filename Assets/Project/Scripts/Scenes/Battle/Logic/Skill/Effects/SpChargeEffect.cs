using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleLogic
{
    public class SpChargeSkillEffectResult : SkillEffectResultBase
    {
        public int ChargeValue {
            get;
            private set;
        }

        public SpChargeSkillEffectResult (bool success, SkillEffectLogicEnum logic, int charge) : base(success, logic)
        {
            ChargeValue = charge;
        }
    }

    public static partial class SkillExecutor
    {
        private static SpChargeSkillEffectResult SpChargeExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;
            int chargeValue = 0;
            var skillEffect = effect.skill_effect;
            bool ignorePassiveEffect = false;

            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.FixedValue)) {
                chargeValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.FixedValue);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupFixedValue)) {
                chargeValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupFixedValue) * (level - 1);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.IgnorePassiveEffect)) {
                ignorePassiveEffect = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.IgnorePassiveEffect) != 0;
            }
            if (element != null) {
                chargeValue = chargeValue + element;
            }
            receiver.ChargeSp(chargeValue, ignorePassiveEffect);

            return new SpChargeSkillEffectResult(true, effect.skill_effect.effect, chargeValue);
        }
    }

    public static partial class SkillEvaluation
    {
        static float SpChargeEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            float evaluation = 1.0f;
            int level = skill.Level;
            int chargeValue = 0;
            var skillEffect = effect.skill_effect;
            bool ignorePassiveEffect = false;
            int totalChargeSp = 0;
            IEnumerable<Parameter> receivers = Calculation.GetReceivers(invoker, target, allyTarget, effect.range);

            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.FixedValue)) {
                chargeValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.FixedValue);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupFixedValue)) {
                chargeValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupFixedValue) * (level - 1);
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.IgnorePassiveEffect)) {
                ignorePassiveEffect = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.IgnorePassiveEffect) != 0;
            }

            float minSpProgress = float.MaxValue;
            var count = receivers.Count ();
            foreach (var receiver in receivers) {
                if (minSpProgress > receiver.SpProgress) {
                    minSpProgress = receiver.SpProgress;
                }
                totalChargeSp += receiver.CalcChargeSp (chargeValue, ignorePassiveEffect);
            }

            if (Mathf.Approximately (minSpProgress, 1.0f)) {
                return 0.0f;
            }

            evaluation *= Mathf.Pow (1.1f, (float)(count - 1));
            evaluation *= Mathf.Pow (1.1f, (float)(totalChargeSp / 1000)); 
            return evaluation;
        }
    }
}