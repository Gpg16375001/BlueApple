#define AI_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleLogic {
    public class BattleAI {
        Parameter m_myParam;
        BattleAIDefine aiDefine;
        BattleAIDefine overrideAiDefine;
        BattleAISetting[] aiSetteings;
        int settingCount = 0;

        bool targetRevesal;
        public bool TargetReversal {
            get {
                if (overrideAiDefine == null) {
                    return aiDefine.target_reversal;
                }
                return overrideAiDefine.target_reversal;
            }
        }

        public BattleAI(Parameter myParam)
        {
            m_myParam = myParam;
            if (m_myParam.Position.isPlayer) {
                aiDefine = MasterDataTable.battle_ai_define[1];
            } else {
                aiDefine = m_myParam.AIDefine;
                if(aiDefine == null) {
                    aiDefine = MasterDataTable.battle_ai_define[5];  
                }
            }
            aiSetteings = MasterDataTable.battle_ai_setting [aiDefine.id];
            settingCount = aiSetteings.Length;
        }

        public void DecideAction(ListItem_BattleUnit unit, System.Action<SkillParameter> ActionStart, BattleAIDefine overrideAI = null)
        {
            bool done = false;

            int enemyTotalHp = 0;
            int allyAveDefense = 0;

            overrideAiDefine = overrideAI;
            bool targetRevesal = aiDefine.target_reversal;
            if (overrideAI != null) {
                aiSetteings = MasterDataTable.battle_ai_setting [overrideAI.id];
                settingCount = aiSetteings.Length;
                targetRevesal = overrideAI.target_reversal;
            }

            if (m_myParam.Position.isPlayer) {
                if (targetRevesal) {
                    enemyTotalHp = AwsModule.BattleData.AllyParameterList.Where (x => x.Hp > 0).Select (x => x.Hp).Sum ();
                    allyAveDefense = AwsModule.BattleData.EnemyParameterList.Where (x => x.Hp > 0).Select (x => x.Defense).Sum () / AwsModule.BattleData.EnemyParameterList.Where (x => x.Hp > 0).Count ();
                } else {
                    enemyTotalHp = AwsModule.BattleData.EnemyParameterList.Where (x => x.Hp > 0).Select (x => x.Hp).Sum ();
                    allyAveDefense = AwsModule.BattleData.AllyParameterList.Where (x => x.Hp > 0).Select (x => x.Defense).Sum () / AwsModule.BattleData.AllyParameterList.Where (x => x.Hp > 0).Count ();
                }
            } else {
                if (targetRevesal) {
                    enemyTotalHp = AwsModule.BattleData.EnemyParameterList.Where (x => x.Hp > 0).Select (x => x.Hp).Sum ();
                    allyAveDefense = AwsModule.BattleData.AllyParameterList.Where (x => x.Hp > 0).Select (x => x.Defense).Sum () / AwsModule.BattleData.AllyParameterList.Where (x => x.Hp > 0).Count ();
                } else {
                    enemyTotalHp = AwsModule.BattleData.AllyParameterList.Where (x => x.Hp > 0).Select (x => x.Hp).Sum ();
                    allyAveDefense = AwsModule.BattleData.EnemyParameterList.Where (x => x.Hp > 0).Select (x => x.Defense).Sum () / AwsModule.BattleData.EnemyParameterList.Where (x => x.Hp > 0).Count ();
                }
            }

#if UNITY_EDITOR && AI_LOG
            Debug.Log(string.Format ("{0} AI =================", m_myParam.Name));
            foreach (var s in m_myParam.ActionSkillList) {
                Debug.Log (string.Format ("{0}", s.Skill.display_name));
            }
#endif
            for (int i = 0; i < settingCount; ++i) {
                var aiSetting = aiSetteings [i];

                // そのアクションを行うかの判定
                if (!IsSatiryAction(aiSetting)) {
                    continue;
                }

                // 該当するスキル系統を所持しているか判断する。
                var satisfySkills = GetSatisfySkills(aiSetting);
                var satisfySkillCount = satisfySkills.Count ();
                if(satisfySkillCount > 0) {
                    // ターゲットに該当するものがあるか。
                    var satisfyTarget = GetSatisfyTarget(aiSetting, targetRevesal);
                    var satisfyTargetCount = satisfyTarget.Count ();

                    if(satisfyTargetCount > 0) {
                        if (Decide (unit, aiSetting, ActionStart, satisfySkills.ToArray(), satisfyTarget.ToArray(), enemyTotalHp, allyAveDefense, targetRevesal)) {
                            done = true;
                            break;
                        }
                    }
                }
            }

            if (!done) {
                // 何も行動していない場合は通常攻撃をとりあえず行う
                DecideNormalAttack(ActionStart);
            }

            if (overrideAI != null) {
                aiSetteings = MasterDataTable.battle_ai_setting [aiDefine.id];
                settingCount = aiSetteings.Length;
            }

#if UNITY_EDITOR && AI_LOG
            Debug.Log(string.Format ("================= {0} AI", m_myParam.Name));
#endif
        }

        private void DecideNormalAttack(System.Action<SkillParameter> ActionStart)
        {
#if UNITY_EDITOR && AI_LOG
            Debug.Log("DecideNormalAttack");
#endif
            var normalAction = m_myParam.ActionSkillList.FirstOrDefault (x => x.IsNormalAction);
            ActionStart (normalAction);
        }

        private bool Decide(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] actions, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            switch (setting.action_type) {
            case BattleAIActionTypeEnum.Attack:
                return DecideAttack(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            case BattleAIActionTypeEnum.Heal:
                return DecideHeal(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            case BattleAIActionTypeEnum.SPCharge:
                return DecideSPCharge(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            case BattleAIActionTypeEnum.Buff:
                return DecideBuff(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            case BattleAIActionTypeEnum.Debuff:
                return DecideDebuff(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            case BattleAIActionTypeEnum.CancellationBuff:
                return DecideCancellationBuff(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            case BattleAIActionTypeEnum.CancellationDebuff:
                return DecideCancellationDebuff(unit, setting, ActionStart, actions, targets, enemyTotalHp, allyAveDefense, targetRevesal);
            }
            return false;
        }

        private bool DecideAttack(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            Dictionary<UniRx.Tuple<Parameter, SkillParameter>, float> evaluation = new Dictionary<UniRx.Tuple<Parameter, SkillParameter>, float> ();

            // ヘイトが1000以上のユニットがいた場合は問答無用でそいつを単体攻撃する。
            var hateUnit = targets.OrderByDescending (x => x.GetHate ()).First ();
            if (hateUnit.GetHate () >= 1000) {
#if UNITY_EDITOR && AI_LOG
                Debug.LogFormat("================= DecideAttack Special Hate Logic In {0}", hateUnit.Name);
#endif
                foreach (var action in skills.Where(x => x.Skill.SkillEffects.Any(effect => effect.skill_effect.IsDamageLogic() && effect.range.IsUnitEnemy))) {
                    evaluation.Add (new UniRx.Tuple<Parameter, SkillParameter> (hateUnit, action), SkillEvaluation.Evaluation (setting, m_myParam, hateUnit, null, action, enemyTotalHp, allyAveDefense));
                }
            } else {
                foreach (var action in skills) {
                    foreach (var target in action.GetTargetCandidate(setting, m_myParam, targets, targetRevesal)) {
                        evaluation.Add (new UniRx.Tuple<Parameter, SkillParameter> (target, action), SkillEvaluation.Evaluation (setting, m_myParam, target, null, action, enemyTotalHp, allyAveDefense));
                    }
                }
            }

#if UNITY_EDITOR && AI_LOG
            Debug.Log("DecideAttack =================");
            foreach (var t in evaluation) {
                Debug.Log (string.Format ("{0} : {1} {2} {3}", t.Key.Item2.Skill.display_name, t.Key.Item1.Position, t.Key.Item1.Name, t.Value));
            }
            Debug.Log("================= DecideAttack");
#endif
            var selectAction = evaluation.OrderByDescending (x => x.Value).FirstOrDefault ();
            if (selectAction.Key.Item1 != null && selectAction.Key.Item2 != null) {
                SetTarget (unit, AwsModule.BattleData.GetBattleUnit (selectAction.Key.Item1.Position), null, selectAction.Key.Item2, enemyTotalHp, allyAveDefense, targetRevesal);
                ActionStart (selectAction.Key.Item2);
                return true;
            }
            return false;
        }

        private bool DecideHeal(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            Dictionary<UniRx.Tuple<Parameter, SkillParameter>, float> evaluation = new Dictionary<UniRx.Tuple<Parameter, SkillParameter>, float> ();
            foreach (var action in skills) {
                foreach (var target in action.GetTargetCandidate(setting, m_myParam, targets, targetRevesal)) {
                    evaluation.Add(new UniRx.Tuple<Parameter, SkillParameter>(target, action), SkillEvaluation.Evaluation (setting, m_myParam, null, target, action, 0, 0));
                }
            }

#if UNITY_EDITOR && AI_LOG
            Debug.Log("DecideHeal =================");
            foreach (var t in evaluation) {
                Debug.Log (string.Format ("{0} : {1} {2} {3}", t.Key.Item2.Skill.display_name, t.Key.Item1.Position, t.Key.Item1.Name, t.Value));
            }
            Debug.Log("================= DecideHeal");
#endif
            var selectAction = evaluation.OrderByDescending (x => x.Value).FirstOrDefault();
            if (selectAction.Key.Item1 != null && selectAction.Key.Item2 != null) {
                SetTarget (unit, null, AwsModule.BattleData.GetBattleUnit (selectAction.Key.Item1.Position), selectAction.Key.Item2, enemyTotalHp, allyAveDefense, targetRevesal);
                ActionStart (selectAction.Key.Item2);
                return true;
            }
            return false;
        }

        private bool DecideSPCharge(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            Dictionary<UniRx.Tuple<Parameter, SkillParameter>, float> evaluation = new Dictionary<UniRx.Tuple<Parameter, SkillParameter>, float> ();
            foreach (var action in skills) {
                foreach (var target in action.GetTargetCandidate(setting, m_myParam, targets, targetRevesal)) {
                    evaluation.Add(new UniRx.Tuple<Parameter, SkillParameter>(target, action), SkillEvaluation.Evaluation (setting, m_myParam, null, target, action, 0, 0));
                }
            }

#if UNITY_EDITOR && AI_LOG
            Debug.Log("DecideSPCharge =================");
            foreach (var t in evaluation) {
                Debug.Log (string.Format ("{0} : {1} {2} {3}", t.Key.Item2.Skill.display_name, t.Key.Item1.Position, t.Key.Item1.Name, t.Value));
            }
            Debug.Log("================= DecideHeal");
#endif
            var selectAction = evaluation.OrderByDescending (x => x.Value).FirstOrDefault();
            if (selectAction.Key.Item1 != null && selectAction.Key.Item2 != null) {
                SetTarget (unit, null, AwsModule.BattleData.GetBattleUnit (selectAction.Key.Item1.Position), selectAction.Key.Item2, enemyTotalHp, allyAveDefense, targetRevesal);
                ActionStart (selectAction.Key.Item2);
                return true;
            }
            return false;
        }

        private bool DecideBuff(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            foreach (var action in skills) {
                var grandingCondition = action.GetGradingCondition ().Where(x => !x.is_debuff);
                foreach (var target in action.GetTargetCandidate(setting, m_myParam, targets, targetRevesal)) {
                    if (grandingCondition.All(x => !target.Conditions.ContainsCondition (x))) {
                        SetTarget (unit, null, AwsModule.BattleData.GetBattleUnit (target.Position), action, enemyTotalHp, allyAveDefense, targetRevesal);
                        ActionStart (action);

#if UNITY_EDITOR && AI_LOG
                        Debug.Log("DecideBuff =================");
                        Debug.Log (string.Format ("{0} : {1} {2}", action.Skill.display_name, target.Position, target.Name));
                        Debug.Log("================= DecideBuff");
#endif
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DecideDebuff(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            foreach (var action in skills) {
                var grandingCondition = action.GetGradingCondition ().Where(x => x.is_debuff);
                foreach (var target in action.GetTargetCandidate(setting, m_myParam, targets, targetRevesal)) {
                    if (grandingCondition.All(x => !target.Conditions.ContainsCondition (x))) {
                        SetTarget (unit, AwsModule.BattleData.GetBattleUnit (target.Position), null, action, enemyTotalHp, allyAveDefense, targetRevesal);
                        ActionStart (action);
#if UNITY_EDITOR && AI_LOG
                        Debug.Log("DecideDebuff =================");
                        Debug.Log (string.Format ("{0} : {1} {2}", action.Skill.display_name, target.Position, target.Name));
                        Debug.Log("================= DecideDebuff");
#endif
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DecideCancellationBuff(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            return false;
        }

        private bool DecideCancellationDebuff(ListItem_BattleUnit unit, BattleAISetting setting, System.Action<SkillParameter> ActionStart, SkillParameter[] skills, Parameter[] targets, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            return false;
        }

        private void SetTarget(ListItem_BattleUnit unit, ListItem_BattleUnit enemy, ListItem_BattleUnit ally, SkillParameter skill, int enemyTotalHp, int allyAveDefense, bool targetRevesal)
        {
            if (enemy != null) {
                unit.SetTarget (enemy);
            } else {
                if (skill.Skill.SkillEffects.Any (x => x.range.IsEnemy)) {
                    // 敵をターゲットにとるスキル効果がある場合
                    List<UniRx.Tuple<float, PositionData>> result = new List<UniRx.Tuple<float, PositionData>>();
                    foreach (var target in GetTargetList(true, targetRevesal)) {
                        result.Add(new UniRx.Tuple<float, PositionData>(  SkillEvaluation.Evaluation (true, m_myParam, target, null, skill, enemyTotalHp, allyAveDefense), target.Position));
                    }

                    var selectTraget = result.OrderByDescending (x => x.Item1).FirstOrDefault();
                    unit.SetTarget (AwsModule.BattleData.GetBattleUnit (selectTraget.Item2));
                }
            }

            if (ally != null) {
                unit.SetAllyTarget (ally);
            } else {
                if (skill.Skill.SkillEffects.Any (x => x.range.IsAlly)) {
                    // 味方をターゲットにとるスキル効果がある場合
                    List<UniRx.Tuple<float, PositionData>> result = new List<UniRx.Tuple<float, PositionData>>();
                    foreach (var target in GetTargetList(false, targetRevesal)) {
                        result.Add(new UniRx.Tuple<float, PositionData>(  SkillEvaluation.Evaluation (false, m_myParam, null, target, skill, enemyTotalHp, allyAveDefense), target.Position));
                    }

                    var selectTraget = result.OrderByDescending (x => x.Item1).FirstOrDefault();
                    unit.SetAllyTarget (AwsModule.BattleData.GetBattleUnit (selectTraget.Item2));
                }
            }

        }
            
        /// <summary>
        /// 条件に合うスキルのリストを取得
        /// </summary>
        /// <returns>The satisfy skills.</returns>
        /// <param name="setting">Setting.</param>
        private IEnumerable<SkillParameter> GetSatisfySkills(BattleAISetting setting)
        {
            foreach (var skill in m_myParam.ActionSkillList.Where(x => x.Enabled && x.IsSatisfy(setting))) {
                yield return skill;
            }

            if (m_myParam.SpecialSkill != null && m_myParam.IsSpMax) {
                if (m_myParam.SpecialSkill.IsSatisfy (setting)) {
                    yield return m_myParam.SpecialSkill;
                }
            }
        }

        /// <summary>
        /// 条件に合うターゲットの一覧を取得
        /// </summary>
        /// <returns>The satisfy target.</returns>
        /// <param name="setting">Setting.</param>
        private IEnumerable<Parameter> GetSatisfyTarget(BattleAISetting setting, bool targetRevesal)
        {
            List<Parameter> targets = new List<Parameter> ();
            if (setting.target == SkillTargetEnum.self) {
                targets.Add(m_myParam);
            } else if(setting.target == SkillTargetEnum.ally) {
                if ((m_myParam.Position.isPlayer && !targetRevesal) || (!m_myParam.Position.isPlayer && targetRevesal)) {
                    targets.AddRange(AwsModule.BattleData.SallyAllyParameterList);
                } else {
                    targets.AddRange(AwsModule.BattleData.SallyEnemyParameterList);
                }
            } else if(setting.target == SkillTargetEnum.enemy) {
                if ((m_myParam.Position.isPlayer && !targetRevesal) || (!m_myParam.Position.isPlayer && targetRevesal)) {
                    targets.AddRange(AwsModule.BattleData.SallyEnemyParameterList);
                } else {
                    targets.AddRange(AwsModule.BattleData.SallyAllyParameterList);
                }
            }

            if (targets.Count > 0) {
                foreach (var target in targets) {
                    if(IsSatisfyTarget(setting, target)) {
                        yield return target;
                    }
                }
            }
        }

        private bool IsSatisfyTarget(BattleAISetting setting, Parameter param)
        {
            if (setting.condition.HasValue) {
                switch (setting.condition.Value) {
                case BattleAITargetConditionEnum.HpMinValue:
                    return param.HpProgress * 100 < setting.GetConditionArg<int> ();
                }
                Debug.LogError (string.Format("Unsupported Target Condition {0}", setting.condition.Value.ToString()));
                return false;
            }

            // 条件がない場合はtrue
            return true;
        }

        private List<Parameter> GetTargetList(bool isEnemy, bool targetRevesal)
        {
            List<Parameter> targets = new List<Parameter> ();
            if(!isEnemy) {
                if ((m_myParam.Position.isPlayer && !targetRevesal) || (!m_myParam.Position.isPlayer && targetRevesal)) {
                    targets.AddRange(AwsModule.BattleData.SallyAllyParameterList);
                } else {
                    targets.AddRange(AwsModule.BattleData.SallyEnemyParameterList);
                }
            } else {
                if ((m_myParam.Position.isPlayer && !targetRevesal) || (!m_myParam.Position.isPlayer && targetRevesal)) {
                    targets.AddRange(AwsModule.BattleData.SallyEnemyParameterList);
                } else {
                    targets.AddRange(AwsModule.BattleData.SallyAllyParameterList);
                }
            }

            return targets;
        }


        private bool IsSatiryAction(BattleAISetting aiSetting)
        {
            if (aiSetting.action_condition.HasValue) {
                switch (aiSetting.action_condition.Value) {
                case BattleAIConditionEnum.RandomAction:
                    if (!string.IsNullOrEmpty(aiSetting.action_condition_arg)) {
                        int val = 0;
                        int.TryParse (aiSetting.action_condition_arg, out val);
                        var rand = Calculation.RandomRange (0, 100);
                        return rand < val;
                    }
                    return false;
                case BattleAIConditionEnum.PreTurnCount:
                    if (!string.IsNullOrEmpty (aiSetting.action_condition_arg)) {
                        int val = 0;
                        int.TryParse (aiSetting.action_condition_arg, out val);

                        return (m_myParam.WaveTurnCount + 1) % val == 0;
                    }
                    return false;
                case BattleAIConditionEnum.TurnCount:
                    if (!string.IsNullOrEmpty (aiSetting.action_condition_arg)) {
                        int val = 0;
                        int.TryParse (aiSetting.action_condition_arg, out val);

                        return (m_myParam.WaveTurnCount + 1) == val;
                    }
                    return false;
                case BattleAIConditionEnum.SPMax:
                    return m_myParam.IsSpMax;
                }
            }
            return true;
        }
    }
}
