using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public partial class Skill
{
    /// <summary> スキル効果リスト </summary>
    public SkillEffectSetting[] SkillEffects { get; private set; }

    public bool HasPerformanceKey(string key, int motionType, SkillPerformanceTargetEnum target)
    {
        var keyData = new SkillPerformanceSettingKey () {
            MotionType = motionType,
            Key = key,
            Target = target
        };
        return SkillEffects.Any(x => x.HasPerformanceKey(keyData)) ||
                MasterDataTable.skill_performance_setting.CommonSkillPerformanceSettings.ContainsKey(keyData);
    }

    private IEnumerable<UniRx.Tuple<float, SkillPerformanceSetting>> GetPerformanceList(SkillPerformanceSettingKey key, SkillEffectLogicEnum[] logics, int? count=null)
    {
        SkillPerformanceSetting[] ret;

        bool isCommonSkip = false;
        foreach (var skillEffect in SkillEffects) {
            if (logics != null && !logics.Contains (skillEffect.skill_effect.effect)) {
                continue;
            }
            if (skillEffect.HasPerformanceKey (key)) {
                float waitTime = 0.0f;
                if (count.HasValue) {
                    waitTime = skillEffect.WaitTime (count.Value);
                }
                foreach (var setting in skillEffect.GetPerformanceList (key, count)) {
                    if (string.IsNullOrEmpty (setting.prefab_name)) {
                        isCommonSkip = true;
                        continue;
                    }
                    yield return new UniRx.Tuple<float, SkillPerformanceSetting>(waitTime, setting);
                }
            }
        }
        if (!isCommonSkip && (!count.HasValue || count.Value == 0)) {
            if (MasterDataTable.skill_performance_setting.CommonSkillPerformanceSettings.TryGetValue (key, out ret)) {
                foreach (var setting in ret) {
                    if (string.IsNullOrEmpty(setting.prefab_name)) {
                        continue;
                    }
                    yield return new UniRx.Tuple<float, SkillPerformanceSetting>(0.0f, setting);
                }
            }
        }
    }

    public IEnumerable<string> GetBattleSkillEffectName()
    {
        foreach (var skillEffect in SkillEffects) {
            foreach (var prefabName in skillEffect.GetBattleSkillEffectName()) {
                yield return prefabName;
            }
        }
    }

    public void PlayPerformance(string key, int motionType, SkillEffectLogicEnum[] logics, SkillPerformanceTargetEnum target, ListItem_BattleUnit unit = null,
        UnityEngine.GameObject parent = null, System.Action<UnityEngine.GameObject> didCreate = null, System.Action<UnityEngine.GameObject> didEnd = null)
    {
        var keyData = new SkillPerformanceSettingKey () {
            MotionType = motionType,
            Key = key,
            Target = target
        };
        for (int i = 0; i < 4; ++i) {
            foreach (var settingTuple in GetPerformanceList (keyData, logics, i)) {
                if (settingTuple.Item2 != null) {
                    var setting = settingTuple.Item2;
                    GameObject boneFollower = null;
                    if (!string.IsNullOrEmpty (setting.bone_name) && unit != null) {
                        boneFollower = unit.ModelController.CreateBoneFollower (setting.bone_name, setting.is_follower_rotation,
                            setting.is_follower_local_scale, setting.is_follower_flip, setting.is_follower_z, unit.EffectAnchor.gameObject);
                        if (setting.is_follower) {
                            parent = boneFollower;
                            boneFollower = null;
                        }
                    }
                    var go = BattleEffectManager.Instance.CreateEffectItem (key, setting.prefab_name, settingTuple.Item1, parent, didCreate, didEnd);
                    if (!setting.is_follower && boneFollower != null) {
                        go.transform.position = boneFollower.transform.position;
                    }
                }
            }
        }
    }

    public bool HasDamageLogic()
    {
        return SkillEffects.Any (x => 
            x.skill_effect.IsDamageLogic()
        );
    }

    public bool HasItemDropUpLogic()
    {
        return SkillEffects.Any (x => 
            x.skill_effect.IsItemDropUp()
        );
    }

    public bool HasExpUpLogic()
    {
        return SkillEffects.Any (x => 
            x.skill_effect.IsExpUp()
        );
    }

    public bool HasMoneyUpLogic()
    {
        return SkillEffects.Any (x => 
            x.skill_effect.IsMoneyUp()
        );
    }

    partial void InitExtension ()
    {
        SkillEffects = MasterDataTable.skill_effect_setting.DataList.Where(x => x.skill_id == id).ToArray();
    }
}
