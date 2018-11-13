using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class SkillEffect 
{
    private SkillEffectLogicArgEnum[] _skillEffectLogicArgs;
    public SkillEffectLogicArgEnum[] skillEffectLogicArgs {
        get {
            _skillEffectLogicArgs = _skillEffectLogicArgs ?? CreateLogicArgArray();
            return _skillEffectLogicArgs;
        }
    }

    SkillEffectLogicArgEnum[] CreateLogicArgArray()
    {
        List<SkillEffectLogicArgEnum> ret = new List<SkillEffectLogicArgEnum>();
        if (arg_name1 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name1);
        }
        if (arg_name2 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name2);
        }
        if (arg_name3 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name3);
        }
        if (arg_name4 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name4);
        }
        if (arg_name5 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name5);
        }
        if (arg_name6 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name6);
        }
        if (arg_name7 != SkillEffectLogicArgEnum.None) {
            ret.Add (arg_name7);
        }
        return ret.ToArray ();
    }

    string GetValueString(SkillEffectLogicArgEnum arg)
    {
        if (arg_name1 == arg) {
            return arg_value1;
        }
        else if (arg_name2 == arg) {
            return arg_value2;
        }
        else if (arg_name3 == arg) {
            return arg_value3;
        }
        else if (arg_name4 == arg) {
            return arg_value4;
        }
        else if (arg_name5 == arg) {
            return arg_value5;
        }
        else if (arg_name6 == arg) {
            return arg_value6;
        }
        else if (arg_name7 == arg) {
            return arg_value7;
        }
        return string.Empty;
    }

    public bool IsEffectLogic(params SkillEffectLogicEnum[] logics)
    {
        return logics.Contains(effect);
    }

    public bool IsDamageLogic()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.damage,
            SkillEffectLogicEnum.special_damage
        );
    }

    public bool IsHealLogic()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.hp_heal
        );
    }

    public bool IsSPChargeLogic()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.sp_charge
        );
    }

    public bool IsConditionGrantingLogic()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.damage,
            SkillEffectLogicEnum.special_damage,
            SkillEffectLogicEnum.condition_granting
        );
    }

    public bool IsConditionRemoveLogic()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.condition_remove
        );
    }

    public bool IsItemDropUp()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.item_drop_up
        );
    }

    public bool IsExpUp()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.exp_up
        );
    }

    public bool IsMoneyUp()
    {
        return IsEffectLogic (
            SkillEffectLogicEnum.money_up
        );
    }

    public bool IsBuffLogic(BattleAISetting aiSetting)
    {
        if (IsConditionGrantingLogic() && ContainsArg (SkillEffectLogicArgEnum.Condition)) {
            Condition targetCondition = null;
            ConditionGroup targetConditionGroup = null;
            if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition) {
                targetCondition = aiSetting.GetSelectionValue<Condition> ();
            } else if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition_group) {
                targetConditionGroup = aiSetting.GetSelectionValue<ConditionGroup> ();
            }
            var condition = GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
            return !condition.is_debuff && 
                (targetCondition == null || targetCondition.id == condition.id) &&
                (targetConditionGroup == null || (condition.condition_group.HasValue && targetConditionGroup.id == condition.condition_group.Value));
        }
        return false;
    }

    public bool IsDebuffLogic(BattleAISetting aiSetting)
    {
        if (IsConditionGrantingLogic() && ContainsArg (SkillEffectLogicArgEnum.Condition)) {
            Condition targetCondition = null;
            ConditionGroup targetConditionGroup = null;
            if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition) {
                targetCondition = aiSetting.GetSelectionValue<Condition> ();
            } else if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition_group) {
                targetConditionGroup = aiSetting.GetSelectionValue<ConditionGroup> ();
            }
            var condition = GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
            return condition.is_debuff && 
                (targetCondition == null || targetCondition.id == condition.id) &&
                (targetConditionGroup == null || (condition.condition_group.HasValue && targetConditionGroup.id == condition.condition_group.Value));
        }
        return false;
    }

    public bool IsCancellatioBuffLogic(BattleAISetting aiSetting)
    {
        if (IsConditionRemoveLogic()) {
            Condition targetCondition = null;
            ConditionGroup targetConditionGroup = null;
            if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition) {
                targetCondition = aiSetting.GetSelectionValue<Condition> ();
            } else if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition_group) {
                targetConditionGroup = aiSetting.GetSelectionValue<ConditionGroup> ();
            }

            if(ContainsArg (SkillEffectLogicArgEnum.BuffCondition)) {
                return GetValue<int> (SkillEffectLogicArgEnum.BuffCondition) == 1;
            }
            if(ContainsArg (SkillEffectLogicArgEnum.ConditionGroup)) {
                var conditionGroup = GetValue<ConditionGroup> (SkillEffectLogicArgEnum.ConditionGroup);
                return MasterDataTable.Condition.DataList.Any (x => x.condition_group.HasValue && x.condition_group.Value == conditionGroup.id && !x.is_debuff) && 
                    (targetConditionGroup == null || targetConditionGroup.id == conditionGroup.id) &&
                    (targetCondition == null || (targetCondition.condition_group.HasValue && conditionGroup.id == targetCondition.condition_group.Value));
            } 
            if (ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                var condition = GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                return !condition.is_debuff  && 
                    (targetCondition == null || targetCondition.id == condition.id) &&
                    (targetConditionGroup == null || (condition.condition_group.HasValue && targetConditionGroup.id == condition.condition_group.Value));
            }
        }
        return false;
    }

    public bool IsCancellatioDebuffLogic(BattleAISetting aiSetting)
    {
        if (IsConditionRemoveLogic()) {
            Condition targetCondition = null;
            ConditionGroup targetConditionGroup = null;
            if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition) {
                targetCondition = aiSetting.GetSelectionValue<Condition> ();
            } else if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.condition_group) {
                targetConditionGroup = aiSetting.GetSelectionValue<ConditionGroup> ();
            }
            if(ContainsArg (SkillEffectLogicArgEnum.DebuffCondition)) {
                return GetValue<int> (SkillEffectLogicArgEnum.DebuffCondition) == 1;
            }
            if(ContainsArg (SkillEffectLogicArgEnum.ConditionGroup)) {
                var conditionGroup = GetValue<ConditionGroup> (SkillEffectLogicArgEnum.ConditionGroup);
                return MasterDataTable.Condition.DataList.Any (x => x.condition_group.HasValue && x.condition_group.Value == conditionGroup.id && x.is_debuff) && 
                    (targetConditionGroup == null || targetConditionGroup.id == conditionGroup.id) &&
                    (targetCondition == null || (targetCondition.condition_group.HasValue && conditionGroup.id == targetCondition.condition_group.Value));
            }
            if (ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                var condition = GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                return condition.is_debuff && 
                    (targetCondition == null || targetCondition.id == condition.id) &&
                    (targetConditionGroup == null || (condition.condition_group.HasValue && targetConditionGroup.id == condition.condition_group.Value));
            }
        }
        return false;
    }

    public bool IsSatisfy(BattleAIActionTypeEnum type, BattleAISetting aiSetting)
    {
        switch (type) {
        case BattleAIActionTypeEnum.Attack:
            return IsDamageLogic();
        case BattleAIActionTypeEnum.Heal:
            return IsHealLogic();
        case BattleAIActionTypeEnum.SPCharge:
            return IsSPChargeLogic();
        case BattleAIActionTypeEnum.Buff:
            return IsBuffLogic(aiSetting);
        case BattleAIActionTypeEnum.Debuff:
            return IsDebuffLogic(aiSetting);
        case BattleAIActionTypeEnum.CancellationBuff:
            return IsCancellatioBuffLogic(aiSetting);
        case BattleAIActionTypeEnum.CancellationDebuff:
            return IsCancellatioDebuffLogic(aiSetting);
        }
        return false;
    }

    /// <summary>
    /// ロジック引数が存在するかの判定
    /// </summary>
    /// <returns><c>true</c>, if argument was containsed, <c>false</c> otherwise.</returns>
    /// <param name="arg">Argument.</param>
    public bool ContainsArg(SkillEffectLogicArgEnum arg)
    {
        return skillEffectLogicArgs.Contains (arg);
    }


    /// <summary>
    /// 複数のロジック引数のどれかが存在するかの判定
    /// </summary>
    /// <returns><c>true</c>, if arguments was containsed, <c>false</c> otherwise.</returns>
    /// <param name="args">Arguments.</param>
    public bool ContainsAnyArgs(params SkillEffectLogicArgEnum[] args)
    {
        return skillEffectLogicArgs.Intersect (args).Count() > 0;
    }

    /// <summary>
    /// 複数のロジックの全てか存在するかの判定
    /// </summary>
    /// <returns><c>true</c>, if all arguments was containsed, <c>false</c> otherwise.</returns>
    /// <param name="args">Arguments.</param>
    public bool ContainsAllArgs(params SkillEffectLogicArgEnum[] args)
    {
        return skillEffectLogicArgs.Intersect (args).Count() >= args.Length;
    }

    public bool IsMeetRequirements(BattleLogic.Parameter my, bool isAttaker = false, BattleLogic.SkillParameter action = null, BattleLogic.Parameter target = null)
    {
        if (my == null) {
            return false;
        }

        // 判定用のパラメータを全く持たない場合はtrue
        if(
            !ContainsAnyArgs (
                SkillEffectLogicArgEnum.RequirementAttackRange,
                SkillEffectLogicArgEnum.RequirementBeloning,
                SkillEffectLogicArgEnum.RequirementElement,
                SkillEffectLogicArgEnum.RequirementFamily,
                SkillEffectLogicArgEnum.RequirementGender,
                SkillEffectLogicArgEnum.RequirementWeaponType,
                SkillEffectLogicArgEnum.RequirementTargetAttackRange,
                SkillEffectLogicArgEnum.RequirementTargetBeloning,
                SkillEffectLogicArgEnum.RequirementTargetElement,
                SkillEffectLogicArgEnum.RequirementTargetFamily,
                SkillEffectLogicArgEnum.RequirementTargetGender,
                SkillEffectLogicArgEnum.RequirementTargetWeaponType,
                SkillEffectLogicArgEnum.RequirementAboveHpRatio,
                SkillEffectLogicArgEnum.RequirementFollowingHpRatio,
                SkillEffectLogicArgEnum.RequirementTargetAboveHpRatio,
                SkillEffectLogicArgEnum.RequirementTargetFollowingHpRatio,
                SkillEffectLogicArgEnum.RequirementAttackElementAffinity,
                SkillEffectLogicArgEnum.RequirementDefenseElementAffinity
            )
        ) {
            return true;
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementAttackRange)) {
            if (action == null) {
                // 判定できないのでfalse
                return false;
            } else if(isAttaker) {
                // 自身が攻撃者の場合に判定
                var attackRange = GetValue<AttackRange> (SkillEffectLogicArgEnum.RequirementAttackRange);
                var myAttackRange = action.Skill.attack_range != AttackRangeEnum.None ? action.Skill.attack_range : 
                    my.Weapon != null ? my.Weapon.weapon.MotionType.attack_range : AttackRangeEnum.None;
                if(attackRange.Enum != myAttackRange) {
                    return false;
                }
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetAttackRange)) {
            if (action == null || target == null) {
                // 判定できないのでfalse
                return false;
            } else if(!isAttaker) {
                // ターゲットが攻撃者の場合に判定
                var attackRange = GetValue<AttackRange> (SkillEffectLogicArgEnum.RequirementTargetAttackRange);
                var targetAttackRange = action.Skill.attack_range != AttackRangeEnum.None ? action.Skill.attack_range : 
                    target.Weapon != null ? target.Weapon.weapon.MotionType.attack_range : AttackRangeEnum.None;
                if(attackRange.Enum != targetAttackRange) {
                    return false;
                }
            }
        }

        if (ContainsAnyArgs (
               SkillEffectLogicArgEnum.RequirementTargetBeloning,
               SkillEffectLogicArgEnum.RequirementTargetElement,
               SkillEffectLogicArgEnum.RequirementTargetFamily,
               SkillEffectLogicArgEnum.RequirementTargetGender,
               SkillEffectLogicArgEnum.RequirementTargetWeaponType,
               SkillEffectLogicArgEnum.RequirementTargetAboveHpRatio,
               SkillEffectLogicArgEnum.RequirementTargetFollowingHpRatio,
               SkillEffectLogicArgEnum.RequirementAttackElementAffinity,
               SkillEffectLogicArgEnum.RequirementDefenseElementAffinity
           )
        ) {
            if (target == null) {
                // 判定できないのでfalse
                return false;
            } else {
                // 条件判定
                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetBeloning)) {
                    var beloning = GetValue<Belonging> (SkillEffectLogicArgEnum.RequirementTargetBeloning);
                    if (beloning.Enum != target.belonging.Enum) {
                        return false;
                    }
                }

                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetElement)) {
                    var element = GetValue<Element> (SkillEffectLogicArgEnum.RequirementTargetElement);
                    if (element.Enum != target.Element.Enum) {
                        return false;
                    }
                }

                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetFamily)) {
                    var family = GetValue<Family> (SkillEffectLogicArgEnum.RequirementTargetFamily);
                    if (family.Enum != target.family.Enum) {
                        return false;
                    }
                }

                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetGender)) {
                    var gender = GetValue<Gender> (SkillEffectLogicArgEnum.RequirementTargetGender);
                    if (gender.Enum != target.gender.Enum) {
                        return false;
                    }
                }

                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetWeaponType)) {
                    var weaponType = GetValue<WeaponType> (SkillEffectLogicArgEnum.RequirementTargetWeaponType);
                    if (
                        target.Weapon == null ||
                        weaponType.index != target.Weapon.weapon.type
                    ) {
                        return false;
                    }
                }

                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetAboveHpRatio)) {
                    var ratio = GetValue<int> (SkillEffectLogicArgEnum.RequirementTargetAboveHpRatio);
                    if (target.HpProgress * 100.0f < (float)ratio) {
                        return false;
                    }
                }

                if (ContainsArg (SkillEffectLogicArgEnum.RequirementTargetFollowingHpRatio)) {
                    var ratio = GetValue<int> (SkillEffectLogicArgEnum.RequirementTargetFollowingHpRatio);
                    if (target.HpProgress * 100.0f > (float)ratio) {
                        return false;
                    }
                }

                if (isAttaker) {
                    if (ContainsArg (SkillEffectLogicArgEnum.RequirementAttackElementAffinity)) {
                        var affinity = BattleLogic.Calculation.GetElementAffinity (my.Element, target.Element);
                        var condition = GetValue<ElementAffinity> (SkillEffectLogicArgEnum.RequirementAttackElementAffinity);
                        if (condition != null && condition.Enum != affinity.Enum) {
                            return false;
                        }
                    } else if (ContainsArg (SkillEffectLogicArgEnum.RequirementDefenseElementAffinity)) {
                        return false;
                    }
                } else {
                    if (ContainsArg (SkillEffectLogicArgEnum.RequirementDefenseElementAffinity)) {
                        var affinity = BattleLogic.Calculation.GetElementAffinity (target.Element, my.Element);
                        var condition = GetValue<ElementAffinity> (SkillEffectLogicArgEnum.RequirementDefenseElementAffinity);
                        if (condition != null && condition.Enum != affinity.Enum) {
                            return false;
                        }
                    } else if (ContainsArg (SkillEffectLogicArgEnum.RequirementAttackElementAffinity)) {
                        return false;
                    }
                }
            }
        }

        // 条件判定
        if (ContainsArg (SkillEffectLogicArgEnum.RequirementBeloning)) {
            var beloning = GetValue<Belonging> (SkillEffectLogicArgEnum.RequirementBeloning);
            if (beloning.Enum != my.belonging.Enum) {
                return false;
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementElement)) {
            var element = GetValue<Element> (SkillEffectLogicArgEnum.RequirementElement);
            if (element.Enum != my.Element.Enum) {
                return false;
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementFamily)) {
            var family = GetValue<Family> (SkillEffectLogicArgEnum.RequirementFamily);
            if (family.Enum != my.family.Enum) {
                return false;
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementGender)) {
            var gender = GetValue<Gender> (SkillEffectLogicArgEnum.RequirementGender);
            if (gender.Enum != my.gender.Enum) {
                return false;
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementWeaponType)) {
            var weaponType = GetValue<WeaponType> (SkillEffectLogicArgEnum.RequirementWeaponType);
            if (
                my.Weapon == null ||
                weaponType.index != my.Weapon.weapon.type
            ) {
                return false;
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementAboveHpRatio)) {
            var ratio = GetValue<int> (SkillEffectLogicArgEnum.RequirementAboveHpRatio);
            if (my.HpProgress * 100.0f < (float)ratio) {
                return false;
            }
        }

        if (ContainsArg (SkillEffectLogicArgEnum.RequirementFollowingHpRatio)) {
            var ratio = GetValue<int> (SkillEffectLogicArgEnum.RequirementFollowingHpRatio);
            if (my.HpProgress * 100.0f > (float)ratio) {
                return false;
            }
        }


        return true;
    }


    private Dictionary<SkillEffectLogicArgEnum, object> _skillEffectLogicArgValueCache;
    public T GetValue<T>(SkillEffectLogicArgEnum arg)
    {
        if (!skillEffectLogicArgs.Contains(arg)) {
#if DEBUG
            UnityEngine.Debug.LogError(string.Format("{0}は引数内に存在しません。", arg.ToString()));
#endif
            return default(T);

        }
        if (_skillEffectLogicArgValueCache == null || !_skillEffectLogicArgValueCache.ContainsKey (arg)) {
            _skillEffectLogicArgValueCache = _skillEffectLogicArgValueCache ?? new Dictionary<SkillEffectLogicArgEnum, object> ();
            _skillEffectLogicArgValueCache.Add (arg, convertFuncs [arg] (GetValueString (arg)));
        }
        return (T)_skillEffectLogicArgValueCache[arg];
    }

    static Dictionary<SkillEffectLogicArgEnum, Func<string, object>> convertFuncs = new Dictionary<SkillEffectLogicArgEnum, Func<string, object>>() {
        {SkillEffectLogicArgEnum.Value, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupValue, ConvertInt},
        {SkillEffectLogicArgEnum.Ratio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupRatio, ConvertInt},
        {SkillEffectLogicArgEnum.AttackCount, ConvertInt},
        {SkillEffectLogicArgEnum.AttackElement, ConvertElement},
        {SkillEffectLogicArgEnum.RequirementElement, ConvertElement},
        {SkillEffectLogicArgEnum.RequirementBeloning, ConvertBeloning},
        {SkillEffectLogicArgEnum.RequirementFamily, ConvertFamily},
        {SkillEffectLogicArgEnum.RequirementGender, ConvertGender},
        {SkillEffectLogicArgEnum.RequirementAttackRange, ConvertAttackRange},
        {SkillEffectLogicArgEnum.RequirementWeaponType, ConvertWeaponType},
        {SkillEffectLogicArgEnum.RequirementTargetElement, ConvertElement},
        {SkillEffectLogicArgEnum.RequirementTargetBeloning, ConvertBeloning},
        {SkillEffectLogicArgEnum.RequirementTargetFamily, ConvertFamily},
        {SkillEffectLogicArgEnum.RequirementTargetGender, ConvertGender},
        {SkillEffectLogicArgEnum.RequirementTargetAttackRange, ConvertAttackRange},
        {SkillEffectLogicArgEnum.RequirementTargetWeaponType, ConvertWeaponType},
        {SkillEffectLogicArgEnum.FixedValue, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupFixedValue, ConvertInt},
        {SkillEffectLogicArgEnum.HPRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupHPRatio, ConvertInt},
        {SkillEffectLogicArgEnum.TargetParameter, ConvertSkillTargetParameter},
        {SkillEffectLogicArgEnum.Probability, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupProbability, ConvertInt},
        {SkillEffectLogicArgEnum.Condition, ConvertCondition},
        {SkillEffectLogicArgEnum.ConditionGroup, ConvertConditionGroup},
        {SkillEffectLogicArgEnum.ConditionType, ConvertConditionType},
        {SkillEffectLogicArgEnum.BuffCondition, ConvertInt},
        {SkillEffectLogicArgEnum.DebuffCondition, ConvertInt},
        {SkillEffectLogicArgEnum.MustHit, ConvertInt},
        {SkillEffectLogicArgEnum.DisableFormationRate, ConvertInt},
        {SkillEffectLogicArgEnum.DefenseIgnored, ConvertInt},
        {SkillEffectLogicArgEnum.EffectPeriod, ConvertInt},
        {SkillEffectLogicArgEnum.EffectTiming, ConvertInt},
        {SkillEffectLogicArgEnum.EffectCount, ConvertInt},
        {SkillEffectLogicArgEnum.Skill, ConvertSkill},
        {SkillEffectLogicArgEnum.HpDecreaseRatio, ConvertInt},
        {SkillEffectLogicArgEnum.ValueOfChangeEveryDecreaseRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupValueOfChangeEveryDecreaseRatio, ConvertInt},
        {SkillEffectLogicArgEnum.RatioOfChangeEveryDecreaseRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupRatioOfChangeEveryDecreaseRatio, ConvertInt},
        {SkillEffectLogicArgEnum.RequirementAboveHpRatio, ConvertInt},
        {SkillEffectLogicArgEnum.RequirementFollowingHpRatio, ConvertInt},
        {SkillEffectLogicArgEnum.RequirementTargetAboveHpRatio, ConvertInt},
        {SkillEffectLogicArgEnum.RequirementTargetFollowingHpRatio, ConvertInt},
        {SkillEffectLogicArgEnum.DamageAbsorption, ConvertInt},
        {SkillEffectLogicArgEnum.HitCorrection, ConvertInt},
        {SkillEffectLogicArgEnum.MustCritical, ConvertInt},
        {SkillEffectLogicArgEnum.AddWeight, ConvertInt},
        {SkillEffectLogicArgEnum.MustConditionGrand, ConvertInt},
        {SkillEffectLogicArgEnum.TurnCountValue, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupTurnCountValue, ConvertInt},
        {SkillEffectLogicArgEnum.TurnCountRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupTurnCountRatio, ConvertInt},
        {SkillEffectLogicArgEnum.AssignAI, ConvertBattleAIDefine},
        {SkillEffectLogicArgEnum.CurrentHPRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupCurrentHPRatio, ConvertInt},
        {SkillEffectLogicArgEnum.DeadInvoke, ConvertInt},
        {SkillEffectLogicArgEnum.DamageCountValue, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageCountValue, ConvertInt},
        {SkillEffectLogicArgEnum.DamageCountRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageCountRatio, ConvertInt},
        {SkillEffectLogicArgEnum.RequirementAttackElementAffinity, ConvertElementAffinity},
        {SkillEffectLogicArgEnum.RequirementDefenseElementAffinity, ConvertElementAffinity},
        {SkillEffectLogicArgEnum.DamageCountMaxValue, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageCountMaxValue, ConvertInt},
        {SkillEffectLogicArgEnum.DamageCountMaxRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageCountMaxRatio, ConvertInt},
        {SkillEffectLogicArgEnum.DamageGivenCountValue, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageGivenCountValue, ConvertInt},
        {SkillEffectLogicArgEnum.DamageGivenCountRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageGivenCountRatio, ConvertInt},
        {SkillEffectLogicArgEnum.DamageGivenCountMaxValue, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageGivenCountMaxValue, ConvertInt},
        {SkillEffectLogicArgEnum.DamageGivenCountMaxRatio, ConvertInt},
        {SkillEffectLogicArgEnum.LevelupDamageGivenCountMaxRatio, ConvertInt},
        {SkillEffectLogicArgEnum.IgnorePassiveEffect, ConvertInt},
    };

    static object ConvertInt(string value)
    {
        int ret = (int)Double.Parse (value);
        return (object)ret;
    }

    static object ConvertElement(string value)
    {
        Element ret = MasterDataTable.element.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertBeloning(string value)
    {
        Belonging ret = MasterDataTable.belonging.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertFamily(string value)
    {
        Family ret = MasterDataTable.family.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertGender(string value)
    {
        Gender ret = MasterDataTable.gender.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertSkillTargetParameter(string value)
    {
        SkillTargetParameter ret = MasterDataTable.skill_target_parameter.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertCondition(string value)
    {
        Condition ret = MasterDataTable.Condition.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertConditionGroup(string value)
    {
        ConditionGroup ret = MasterDataTable.condition_group.DataList.FirstOrDefault (x => x.group_name == value);
        return ret;
    }

    static object ConvertConditionType(string value)
    {
        ConditionType ret = MasterDataTable.condition_type.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertAttackRange(string value)
    {
        AttackRange ret = MasterDataTable.attack_range.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertWeaponType(string value)
    {
        WeaponType ret = MasterDataTable.weapon_type.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertSkill(string value)
    {
        int skill_id = 0;
        if (int.TryParse (value, out skill_id)) {
            return MasterDataTable.skill.DataList.FirstOrDefault (x => x.id == skill_id);
        }
        Skill ret = MasterDataTable.skill.DataList.FirstOrDefault (x => x.display_name == value);
        return ret;
    }

    static object ConvertBattleAIDefine(string value)
    {
        int ai_id = 0;
        if (int.TryParse (value, out ai_id)) {
            return MasterDataTable.battle_ai_define [ai_id];
        }
        BattleAIDefine ret = MasterDataTable.battle_ai_define.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }

    static object ConvertElementAffinity(string value)
    {
        ElementAffinity ret = MasterDataTable.element_affinity.DataList.FirstOrDefault (x => x.name == value);
        return ret;
    }
}
