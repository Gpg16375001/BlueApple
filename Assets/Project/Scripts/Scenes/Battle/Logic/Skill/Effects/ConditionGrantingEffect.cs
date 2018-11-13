using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleLogic
{
    public class ConditionGrantingSkillEffectResult : SkillEffectResultBase
    {
        public Condition GrantingCondition {
            get;
            private set;
        }

        public ConditionGrantingSkillEffectResult (bool success, SkillEffectLogicEnum logic, Condition granting) : base(success, logic)
        {
            GrantingCondition = granting;
        }
    }

    public static partial class SkillExecutor
    {
        private static ConditionGrantingSkillEffectResult ConditionGrantingExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;
            bool isSuccess = false;
            var skillEffect = effect.skill_effect;

            Condition grantingCondition = skillEffect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition); 
            if (Calculation.ReceiveConditionJudgment (invoker, receiver, skillEffect, level, grantingCondition)) {
                receiver.AddCondition (grantingCondition);
                isSuccess = true;
            }

            return new ConditionGrantingSkillEffectResult(isSuccess, skillEffect.effect, grantingCondition);
        }
    }

    public static partial class SkillEvaluation
    {
        static float ConditionGrantingEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            return 1.0f;
        }
    }
}