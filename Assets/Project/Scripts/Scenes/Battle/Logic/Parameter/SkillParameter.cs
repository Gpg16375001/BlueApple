using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleLogic {
    /// <summary>
    /// バトルのスキル用データ
    /// </summary>
    [Serializable]
    public class SkillParameter : ISaveParameter<SkillParameter.SaveParameter> {
        /// <summary> スキルレベル. </summary>
        public int Level {
            get {
                return m_Level;
            }
        }

        /// <summary> スキル情報. </summary>
        public Skill Skill {
            get {
                return m_Skill;
            }
        }

        /// <summary> 現在のチャージ数. </summary>
        public int ChargedTime {
            get {
                return m_ChargedTime;
            }
        }

        /// <summary> 使用できるまでの残りチャージタイム. </summary>
        public int RemainChargeTime {
            get {
                return Skill.charge_time - m_ChargedTime;
            }
        }

        public float ChargeProgress {
            get {
                if (Skill.charge_time > 0) {
                    return (float)m_ChargedTime / (float)Skill.charge_time;
                }
                return 1.0f;
            }
        }

        /// <summary> 使用可能か. </summary>
        public bool Enabled {
            get {
                return Skill.charge_time <= m_ChargedTime;
            }
        }

        /// <summary> アクションスキルか. </summary>
        public bool IsAction {
            get {
                return Skill.skill_type == SkillTypeEnum.action;
            }
        }

        /// <summary> 通常攻撃スキルか. </summary>
        public bool IsNormalAction {
            get {
                return Skill.is_normal_action;
            }
        }

        /// <summary> パッシブスキルか. </summary>
        public bool IsPassive {
            get {
                return Skill.skill_type == SkillTypeEnum.passive;
            }
        }

        /// <summary> スペシャルスキルか. </summary>
        public bool IsSpecial {
            get {
                return Skill.skill_type == SkillTypeEnum.special;
            }
        }

        /// <summary> 武器が所持しているスキルか. </summary>
        public bool IsWeaponSkill {
            get;
            private set;
        }

        public SkillParameter()
        {
        }

        public SkillParameter(int level, Skill skill, bool isWeaponSkill=false)
        {
            this.m_Level = level;
            this.m_Skill = skill;
            this.m_ChargedTime = 0;
            this.IsWeaponSkill = isWeaponSkill;
        }

        /// <summary>
        /// スキルを使用した時に必要な処理
        /// </summary>
        public void Use()
        {
            // 使用したら0にリセット
            m_ChargedTime = 0;
        }

        /// <summary>
        /// スキルのチャージタイム充填処理
        /// </summary>
        /// <param name="time">Time.</param>
        public void Charge(int time)
        {
            // チャージタイムより大きくならないようにしておく。
            m_ChargedTime = Mathf.Min(Skill.charge_time, m_ChargedTime + time);
        }

        public bool IsSatisfy(BattleAISetting aiSetting)
        {
            // スキル指定がある場合の処理
            if (aiSetting.action_selection.HasValue && aiSetting.action_selection.Value == BattleAIActionSelectionEnum.skill) {

                switch(aiSetting.action_selection.Value) {
                case BattleAIActionSelectionEnum.skill:
                    {
                        var skill = aiSetting.GetSelectionValue<Skill> ();
                        if (skill != null && Skill.id != skill.id) {
                            return false;
                        }
                    }
                    break;
                case BattleAIActionSelectionEnum.condition:
                    {
                        var condition = aiSetting.GetSelectionValue<Condition> ();
                        var conditions = Skill.SkillEffects.Where (x => x.skill_effect.IsConditionGrantingLogic() && x.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Condition)).
                        Select (x => x.skill_effect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition));
                        if (condition != null && (conditions.Count () == 0 || !conditions.Any (x => x.id == condition.id))) {
                            return false;
                        }
                    }
                    break;
                case BattleAIActionSelectionEnum.condition_group:
                    {
                        var conditionGroup = aiSetting.GetSelectionValue<ConditionGroup> ();
                        var conditions = Skill.SkillEffects.Where (x => x.skill_effect.IsConditionGrantingLogic() && x.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Condition)).
                        Select (x => x.skill_effect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition));
                        if (conditionGroup != null && (conditions.Count () == 0 || !conditions.Any (x => x.condition_group.HasValue && x.condition_group.Value == conditionGroup.id))) {
                            return false;
                        }
                    }
                    break;
                case BattleAIActionSelectionEnum.skill_type:
                    {
                        var skillType = aiSetting.GetSelectionValue<SkillTypeEnum> ();
                        if (skillType != SkillTypeEnum.none && Skill.skill_type != skillType) {
                            return false;
                        }
                    }
                    break;
                case BattleAIActionSelectionEnum.exclude_skill_type:
                    {
                        var skillType = aiSetting.GetSelectionValue<SkillTypeEnum> ();
                        if (skillType != SkillTypeEnum.none && Skill.skill_type == skillType) {
                            return false;
                        }
                    }
                    break;
                }
            }
            // 自分自身を選択の場合味方範囲も可能許可する
            var effects = Skill.SkillEffects.Where (x => x.range.target == aiSetting.target || (aiSetting.target == SkillTargetEnum.self && x.range.target == SkillTargetEnum.ally)).Select(x => x.skill_effect);

            return effects.Count() > 0 && effects.Any (x => x.IsSatisfy (aiSetting.action_type, aiSetting));
        }

        public IEnumerable<Condition> GetGradingCondition()
        {
            foreach (var effect in Skill.SkillEffects) {
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Condition)) {
                    yield return effect.skill_effect.GetValue<Condition> (SkillEffectLogicArgEnum.Condition);
                }
            }
        }

        /// <summary>
        /// AI設定に合致するtargetsが最低一体含まれるターゲットの候補を返す。
        /// </summary>
        /// <returns>The position list candidate.</returns>
        /// <param name="aiSetting">Ai setting.</param>
        /// <param name="my">My.</param>
        /// <param name="targets">Targets.</param>
        public IEnumerable<Parameter> GetTargetCandidate(BattleAISetting aiSetting, Parameter my, Parameter[] targets, bool targetRevesal)
        {
            var targetPositions = targets.Select (x => x.Position);
            var enemyList = AwsModule.BattleData.SallyEnemyParameterList;
            var allyList = AwsModule.BattleData.SallyAllyParameterList;
            if ((!my.Position.isPlayer && !targetRevesal) || (my.Position.isPlayer && targetRevesal)) {
                allyList = AwsModule.BattleData.SallyEnemyParameterList;
                enemyList = AwsModule.BattleData.SallyAllyParameterList;
            }
            if (aiSetting.target == SkillTargetEnum.enemy) {
                foreach (var target in enemyList) {
                    var positionList = Calculation.GetRangeInPositionList (my, target, null, this);
                    if(positionList.Any(x => targetPositions.Any(y => x == y))) {
                        yield return target;
                    }
                }
            } else if(aiSetting.target == SkillTargetEnum.ally || aiSetting.target == SkillTargetEnum.self) {
                foreach (var target in allyList) {
                    var positionList = Calculation.GetRangeInPositionList (my, null, target, this);
                    if(positionList.Any(x => targetPositions.Any(y => x == y))) {
                        yield return target;
                    }
                }
            }
        }



        public int GetItemDropUp()
        {
            int ret = 0;
            var effects = Skill.SkillEffects.Where (x => x.skill_effect.IsItemDropUp ());
            foreach (var effect in effects) {
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                    ret += effect.skill_effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
                }
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                    ret += effect.skill_effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (m_Level - 1);
                }
            }

            return ret;
        }

        public int GetExpUp()
        {
            int ret = 0;
            var effects = Skill.SkillEffects.Where (x => x.skill_effect.IsExpUp ());
            foreach (var effect in effects) {
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                    ret += effect.skill_effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
                }
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                    ret += effect.skill_effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (m_Level - 1);
                }
            }

            return ret;
        }

        public int GetMoneyUp()
        {
            int ret = 0;
            var effects = Skill.SkillEffects.Where (x => x.skill_effect.IsMoneyUp ());
            foreach (var effect in effects) {
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.Ratio)) {
                    ret += effect.skill_effect.GetValue<int> (SkillEffectLogicArgEnum.Ratio);
                }
                if (effect.skill_effect.ContainsArg (SkillEffectLogicArgEnum.LevelupRatio)) {
                    ret += effect.skill_effect.GetValue<int> (SkillEffectLogicArgEnum.LevelupRatio) * (m_Level - 1);
                }
            }

            return ret;
        }

        private int m_Level;

        private Skill m_Skill;

        private int m_ChargedTime;

        [Serializable]
        public class SaveParameter
        {
            public int Level;
            public int SkillId;
            public int ChargedTime;
            public bool IsWeaponSkill;
        }

        SaveParameter _saveData = new SaveParameter();

        public bool IsModify()
        {
            return m_ChargedTime != _saveData.ChargedTime;
        }

        public SkillParameter.SaveParameter CreateSaveData()
        {
            var saveData = _saveData;
            saveData.Level = m_Level;
            saveData.SkillId = m_Skill.id;
            saveData.ChargedTime = m_ChargedTime;
            saveData.IsWeaponSkill = IsWeaponSkill;

            return saveData;
        }

        public SkillParameter.SaveParameter UpdateSaveData()
        {
            var saveData = _saveData;
            saveData.ChargedTime = m_ChargedTime;

            return saveData;
        }

        public void Load(SkillParameter.SaveParameter saveData)
        {
            m_Level = saveData.Level;
            m_Skill = MasterDataTable.skill [saveData.SkillId];
            m_ChargedTime = saveData.ChargedTime;
            IsWeaponSkill = saveData.IsWeaponSkill;
            _saveData = saveData;
        }

        public void Reversion(Parameter unit, SkillParameter.SaveParameter saveData)
        {
        }
    }
        
    public class PassiveSkillParameter {
        public ConditionParameter Condition;
        public SkillEffect InvokeEffect;
        public SkillParameter ParentSkill;

        // 計算結果が状況によるもの
        // これは行動時ごとに計算し直す
        public bool HasSituationArg {
            get {
                return InvokeEffect.ContainsAnyArgs(
                    SkillEffectLogicArgEnum.RequirementAttackRange,
                    SkillEffectLogicArgEnum.RequirementTargetAttackRange,
                    SkillEffectLogicArgEnum.RequirementTargetBeloning,
                    SkillEffectLogicArgEnum.RequirementTargetElement,
                    SkillEffectLogicArgEnum.RequirementTargetFamily,
                    SkillEffectLogicArgEnum.RequirementTargetGender,
                    SkillEffectLogicArgEnum.RequirementTargetWeaponType,
                    SkillEffectLogicArgEnum.RequirementTargetAboveHpRatio,
                    SkillEffectLogicArgEnum.RequirementTargetFollowingHpRatio,
                    SkillEffectLogicArgEnum.TurnCountValue,
                    SkillEffectLogicArgEnum.LevelupTurnCountValue,
                    SkillEffectLogicArgEnum.TurnCountRatio,
                    SkillEffectLogicArgEnum.LevelupTurnCountRatio,
                    SkillEffectLogicArgEnum.RequirementAttackElementAffinity,
                    SkillEffectLogicArgEnum.RequirementDefenseElementAffinity,
                    SkillEffectLogicArgEnum.DamageCountValue,
                    SkillEffectLogicArgEnum.DamageCountRatio,
                    SkillEffectLogicArgEnum.LevelupDamageCountValue,
                    SkillEffectLogicArgEnum.LevelupDamageCountRatio,
                    SkillEffectLogicArgEnum.DamageGivenCountValue,
                    SkillEffectLogicArgEnum.DamageGivenCountRatio,
                    SkillEffectLogicArgEnum.LevelupDamageGivenCountValue,
                    SkillEffectLogicArgEnum.LevelupDamageGivenCountRatio
                );
            }
        }

        public bool HasHpLogic {
            get {
                return InvokeEffect.ContainsAnyArgs(
                    SkillEffectLogicArgEnum.HpDecreaseRatio,
                    SkillEffectLogicArgEnum.RequirementAboveHpRatio,
                    SkillEffectLogicArgEnum.RequirementFollowingHpRatio
                );
            }
        }

        public bool HasAutoTurnLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.auto_turn);
            }
        }

        public bool HasAutoWaveStartLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.auto_wavestart);
            }
        }

        public bool HasPerfectGuardLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.perfect_guard);
            }
        }

        public bool HasAbsoluteAvoidanceLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.absolute_avoidance);
            }
        }

        public bool HasGutsLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.guts);
            }
        }

        public bool HasForcedAILogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.forced_ai);
            }
        }

        public bool HasCounterLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.counter);
            }
        }

        public bool HasReverseDamageLogic {
            get {
                return InvokeEffect.IsEffectLogic (SkillEffectLogicEnum.reverse_damage);
            }
        }


        public PassiveSkillParameter(Parameter unit, ConditionParameter condition, SkillParameter parent, SkillEffect invoke)
        {
            Condition = condition;
            InvokeEffect = invoke;
            ParentSkill = parent;

        }

        public void AddExecuteCount()
        {
            if (Condition != null) {
                Condition.AddExecuteCount ();
            }
        }

        public bool EnableCondition()
        {
            if (Condition != null) {
                return Condition.IsEnable;
            }
            return true;
        }

        public int GetDamageCount(Parameter unit)
        {
            if (Condition != null) {
                return Condition.GetDamageCount (unit);
            }
            return unit.DamageCount;
        }

        public int GetDamageGivenCount(Parameter unit)
        {
            if (Condition != null) {
                return Condition.GetDamageGivenCount (unit);
            }
            return unit.DamageGivenCount;
        }
    }
}