using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FormationInvocationCondition {
    public bool CheckValue(BattleLogic.Parameter unit)
    {
        switch (condition) {
        case FormationInvocationConditionEnum.Belonging:
            return condition_value == unit.belonging.name;
        case FormationInvocationConditionEnum.Element:
            return condition_value == unit.Element.name;
        case FormationInvocationConditionEnum.Family:
            return condition_value == unit.family.name;
        case FormationInvocationConditionEnum.Gender:
            return condition_value == unit.gender.name;
        }
        return false;
    }
}
