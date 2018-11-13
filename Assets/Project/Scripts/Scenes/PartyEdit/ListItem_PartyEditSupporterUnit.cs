using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

using BattleLogic;

/// <summary>
/// パーティ編成画面のユニット情報表示制御クラス
/// </summary>
public class ListItem_PartyEditSupporterUnit : ViewBase
{
    public bool ForceHighlight {
        get {
            return (m_Button != null) ? m_Button.ForceHighlight : false;
        }
        set {
            if (m_Button != null) {
                m_Button.ForceHighlight = value;
            }
        }
    }

    public bool EnableLongPress {
        get {
            return (m_Button != null) ? m_Button.m_EnableLongPress : false;
        }
        set {
            if (m_Button != null) {
                m_Button.m_EnableLongPress = value;
            }
        }
    }

    public void Init(SupporterCardData card, FormationData formation, int position, Func<int, int, bool> DidTap, Action<SupporterCardData, bool> DidLongPress)
    {
        m_Position = position;
        m_SupporterCardData = card;
        m_DidTap = DidTap;
        m_DidLongPress = DidLongPress;
        m_IsSupportCard = m_Position == 6;
        // 陣形情報の表示
        if (formation != null) {
            SetFormationParameter (formation.Formation, position);
        }

        m_Button = this.GetScript<CustomButton>("bt_PartyEditUnitFrame");
        m_Button.onClick.AddListener (DidTapPartyEditUnit);
        m_Button.onLongPress.AddListener (DidLongTapPartyEditUnit);

        UpdateUnit (card, formation, position);
    }

    public void UpdateUnit(SupporterCardData card, FormationData formation, int position)
    {
        m_SupporterCardData = card;

        var unitInfoGo = this.GetScript<Transform> ("UnitInfo").gameObject;
        var emptyImgGo = this.GetScript<Transform> ("img_PartyEditEmpty").gameObject;

        // cardがnullの場合はEmpty表示
        if (card == null) {
            unitInfoGo.SetActive (false);
            emptyImgGo.SetActive (true);
            return;
        }

        unitInfoGo.SetActive (true);
        emptyImgGo.SetActive (false);

        // パラメータの計算
        Parameter parameter = new Parameter (
            card, position,
            formation != null ? formation.Formation.GetPostionRow(position) : 0, 
            formation != null ? formation.Formation.GetPostionColumn(position) : 0,
            true
        );
        SetUnitParameter (card, parameter);

        LoadPartyImage (card);

        SetStar (card);

        if (m_IsSupportCard) {
            if (card.EquippedWeaponData != null && card.EquippedWeaponData.BagId != 0) {
                UpdateWeapon (card.EquippedWeaponData);
            } else {
                UpdateWeapon (null);
            }
        }
    }

    public void UpdateWeapon(WeaponData weapon)
    {
        var NoIconBg = GetScript<RectTransform> ("img_NoIconBg").gameObject;
        if (weapon != null) {
            if (m_WeaponIcon == null) {
                var icon = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_WeaponIcon", GetScript<RectTransform> ("WeaponRoot").gameObject);
                m_WeaponIcon = icon.GetOrAddComponent<ListItem_WeaponIcon> ();
            }
            m_WeaponIcon.gameObject.SetActive (true);
            m_WeaponIcon.Init(weapon, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);
            NoIconBg.SetActive (false);
            return;
        }

        if (m_WeaponIcon != null) {
            m_WeaponIcon.gameObject.SetActive (false);
        }
        NoIconBg.SetActive (true);
    }

    private void SetUnitParameter(SupporterCardData card, Parameter parameter)
    {
        // 属性アイコンの入れ替え
        this.GetScript<Image>("ElementIcon").sprite = IconLoader.LoadElementIcon(parameter.Element);

        // パラメータの表示設定
        this.GetScript<TextMeshProUGUI> ("txtp_UnitName").SetText(card.Card.nickname);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitLv").SetText(parameter.Level.ToString());
        this.GetScript<TextMeshProUGUI> ("txtp_UnitHP").SetText(parameter.MaxHp.ToString());
        this.GetScript<TextMeshProUGUI> ("txtp_UnitATK").SetText(parameter.Attack.ToString());
        this.GetScript<TextMeshProUGUI> ("txtp_UnitDEF").SetText(parameter.Defense.ToString());
        this.GetScript<TextMeshProUGUI> ("txtp_UnitSPD").SetText(parameter.Agility.ToString());
    }

