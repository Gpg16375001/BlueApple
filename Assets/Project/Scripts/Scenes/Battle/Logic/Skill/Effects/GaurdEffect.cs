using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic {
    public class GaurdSkillEffectResult : SkillEffectResultBase
    {
        public GaurdSkillEffectResult (bool success, SkillEffectLogicEnum logic) : base(success, logic)
        {
        }
    }

    public static partial class SkillExecutor
    {
        private static GaurdSkillEffectResult GuardExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;

            int addValue = 0;
            int ratioValue = 0;
            var skillEffect = effect.skill_effect;
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                addValue = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.Value);
                if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                    addValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (level - 1);
                }
            }
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                ratioValue = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
                if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                    ratioValue += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (level - 1);
                }
            }    
            receiver.Guard (addValue, ratioValue);

            return new GaurdSkillEffectResult(true, effect.skill_effect.effect);
        }
    }

    public static partial class SkillEvaluation
    {
        static float GuardEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            return 1.0f;
        }
    }
}