using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_PartyDetailsUnit : ViewBase
{
    public enum DisplayMode
    {
        Status,
        Skill,

        Init
    }
        
    public void Init(CardData card, bool isSupport=false)
    {
        m_Card = card;
        m_IsEmpty = card == null;

        // カード情報がない場合は
        if (m_IsEmpty) {
            SetEmptyDisplay ();
            return;
        }

        // アイコンの設定
        var unitIconSet = this.GetScript<Transform>("UnitIconSet");
        unitIconSet.gameObject.SetActive (true);
        var unitIcon = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", unitIconSet.gameObject);
        unitIcon.GetOrAddComponent<ListItem_UnitIcon> ().Init (m_Card, ListItem_UnitIcon.DispStatusType.Default);

        // ステータス設定
        this.GetScript<TextMeshProUGUI>("txtp_UnitName").SetText(m_Card.Card.nickname);
        this.GetScript<TextMeshProUGUI>("txtp_UnitLv").SetText(m_Card.Level);
        this.GetScript<TextMeshProUGUI>("txtp_UnitHP").SetText(m_Card.Parameter.MaxHp);
        this.GetScript<TextMeshProUGUI>("txtp_UnitATK").SetText(m_Card.Parameter.Attack);
        this.GetScript<TextMeshProUGUI>("txtp_UnitDEF").SetText(m_Card.Parameter.Defense);
        this.GetScript<TextMeshProUGUI>("txtp_UnitSPD").SetText(m_Card.Parameter.Agility);

        // スキル設定
        var index = System.Array.FindIndex(m_Card.Parameter.UnitActionSkillList, x => !x.IsNormalAction);
        if (m_Card.Parameter.UnitActionSkillList.Length > 0 && index >= 0) {
            this.GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText (m_Card.Parameter.UnitActionSkillList[index].Skill.display_name);
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText ("なし");
        }
        if (m_Card.Parameter.Weapon.ActionSkillList.Length > 0) {
            this.GetScript<TextMeshProUGUI> ("txtp_WeaponSkillName").SetText (m_Card.Parameter.Weapon.ActionSkillList[0].Skill.display_name);
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_WeaponSkillName").SetText ("なし");
        }

        if (m_Card.Parameter.SpecialSkill != null) {
            this.GetScript<TextMeshProUGUI> ("txtp_SPName").SetText (m_Card.Parameter.SpecialSkill.Skill.display_name);
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_SPName").SetText ("なし");
        }

        this.GetScript<Transform> ("img_SupportUnitMark").gameObject.SetActive (isSupport);

        m_Now = DisplayMode.Init;
        SetDisplayMode (DisplayMode.Status);
    }

    public void SetDisplayMode(DisplayMode mode)
    {
        if (m_IsEmpty || m_Now == mode) {
            return;
        }
        this.GetScript<Transform> ("Status").gameObject.SetActive (mode == DisplayMode.Status);
        this.GetScript<Transform> ("Skill").gameObject.SetActive (mode == DisplayMode.Skill);
    }

    private void SetEmptyDisplay()
    {
        this.GetScript<Transform> ("UnitIconSet").gameObject.SetActive (false);

        this.GetScript<Transform> ("Status").gameObject.SetActive (false);
        this.GetScript<Transform> ("Skill").gameObject.SetActive (false);

        this.GetScript<Transform> ("img_EmptyList").gameObject.SetActive (true);
    }

    void SetIcon(IconLoadSetting data, Sprite icon)
    {
        var iconImg = this.GetScript<Image> ("img_IconChDummy");
        iconImg.sprite = icon;
    }

    void OnDestory()
    {
		IconLoader.RemoveLoadedEvent (ItemTypeEnum.card, m_Card.CardId, SetIcon);
    }

    // 表示しているカードデータ
    private CardData m_Card;
    private bool m_IsEmpty;

    private DisplayMode m_Now;
}
