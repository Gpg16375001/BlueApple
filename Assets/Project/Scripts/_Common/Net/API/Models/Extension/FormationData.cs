using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmileLab.Net.API
{
    public partial class FormationData
    {
        /// <summary>
        /// 陣形マスター
        /// </summary>
        private Formation _formation;

        public Formation Formation {
            get {
                _formation = _formation ?? MasterDataTable.formation [FormationId];
                return _formation;
            }
        }

        /// <summary>
        /// 陣形の最大レベル
        /// </summary>
        private int? _maxLevel;

        public int MaxLevel {
            get {
                if (!_maxLevel.HasValue) {
                    _maxLevel = MasterDataTable.formation_level.GetMaxLevel (Formation.level_table_id);
                }
                return _maxLevel.Value;
            }
        }
    }
}
