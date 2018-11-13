using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public partial class BattleAISetting {
    public T GetSelectionValue<T>()
    {
        if (action_selection.HasValue && convertSelectionValueFuncs.ContainsKey(action_selection.Value)) {
            return (T)convertSelectionValueFuncs [action_selection.Value] (action_selection_arg);
        }
        return default(T);
    }

    public T GetConditionArg<T>()
    {
        if (condition.HasValue && convertConditionValueFuncs.ContainsKey(condition.Value)) {
            return (T)convertConditionValueFuncs [condition.Value] (condition_arg);
        }
        return default(T);
    }

    static Dictionary<BattleAITargetConditionEnum, Func<string, object>> convertConditionValueFuncs = new Dictionary<BattleAITargetConditionEnum, Func<string, object>>() {
        {BattleAITargetConditionEnum.HpMinValue, ConvertInt},
    };

    static Dictionary<BattleAIActionSelectionEnum, Func<string, object>> convertSelectionValueFuncs = new Dictionary<BattleAIActionSelectionEnum, Func<string, object>>() {
        {BattleAIActionSelectionEnum.skill, ConvertSkill},
        {BattleAIActionSelectionEnum.condition, ConvertCondition},
        {BattleAIActionSelectionEnum.condition_group, ConvertConditionGroup},
        {BattleAIActionSelectionEnum.skill_type, ConvertSkillType},
        {BattleAIActionSelectionEnum.exclude_skill_type, ConvertSkillType},
    };

    static object ConvertSkill(string value)
    {
        int skill_id = 0;
        if (int.TryParse (value, out skill_id)) {
            return MasterDataTable.skill.DataList.FirstOrDefault (x => x.id == skill_id);
        }
        return MasterDataTable.skill.DataList.FirstOrDefault (x => x.display_name == value);
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

    static object ConvertSkillType(string value)
    {
        int skill_type = 0;
        if (int.TryParse (value, out skill_type)) {
            return (SkillTypeEnum)skill_type;
        }
        var ret = MasterDataTable.ai_skill_type[value];
        if (ret != null) {
            return ret.Enum;
        }
        return SkillTypeEnum.none;
    }

    static object ConvertInt(string value)
    {
        int ret = 0;
        int.TryParse (value, out ret);
        return ret;
    }
}
