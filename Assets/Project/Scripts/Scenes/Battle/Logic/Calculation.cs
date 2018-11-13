using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace BattleLogic {
    /// <summary>
    /// ダメージ情報
    /// </summary>
    public struct DamageInfo {
        /// <summary> 吸収回復量 </summary>
        public int heal;
        /// <summary> ダメージ </summary>
        public int damage;
        /// <summary> クリティカルしたか </summary>
        public bool isCritical;
        /// <summary> ヒットしたか </summary>
        public bool isHit;
        /// <summary> 重さ付加 </summary>
        public int addWeight;
        /// <summary> カウンターで受けるダメージ量 </summary>
        public int counterDamage;
        /// <summary> ダメージ上けた側が回復する量 </summary>
        public int reverseDamage;
        /// <summary> 攻撃属性の相性 </summary>
        public ElementAffinityEnum affinityEnum;
    }

    public class ActionResult
    {
        public Parameter invoker;
        public SkillParameter action;
        public SkillExecutionResult skillResult;
    }

    public class ChangeParameterElement {
        public int AddValue;
        public int RatioValue;

        public ChangeParameterElement()
        {
            AddValue = 0;
            RatioValue = 0;
        }

        public ChangeParameterElement(int addValue, int ratioValue)
        {
            AddValue = addValue;
            RatioValue = ratioValue;
        }

        public void StateEffectChange(int addValue, int ratioValue)
        {
            AddValue = AddValue + addValue + ((AddValue * ratioValue) / 100);
            RatioValue = RatioValue + ((RatioValue * ratioValue) / 100);
        }

        public void Add(ChangeParameterElement value)
        {
            AddValue += value.AddValue;
            RatioValue += value.RatioValue;
        }

        public int CalcChangeParameter(int baseParameter)
        {
            return AddValue + (baseParameter * RatioValue) / 100;
        }

        public void Sub(ChangeParameterElement value)
        {
            AddValue -= value.AddValue;
            RatioValue -= value.RatioValue;
        }

        public static ChangeParameterElement operator+ (ChangeParameterElement a, ChangeParameterElement b)
        {
            if (a == null && b == null) {
                return new ChangeParameterElement();
            } else if (a == null) {
                return new ChangeParameterElement (b.AddValue, b.RatioValue);
            } else if (b == null) {
                return new ChangeParameterElement (a.AddValue, a.RatioValue);
            }
            return new ChangeParameterElement (a.AddValue + b.AddValue, a.RatioValue + b.RatioValue);
        }

        public static ChangeParameterElement operator- (ChangeParameterElement a, ChangeParameterElement b)
        {
            if (a == null && b == null) {
                return new ChangeParameterElement();
            } else if (a == null) {
                return new ChangeParameterElement (-1 * b.AddValue, -1 * b.RatioValue);
            } else if (b == null) {
                return new ChangeParameterElement (a.AddValue, a.RatioValue);
            }
            return new ChangeParameterElement (a.AddValue - b.AddValue, a.RatioValue - b.RatioValue);
        }

        public static int operator+ (int a, ChangeParameterElement b)
        {
            if (b == null) {
                return a;
            }

            return a + b.CalcChangeParameter (a);
        }

        public static int operator- (int a, ChangeParameterElement b)
        {
            if (b == null) {
                return a;
            }

            return a - b.CalcChangeParameter (a);
        }
    }

    /// <summary>
    /// バトル関連計算
    /// </summary>
    public static class Calculation {

        public const int ACTION_ADD_SP = 16;
        public const int DAMAGE_ADD_SP = 4;

        public static bool IsHit(this IEnumerable<DamageInfo> self)
        {
            if (self == null) {
                return false;
            }
            return self.Any (x => x.isHit);
        }

        public static bool IsCritical(this IEnumerable<DamageInfo> self)
        {
            if (self == null) {
                return false;
            }
            return self.Any (x => x.isHit && x.isCritical);
        }

        public static bool IsHeal(this IEnumerable<DamageInfo> self)
        {
            if (self == null) {
                return false;
            }
            return self.Any (x => x.isHit && x.heal > 0);
        }

        // 一意の乱数発生を保証するため
        private static UnityEngine.Random.State battleLogicState;
        /// <summary>
        /// 乱数の初期化
        /// </summary>
        /// <param name="uuid">UUID.</param>
        public static void Init(string uuid)
        {
            UnityEngine.Random.InitState (uuid.GetHashCode ());
            battleLogicState = UnityEngine.Random.state;
        }

        /// <summary>
        /// 特定の状況の乱数発生を引き継ぐ
        /// </summary>
        /// <param name="state">特定の状態のstate</param>
        public static void Init(UnityEngine.Random.State state)
        {
            battleLogicState = state;
        }

        /// <summary>
        /// 乱数発生の状態を取得
        /// </summary>
        /// <returns>乱数状態を引き継ぐための情報を返す</returns>
        public static UnityEngine.Random.State GetRandomState()
        {
            return battleLogicState;
        }

        /// <summary>
        /// 指定ユニットの戦闘速度を計算する
        /// </summary>
        /// <returns>戦闘速度</returns>
        /// <param name="unit">ユニットデータ</param>
        public static int CalcBattleSpeed(Parameter unit)
        {
            // 単純に素早さを返すようにしておく
            return unit.Agility;
        }

        /// <summary>
        /// 状況によらないパッシブスキルによるパラメータの変動を計算する。
        /// </summary>
        /// <returns>The parameter variation.</returns>
        /// <param name="unit">Unit.</param>
        public static Dictionary<SkillTargetParameterEnum, ChangeParameterElement> CalcParameterVariation(Parameter unit)
        {
            Dictionary<SkillTargetParameterEnum, ChangeParameterElement> ret = new Dictionary<SkillTargetParameterEnum, ChangeParameterElement> ();
            var skillCount = unit.InvokePassiveSkillList.Length;
            for (int i = 0; i < skillCount; ++i) {
                var passiveSkill = unit.InvokePassiveSkillList [i];
                if (passiveSkill != null &&
                    passiveSkill.InvokeEffect.IsEffectLogic(SkillEffectLogicEnum.buff, SkillEffectLogicEnum.debuff) &&
                    !passiveSkill.HasSituationArg)
                {
                    var skillLevel = passiveSkill.ParentSkill.Level;
                    var skillEffect = passiveSkill.InvokeEffect;
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.TargetParameter) && skillEffect.IsMeetRequirements(unit)) {
                        var targetParam = skillEffect.GetValue<SkillTargetParameter> (SkillEffectLogicArgEnum.TargetParameter);
                        if (targetParam != null) {
                            ChangeParameterElement value = Calculation.CalcParameterVariation (unit, skillLevel, skillEffect);

                            if (passiveSkill.Condition != null) {
                                var stateEffectChange = CalcStateEffectChange (unit, passiveSkill.Condition.ConditionData);
                                value.StateEffectChange (stateEffectChange.AddValue, stateEffectChange.RatioValue);
                            }

                            if (!ret.ContainsKey (targetParam.Enum)) {
                                ret [targetParam.Enum] = new ChangeParameterElement();
                            }
                            if (skillEffect.effect == SkillEffectLogicEnum.buff) {
                                ret [targetParam.Enum].Add(value);
                            } else if (skillEffect.effect == SkillEffectLogicEnum.debuff) {
                                ret [targetParam.Enum].Sub(value);
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 状況によるパッシブスキルによるパラメータの変動を計算する。
        /// </summary>
        /// <returns>The parameter variation.</returns>
        /// <param name="unit">Unit.</param>
        public static Dictionary<SkillTargetParameterEnum, ChangeParameterElement> CalcSituationParameterVariation(Parameter unit, bool isAttacker, SkillParameter action, Parameter target)
        {
            Dictionary<SkillTargetParameterEnum, ChangeParameterElement> ret = new Dictionary<SkillTargetParameterEnum, ChangeParameterElement> ();
            var skillCount = unit.InvokePassiveSkillList.Length;
            for (int i = 0; i < skillCount; ++i) {
                var passiveSkill = unit.InvokePassiveSkillList [i];
                if (passiveSkill != null &&
                    passiveSkill.InvokeEffect.IsEffectLogic(SkillEffectLogicEnum.buff, SkillEffectLogicEnum.debuff) &&
                    passiveSkill.HasSituationArg)
                {
                    var skillLevel = passiveSkill.ParentSkill.Level;
                    var skillEffect = passiveSkill.InvokeEffect;
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.TargetParameter) &&
                        skillEffect.IsMeetRequirements(unit, isAttacker, action, target)) {
                        var targetParam = skillEffect.GetValue<SkillTargetParameter> (SkillEffectLogicArgEnum.TargetParameter);
                        if (targetParam != null) {
                            ChangeParameterElement value = Calculation.CalcSituationParameterVariation (unit, skillLevel, skillEffect, passiveSkill);

                            if (passiveSkill.Condition != null) {
                                var stateEffectChange = CalcStateEffectChange (unit, passiveSkill.Condition.ConditionData);
                                value.StateEffectChange (stateEffectChange.AddValue, stateEffectChange.RatioValue);
                            }

                            if (!ret.ContainsKey (targetParam.Enum)) {
                                ret [targetParam.Enum] = new ChangeParameterElement();
                            }
                            if (skillEffect.effect == SkillEffectLogicEnum.buff) {
                                ret [targetParam.Enum].Add(value);
                            } else if (skillEffect.effect == SkillEffectLogicEnum.debuff) {
                                ret [targetParam.Enum].Sub(value);
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Calculates the state grant resistance.
        /// </summary>
        /// <returns>The state grant resistance.</returns>
        /// <param name="unit">Unit.</param>
        /// <param name="probability">Probability.</param>
        /// <param name="condition">Condition.</param>
        public static int CalcStateGrantResistance(Parameter unit, int probability, Condition condition)
        {
            var skillCount = unit.InvokePassiveSkillList.Length;
            var resistance = new ChangeParameterElement();
            for (int i = 0; i < skillCount; ++i) {
                var passiveSkill = unit.InvokePassiveSkillList [i];
                if (passiveSkill != null && passiveSkill.InvokeEffect.IsEffectLogic(SkillEffectLogicEnum.state_grant_resistance)) {
                    var skillLevel = passiveSkill.ParentSkill.Level;
                    var skillEffect = passiveSkill.InvokeEffect;
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionType)) {
                        var ctype = skillEffect.GetValue<ConditionType> (SkillEffectLogicArgEnum.ConditionType);
                        if(ctype != null) {
                            if (ctype.Enum != condition.condition_type) {
                                continue;
                            }
                        }
                    }
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionGroup)) {
                        var cGroup = skillEffect.GetValue<ConditionGroup> (SkillEffectLogicArgEnum.ConditionGroup);
                        if(cGroup != null) {
                            if (!condition.condition_group.HasValue || cGroup.id != condition.condition_group.Value) {
                                continue;
                            }
                        }
                    }
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                        var c = skillEffect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                        if(c != null) {
                            if (c.id != condition.id) {
                                continue;
                            }
                        }
                    }
                    resistance.Add(CalcSituationParameterVariation (unit, skillLevel, skillEffect, passiveSkill));
                }
            }
            return probability - resistance;
        }

        /// <summary>
        /// Calculates the state effect change.
        /// </summary>
        /// <returns>The state effect change.</returns>
        /// <param name="unit">Unit.</param>
        /// <param name="condition">Condition.</param>
        public static ChangeParameterElement CalcStateEffectChange(Parameter unit, Condition condition)
        {
            var skillCount = unit.InvokePassiveSkillList.Length;
            var change = new ChangeParameterElement();
            for (int i = 0; i < skillCount; ++i) {
                var passiveSkill = unit.InvokePassiveSkillList [i];
                if (passiveSkill != null && passiveSkill.InvokeEffect.IsEffectLogic(SkillEffectLogicEnum.state_effect_change)) {
                    var skillLevel = passiveSkill.ParentSkill.Level;
                    var skillEffect = passiveSkill.InvokeEffect;
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionType)) {
                        var ctype = skillEffect.GetValue<ConditionType> (SkillEffectLogicArgEnum.ConditionType);
                        if(ctype != null) {
                            if (ctype.Enum != condition.condition_type) {
                                continue;
                            }
                        }
                    }
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.ConditionGroup)) {
                        var cGroup = skillEffect.GetValue<ConditionGroup> (SkillEffectLogicArgEnum.ConditionGroup);
                        if(cGroup != null) {
                            if (!condition.condition_group.HasValue || cGroup.id != condition.condition_group.Value) {
                                continue;
                            }
                        }
                    }
                    if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                        var c = skillEffect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                        if(c != null) {
                            if (c.id != condition.id) {
                                continue;
                            }
                        }
                    }
                    change.Add(CalcSituationParameterVariation (unit, skillLevel, skillEffect, passiveSkill));
                }
            }
            return change;
        }

        /// <summary>
        /// パッシブスキルを追加する
        /// </summary>
        /// <param name="unit">Unit.</param>
        /// <param name="targets">Targets.</param>
        public static void AdditionPassiveSkill(BattleLogic.Parameter unit, BattleLogic.Parameter[] targets = null)
        {
            if (unit.PassiveSkillList.Length <= 0) {
                return;
            }

            // ターゲットに指定されたものがいればそいつだけに効果付与を行う。
            foreach (var skill in unit.PassiveSkillList) {
                foreach (var skillEffectSetting in skill.Skill.SkillEffects) {
                    foreach (var target in Calculation.GetReceivers (unit, null, null, skillEffectSetting.range, true)) {
                        if (targets == null || targets.Length <= 0 || targets.Contains (target)) {
                            target.AddPassiveSkill (null, skill, skillEffectSetting.skill_effect); 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 指定ユニットが持っているパッシブスキルを全て削除する
        /// </summary>
        /// <param name="unit">Unit.</param>
        public static void RemovePassiveSkill(BattleLogic.Parameter unit)
        {
            if (unit.PassiveSkillList.Length <= 0) {
                return;
            }

            foreach (var skill in unit.PassiveSkillList) {
				
				var allyList = AwsModule.BattleData.SallyAllyParameterList;
				foreach (var target in allyList) {
                    target.RemovePassiveSkill (skill);
                }

				var enemyList = AwsModule.BattleData.SallyEnemyParameterList;
				foreach (var target in enemyList) {
                    target.RemovePassiveSkill (skill);
                }
            }
        }

        public static ElementAffinity GetElementAffinity(Element attackElement, Element defenseElement)
        {
            return MasterDataTable.element_affinity_setting.GetElementAffinity (
                attackElement.Enum, defenseElement.Enum);
        }

        public static ElementAffinityEnum GetElementAffinityEnum(Element attackElement, Element defenseElement)
        {
            return GetElementAffinity(attackElement, defenseElement).Enum;
        }

        public static ElementAffinity GetElementAffinity(Parameter attacker, Parameter defender)
        {
            return MasterDataTable.element_affinity_setting.GetElementAffinity (
                attacker.Element.Enum, defender.Element.Enum);
        }

        public static ElementAffinityEnum GetElementAffinityEnum(Parameter attacker, Parameter defender)
        {
            return GetElementAffinity(attacker, defender).Enum;
        }

        /// <summary>
        /// スキルの実行
        /// </summary>
        /// <param name="invoker">発動者</param>
        /// <param name="target">ターゲット</param>
        /// <param name="action">発動スキル</param>
        public static ActionResult Action(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter action)
        {
            if (!action.IsAction && !action.IsSpecial) {
                throw new DoNotActionSkillException ();
            } else if(action.IsAction && !action.Enabled) {
                throw new NotSkillChargedException ();
            } else if(action.IsSpecial && !invoker.IsSpMax) {
                throw new NotMaxSpException ();
            }

            // 行動する時はガード状態を解除する。
            invoker.ReleaseGuard ();
            invoker.AddTurnCount ();

            ActionResult ret = new ActionResult ();
            ret.invoker = invoker;
            ret.action = action;
            ret.skillResult = SkillExecutor.Execute(invoker, target, allyTarget, action);

            // アクションスキルをチャージする。
            Array.ForEach (invoker.ActionSkillList, x => x.Charge(1));

            // 使用したスキルは使用関数を呼ぶ
            // これでチャージターンが0になる。
            action.Use ();

            if (action.IsSpecial) {
                // 必殺技を使用したらSpを0にする。
                invoker.ResetSp();
            } else if(ret.skillResult.IsSuccess) {
                // SPを加算する
                invoker.ChargeSp(ACTION_ADD_SP, false);
                if (ret.skillResult.IsDamageCritical)
                {
                    invoker.ChargeSp(ACTION_ADD_SP, false);
                }
            }

            Dictionary<Parameter, int> spAdd = new Dictionary<Parameter, int> ();
            foreach (var result in ret.skillResult.GetResults(SkillEffectLogicEnum.damage, SkillEffectLogicEnum.special_damage)) {
                var receiver = result.Key;
                var damageInfos = result.Value.Cast<DamageSkillEffectResult> ().SelectMany (x => x.DamageInfos);
                int addValue = 0;
                if (damageInfos.IsHit()) {
                    addValue += DAMAGE_ADD_SP;
                    if (damageInfos.IsCritical ()) {
                        addValue += DAMAGE_ADD_SP;
                    }

                    if(damageInfos.Any(x => x.counterDamage > 0)) {
                        invoker.ChargeSp (DAMAGE_ADD_SP, false);
                    }

                    // ダメージ回数周りのカウント
                    invoker.AddDamageGivenCount ();
                    receiver.AddDamageCount ();
                }

                if (addValue > 0) {
                    if (spAdd.ContainsKey (receiver)) {
                        spAdd [receiver] = addValue > spAdd [receiver] ? addValue : spAdd [receiver];
                    } else {
                        spAdd.Add (receiver, addValue);
                    }
                }
            }
            foreach (var pair in spAdd) {
                pair.Key.ChargeSp(pair.Value, false);
            }

            invoker.SetWeight (action.Skill.weight);
            invoker.Actioned ();

            return ret;
        }

        /// <summary>
        /// 状態異常の効果発動ロジック
        /// </summary>
        /// <returns>The effect.</returns>
        /// <param name="unit">状態異常の効果を受けるユニット</param>
        /// <param name="conditionEffect">状態異常の効果設定スキル</param>
        public static ActionResult ConditionEffect(ConditionEffectTiming condition)
        {
            ActionResult ret = condition.parent.TimingExexute();

            Dictionary<Parameter, int> spAdd = new Dictionary<Parameter, int> ();
            foreach (var result in ret.skillResult.GetResults(SkillEffectLogicEnum.damage, SkillEffectLogicEnum.special_damage)) {
                var receiver = result.Key;
                var damageInfos = result.Value.Cast<DamageSkillEffectResult> ().SelectMany (x => x.DamageInfos);
                int addValue = 0;
                if (damageInfos.IsHit ()) {
                    addValue += DAMAGE_ADD_SP;
                    if (damageInfos.IsCritical ()) {
                        addValue += DAMAGE_ADD_SP;
                    }
                }
                if (addValue > 0) {
                    if (spAdd.ContainsKey (receiver)) {
                        spAdd [receiver] = addValue > spAdd [receiver] ? addValue : spAdd [receiver];
                    } else {
                        spAdd.Add (receiver, addValue);
                    }
                }
            }
            foreach (var pair in spAdd) {
                pair.Key.ChargeSp (pair.Value, false);
            }

            condition.SetWeight ();
            return ret;
        }

        public static bool CallingOffJudgment(int probability)
        {
            return RandomRange(0, 100) < probability;
        }

        public static List<ActionResult> AutoAction(Parameter invoker, bool isTurn)
        {
            List<ActionResult> ret = new List<ActionResult> ();
            var skills = invoker.InvokePassiveSkillList;
            int passiveSkillCount = invoker.InvokePassiveSkillList.Length;
            for (int i = 0; i < passiveSkillCount; i++) {
                if (skills [i] == null) {
                    continue;
                }
                    
                var invokeEffect = skills [i].InvokeEffect;
                if (!((isTurn && skills [i].HasAutoTurnLogic) || (!isTurn && skills [i].HasAutoWaveStartLogic))) {
                    continue;
                }
                // スキル効果を発動
                var invokeSkill = invokeEffect.GetValue<Skill> (SkillEffectLogicArgEnum.Skill);
                if (invokeSkill == null) {
                    Debug.LogError ("Not Fount Skill");
                    continue;
                }
                var invokeSkillParameter = new SkillParameter (skills [i].ParentSkill.Level, invokeSkill, true);

                ActionResult result = new ActionResult ();
                result.invoker = invoker;
                result.action = invokeSkillParameter;
                if (skills [i].Condition != null) {
                    result.skillResult = SkillExecutor.Execute (skills [i].Condition.ConditionData, invoker, invokeSkillParameter);
                } else {
                    result.skillResult = SkillExecutor.Execute (invoker, null, null, invokeSkillParameter);
                }

                ret.Add (result);
            }

            return ret;
        }

        /// <summary>
        /// スキルの効果範囲を取得する
        /// </summary>
        /// <returns>The range in position list.</returns>
        /// <param name="invoker">Invoker.</param>
        /// <param name="target">Target.</param>
        /// <param name="action">Action.</param>
        public static List<PositionData> GetRangeInPositionList(Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter action)
        {
            HashSet<PositionData> ret = new HashSet<PositionData> ();
            int effectCount = action.Skill.SkillEffects.Length;
            for (int i = 0; i < effectCount; i++) {
                var effect = action.Skill.SkillEffects [i];
                foreach (var positionData in GetRangeInPositionDatas(invoker, target, allyTarget, effect.range)) {
                    ret.Add (positionData);
                }
            }
            return ret.ToList();
        }
        private static IEnumerable<PositionData> GetRangeInPositionDatas(Parameter invoker, Parameter target, Parameter allyTarget, SkillRange range)
        {
            // 敵味方のリストを取得
            PositionData basePosition = null;
            if (range.target == SkillTargetEnum.self) {
                basePosition = invoker.Position;
            } else if (range.target == SkillTargetEnum.enemy && target != null) {
                basePosition = target.Position;
            } else if (range.target == SkillTargetEnum.ally) {
                if (allyTarget != null) {
                    basePosition = allyTarget.Position;
                } else { // 味方が設定されていないときは自分を設定する。
                    basePosition = invoker.Position;
                }
            }

            HashSet<PositionData> positionDatas = new HashSet<PositionData> ();
            // 全体ロジック
            if (range.range_setting == (int)SkillRangeSettingEnum.All && basePosition != null) {
                for (int row = 0; row < PositionData.MAX_ROW; ++row) {
                    for (int column = 0; column < PositionData.MAX_ROW; ++column) {
                        var positionData = new PositionData () {
                            isPlayer = basePosition.isPlayer,
                            row = row,
                            column = column
                        };
                        positionDatas.Add (positionData);
                    }
                }
            } else if(basePosition != null) {
                int rangeDataCount = range.rangeSettings.Length;
                for (int i = 0; i < rangeDataCount; ++i) {
                    var rangeData = range.rangeSettings [i];
                    var column = basePosition.column;
                    var row = basePosition.row;

                    if (rangeData.range_base == SkillRangeBaseEnum.fixation) {
                        column = rangeData.x;
                        row = rangeData.y;
                    } else if(rangeData.range_base == SkillRangeBaseEnum.target) {
                        column += rangeData.x;
                        row += rangeData.y;
                    }
                    if (column < 0 || row < 0 || column >= PositionData.MAX_COLUMN || row >= PositionData.MAX_ROW) {
                        continue;
                    }

                    var positionData = new PositionData () {
                        isPlayer = basePosition.isPlayer,
                        row = row,
                        column = column
                    };
                    positionDatas.Add (positionData);
                }
            }

            return positionDatas;
        }

        /// <summary>
        /// スキルの範囲情報から対象ユニットを取得
        /// </summary>
        /// <returns>The receivers.</returns>
        /// <param name="invoker">Invoker.</param>
        /// <param name="target">Target.</param>
        /// <param name="range">Range.</param>
        public static IEnumerable<Parameter> GetReceivers(Parameter invoker, Parameter target, Parameter allyTarget, SkillRange range, bool disableTargetReversal = false)
        {
            // 敵味方のリストを取得
            bool targetReversal = false;
            if (invoker.BattleAI != null && !disableTargetReversal) {
                targetReversal = invoker.BattleAI.TargetReversal;
            }
            List<Parameter> targetList = null;
            if (invoker.Position.isPlayer) {
                if (range.target == SkillTargetEnum.ally || range.target == SkillTargetEnum.self) {
                    if (targetReversal) {
                        targetList = AwsModule.BattleData.SallyEnemyParameterList.ToList();
                    } else {
                        targetList = AwsModule.BattleData.SallyAllyParameterList.ToList ();
                    }
                } else if (range.target == SkillTargetEnum.enemy) {
                    if (targetReversal) {
                        targetList = AwsModule.BattleData.SallyAllyParameterList.ToList ();
                    } else {
                        targetList = AwsModule.BattleData.SallyEnemyParameterList.ToList();
                    }
                }
            } else {
                if (range.target == SkillTargetEnum.ally || range.target == SkillTargetEnum.self) {
                    if (targetReversal) {
                        targetList = AwsModule.BattleData.SallyAllyParameterList.ToList ();
                    } else {
                        targetList = AwsModule.BattleData.SallyEnemyParameterList.ToList();
                    }
                } else if (range.target == SkillTargetEnum.enemy) {
                    if (targetReversal) {
                        targetList = AwsModule.BattleData.SallyEnemyParameterList.ToList();
                    } else {
                        targetList = AwsModule.BattleData.SallyAllyParameterList.ToList ();
                    }

                }
            }

            SkillElementTargetSetting elementSetting = range.element_target;
            // 属性指定
            if (elementSetting != null) {
                targetList = elementSetting.Extraction (targetList);
            }
   
            PositionData basePosition = null;
			if(range.target == SkillTargetEnum.self){
				basePosition = invoker.Position;
			} else if(range.target == SkillTargetEnum.enemy && target != null) {
				basePosition = target.Position;
			}else if(range.target == SkillTargetEnum.ally && allyTarget != null) {
				basePosition = allyTarget.Position;
			}
            List<Parameter> receivers = new List<Parameter>();

            // 全体ロジック
            if (range.IsAll) {
                // 大型敵の場合に二回ダメージが入る可能性があるので全てのパネルを舐める
                for (int row = 0; row < PositionData.MAX_ROW; ++row) {
                    for (int col = 0; col < PositionData.MAX_COLUMN; ++col) {
                        var receiver = targetList.FirstOrDefault (x => x.Position.InArea (x.Position.isPlayer, row, col));
                        if (receiver != null) {
                            receivers.Add (receiver);
                        }
                    }
                }
            } else if (basePosition != null && targetList.Count > 0) {
                int maxColumn = targetList.Max (x => x.Position.MaxColumn);
                int maxRow = targetList.Max (x => x.Position.MaxRow);
                int rangeDataCount = range.rangeSettings.Length;
                for (int i = 0; i < rangeDataCount; ++i) {
                    var rangeData = range.rangeSettings [i];
                    var column = basePosition.column;
                    var row = basePosition.row;

                    if (rangeData.range_base == SkillRangeBaseEnum.fixation) {
                        column = rangeData.x;
                        row = rangeData.y;
                    } else if (rangeData.range_base == SkillRangeBaseEnum.target) {
                        column += rangeData.x;
                        row += rangeData.y;
                    }
                    if (column < 0 || row < 0 || maxRow < row || maxColumn < column) {
                        continue;
                    }

                    var receiver = targetList.FirstOrDefault (x => x.Position.InArea (x.Position.isPlayer, row, column));
                    if (receiver != null) {
                        receivers.Add (receiver);
                    }
                }
            } else {
                Debug.LogError (string.Format("Error: basePosition is null target: {0} allyTarget:{1} basePosition:{2} targetList.Count:{3}", target.Name, allyTarget.Name, basePosition, targetList.Count));
            }
             
            if (range.random_min_target > 0) {
                int maxTarget = Mathf.Max (range.random_min_target, range.random_max_target);
                if (!range.is_duplication_target && maxTarget > receivers.Count) {
                    return receivers;
                }

                List<Parameter> randomTargetList = new List<Parameter>();
                int targetCount = RandomRange (range.random_min_target, maxTarget + 1);
                for (int i = 0; i < targetCount; ++i) {
                    var randomTarget = receivers.OrderBy ((x) => Guid.NewGuid()).First();
                    if (!range.is_duplication_target) {
                        receivers.Remove (randomTarget);
                    }
                    randomTargetList.Add (randomTarget);
                }
                return randomTargetList;
            }
            return receivers;
        }

        /// <summary>
        /// ダメージ計算
        /// </summary>
        /// <returns>ダメージ情報のリスト</returns>
        /// <param name="attacker">攻撃側Unitデータ</param>
        /// <param name="defender">防御側Unitデータ</param>
        public static List<DamageInfo> CalcDamage(Parameter attacker, Parameter defender, int skillLevel, AttackElementEnum? attackElement, SkillEffect effect, bool isSpecial, ChangeParameterElement element)
        {
            
            List<DamageInfo> damageList = new List<DamageInfo> ();

            if (attacker == null) {
                return damageList;
            }

            bool isMustHit = false;
            bool isMustCritical = false;
            bool disableFormationRate = false;
            bool defenseIgnored = false;
            int addValue = 0;
            int ratioValue = 0;
            int attackCount = attacker.AttackCount;
            int addWeight = 0;
            int counterDamage = 0;
            int reverseDamage = 0;
            int? hitCorrection = null;
            int? damageAbsorption = null;

            if (effect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Value);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.AttackCount)) {
                attackCount = effect.GetValue<int> (SkillEffectLogicArgEnum.AttackCount);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.MustHit)) {
                isMustHit = effect.GetValue<int> (SkillEffectLogicArgEnum.MustHit) != 0;
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.DisableFormationRate)) {
                disableFormationRate = effect.GetValue<int> (SkillEffectLogicArgEnum.DisableFormationRate) != 0;
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.DefenseIgnored)) {
                defenseIgnored = effect.GetValue<int> (SkillEffectLogicArgEnum.DefenseIgnored) != 0;
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.AddWeight)) {
                addWeight = effect.GetValue<int> (SkillEffectLogicArgEnum.AddWeight);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.MustCritical)) {
                isMustCritical = effect.GetValue<int> (SkillEffectLogicArgEnum.MustCritical) != 0;
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.HitCorrection)) {
                hitCorrection = effect.GetValue<int> (SkillEffectLogicArgEnum.HitCorrection);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageAbsorption)) {
                damageAbsorption = effect.GetValue<int> (SkillEffectLogicArgEnum.DamageAbsorption);
            }

            /*
             *  ダメージ
             *      Atc＝攻撃側攻撃力に±7.5％を４回(攻撃力の70％～130％をベルカーブに)
             *      Def＝防御側防御力に±7.5％を４回(防御力の70％～130％をベルカーブに)
             *      ダメージ=Atc-Def
             */
            var elementAffinity = GetElementAffinity (attacker.Element, defender.Element);
            var elementAffinityCorrectionValue = elementAffinity.correction_value + (attacker.GetElementRateChangeParameter () + defender.GetElementRateChangeParameter ());
            Debug.LogFormat ("{0} elementAffinityCorrectionValue: {1} => {2}", elementAffinity.Enum, elementAffinity.correction_value, elementAffinityCorrectionValue);

            var attackerRate = 1.0f;
            var defenderRate = 1.0f;
            if (!disableFormationRate) {
                var attackerFormationRate = GetFormationAttackRate (attacker);
                if (attackerFormationRate != null) {
                    if (!attackElement.HasValue || attackElement.Value == AttackElementEnum.Physics) {
                        attackerRate = (float)attackerFormationRate.physics_attack_rate / 100.0f;
                    } else if (attackElement.Value == AttackElementEnum.Physics) {
                        attackerRate = (float)attackerFormationRate.magic_attack_rate / 100.0f;
                    }
                }
                var defenderFormationRate = GetFormationAttackRate (defender);
                if (defenderFormationRate != null) {
                    defenderRate = (float)defenderFormationRate.defense_rate / 100.0f;
                }
            }

            int realAttack = isSpecial ? attacker.RealSpAttack : attacker.RealAttack;

            int attack = (
                realAttack + addValue +
                (realAttack * ratioValue / 100) +
                (realAttack * elementAffinityCorrectionValue / 100)
            );
            int defense = !defenseIgnored ? defender.RealDefense + defender.GuardAddValue +
                (defender.RealDefense * defender.GuardRatioValue / 100) : 0;
            
            for (int i = 0; i < attackCount; ++i) {
                bool isHit = isMustHit || HitJudgment (attacker, defender, hitCorrection);
                // 絶対回避の発動判定
                if (defender.EnableAbsoluteAvoidance ()) {
                    isHit = false;
                }

                bool isCritical = false;
                int damage = 0;
                int absorption = 0;
                if (isHit) {
                    isCritical = isMustCritical || CriticalJudgment (attacker);

                    float a1 = Mathf.Pow (attack, 0.9f) * GetCriticalRate(attacker, isCritical) * attackerRate;
                    float d1 = Mathf.Pow (defense, 0.9f) * defenderRate;
                    float atk = a1 * 0.95f + a1 * 0.05f * RandomRange (0.0f, 1.0f) + a1 * 0.05f * RandomRange (0.0f, 1.0f);
                    float def = d1 * 0.95f + d1 * 0.05f * RandomRange (0.0f, 1.0f) + d1 * 0.05f * RandomRange (0.0f, 1.0f);

                    // 計算結果確認に使うので残しておきます。
                    //Debug.Log (string.Format("Attack:{0} Defense:{1} isCritical:{6} attackerRate:{7} defenderRate:{8} correction_value:{9} a1:{2} d1:{3} atk:{4} def:{5}", 
                    //    attacker.Attack, defender.Defense, a1, d1, atk, def, isCritical, attackerRate, defenderRate, elementAffinity.correction_value));

                    var d = (atk - def);

                    // 最終ダメージをみて50以下ならいじる
                    if(d < 50) {
                        d = RandomRange (0.0f, 1.0f) * Mathf.Pow (a1, 0.6f);
                    }

                    // ダメージの補正処理
                    damage = attacker.AddDamage ((int)d);
                    damage = defender.DecreaseDamage(damage);
                    if (element != null) {
                        damage = damage + element;
                    }


                    // 完全防御の発動判定
                    if (defender.EnablePerfectGuard ()) {
                        damage = 0;
                    }
                    // 根性の発動判定
                    if (defender.EnableGuts (damage)) {
                        damage = defender.Hp - 1;
                    }
                    damage = Mathf.Max (0, damage);

                    // 与えたダメージ吸収の処理
                    if (damageAbsorption.HasValue) {
                        absorption = damage * damageAbsorption.Value / 100;
                    }

                    // ダメージカウンターの処理
                    if (defender.EnableCounter (damage)) {
                        counterDamage = CalcCounter (defender, damage);
                    }
                    // 受けたダメージの吸収処理
                    if (defender.EnableReverseDamage (damage)) {
                        reverseDamage = CalcReverseDamage (defender, damage);
                    }
                }
                // ダメージ情報を追加
                damageList.Add (new DamageInfo () {
                    isHit=isHit,
                    isCritical=isCritical,
                    damage=damage,
                    heal=absorption,
                    addWeight=isHit ? addWeight : 0,
                    affinityEnum=elementAffinity.Enum,
                    counterDamage=counterDamage,
                    reverseDamage=reverseDamage
                });
            }

            return damageList;
        }

        /// <summary>
        /// 特殊ダメージ計算
        /// </summary>
        /// <returns>The special damege.</returns>
        /// <param name="attacker">Attacker.</param>
        /// <param name="defender">Defender.</param>
        /// <param name="skill_level">Skill level.</param>
        /// <param name="effect">Effect.</param>
        public static List<DamageInfo> CalcSpecialDamege(Parameter attacker, Parameter defender, int skillLevel, AttackElementEnum? attackElement, SkillEffect effect, ChangeParameterElement element)
        {
            List<DamageInfo> damageList = new List<DamageInfo> ();

            int fixedValue = 0;
            int hpRatioValue = 0;
            int currentHpRatioValue = 0;
            if (effect.ContainsArg (SkillEffectLogicArgEnum.FixedValue)) {
                fixedValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Value);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupFixedValue)) {
                fixedValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.HPRatio)) {
                hpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.HPRatio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupHPRatio)) {
                hpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupHPRatio) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.CurrentHPRatio)) {
                currentHpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.CurrentHPRatio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupCurrentHPRatio)) {
                currentHpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupCurrentHPRatio) * (skillLevel - 1);
            }
                
            int damage = 0;
            damage = (fixedValue + defender.MaxHp * hpRatioValue / 100 + defender.Hp * currentHpRatioValue / 100);

            if (element != null) {
                damage = damage + element;
            }
            if (defender.EnableGuts (damage)) {
                damage = defender.Hp - 1;
            }
            damage = Mathf.Max (0, damage);

            // ダメージ情報を追加
            damageList.Add (new DamageInfo () {
                isHit=true,
                isCritical=false,
                damage=damage,
                heal=0,
                addWeight=0,
                affinityEnum=ElementAffinityEnum.normal,
                counterDamage=0,
                reverseDamage=0
            });

            return damageList;
        }

        /// <summary>
        /// Conditionを受けるかどうかの判定
        /// </summary>
        /// <returns><c>true</c>, if condition judgment was received, <c>false</c> otherwise.</returns>
        /// <param name="attacker">Attacker.</param>
        /// <param name="defender">Defender.</param>
        /// <param name="skillEffect">Skill effect.</param>
        public static bool ReceiveConditionJudgment(Parameter attacker, Parameter defender, SkillEffect skillEffect, int skillLevel, Condition condition)
        {
            if (condition.is_debuff) {
                int probability = 0;
                // 必ず付与フラグ
                if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.MustConditionGrand)) {
                    return true;
                }
                if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.Probability)) {
                    probability += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.Probability);
                }
                if (skillEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupProbability)) {
                    probability += skillEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupProbability) * (skillLevel - 1);
                }
                probability = defender.StateGrantResistance (probability, condition);

                return RandomRange (0, 100) < probability;
            }

            return true;
        }

        /// <summary>
        /// カウンターダメージ計算
        /// </summary>
        /// <returns>The counter.</returns>
        /// <param name="unit">Unit.</param>
        /// <param name="damage">Damage.</param>
        private static int CalcCounter(Parameter unit, int damage)
        {
            int ret = 0;
            var skills = unit.InvokePassiveSkillList.Where (x => x != null && x.HasCounterLogic);
            foreach (var skill in skills) {
                if (!skill.EnableCondition ()) {
                    continue;
                }

                bool deadInvoke = false;
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.DeadInvoke)) {
                    deadInvoke = skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.DeadInvoke) != 0;
                }
                if (!deadInvoke && unit.Hp <= damage) {
                    continue;
                }                    
                int value = 0;
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                    value += skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.Value);
                }
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                    value += skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (skill.ParentSkill.Level - 1);
                }
                ret += damage * value / 100;
                skill.AddExecuteCount ();
            }

            return ret;
        }

        /// <summary>
        /// ダメージ吸収計算
        /// </summary>
        /// <returns>The counter.</returns>
        /// <param name="unit">Unit.</param>
        /// <param name="damage">Damage.</param>
        private static int CalcReverseDamage(Parameter unit, int damage)
        {
            int ret = 0;
            var skills = unit.InvokePassiveSkillList.Where (x => x != null && x.HasReverseDamageLogic);
            foreach (var skill in skills) {
                if (!skill.EnableCondition ()) {
                    continue;
                }

                bool deadInvoke = false;
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.DeadInvoke)) {
                    deadInvoke = skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.DeadInvoke) != 0;
                }
                if (!deadInvoke && unit.Hp <= damage) {
                    continue;
                }
                int value = 0;
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                    value += skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.Value);
                }
                if (skill.InvokeEffect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                    value += skill.InvokeEffect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (skill.ParentSkill.Level - 1);
                }
                ret += damage * value / 100;
                skill.AddExecuteCount ();
            }
            return ret;
        }

        /// <summary>
        /// ヒット判定
        /// </summary>
        /// <returns><c>true</c>攻撃が当たった<c>false</c>攻撃が外れた</returns>
        /// <param name="attacker">攻撃側Unitデータ</param>
        /// <param name="defender">防御側Unitデータ</param>
        private static bool HitJudgment(Parameter attacker, Parameter defender, int? hitCorrection)
        {
            /*
             * 命中判定 95+((((攻撃側素早さ/300)^(1/2))-((防御側素早さ/300)^(1/2)))*100)
             */
            int hit = 95 + attacker.RealHit - defender.RealEvasion;
            if (hitCorrection.HasValue) {
                hit = hit + (hit * hitCorrection.Value / 100);
            }
            return RandomRange(0, 100) < hit;
        }

        /// <summary>
        /// クリティカル判定
        /// </summary>
        /// <returns><c>true</c>クリティカルした<c>false</c>クリティカルしなかった</returns>
        /// <param name="attacker">攻撃側Unitデータ</param>
        private static bool CriticalJudgment(Parameter attacker)
        {
            /*
             * クリティカル発生率％   1+(運^(2/5))
             * キャラの運が0で1%、1で2%、30で4.8%、60で6.1%、100で7.3%
             */
            int critical = 1 + attacker.RealCritical;
            return RandomRange(0, 100) < critical;
        }

        private static float GetCriticalRate(Parameter attacker, bool isCritical)
        {
            if (!isCritical) {
                return 1.0f;
            }

            return (float)attacker.CriticalDamageRate (200) / 100.0f;
        }

        /// <summary>
        /// ダメージ計算用ベルカーブ関数
        /// </summary>
        /// <returns>ベルカーブ値</returns>
        /// <param name="baseValue">ベース値</param>
        /// <param name="correctionValue">補正値(±百分率)</param>
        /// <param name="correctionCount">補正回数</param>
        private static int BellCurve(int baseValue, float correctionValue, int correctionCount)
        {
            int ret = baseValue;
            for (int i = 0; i < correctionCount; ++i) {
                ret += (int)(baseValue * RandomRange (-1 * correctionValue, correctionValue) / 100.0f);
            }
            return ret;
        }


        /// <summary>
        /// 陣形攻撃倍率データの取得
        /// </summary>
        /// <returns>陣形倍率データ.該当するものがない場合はnull</returns>
        /// <param name="unit">Unit.</param>
        private static FormationAttackRate GetFormationAttackRate(Parameter unit)
        {
            int numberOfFront = GetNumberOfFront (unit);
            return MasterDataTable.formation_attack_rate.DataList.FirstOrDefault(x => x.number_of_front == numberOfFront);
        }

        public static int GetNumberOfFront(Parameter unit)
        {
            var unitList = AwsModule.BattleData.SallyAllyParameterList;

            if (!unit.Position.isPlayer) {
                unitList = AwsModule.BattleData.SallyEnemyParameterList;
            }

            // 全ての値を取得し重複を削除する。
            var rows = unitList.Where(x => x.Hp > 0).Select (x => x.Position.row).Distinct().OrderBy(x => x);
            var rowCount = rows.Count ();

            int targetRow = unit.Position.row;

            int numberOfFront = 0;
            for (int i = 0; i < rowCount; ++i) {
                if (targetRow == rows.ElementAt (i)) {
                    numberOfFront = i;
                    break;
                }
            }
            return numberOfFront;
        }

        /// <summary>
        /// 乱数取得用ラッパー関数
        /// 仕様として最小値から最大値-1の値の乱数を返すとする
        /// </summary>
        /// <returns>乱数</returns>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        public static int RandomRange(int min, int max)
        {
            UnityEngine.Random.state = battleLogicState;
            var ret = UnityEngine.Random.Range(min, max);
            battleLogicState = UnityEngine.Random.state;
            return ret;
        }

        /// <summary>
        /// 乱数取得用ラッパー関数
        /// 仕様として最小値から最大値の値の乱数を返すとする
        /// </summary>
        /// <returns>乱数</returns>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        public static float RandomRange(float min, float max)
        {
            UnityEngine.Random.state = battleLogicState;
            var ret = UnityEngine.Random.Range(min, max);
            battleLogicState = UnityEngine.Random.state;
            return ret;
        }

        /// <summary>
        /// スキルによる加減算値を求める
        /// </summary>
        /// <returns>The parameter variation.</returns>
        /// <param name="baseValue">Base value.</param>
        /// <param name="level">Level.</param>
        /// <param name="effect">Effect.</param>
        public static ChangeParameterElement CalcParameterVariation(Parameter target, int level, SkillEffect effect)
        {
            int addValue = 0;
            int ratioValue = 0;
            if (effect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Value);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (level - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (level - 1);
            }

            int hpDecreaseValue = 0;
            int hpDecreaseRatio = 0;
            if (effect.ContainsArg (SkillEffectLogicArgEnum.HpDecreaseRatio)) {
                int decreaseRatio = effect.GetValue<int> (SkillEffectLogicArgEnum.HpDecreaseRatio);
                var scale = Mathf.CeilToInt((1.0f - target.HpProgress) * 100.0f) / decreaseRatio;
                if (effect.ContainsArg (SkillEffectLogicArgEnum.ValueOfChangeEveryDecreaseRatio)) {
                    hpDecreaseValue += effect.GetValue<int> (SkillEffectLogicArgEnum.ValueOfChangeEveryDecreaseRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupValueOfChangeEveryDecreaseRatio)) {
                    hpDecreaseValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValueOfChangeEveryDecreaseRatio) * (level - 1);
                }
                hpDecreaseValue *= scale;

                if (effect.ContainsArg (SkillEffectLogicArgEnum.RatioOfChangeEveryDecreaseRatio)) {
                    hpDecreaseRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.RatioOfChangeEveryDecreaseRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatioOfChangeEveryDecreaseRatio)) {
                    hpDecreaseRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatioOfChangeEveryDecreaseRatio);
                }
                hpDecreaseRatio *= scale;
            }

            return new ChangeParameterElement (addValue + hpDecreaseValue, ratioValue + hpDecreaseRatio);
        }

        public static ChangeParameterElement CalcSituationParameterVariation(Parameter target, int level, SkillEffect effect, PassiveSkillParameter passiveSkill)
        {
            int addValue = 0;
            int ratioValue = 0;
            if (effect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Value);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (level - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (level - 1);
            }

            // ターン経過によるパラメータ変化
            if (effect.ContainsArg (SkillEffectLogicArgEnum.TurnCountValue)) {
                addValue += target.WaveTurnCount * effect.GetValue<int> (SkillEffectLogicArgEnum.TurnCountValue);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupTurnCountValue)) {
                addValue += target.WaveTurnCount * effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupTurnCountValue) * (level - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.TurnCountRatio)) {
                ratioValue += target.WaveTurnCount * effect.GetValue<int> (SkillEffectLogicArgEnum.TurnCountRatio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupTurnCountRatio)) {
                ratioValue += target.WaveTurnCount * effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupTurnCountRatio) * (level - 1);
            }

            int hpDecreaseValue = 0;
            int hpDecreaseRatio = 0;
            if (effect.ContainsArg (SkillEffectLogicArgEnum.HpDecreaseRatio)) {
                int decreaseRatio = effect.GetValue<int> (SkillEffectLogicArgEnum.HpDecreaseRatio);
                var scale = Mathf.CeilToInt((1.0f - target.HpProgress) * 100.0f) / decreaseRatio;
                if (effect.ContainsArg (SkillEffectLogicArgEnum.ValueOfChangeEveryDecreaseRatio)) {
                    hpDecreaseValue += effect.GetValue<int> (SkillEffectLogicArgEnum.ValueOfChangeEveryDecreaseRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupValueOfChangeEveryDecreaseRatio)) {
                    hpDecreaseValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValueOfChangeEveryDecreaseRatio) * (level - 1);
                }
                hpDecreaseValue *= scale;

                if (effect.ContainsArg (SkillEffectLogicArgEnum.RatioOfChangeEveryDecreaseRatio)) {
                    hpDecreaseRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.RatioOfChangeEveryDecreaseRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatioOfChangeEveryDecreaseRatio)) {
                    hpDecreaseRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatioOfChangeEveryDecreaseRatio);
                }
                hpDecreaseRatio *= scale;
            }

            int damageCountValue = 0;
            int damageCountRatio = 0;
            if (effect.ContainsAnyArgs (SkillEffectLogicArgEnum.DamageCountValue, SkillEffectLogicArgEnum.LevelupDamageCountValue)) {
                int damageCountPreValue = 0;
                int damageCountMaxValue = int.MaxValue;
                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageCountValue)) {
                    damageCountPreValue += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageCountValue);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageCountValue)) {
                    damageCountPreValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageCountValue) * (level - 1);
                }

                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageCountMaxValue)) {
                    damageCountMaxValue += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageCountMaxValue);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageCountMaxValue)) {
                    damageCountMaxValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageCountMaxValue) * (level - 1);
                }
                damageCountValue = Mathf.Min (damageCountMaxValue, damageCountPreValue * passiveSkill.GetDamageCount (target));
            }

            if (effect.ContainsAnyArgs (SkillEffectLogicArgEnum.DamageCountRatio, SkillEffectLogicArgEnum.LevelupDamageCountRatio)) {
                int damageCountPreRatio = 0;
                int damageCountMaxRatio = int.MaxValue;
                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageCountRatio)) {
                    damageCountPreRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageCountRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageCountRatio)) {
                    damageCountPreRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageCountRatio) * (level - 1);
                }

                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageCountMaxRatio)) {
                    damageCountMaxRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageCountMaxRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageCountMaxRatio)) {
                    damageCountMaxRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageCountMaxRatio) * (level - 1);
                }
                damageCountRatio = Mathf.Min (damageCountMaxRatio, damageCountPreRatio * passiveSkill.GetDamageCount (target));
            }

            int damageGivenCountValue = 0;
            int damageGivenCountRatio = 0;
            if(effect.ContainsAnyArgs(SkillEffectLogicArgEnum.DamageGivenCountValue, SkillEffectLogicArgEnum.LevelupDamageGivenCountValue)) {
                int damageGivenCountPreValue = 0;
                int damageGivenCountMaxValue = int.MaxValue;
                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageGivenCountValue)) {
                    damageGivenCountPreValue += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageGivenCountValue);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageGivenCountValue)) {
                    damageGivenCountPreValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageGivenCountValue) * (level - 1);
                }

                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageGivenCountMaxValue)) {
                    damageGivenCountMaxValue += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageGivenCountMaxValue);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageGivenCountMaxValue)) {
                    damageGivenCountMaxValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageGivenCountMaxValue) * (level - 1);
                }
                damageGivenCountValue = Mathf.Min (damageGivenCountMaxValue, damageGivenCountPreValue * passiveSkill.GetDamageGivenCount(target));
            }

            if (effect.ContainsAnyArgs (SkillEffectLogicArgEnum.DamageGivenCountRatio, SkillEffectLogicArgEnum.LevelupDamageGivenCountRatio)) {
                int damageGivenCountPreRatio = 0;
                int damageGivenCountMaxRatio = int.MaxValue;
                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageGivenCountRatio)) {
                    damageGivenCountPreRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageGivenCountRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageGivenCountRatio)) {
                    damageGivenCountPreRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageGivenCountRatio) * (level - 1);
                }

                if (effect.ContainsArg (SkillEffectLogicArgEnum.DamageGivenCountMaxRatio)) {
                    damageGivenCountMaxRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.DamageGivenCountMaxRatio);
                }
                if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupDamageGivenCountMaxRatio)) {
                    damageGivenCountMaxRatio += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupDamageGivenCountMaxRatio) * (level - 1);
                }
                damageGivenCountRatio = Mathf.Min (damageGivenCountMaxRatio, damageGivenCountPreRatio * passiveSkill.GetDamageGivenCount(target));
            }




            return new ChangeParameterElement (addValue + hpDecreaseRatio + damageCountValue + damageGivenCountValue,
                ratioValue + hpDecreaseRatio + damageCountRatio + damageGivenCountRatio);
        }

        public static UniRx.Tuple<int, int> CalcEvaluationDamege(Parameter attacker, Parameter defender, int skillLevel, AttackElementEnum? attackElement, SkillEffect effect, bool isSpecial)
        {
            bool isMustHit = false;
            bool disableFormationRate = false;
            int addValue = 0;
            int ratioValue = 0;
            int attackCount = attacker.AttackCount;

            if (effect.ContainsArg (SkillEffectLogicArgEnum.Value)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Value);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupValue)) {
                addValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                ratioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.AttackCount)) {
                attackCount = effect.GetValue<int> (SkillEffectLogicArgEnum.AttackCount);
            }
            isMustHit = effect.ContainsArg (SkillEffectLogicArgEnum.MustHit);
            if (effect.ContainsArg (SkillEffectLogicArgEnum.DisableFormationRate)) {
                disableFormationRate = effect.GetValue<int> (SkillEffectLogicArgEnum.DisableFormationRate) != 0;
            }

            /*
             *  ダメージ
             *      Atc＝攻撃側攻撃力に±7.5％を４回(攻撃力の70％～130％をベルカーブに)
             *      Def＝防御側防御力に±7.5％を４回(防御力の70％～130％をベルカーブに)
             *      ダメージ=Atc-Def
             */
            var elementAffinity = GetElementAffinity (attacker.Element, defender.Element);

            var attackerRate = 1.0f;
            var defenderRate = 1.0f;
            if (!disableFormationRate) {
                var attackerFormationRate = GetFormationAttackRate (attacker);
                if (attackerFormationRate != null) {
                    if (!attackElement.HasValue || attackElement.Value == AttackElementEnum.Physics) {
                        attackerRate = (float)attackerFormationRate.physics_attack_rate / 100.0f;
                    } else if (attackElement.Value == AttackElementEnum.Physics) {
                        attackerRate = (float)attackerFormationRate.magic_attack_rate / 100.0f;
                    }
                }
                var defenderFormationRate = GetFormationAttackRate (defender);
                if (defenderFormationRate != null) {
                    defenderRate = (float)defenderFormationRate.defense_rate / 100.0f;
                }
            }

            int realAttack = isSpecial ? attacker.RealSpAttack : attacker.RealAttack;

            int attack = (
                realAttack + addValue +
                (realAttack * ratioValue / 100) +
                (realAttack * elementAffinity.correction_value / 100)
            );
            int defense = defender.RealDefense + defender.GuardAddValue +
                (defender.RealDefense * defender.GuardRatioValue / 100);

            float a1 = Mathf.Pow (attack, 0.95f) * attackerRate;
            float d1 = Mathf.Pow (defense, 0.95f) * defenderRate;

            int damage = Mathf.Max(0, defender.DecreaseDamage((int)(a1 - d1)));
            int hit = isMustHit ? 100 : 90 + attacker.RealHit - defender.RealEvasion;

            return new UniRx.Tuple<int, int>(damage, hit);
        }

        /// <summary>
        /// 特殊ダメージ計算
        /// </summary>
        /// <returns>The special damege.</returns>
        /// <param name="attacker">Attacker.</param>
        /// <param name="defender">Defender.</param>
        /// <param name="skill_level">Skill level.</param>
        /// <param name="effect">Effect.</param>
        public static UniRx.Tuple<int, int> CalcEvaluationSpecialDamege(Parameter attacker, Parameter defender, int skillLevel, AttackElementEnum? attackElement, SkillEffect effect)
        {
            bool isMustHit = false;

            int fixedValue = 0;
            int hpRatioValue = 0;
            int currentHpRatioValue = 0;

            if (effect.ContainsArg (SkillEffectLogicArgEnum.FixedValue)) {
                fixedValue += effect.GetValue<int> (SkillEffectLogicArgEnum.Value);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupFixedValue)) {
                fixedValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupValue) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.HPRatio)) {
                hpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.HPRatio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupHPRatio)) {
                hpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupHPRatio) * (skillLevel - 1);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.CurrentHPRatio)) {
                currentHpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.CurrentHPRatio);
            }
            if (effect.ContainsArg (SkillEffectLogicArgEnum.LevelupCurrentHPRatio)) {
                currentHpRatioValue += effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupCurrentHPRatio) * (skillLevel - 1);
            }
            isMustHit = effect.ContainsArg (SkillEffectLogicArgEnum.MustHit);

            int damage = fixedValue + defender.MaxHp * hpRatioValue / 100 + defender.Hp * currentHpRatioValue / 100;
            int hit = isMustHit ? 100 : 90 + attacker.RealHit - defender.RealEvasion;

            return new UniRx.Tuple<int, int>(damage, hit);
        }
    }
}