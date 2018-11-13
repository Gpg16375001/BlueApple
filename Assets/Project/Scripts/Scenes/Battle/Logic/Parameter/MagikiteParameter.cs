using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SmileLab.Net.API;

namespace BattleLogic {
    /// <summary>
    /// マギカイトのパラメータ
    /// </summary>
    public class MagikiteParameter : ISaveParameter<MagikiteParameter.SaveParameter> {

        private int MagikiteId;

        private int HPGain;
        /// <summary>
        /// The hp.
        /// サーバーから受け取ったデータをそのまま使う
        /// </summary>
        public int Hp {
            get {
                return HPGain;
            }
        }
            
        private int ATKGain;
        /// <summary>
        /// The attack.
        /// サーバーから受け取ったデータをそのまま使う
        /// </summary>
        public int Attack {
            get {
                return ATKGain;
            }
        }
         
        private int DEFGain;
        /// <summary>
        /// The defense.
        /// サーバーから受け取ったデータをそのまま使う
        /// </summary>
        public int Defense {
            get {
                return DEFGain;
            }
        }

        private int SPDGain;
        /// <summary>
        /// The agility.
        /// サーバーから受け取ったデータをそのまま使う
        /// </summary>
        public int Agility {
            get {
                return SPDGain;
            }
        }

        private SkillParameter[] _passiveSkillList;
        /// <summary> パッシブスキル情報 </summary>
        public SkillParameter[] PassiveSkillList { 
            get {
                return _passiveSkillList;
            }
        }

        public MagikiteParameter()
        {
            HPGain = 0;
            ATKGain = 0;
            DEFGain = 0;
            SPDGain = 0;
            _passiveSkillList = new SkillParameter[0];
        }

        public MagikiteParameter(long magikiteDataBagID)
        {
            var magikiteData = MagikiteData.CacheGet (magikiteDataBagID);
            Init (magikiteData);
        }

        public MagikiteParameter(MagikiteData magikiteData)
        {
            Init (magikiteData);
        }

        private void Init(MagikiteData magikiteData)
        {
            HPGain = magikiteData.HPGain;
            ATKGain = magikiteData.ATKGain;
            DEFGain = magikiteData.DEFGain;
            SPDGain = magikiteData.SPDGain;
            MagikiteId = magikiteData.MagikiteId;
            var magikiteMasterData = MasterDataTable.magikite [MagikiteId];
            // スキルデータの受け渡し
            if (magikiteMasterData != null && magikiteMasterData.skill_id.HasValue) {
                _passiveSkillList = new SkillParameter[1];
                _passiveSkillList [0] = new SkillParameter(magikiteMasterData.skill_level, MasterDataTable.skill [magikiteMasterData.skill_id.Value], true);
            } else {
                _passiveSkillList = new SkillParameter[0];
            }
        }

        [Serializable]
        public struct SaveParameter
        {
            public int MagikiteId;
            public int HPGain;
            public int ATKGain;
            public int DEFGain;
            public int SPDGain;
        }

        public bool IsModify()
        {
            return false;
        }

        public MagikiteParameter.SaveParameter CreateSaveData()
        {
            var saveData = new SaveParameter ();
            saveData.MagikiteId = MagikiteId;
            saveData.HPGain = HPGain;
            saveData.ATKGain = ATKGain;
            saveData.DEFGain = DEFGain;
            saveData.SPDGain = SPDGain;
            return saveData;
        }

        public MagikiteParameter.SaveParameter UpdateSaveData()
        {
            return CreateSaveData();
        }

        public void Load(MagikiteParameter.SaveParameter saveData)
        {
            HPGain = saveData.HPGain;
            ATKGain = saveData.ATKGain;
            DEFGain = saveData.DEFGain;
            SPDGain = saveData.SPDGain;
            var magikiteMasterData = MasterDataTable.magikite [saveData.MagikiteId];
            // スキルデータの受け渡し
            if (magikiteMasterData != null && magikiteMasterData.skill_id.HasValue) {
                _passiveSkillList = new SkillParameter[1];
                _passiveSkillList [0] = new SkillParameter(magikiteMasterData.skill_level, MasterDataTable.skill [magikiteMasterData.skill_id.Value], true);
            } else {
                _passiveSkillList = new SkillParameter[0];
            }
        }

        public void Reversion(Parameter unit, MagikiteParameter.SaveParameter saveData)
        {
        }
    }
}