using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleStageEnemy {

    public Skill[] ToUseSkills {
        get {
            return MasterDataTable.enemy_skill_setting.DataList.Where (x => x.group_id == skill_group_id).Select(x => x.skill).ToArray();
        }
    }
}
