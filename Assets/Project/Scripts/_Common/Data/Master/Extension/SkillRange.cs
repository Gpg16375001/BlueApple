using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SkillRange {
    private SkillRangeSetting[] _rangeSettings; 
    public SkillRangeSetting[] rangeSettings { 
        get {
            _rangeSettings = _rangeSettings ?? MasterDataTable.skill_range_setting.DataList.Where (
                x => x.range == this.range_setting
            ).ToArray();
            return _rangeSettings;
        }
    }

    // 敵指定
    public bool IsEnemy
    {
        get {
            return target == SkillTargetEnum.enemy;
        }
    }

    // 自分指定
    public bool IsSelf
    {
        get {
            return target == SkillTargetEnum.self;
        }
    }

    // 味方指定
    public bool IsAlly
    {
        get {
            return (target == SkillTargetEnum.ally || target == SkillTargetEnum.self);
        }
    }

    // 全体指定
    public bool IsAll
    {
        get {
            return range_setting == (int)SkillRangeSettingEnum.All;
        }
    }

    // 敵全体指定
    public bool IsAllEnemy
    {
        get {
            return IsEnemy && IsAll;
        }
    }

    // 味方全体指定
    public bool IsAllAlly
    {
        get {
            return IsAlly && IsAll;
        }
    }

    // 範囲指定
    public bool IsRange
    {
        get {
            return rangeSettings.Length > 1;
        }
    }

    // 敵範囲指定
    public bool IsRangeEnemy
    {
        get {
            return IsEnemy && IsRange;
        }
    }

    // 味方範囲指定
    public bool IsRangeAlly
    {
        get {
            return IsAlly && IsRange;
        }
    }

    // 単体指定であるか
    public bool IsUnit
    {
        get {
            return rangeSettings.Length == 1;
        }
    }

    // 単体敵指定であるか
    public bool IsUnitEnemy
    {
        get {
            return IsEnemy && IsUnit;
        }
    }

    // 単体自分指定であるか
    public bool IsUnitSelf
    {
        get {
            return IsSelf && IsUnit &&
                rangeSettings[0].range_base == SkillRangeBaseEnum.target && 
                rangeSettings[0].x == 0 && rangeSettings[0].y == 0;
        }
    }

    // 単体味方指定であるか
    public bool IsUnitAlly
    {
        get {
            return IsAlly && IsUnit;
        }
    }


}
