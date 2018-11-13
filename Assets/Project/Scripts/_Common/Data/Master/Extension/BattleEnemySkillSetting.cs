using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleEnemySkillSetting {

    public BattleLogic.SkillParameter CreatePlayerSkill()
    {
        return new BattleLogic.SkillParameter (skill_level, skill);
    }
}
