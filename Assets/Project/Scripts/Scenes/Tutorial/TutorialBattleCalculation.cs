using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleLogic
{   
	/// <summary>
    /// チュートリアルバトル関連計算
    /// </summary>
    public static class TutorialBattleCalculation
    {      
        /// <summary>
		/// スキルの実行.決められた結果(ActionResult)を予め指定する.
        /// </summary>
        /// <param name="invoker">発動者</param>
        /// <param name="target">ターゲット</param>
        /// <param name="action">発動スキル</param>
		public static ActionResult SimpleAction(List<Parameter> allyTargetList, List<Parameter> enemyTargetList, Parameter invoker, Parameter target, Parameter allyTarget, SkillParameter action, SkillEffectResultBase skillEffRes)
        {
			// 結果を決めうちしないのであれば通常フロー.
			if(skillEffRes == null){
				return Calculation.Action(invoker, target, allyTarget, action);
			}

            if (!action.IsAction && !action.IsSpecial) {
                throw new DoNotActionSkillException();
            } else if (action.IsAction && !action.Enabled) {
                throw new NotSkillChargedException();
            } else if (action.IsSpecial && !invoker.IsSpMax) {
                throw new NotMaxSpException();
            }

			ActionResult ret = new ActionResult();
            ret.invoker = invoker;
            ret.action = action;

            // スキル計算.
			var skillRes = new SkillExecutionResult();
			int effectCount = action.Skill.SkillEffects.Length;
            for (int i = 0; i < effectCount; i++) {
				var effect = action.Skill.SkillEffects[i];
				var receivers = GetReceiversFromList(allyTargetList, enemyTargetList, invoker, target, allyTarget, effect.range);
                var uniqReceivers = receivers.GroupBy(x => x);
                foreach (var uniqReceiver in uniqReceivers) {
                    var loopCount = uniqReceiver.Count();
                    var receiver = uniqReceiver.Key;
                    for (int exeCount = 0; exeCount < loopCount; ++exeCount) {                  
						if(skillEffRes.LogicEnum == effect.skill_effect.effect){
							skillRes.AddResult(receiver, skillEffRes);
						}
                    }
                }
            }
			ret.skillResult = skillRes;

			// 計算結果を反映する。         
			if(skillEffRes.LogicEnum == SkillEffectLogicEnum.damage){
				// チュートリアルなのでダメージ判定だけ.
				for (int i = 0; i < effectCount; i++) { 
					var effect = action.Skill.SkillEffects[i];
					if(effect.skill_effect.effect != SkillEffectLogicEnum.damage && effect.skill_effect.effect != SkillEffectLogicEnum.special_damage){
						continue;
					}
					Debug.Log((SkillRangeSettingEnum)effect.range.range_setting + " " + effect.range.target+" "+effect.skill_effect.effect);
					if(effect.range.IsAll){
						var targetList = invoker.Position.isPlayer ? enemyTargetList : allyTargetList;
						foreach(var t in targetList){
							Debug.Log(t.Name);
							var damageRes = skillEffRes as DamageSkillEffectResult;
                            var damageCount = damageRes.DamageInfos.Length;
                            for (int j = 0; j < damageCount; ++j) {
                                var damageInfo = damageRes.DamageInfos[j];
                                if (damageInfo.isHit) {
                                    t.Hp -= damageInfo.damage;
                                }
                            }
						}
					}else{
						var damageRes = skillEffRes as DamageSkillEffectResult;
                        var damageCount = damageRes.DamageInfos.Length;
                        for (int j = 0; j < damageCount; ++j) {
                            var damageInfo = damageRes.DamageInfos[j];
                            if (damageInfo.isHit) {
                                target.Hp -= damageInfo.damage;
                            }
                        }
					}
				}            
			}else if(skillEffRes.LogicEnum == SkillEffectLogicEnum.hp_heal){
				var healRes = skillEffRes as HealSkillEffectResult;
				allyTarget.Hp += healRes.HealValue;
			}

            return ret;
        }
  
        // スキルの範囲情報から対象ユニットを取得
        private static IEnumerable<Parameter> GetReceiversFromList(List<Parameter> allyTargetList, List<Parameter> enemyTargetList, Parameter invoker, Parameter target, Parameter allyTarget, SkillRange range)
        {
            // 敵味方のリストを取得
            List<Parameter> targetList = null;
            if (invoker.Position.isPlayer) {
                if (range.target == SkillTargetEnum.ally || range.target == SkillTargetEnum.self) {
                    targetList = allyTargetList;
                } else if (range.target == SkillTargetEnum.enemy) {
                    targetList = enemyTargetList;
                }
            } else {
                if (range.target == SkillTargetEnum.ally || range.target == SkillTargetEnum.self) {
                    targetList = enemyTargetList;
                } else if (range.target == SkillTargetEnum.enemy) {
                    targetList = allyTargetList;
                }
            }

            SkillElementTargetSetting elementSetting = range.element_target;
            // 属性指定
            if (elementSetting != null) {
                targetList = elementSetting.Extraction(targetList);
            }

            PositionData basePosition = null;
            if (range.target == SkillTargetEnum.self) {
                basePosition = invoker.Position;
            } else if (range.target == SkillTargetEnum.enemy && target != null) {
                basePosition = target.Position;
            } else if (range.target == SkillTargetEnum.ally && allyTarget != null) {
                basePosition = allyTarget.Position;
            }
            HashSet<Parameter> receivers = null;

            // 全体ロジック
            if (range.IsAll) {
                receivers = new HashSet<Parameter>(targetList);
            } else if (basePosition != null && targetList.Count > 0) {
                receivers = new HashSet<Parameter>();
                int maxColumn = targetList.Max(x => x.Position.column);
                int maxRow = targetList.Max(x => x.Position.row);
                int rangeDataCount = range.rangeSettings.Length;
                for (int i = 0; i < rangeDataCount; ++i) {
                    var rangeData = range.rangeSettings[i];
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

                    var receiver = targetList.FirstOrDefault(x => x.Position.Equals(x.Position.isPlayer, row, column));
                    if (receiver != null) {
                        receivers.Add(receiver);
                    }
                }
            } else {
                Debug.LogError(string.Format("Error: basePosition is null target: {0} allyTarget:{1}", target, allyTarget));
            }

            if (range.random_min_target > 0) {
                int maxTarget = Mathf.Max(range.random_min_target, range.random_max_target);
                if (!range.is_duplication_target && maxTarget > receivers.Count) {
                    return receivers;
                }

                List<Parameter> randomTargetList = new List<Parameter>();
                int targetCount = Calculation.RandomRange(range.random_min_target, maxTarget + 1);
                for (int i = 0; i < targetCount; ++i) {
                    var randomTarget = receivers.OrderBy((x) => Guid.NewGuid()).First();
                    if (!range.is_duplication_target) {
                        receivers.Remove(randomTarget);
                    }
                    randomTargetList.Add(randomTarget);
                }
                return randomTargetList;
            }
            return receivers;
        }

    }   
}