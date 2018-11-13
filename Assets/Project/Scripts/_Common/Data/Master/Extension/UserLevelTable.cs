using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class UserLevelTable {
    public int GetLevel(int exp)
    {
        return DataList.Where (x => x.required_experience <= exp).Max (x => x.level);
    }

    public int GetNextLevelRequiredExp(int exp)
    {
        var level = GetLevel(exp);
        if (_dataDict.ContainsKey (level + 1)) {
            return _dataDict [level + 1].required_experience - _dataDict [level].required_experience;
        }
        return 0;
    }

    /// 次のレベルまでの経験値.
    public int GetNextLevelExp(int exp)
    {
        var level = GetLevel (exp);

        if (_dataDict.ContainsKey (level + 1)) {
            return _dataDict [level + 1].required_experience - _dataDict [level].required_experience - GetCurrentLevelExp(exp);
        }
        return 0;
    }
    /// 現在のレベルまでの経験値.
    public int GetCurrentLevelExp(int exp)
    {
        var level = GetLevel(exp);

        if (_dataDict.ContainsKey(level)) {
            return exp - _dataDict[level].required_experience;
        }

        return 0;
    }
    /// 現在のレベルまでの経験値.
    public float GetCurrentLevelProgress(int exp)
    {
        var level = GetLevel(exp);
        if (_dataDict.ContainsKey (level + 1)) {
            var nextExp = _dataDict [level + 1].required_experience - _dataDict [level].required_experience;
            var nowProgress = exp - _dataDict[level].required_experience;

            return (float)nowProgress / (float)nextExp;
        }

        return 0.0f;
    }

    public int GetMaxAp(int level)
    {
        return this[level].ap_max;
    }
}
