using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SkillEffectSetting
{
    public const int SkillPerformanceSettingMax = 4;
    private int[] SkillPerformanceSettingIds;
    private float[] SkillPerformanceWaitTimes;

    /// <summary> スキル演出リスト </summary>
    private Dictionary<SkillPerformanceSettingKey, SkillPerformanceSetting[]>[] _skillPerformanceSettings;
    private Dictionary<SkillPerformanceSettingKey, SkillPerformanceSetting[]>[] SkillPerformanceSettings {
        get {
            // performance_patternが0の場合は共通のみ使用のため特に何もしない
            // 初アクセス時に生成する。
            if (_skillPerformanceSettings == null) {
                _skillPerformanceSettings = new Dictionary<SkillPerformanceSettingKey, SkillPerformanceSetting[]>[4];

                // 演出1
                for (int i = 0; i < SkillPerformanceSettingMax; ++i) {
                    int performance_setting_id = SkillPerformanceSettingIds[i];
                    var peformanceSettings = MasterDataTable.skill_performance_setting [performance_setting_id];
                    if (performance_setting_id > 0 && peformanceSettings != null) {
                        _skillPerformanceSettings[i] = peformanceSettings.
                            GroupBy (x => new SkillPerformanceSettingKey () {
                                MotionType = x.motion_type,
                                Key = x.plaing_key,
                                Target = x.performance_target
                            }).
                            ToDictionary (x => x.Key, x => x.ToArray ());
                    } else {
                        _skillPerformanceSettings[i] = new Dictionary<SkillPerformanceSettingKey, SkillPerformanceSetting[]> ();
                    }
                }
            }
            return _skillPerformanceSettings;
        }
    }

    public bool HasPerformanceKey(SkillPerformanceSettingKey key)
    {
        return SkillPerformanceSettings.Any(x => x.ContainsKey (key));
    }

    public IEnumerable<SkillPerformanceSetting> GetPerformanceList(SkillPerformanceSettingKey key, int? count = null)
    {
        SkillPerformanceSetting[] ret;

        if (count.HasValue) {
            var performanceSetting = SkillPerformanceSettings [count.Value];
            if (performanceSetting.TryGetValue (key, out ret)) {
                foreach (var setting in ret) {
                    yield return setting;
                }
            }
        } else {
            foreach (var performanceSetting in SkillPerformanceSettings) {
                if (performanceSetting.TryGetValue (key, out ret)) {
                    foreach (var setting in ret) {
                        yield return setting;
                    }
                }
            }
        }
    }

    public float WaitTime(int count)
    {
        if (SkillPerformanceWaitTimes.Length <= count || count < 0) {
            return 0.0f;
        }
        return SkillPerformanceWaitTimes [count];
    }

    public IEnumerable<string> GetBattleSkillEffectName()
    {
        return SkillPerformanceSettings.SelectMany(x => x.Values.SelectMany(performanceSetting => performanceSetting.Select (y => y.prefab_name)));
    }

    partial void InitExtension ()
    {
        SkillPerformanceSettingIds = new int[SkillPerformanceSettingMax];

        SkillPerformanceSettingIds[0] = performance_pattern_1;
        SkillPerformanceSettingIds[1] = performance_pattern_2;
        SkillPerformanceSettingIds[2] = performance_pattern_3;
        SkillPerformanceSettingIds[3] = performance_pattern_4;

        SkillPerformanceWaitTimes = new float[SkillPerformanceSettingMax];
        SkillPerformanceWaitTimes[0] = 0.0f;
		SkillPerformanceWaitTimes[1] = SkillPerformanceWaitTimes[0] + performance_wait_time_1_2;
		SkillPerformanceWaitTimes[2] = SkillPerformanceWaitTimes[1] + performance_wait_time_2_3;
		SkillPerformanceWaitTimes[3] = SkillPerformanceWaitTimes[2] + performance_wait_time_3_4;
    }
}