    UnitResourceLoader loader;
    private void LoadPartyImage (SupporterCardData card)
    {
        var partyCardImage = this.GetScript<Image> ("PartyCard");
        loader = new UnitResourceLoader (card.ConvertCardData());
        loader.LoadFlagReset ();
        loader.IsLoadPartyCardImage = true;

        loader.LoadResource ((resource) => {
            if(this.gameObject == null) {
                return;
            }
            partyCardImage.overrideSprite = resource.GetPartyCardImage(card.Rarity);
        });
    }

    private void SetStar(SupporterCardData card)
    {
        int maxRarity = card.MaxRarity;
        int nowRarity = card.Rarity;
        for(int i = 1; i <= 6; ++i)
        {
            var starObj = this.GetScript<Transform> (string.Format ("StarGrid/Star{0}", i));
            if (maxRarity >= i) {
                starObj.gameObject.SetActive (true);
                var starOn = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOn", i));
                var starOff = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOff", i));
                if (nowRarity >= i) {
                    starOn.gameObject.SetActive (true);
                    starOff.gameObject.SetActive (false);
                } else {
                    starOn.gameObject.SetActive (false);
                    starOff.gameObject.SetActive (true);
                }
            } else {
                starObj.gameObject.SetActive (false);
            }
        }
        // Gridの並び替えをさせる
        GetScript<GridLayoutGroup> ("StarGrid").CalculateLayoutInputHorizontal();
    }

    private void SetFormationParameter(Formation formation, int position)
    {
        var FormationEffectGo = this.GetScript<Transform> ("FormationEffect").gameObject;
        var BenchStartIconGo = this.GetScript<Transform> ("img_BenchStartIcon").gameObject;
        if (m_IsSupportCard) {
            FormationEffectGo.SetActive (false);
            BenchStartIconGo.SetActive (true);
            return;
        }

        FormationEffectGo.SetActive (true);
        BenchStartIconGo.SetActive (false);

        var formationIconRoot = this.GetScript<Transform> ("FormationIconRoot");
        var foramtionIcon = GameObjectEx.LoadAndCreateObject ("PartyEdit/FormationIcon", formationIconRoot.gameObject);
        var foramtionIconScript = foramtionIcon.GetComponent<FormationIcon> ();
        foramtionIconScript.Init (formation, position);

        int count = 1;
        if (formation.HasPositionSkill (position)) {
            var skill = formation.GetPositionSkill (position);
            foreach (var skillEffectSetting in skill.SkillEffects) {
                if (skillEffectSetting.skill_effect.effect == SkillEffectLogicEnum.buff || skillEffectSetting.skill_effect.effect == SkillEffectLogicEnum.debuff) {
                    if (skillEffectSetting.skill_effect.ContainsArg (SkillEffectLogicArgEnum.TargetParameter)) {
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
            }
        }
        for (int i = count; i <= 4; ++i) {
            var sprite = GetScript<uGUISprite> (string.Format("FormationEffectIcon{0}", count));
            sprite.gameObject.SetActive (false);
        }
    }

    void DidTapPartyEditUnit()
    {
        if (m_DidTap != null) {
            if (m_DidTap (m_Position, m_SupporterCardData != null ? m_SupporterCardData.CardId : 0)) {
                m_Button.ForceHighlight = true;
            } else {
                m_Button.ForceHighlight = false;
            }
        }
    }

    void DidLongTapPartyEditUnit()
    {
        if (m_DidLongPress != null) {
            m_DidLongPress (m_SupporterCardData, m_IsSupportCard);
        }
    }


    void OnDestory()
    {
        if (loader != null) {
            loader.Dispose ();
        }
        loader = null;
    }

    private bool m_IsSupportCard;
    private int m_Position;
    private SupporterCardData m_SupporterCardData;
    private CustomButton m_Button;
    private Func<int, int, bool> m_DidTap;
    private Action<SupporterCardData, bool> m_DidLongPress;
    private ListItem_WeaponIcon m_WeaponIcon;
}
