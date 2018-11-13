using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct SkillPerformanceSettingKey
{
    public int? MotionType;
    public string Key;
    public SkillPerformanceTargetEnum Target;

    public override bool Equals (object obj)
    {
        if (obj != null && obj is SkillPerformanceSettingKey) {
            SkillPerformanceSettingKey tmp = (SkillPerformanceSettingKey)obj;

            return Target == tmp.Target && 
                (!MotionType.HasValue || !tmp.MotionType.HasValue || MotionType.Value == tmp.MotionType.Value) &&
                Key == tmp.Key;
        }
        return false;
    }
        
    public override int GetHashCode ()
    {
        // motionTypeはnullableなのでEquals側で判定させる
        int hash = 27;
        hash = (13 * hash) + Key.GetHashCode();
        hash = (13 * hash) + Target.GetHashCode();
        return hash;
    }
}

public partial class SkillPerformanceSettingTable {
    const int COMMON_PERFORMANCE = 0;
    /// <summary> スキル演出リスト </summary>
    private Dictionary<SkillPerformanceSettingKey, SkillPerformanceSetting[]> _commonSkillPerformanceSettings;
    public Dictionary<SkillPerformanceSettingKey, SkillPerformanceSetting[]> CommonSkillPerformanceSettings {
        get {
            if (_commonSkillPerformanceSettings == null) {
                // 初アクセス時に生成する。
                _commonSkillPerformanceSettings = MasterDataTable.skill_performance_setting[COMMON_PERFORMANCE].
                    GroupBy(x => new SkillPerformanceSettingKey() {
                        MotionType = x.motion_type,
                        Key = x.plaing_key,
                        Target = x.performance_target
                    }).ToDictionary(x => x.Key, x => x.ToArray());
            }

            return _commonSkillPerformanceSettings;
        }
    }
}
