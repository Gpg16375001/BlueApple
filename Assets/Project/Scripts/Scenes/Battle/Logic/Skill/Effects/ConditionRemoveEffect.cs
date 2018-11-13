using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleLogic
{
    public class ConditionRemoveSkillEffectResult : SkillEffectResultBase
    {
        public Condition[] RemoveCondition {
            get;
            private set;
        }

        public ConditionRemoveSkillEffectResult (bool success, SkillEffectLogicEnum logic, Condition[] removeCondition) : base(success, logic)
        {
            RemoveCondition = removeCondition;
        }
    }

    public static partial class SkillExecutor
    {
        private static ConditionRemoveSkillEffectResult ConditionRemoveExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;
            var skillEffect = effect.skill_effect;

            int? conditionId = null;
            int? conditionGroup = null;
            ConditionTypeEnum? conditionType = null;
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                var conditoin = skillEffect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                if (conditoin != null) {
                    conditionId = conditoin.id;
                }
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionGroup)) {
                var conditoinGroup = skillEffect.GetValue<ConditionGroup> (SkillEffectLogicArgEnum.ConditionGroup);
                if (conditoinGroup != null) {
                    conditionGroup = conditoinGroup.id;
                }
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionType)) {
                var conditoinType = skillEffect.GetValue<ConditionType> (SkillEffectLogicArgEnum.ConditionType);
                if (conditoinType != null) {
                    conditionType = conditoinType.Enum;
                }
            }

            var removeCondition = receiver.RemoveCondition (conditionId, conditionGroup, conditionType);

            return new ConditionRemoveSkillEffectResult(true, effect.skill_effect.effect, removeCondition);
        }
    }

    public static partial class SkillEvaluation
    {
        static float ConditionRemoveEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            int level = skill.Level;
            var skillEffect = effect.skill_effect;

            int? conditionId = null;
            int? conditionGroup = null;
            ConditionTypeEnum? conditionType = null;
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                var conditoin = skillEffect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                if (conditoin != null) {
                    conditionId = conditoin.id;
                }
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionGroup)) {
                var conditoinGroup = skillEffect.GetValue<ConditionGroup> (SkillEffectLogicArgEnum.ConditionGroup);
                if (conditoinGroup != null) {
                    conditionGroup = conditoinGroup.id;
                }
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionType)) {
                var conditoinType = skillEffect.GetValue<ConditionType> (SkillEffectLogicArgEnum.ConditionType);
                if (conditoinType != null) {
                    conditionType = conditoinType.Enum;
                }
            }

            return target.HasCondition(conditionId, conditionGroup, conditionType) ? 1.0f: 0.0f;
        }
    }
}
