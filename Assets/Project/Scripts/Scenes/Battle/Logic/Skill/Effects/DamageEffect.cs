using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace BattleLogic {
    // ダメージスキルの計算結果
    public class DamageSkillEffectResult : SkillEffectResultBase
    {
        public DamageInfo[] DamageInfos {
            get;
            private set;
        }

        public Condition GrantingCondition {
            get;
            private set;
        }

        public DamageSkillEffectResult (bool success, SkillEffectLogicEnum logic, IEnumerable<DamageInfo> damageInfos, Condition granting) : base(success, logic)
        {
            DamageInfos = damageInfos.ToArray ();
            GrantingCondition = granting;
        }
    }


    public static partial class SkillExecutor
    {
        private static DamageSkillEffectResult DamageExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;

            var skillEffect = effect.skill_effect;
            bool isGrandingCondition = skillEffect.ContainsArg (SkillEffectLogicArgEnum.Condition);

            bool isSuccess = false;
            var damageList = Calculation.CalcDamage (invoker, receiver, level, skill.Skill.attack_element, skillEffect, skill.IsSpecial, element);
            int damageCount = damageList.Count;

            Condition grantingCondition = null;
            if (isGrandingCondition) {
                grantingCondition = skillEffect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition); 
            }
            // 計算結果を反映する。
            for (int j = 0; j < damageCount; ++j) {
                var damageInfo = damageList [j];
                if (damageInfo.isHit) {
                    if (damageInfo.heal > 0) {
                        invoker.Hp += damageInfo.heal;
                    }
                    if (damageInfo.counterDamage > 0) {
                        invoker.Hp -= damageInfo.counterDamage;
                    }
                    receiver.Hp -= damageInfo.damage;
                    if (damageInfo.reverseDamage > 0) {
                        receiver.Hp += damageInfo.reverseDamage;
                    }
                    if (receiver.Conditions.HasCondition) {
                        receiver.ConditionCallingOffDamage ();
                    }
                    if (damageInfo.addWeight > 0) {
                        receiver.AddWeight (damageInfo.addWeight);
                    }
                    // ダメージ食らうごとに状態付与判定
                    if (isGrandingCondition && Calculation.ReceiveConditionJudgment (invoker, receiver, skillEffect, level, grantingCondition)) {
                        receiver.AddCondition (grantingCondition);
                    }
                    isSuccess = true;
                }
            }
            return new DamageSkillEffectResult(isSuccess, effect.skill_effect.effect, damageList, grantingCondition);
        }

        private static DamageSkillEffectResult SpecialDamageExecuteCore(Parameter invoker, Parameter receiver, SkillParameter skill, SkillEffectSetting effect, ChangeParameterElement element)
        {
            int level = skill.Level;

            var skillEffect = effect.skill_effect;

            bool isSuccess = false;
            var damageList = Calculation.CalcSpecialDamege (invoker, receiver, level, skill.Skill.attack_element, skillEffect, element);
            int damageCount = damageList.Count;
            for (int j = 0; j < damageCount; ++j) {
                if (damageList [j].isHit) {
                    receiver.Hp -= damageList [j].damage;
                    if (receiver.Conditions.HasCondition) {
                        receiver.ConditionCallingOffDamage ();
                    }
                    isSuccess = true;
                }
            }

            return new DamageSkillEffectResult(isSuccess, effect.skill_effect.effect, damageList, null);
        }
    }


    public static partial class SkillEvaluation
    {
        static float DamageEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            int level = skill.Level;
            IEnumerable<Parameter> receivers = Calculation.GetReceivers(invoker, target, allyTarget, effect.range);
            var skillEffect = effect.skill_effect;

            float evaluation = 1.0f;

            int targetCount = 0;
            int totalDamage = 0;
            int hitTotal = 0;
            int minNumberOfFront = int.MaxValue;
            int minOrderIndex = int.MaxValue;
            int maxAttack = int.MinValue;

            int deathCount = 0;

            foreach (var defender in receivers) {
                invoker.CalcSituationParameterVariation(true, skill, defender);
                defender.CalcSituationParameterVariation (false, skill, invoker);

                var damage = Calculation.CalcEvaluationDamege (invoker, defender, level, skill.Skill.attack_element, skillEffect, skill.IsSpecial);
                ++targetCount;
                totalDamage += damage.Item1;
                hitTotal += damage.Item2;
                // 倒せる可能性がある
                if (defender.Hp <= damage.Item1) {
                    deathCount++;
                }

                int n = Calculation.GetNumberOfFront (defender);
                if (minNumberOfFront > n) {
                    minNumberOfFront = n;
                }

                int order = BattleProgressManager.Shared.OrderQueue.GetOrder (AwsModule.BattleData.GetBattleUnit (defender.Position));
                if (minOrderIndex > order) {
                    minOrderIndex = order;
                }

                if (maxAttack < defender.Attack) {
                    maxAttack = defender.Attack;
                }
            }

            evaluation *= Mathf.Pow(1.5f, (float)deathCount) * (Mathf.Pow(1.1f, (float)totalDamage / (float)enemyTotalHp)) * (Mathf.Pow(1.1f, (float)(totalDamage / 1000)));

            // hit対応
            int aveHit = hitTotal / targetCount;
            if (aveHit >= 90) {
                //evaluation *= 1.0f;
            } else if (aveHit >= 70) {
                evaluation *= (1.0f - 0.01f * (90 - aveHit));
            } else if (aveHit >= 50) {
                evaluation *= 0.5f;
            } else if (aveHit >= 30) {
                evaluation *= 0.3f;
            } else {
                evaluation *= 0.1f;
            }

            if (maxAttack < allyAverageDefense) {
                evaluation *= 0.8f;
            }

            bool disableFormationRate = false;
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.DisableFormationRate)) {
                disableFormationRate = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.DisableFormationRate) != 0;
            }
            if (!disableFormationRate) {
                if (minNumberOfFront == 0) {
                    //evaluation *= 1.0f;
                } else if (minNumberOfFront == 1) {
                    evaluation *= 0.6f;
                } else {
                    evaluation *= 0.1f;
                }
            }

            evaluation *= (1.0f - (0.02f * minOrderIndex));

            // Hateによる評価値の変化
            evaluation = evaluation + (evaluation * (float)target.GetHate ()) / 100.0f;

            return evaluation;
        }

        static float SpecialDamageEvaluationCore(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter skill, SkillEffectSetting effect, int enemyTotalHp, int allyAverageDefense)
        {
            int level = skill.Level;
            IEnumerable<Parameter> receivers = Calculation.GetReceivers(invoker, target, allyTarget, effect.range);
            var skillEffect = effect.skill_effect;

            float evaluation = 1.0f;

            int targetCount = 0;
            int totalDamage = 0;
            int hitTotal = 0;
            int minNumberOfFront = int.MaxValue;
            int minOrderIndex = int.MaxValue;
            int maxAttack = int.MinValue;

            int deathCount = 0;

            foreach (var defender in receivers) {
                invoker.CalcSituationParameterVariation(true, skill, defender);
                defender.CalcSituationParameterVariation (false, skill, invoker);

                var damage = Calculation.CalcEvaluationSpecialDamege (invoker, defender, level, skill.Skill.attack_element, skillEffect);
                ++targetCount;
                totalDamage += damage.Item1;
                hitTotal += damage.Item2;
                // 倒せる可能性がある
                if (defender.Hp <= damage.Item1) {
                    deathCount++;
                }

                int n = Calculation.GetNumberOfFront (defender);
                if (minNumberOfFront > n) {
                    minNumberOfFront = n;
                }

                int order = BattleProgressManager.Shared.OrderQueue.GetOrder (AwsModule.BattleData.GetBattleUnit (defender.Position));
                if (minOrderIndex > order) {
                    minOrderIndex = order;
                }

                if (maxAttack < defender.Attack) {
                    maxAttack = defender.Attack;
                }
            }

            evaluation *= Mathf.Pow(1.5f, (float)deathCount) * ((float)totalDamage / (float)enemyTotalHp) * (Mathf.Pow(1.1f, (float)(totalDamage / 1000)));

            // hit対応
            int aveHit = hitTotal / targetCount;
            if (aveHit >= 90) {
                //evaluation *= 1.0f;
            } else if (aveHit >= 70) {
                evaluation *= (1.0f - 0.01f * (90 - aveHit));
            } else if (aveHit >= 50) {
                evaluation *= 0.5f;
            } else if (aveHit >= 30) {
                evaluation *= 0.3f;
            } else {
                evaluation *= 0.1f;
            }

            if (maxAttack < allyAverageDefense) {
                evaluation *= 0.8f;
            }

            bool disableFormationRate = false;
            if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.DisableFormationRate)) {
                disableFormationRate = skillEffect.GetValue<int> (SkillEffectLogicArgEnum.DisableFormationRate) != 0;
            }
            if (!disableFormationRate) {
                if (minNumberOfFront == 0) {
                    //evaluation *= 1.0f;
                } else if (minNumberOfFront == 1) {
                    evaluation *= 0.6f;
                } else {
                    evaluation *= 0.1f;
                }
            }

            evaluation *= (1.0f - (0.02f * minOrderIndex));

            return evaluation;
        }
    }
}