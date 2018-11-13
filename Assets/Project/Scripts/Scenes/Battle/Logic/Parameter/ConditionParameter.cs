using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleLogic {
    [System.Serializable]
    public class ConditionParameter : ISaveParameter<ConditionParameter.SaveParameter> {
        public bool IsEnable {
            get {
                return (!m_ExistPriod || m_Remain > 0) && (!m_ExistExecuteLimit || m_ExcuteLimit > m_ExcuteCount) && (!m_ExistTurnLimit || m_PassageTurn < m_TurnLimit) && !m_CallingOff;
            }
        }

        public bool IsInvokeTiming {
            get {
                return m_ConditionData.invoke_timing > 0 && m_TimingSkillParameter != null;
            }
        }

        public SkillParameter TimingSkillParameter {
            get {
                return m_TimingSkillParameter;
            }
        }

        public bool HasDamegeLogic {
            get {
                if (m_TimingSkillParameter != null) {
                    return m_TimingSkillParameter.Skill.HasDamageLogic();
                }
                return false;
            }
        }

        public int ConditionGroup {
            get {
                if (m_ConditionData.condition_group.HasValue) {
                    return m_ConditionData.condition_group.Value;
                }
                return -1;
            }
        }

        public int ConditionID {
            get {
                return m_ConditionData.id;
            }
        }

        public Condition ConditionData {
            get {
                return m_ConditionData;
            }
        }

        public bool isBuff {
            get {
                return !m_ConditionData.is_debuff;
            }
        }

        public bool isDebuff {
            get {
                return m_ConditionData.is_debuff;
            }
        }

        public int NextTiming {
            get {
                return m_ConditionData.invoke_timing;
            }
        }

        public ConditionEffectTiming ConditionEffectItem {
            get {
                return m_ConditionEffectItem;
            }
        }

        public ConditionParameter()
        {
        }

        public ConditionParameter(Parameter unit, Condition condition)
        {
            InitParameter (unit, condition);
        }

        public void OverrideCondition(Parameter unit, Condition condition)
        {
            // グループ情報を持っていなければ無視
            if (!m_ConditionData.condition_group.HasValue || !condition.condition_group.HasValue) {
                return;
            }
            // グループが違えば無視
            if (m_ConditionData.condition_group.Value != condition.condition_group.Value) {
                return;
            }
            // 優先順位が高いものが入っていれば無視
            if (m_ConditionData.condition_group_priority > condition.condition_group_priority) {
                return;
            }
            // 同じ優先順位の場合は効果時間のみ初期化
            if (m_ConditionData.condition_group_priority == condition.condition_group_priority) {
                m_Remain = condition.period;
                m_ExcuteCount = 0;
                m_PassageTurn = 0;
                return;
            }
            InitParameter (unit, condition);
        }

        private void InitParameter(Parameter unit, Condition condition)
        {
            if (m_ConditionData != null) {
                RemoveWithInSkill (unit);
            }

            m_ConditionData = condition;

            m_ExistPriod = condition.period > 0;
            m_Remain = condition.period;

            m_ExcuteLimit = condition.invoke_count;
            m_ExistExecuteLimit = m_ExcuteLimit > 0;
            m_ExcuteCount = 0;

            m_TurnLimit = condition.turn_count;
            m_ExistTurnLimit = m_TurnLimit > 0;
            m_PassageTurn = 0;

            m_CallingOff = false;

            m_StartDamageCount = unit.DamageCount;
            m_StartDamageGivenCount = unit.DamageGivenCount;

            m_WithInSkillParameter = null;
            if (m_ConditionData.within_skill_id.HasValue) {
                var skill = MasterDataTable.skill [m_ConditionData.within_skill_id.Value];
                m_WithInSkillParameter = new SkillParameter (1, skill);
            }

            m_TimingSkillParameter = null;
            if (m_ConditionData.timing_skill_id.HasValue) {
                var skill = MasterDataTable.skill [m_ConditionData.timing_skill_id.Value];
                m_TimingSkillParameter = new SkillParameter (1, skill);
            }

            AddWithInSkill (unit);
        }

        public void Elapsed(int weight)
        {
            m_Remain -= weight;
        }

        public void Actioned()
        {
            m_PassageTurn++;
        }

        public void AddWithInSkill(Parameter unit)
        {
            if (m_WithInSkillParameter != null) {
                foreach (var skillEffectSetting in m_WithInSkillParameter.Skill.SkillEffects) {
                    unit.AddPassiveSkill (this, m_WithInSkillParameter, skillEffectSetting.skill_effect);
                }
            }
            if (IsInvokeTiming) {
                m_ConditionEffectItem = new ConditionEffectTiming (unit, this);
            }
        }

        public void RemoveWithInSkill(Parameter unit)
        {
            if (m_WithInSkillParameter != null) {
                unit.RemovePassiveSkill (m_WithInSkillParameter);
            }
        }

        public ActionResult TimingExexute()
        {
            ActionResult ret = new ActionResult ();
            ret.action = m_TimingSkillParameter;
            ret.invoker = null;
            ret.skillResult = SkillExecutor.Execute (m_ConditionData, m_ConditionEffectItem.unit, m_TimingSkillParameter);

            AddExecuteCount ();
            return ret;
        }

        public void AddExecuteCount()
        {
            m_ExcuteCount++;
        }

        public void ChangeWave()
        {
            // Wave持ち越ししないデータは削除する
            if (!m_ConditionData.is_carryover) {
                CallingOff ();
            }
        }
            
        public void CallingOff()
        {
            m_CallingOff = true;
        }

        public int GetDamageCount(Parameter unit)
        {
            return unit.DamageCount - m_StartDamageCount;
        }

        public int GetDamageGivenCount(Parameter unit)
        {
            return unit.DamageGivenCount - m_StartDamageGivenCount;
        }

        // 回数制限制御用
        bool m_ExistExecuteLimit;
        int m_ExcuteLimit;
        int m_ExcuteCount;

        // 期間制御用
        bool m_ExistPriod;
        int m_Remain;

        bool m_ExistTurnLimit;
        int m_TurnLimit;
        int m_PassageTurn;

        bool m_CallingOff;
        Condition m_ConditionData;
        SkillParameter m_WithInSkillParameter;
        SkillParameter m_TimingSkillParameter;

        // 付与時のダメージ回数を保存しておく
        int m_StartDamageCount;
        int m_StartDamageGivenCount;

        // タイムライン管理オブジェクト
        ConditionEffectTiming m_ConditionEffectItem;

        [System.Serializable]
        public class SaveParameter
        {
            public int ConditionId;
            public int ExcuteCount;
            public int Remain;
            public int PassageTurn;

            public int EffectItemWeight;

            public int StartDamageCount;
            public int StartDamageGivenCount;
        }
        SaveParameter _saveData = new SaveParameter();

        public bool IsModify()
        {
            return _saveData.ExcuteCount != m_ExcuteCount || _saveData.Remain != m_Remain || _saveData.PassageTurn != m_PassageTurn ||
                m_StartDamageCount != _saveData.StartDamageCount || m_StartDamageGivenCount != _saveData.StartDamageGivenCount ||
                (m_ConditionEffectItem != null && m_ConditionEffectItem.Weight != _saveData.EffectItemWeight);
        }

        public ConditionParameter.SaveParameter CreateSaveData()
        {
            var saveData = _saveData;
            saveData.ConditionId = m_ConditionData.id;
            saveData.ExcuteCount = m_ExcuteCount;
            saveData.Remain = m_Remain;
            saveData.PassageTurn = m_PassageTurn;
            saveData.StartDamageCount = m_StartDamageCount;
            saveData.StartDamageGivenCount = m_StartDamageGivenCount;
            if (IsInvokeTiming && m_ConditionEffectItem != null) {
                saveData.EffectItemWeight = m_ConditionEffectItem.Weight;
            }
            return saveData;
        }

        public ConditionParameter.SaveParameter UpdateSaveData()
        {
            return CreateSaveData();
        }

        public void Load(ConditionParameter.SaveParameter saveData)
        {
            m_ConditionData = MasterDataTable.Condition [saveData.ConditionId];
            if (m_ConditionData != null) {
                m_ExistPriod = m_ConditionData.period > 0;
                m_Remain = saveData.Remain;

                m_ExcuteLimit = m_ConditionData.invoke_count;
                m_ExistExecuteLimit = m_ExcuteLimit > 0;
                m_ExcuteCount = saveData.ExcuteCount;

                m_TurnLimit = m_ConditionData.turn_count;
                m_ExistTurnLimit = m_TurnLimit > 0;
                m_PassageTurn = saveData.PassageTurn;

                m_CallingOff = false;

                m_StartDamageCount = saveData.StartDamageCount;
                m_StartDamageGivenCount = saveData.StartDamageGivenCount;

                m_WithInSkillParameter = null;
                if (m_ConditionData.within_skill_id.HasValue) {
                    var skill = MasterDataTable.skill [m_ConditionData.within_skill_id.Value];
                    m_WithInSkillParameter = new SkillParameter (1, skill);
                }

                m_TimingSkillParameter = null;
                if (m_ConditionData.timing_skill_id.HasValue) {
                    var skill = MasterDataTable.skill [m_ConditionData.timing_skill_id.Value];
                    m_TimingSkillParameter = new SkillParameter (1, skill);
                }
            }
            _saveData = saveData;
        }

        public void Reversion(Parameter unit, ConditionParameter.SaveParameter saveData)
        {
            if (m_WithInSkillParameter != null) {
                foreach (var skillEffectSetting in m_WithInSkillParameter.Skill.SkillEffects) {
                    unit.AddPassiveSkill (this, m_WithInSkillParameter, skillEffectSetting.skill_effect);
                }
            }
            if (IsInvokeTiming) {
                m_ConditionEffectItem = new ConditionEffectTiming (unit, this, saveData.EffectItemWeight, false);
            }
        }
    }

    [System.Serializable]
    public class ConditionParameterList : ISaveParameter<ConditionParameterList.SaveParameter> {
        public bool HasCondition {
            get {
                return Conditions.Count > 0 && Conditions.Any(x => x.IsEnable);
            }
        }

        public bool HasDebuffCondition {
            get {
                return Conditions.Any (x => x.ConditionData.is_debuff);
            }
        }

        public int Count {
            get {
                return Conditions.Count;
            }
        }

        public ConditionParameter this[int index] {
            get {
                if (index < 0 || index >= Conditions.Count) {
                    return null;
                }
                return Conditions [index];
            }
        }

        public bool IsNotAction {
            get {
                return Conditions.Any(x => x.ConditionData.is_no_action);
            }
        }

        public int NotActionWait {
            get {
                if (IsNotAction) {
                    return Conditions.Where (x => x.ConditionData.is_no_action).Max (x => x.ConditionData.no_action_wait);
                }
                return 0;
            }
        }

        public ConditionParameterList()
        {
            Conditions.Clear ();
        }

        public void AddCondition(Parameter unit, Condition condition)
        {
            if (condition == null) {
                return;
            }

            int index = -1;
            if (condition.condition_group.HasValue) {
                int conditionGroup = condition.condition_group.Value;
                index = Conditions.FindIndex (x => x.ConditionGroup > 0 && x.ConditionGroup == conditionGroup);
            } else {
                // 同じ状態は上書き
                index = Conditions.FindIndex (x => x.ConditionID == condition.id);
            }

            if (index >= 0) {
                Conditions [index].OverrideCondition (unit, condition);
            } else {
                Conditions.Add (new ConditionParameter (unit, condition));
            }
            _isModify = true;
        }

        public void Elapsed(Parameter unit, int weight)
        {
            if (Conditions.Count <= 0) {
                return;
            }
            foreach (var condition in Conditions) {
                if (condition.IsEnable) {
                    condition.Elapsed (weight);
                }
            }
        }

        public void Actioned(Parameter unit)
        {
            foreach (var condition in Conditions) {
                if (condition.IsEnable) {
                    condition.Actioned();
                }
            }          
        }

        public void CallingOffAction(Parameter unit)
        {
            if (Conditions == null || Conditions.Count == 0) {
                return;
            }

            bool isCallingOff = false;
            foreach (var condition in Conditions.Where(x => x.ConditionData.action_calling_off_probability > 0).ToArray()) {
                if(Calculation.CallingOffJudgment(condition.ConditionData.action_calling_off_probability)) {
                    condition.CallingOff();
                    isCallingOff = true;
                }
            }

            if (isCallingOff) {
                RemoveDisableConditions (unit);
            }
        }

        public void CallingOffDamage(Parameter unit)
        {
            if (Conditions == null || Conditions.Count == 0) {
                return;
            }

            bool isCallingOff = false;
            foreach (var condition in Conditions.Where(x => x.ConditionData.damage_calling_off_probability > 0).ToArray()) {
                if(Calculation.CallingOffJudgment(condition.ConditionData.damage_calling_off_probability)) {
                    condition.CallingOff();
                    isCallingOff = true;
                }
            }

            if (isCallingOff) {
                RemoveDisableConditions (unit);
            }
        }

        /// <summary>
        /// 指定条件に沿う状態を削除
        /// </summary>
        /// <param name="conditionID">Condition I.</param>
        /// <param name="conditionGroup">Condition group.</param>
        /// <param name="isBuff">If set to <c>true</c> is buff.</param>
        /// <param name="isDebuff">If set to <c>true</c> is debuff.</param>
        public Condition[] RemoveConditions(Parameter unit, int? conditionID, int? conditionGroup, ConditionTypeEnum? type)
        {
            // 条件に沿うデータの取得
            var removeConditions = Conditions.Where (x => 
                (conditionID.HasValue && x.ConditionID == conditionID.Value) ||
                (conditionGroup.HasValue && x.ConditionGroup == conditionGroup.Value) ||
                (type.HasValue && type == x.ConditionData.condition_type)
            ).ToArray();

            bool isRemove = removeConditions.Count() > 0;
            if (isRemove) {
                // 状態を削除
                foreach (var removeCondition in removeConditions) {
                    removeCondition.RemoveWithInSkill (unit);
                    Conditions.Remove (removeCondition);
                }
                _isModify = true;
            }

            return removeConditions.Select (x => x.ConditionData).ToArray ();
        }

        public void WaveChange(Parameter unit)
        {
            // Wave持ち越ししないデータは削除する
            foreach (var condition in Conditions) {
                if (condition.IsEnable) {
                    condition.ChangeWave ();
                }
            }

            // 無効になった状態異常を削除する。
            RemoveDisableConditions (unit);
        }

        public bool ContainsConditions(Parameter unit, int? conditionID, int? conditionGroup, ConditionTypeEnum? type)
        {
            return Conditions.Any (x => 
                (conditionID.HasValue && x.ConditionID == conditionID.Value) ||
                (conditionGroup.HasValue && x.ConditionGroup == conditionGroup.Value) ||
                (type.HasValue && type == x.ConditionData.condition_type)
            );
        }

        public void RemoveDisableConditions(Parameter unit)
        {
            var removeConditions = Conditions.FindAll (x => !x.IsEnable);
            bool isRemove = removeConditions.Count > 0;
            if (isRemove) {
                foreach (var removeCondition in removeConditions) {
                    removeCondition.RemoveWithInSkill (unit);
                    Conditions.Remove (removeCondition);
                }
                _isModify = true;
            }
        }

        public IEnumerable<Condition> ConditionDataList() {
            if (Conditions != null && Conditions.Count > 0) {
                foreach (var condition in Conditions) {
                    if (condition.IsEnable) {
                        yield return condition.ConditionData;
                    }
                }
            }
        }

        public IEnumerable<ConditionEffectTiming> ConditionEffectItemList() {
            foreach (var condition in Conditions) {
                if(condition.IsEnable && condition.ConditionEffectItem != null) {
                    yield return condition.ConditionEffectItem;
                }
            }            
        }

        public bool ContainsCondition(Condition condition)
        {
            return Conditions.Any (x => x.ConditionID == condition.id);
        }

        public void AddWithInSkill(Parameter unit)
        {
            if (Conditions != null && Conditions.Count > 0) {
                foreach (var condition in Conditions) {
                    if (condition.IsEnable) {
                        condition.AddWithInSkill(unit);
                    }
                }
            }
        }
            
        private List<ConditionParameter> Conditions = new List<ConditionParameter> ();

        [System.Serializable]
        public class SaveParameter
        {
            public List<ConditionParameter.SaveParameter> ConditionSaveDatas;
        }

        SaveParameter _saveData = new SaveParameter();
        bool _isModify;
        public bool IsModify()
        {
            return _isModify || Conditions.Any (x => x.IsModify ());
        }

        public ConditionParameterList.SaveParameter CreateSaveData()
        {
            _isModify = false;
            var saveData = _saveData;
            saveData.ConditionSaveDatas = new List<ConditionParameter.SaveParameter>(); 
            for (int i = 0; i < Conditions.Count; ++i) {
                saveData.ConditionSaveDatas.Add(Conditions [i].CreateSaveData ());
            }
            return saveData;
        }

        public ConditionParameterList.SaveParameter UpdateSaveData()
        {
            _isModify = false;
            var saveData = _saveData;

            saveData.ConditionSaveDatas.Clear ();
            int count = Conditions.Count;
            for (int i = 0; i < Conditions.Count; ++i) {
                saveData.ConditionSaveDatas.Add(Conditions [i].UpdateSaveData ());
            }
            return saveData;
        }

        public void Load(ConditionParameterList.SaveParameter saveData)
        {
            for (int i = 0; i < saveData.ConditionSaveDatas.Count; ++i) {
                var condition = new ConditionParameter ();
                condition.Load (saveData.ConditionSaveDatas [i]);
                Conditions.Add (condition);
            }
            _saveData = saveData;
        }

        public void Reversion(Parameter unit, ConditionParameterList.SaveParameter saveData)
        {
            int count = Conditions.Count;
            for (int i = 0; i < count; ++i) {
                Conditions [i].Reversion (unit, saveData.ConditionSaveDatas[i]);
            }
        }
    }
}
