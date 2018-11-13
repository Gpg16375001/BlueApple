using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BattleLogic {
    /// <summary>
    /// バトル用武器パラメータ関連
    /// </summary>
    [Serializable]
    public class WeaponParameter : ISaveParameter<WeaponParameter.SaveParameter> {
        private int _weaponID;
        private int _level;
        private int _breakthroght;

        private Weapon _weapon;
        /// <summary> 武器マスターデータ </summary>
        public Weapon weapon {
            get {
                return _weapon;
            }
        }

        private int _hp;
        /// <summary> HP加算パラメータ </summary>
        public int Hp {
            get{ 
                return _hp;
            }
        }

        private int _attack;
        /// <summary> 攻撃加算パラメータ </summary>
        public int Attack {
            get{ 
                return _attack;
            }
        }

        private int _defense;
        /// <summary> 防御加算パラメータ </summary>
        public int Defense {
            get{ 
                return _defense;
            }
        }

        private int _agility;
        /// <summary> 素早さ加算パラメータ </summary>
        public int Agility {
            get{ 
                return _agility;
            }
        }

        private SkillParameter[] _actionSkillList;
        /// <summary> アクションスキル情報 </summary>
        public SkillParameter[] ActionSkillList {
            get{
                return _actionSkillList;
            }
        }
            
        private SkillParameter[] _passiveSkillList;
        /// <summary> パッシブスキル情報 </summary>
        public SkillParameter[] PassiveSkillList { 
            get {
                return _passiveSkillList;
            }
        }

        public WeaponParameter()
        {
        }

        public WeaponParameter(int weaponID, int level, int breakthroght)
        {
            _weaponID = weaponID;
            _level = level;
            _breakthroght = breakthroght;

            _weapon = MasterDataTable.weapon [_weaponID];

            double lerp = 0.0;
            // 最大レベル取得
            var maxLevel = MasterDataTable.weapon_level.GetMaxLevel(weapon.level_table_id);
            if (maxLevel == 0) {
                lerp = 1;
            } else {
                lerp = (double)(level - 1) / (double)(maxLevel - 1);
            }
            // パラメータ情報の生成
            _hp = (int)Math.Ceiling (
                weapon.initial_hp + (weapon.max_hp - weapon.initial_hp) * lerp
            );
            _attack = (int)Math.Ceiling (
                (double)weapon.initial_attack + (double)(weapon.max_attack - weapon.initial_attack) * lerp
            );
            _defense = (int)Math.Ceiling (
                (double)weapon.initial_defence + (double)(weapon.max_defence - weapon.initial_defence) * lerp
            );
            _agility = (int)Math.Ceiling (
                (double)weapon.initial_agility + (double)(weapon.max_agility - weapon.initial_agility) * lerp
            );

            // スキル情報の生成
            var skillList = MasterDataTable.weapon_skill.DataList.Where(
                x => x.weapon_id == weapon.id && MasterDataTable.skill[x.skill_id] != null
            );
            _actionSkillList = skillList.Where (x => MasterDataTable.skill [x.skill_id].skill_type == SkillTypeEnum.action).
                Select (x => new SkillParameter (breakthroght + 1, MasterDataTable.skill [x.skill_id], true)).ToArray();
            _passiveSkillList = skillList.Where (x => MasterDataTable.skill [x.skill_id].skill_type == SkillTypeEnum.passive).
                Select (x => new SkillParameter (breakthroght + 1, MasterDataTable.skill [x.skill_id], true)).ToArray();
        }

        [Serializable]
        public class SaveParameter
        {
            public int WeaponID;
            public int Level;
            public int Breakthroght;

            public SkillParameter.SaveParameter[] ActionSkillList;
        }
        SaveParameter _saveData = new SaveParameter();

        public bool IsModify()
        {
            return ActionSkillList.Any (x => x.IsModify ());
        }


        public WeaponParameter.SaveParameter CreateSaveData()
        {
            var saveData = _saveData;
            saveData.WeaponID = _weaponID;
            saveData.Level = _level;
            saveData.Breakthroght = _breakthroght;

            if (_actionSkillList != null) {
                saveData.ActionSkillList = new SkillParameter.SaveParameter[_actionSkillList.Length];
                for (int i = 0; i < _actionSkillList.Length; ++i) {
                    saveData.ActionSkillList [i] = (SkillParameter.SaveParameter)_actionSkillList [i].CreateSaveData ();
                }
            }
            return saveData;
        }

        public WeaponParameter.SaveParameter UpdateSaveData()
        {
            var saveData = _saveData;
            if (_actionSkillList != null) {
                for (int i = 0; i < _actionSkillList.Length; ++i) {
                    _actionSkillList [i].UpdateSaveData ();
                }
            }
            return saveData;
        }

        public void Load(WeaponParameter.SaveParameter saveData)
        {
            _weaponID = saveData.WeaponID;
            _level = saveData.Level;
            _breakthroght = saveData.Breakthroght;

            if (saveData.ActionSkillList != null) {
                _actionSkillList = new SkillParameter[saveData.ActionSkillList.Length];
                for (int i = 0; i < _actionSkillList.Length; ++i) {
                    _actionSkillList [i] = new SkillParameter ();
                    _actionSkillList [i].Load (saveData.ActionSkillList [i]);
                }
            } else {
                _actionSkillList = new SkillParameter[0];
            }

            _weapon = MasterDataTable.weapon [_weaponID];

            // 最大レベル取得
            var maxLevel = MasterDataTable.weapon_level.GetMaxLevel(weapon.level_table_id);

            // パラメータ情報の生成
            var lerp = (double)Mathf.Max(0, _level - 1) / (double)Mathf.Max(1, maxLevel - 1);
            _hp = (int)Math.Ceiling (
                weapon.initial_hp + (weapon.max_hp - weapon.initial_hp) * lerp
            );
            _attack = (int)Math.Ceiling (
                (double)weapon.initial_attack + (double)(weapon.max_attack - weapon.initial_attack) * lerp
            );
            _defense = (int)Math.Ceiling (
                (double)weapon.initial_defence + (double)(weapon.max_defence - weapon.initial_defence) * lerp
            );
            _agility = (int)Math.Ceiling (
                (double)weapon.initial_agility + (double)(weapon.max_agility - weapon.initial_agility) * lerp
            );

            // スキル情報の生成
            var skillList = MasterDataTable.weapon_skill.DataList.Where(
                x => x.weapon_id == weapon.id && MasterDataTable.skill[x.skill_id] != null
            );
            _passiveSkillList = skillList.Where (x => MasterDataTable.skill [x.skill_id].skill_type == SkillTypeEnum.passive).
                Select (x => new SkillParameter (_breakthroght + 1, MasterDataTable.skill [x.skill_id], true)).ToArray();
            _saveData = saveData;
        }

        public void Reversion(Parameter unit, WeaponParameter.SaveParameter saveData)
        {
        }
    }
}
