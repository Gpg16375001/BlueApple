using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class ListItem_FormationSquare : ViewBase
{
    public void Init(Formation formation, int position)
    {
        var NoneGo = this.GetScript<Transform> ("None").gameObject;
        var ActiveGo = this.GetScript<Transform> ("ActiveSquare").gameObject;
        // indexが５より大きいかつformationがnullなら強制でNone状態に
        if (formation == null || position <= 0 || position > 5) {
            NoneGo.SetActive (true);
            ActiveGo.SetActive (false);
            return;
        }
        NoneGo.SetActive (false);
        ActiveGo.SetActive (true);

        // 属性アイコンの設定
        var conditions = formation.GetInvocationConditions(position);
        var iconImage = this.GetScript<Image> ("IconImage");
        if (conditions.Length <= 0) {
            // アイコン非表示
            iconImage.gameObject.SetActive(false);
        } else {
            // アイコン表示
            var condition = conditions [0];
            Sprite icon = null;
            switch (condition.condition) {
            case FormationInvocationConditionEnum.Element:
                var element = MasterDataTable.element.DataList.FirstOrDefault (x => x.name == condition.condition_value);
                if (element != null) {
                    icon = IconLoader.LoadElementIcon (element);
                }
                break;
            case FormationInvocationConditionEnum.Belonging:
                var belonging = MasterDataTable.belonging.DataList.FirstOrDefault (x => x.name == condition.condition_value);
                if (belonging != null) {
                    icon = IconLoader.LoadEmblem (belonging);
                }
                break;
            case FormationInvocationConditionEnum.Family:
            case FormationInvocationConditionEnum.Gender:
            default:
                break;
            }

            if (icon != null) {
                iconImage.gameObject.SetActive(true);
                iconImage.sprite = icon;
            }
        }

        // スキル効果の設定
        int count = 1;
        if (formation.HasPositionSkill (position)) {
            var skill = formation.GetPositionSkill (position);
            foreach (var skillEffectSetting in skill.SkillEffects) {
                if (skillEffectSetting.skill_effect.effect != SkillEffectLogicEnum.buff && skillEffectSetting.skill_effect.effect != SkillEffectLogicEnum.debuff) {
                    continue;
                }

                var sprite = GetScript<uGUISprite> (string.Format("FormationEffectIcon{0}", count));
                sprite.gameObject.SetActive (true);


                string footer = string.Empty;
                if (skillEffectSetting.skill_effect.effect == SkillEffectLogicEnum.buff) {
                    footer = "Up";
                }
                else if (skillEffectSetting.skill_effect.effect == SkillEffectLogicEnum.debuff) {
                    footer = "Down";
                }
                var targetparam = skillEffectSetting.skill_effect.GetValue<SkillTargetParameter> (SkillEffectLogicArgEnum.TargetParameter);
                string spriteName = string.Format("img_FormationEffect{0}{1}", targetparam.short_name, footer);
                sprite.ChangeSprite (spriteName);
                count++;
                if (count > 4) {
                    break;
                }
            }
        }
        for (int i = count; i <= 4; ++i) {
            var sprite = GetScript<uGUISprite> (string.Format("FormationEffectIcon{0}", i));
            sprite.gameObject.SetActive (false);
        }
    }
}
