using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using BattleLogic;

public partial class SkillElementTargetSetting {
    public List<Parameter> Extraction(List<Parameter> parameter)
    {
        return parameter.Where (x => IsSatisfy (x.Element.Enum)).ToList ();
    }

    // 条件を満たすかの判定
    private bool IsSatisfy(ElementEnum element)
    {
        bool ret = false;
        if (is_fire) {
            ret |= element == ElementEnum.fire;
        }
        if (is_water) {
            ret |= element == ElementEnum.water;
        }
        if (is_wind) {
            ret |= element == ElementEnum.wind;
        }
        if (is_soil) {
            ret |= element == ElementEnum.soil;
        }
        if (is_light) {
            ret |= element == ElementEnum.light;
        }
        if (is_dark) {
            ret |= element == ElementEnum.dark;
        }
        if (is_naught) {
            ret |= element == ElementEnum.naught;
        }
        return ret;
    }
}
