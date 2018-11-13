using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : 武器装備確認ぽっぷ.
/// </summary>
public class View_WeaponEquip : PopupViewBase
{   
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_WeaponEquip Create(WeaponData data, CardData card, Action<CardData> didTapEquip)
    {
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponEquip");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponEquip>();
		c.InitInternal(data, card, didTapEquip);
        return c;
    }
	private void InitInternal(WeaponData data, CardData card, Action<CardData> didTapEquip)
	{
		m_weapon = data;
		m_card = card;
		m_didTapEquip = didTapEquip;

		// TODO : カード側で装備がない＝初期装備として装備情報を取得できるモジュールが欲しい.
		WeaponData current;
		if(card.EquippedWeaponBagId <= 0){
			var initialWeapon = MasterDataTable.weapon[card.Parameter.Weapon.weapon.id];
			current = new WeaponData(initialWeapon);
		}else{
			current = WeaponData.CacheGet(card.EquippedWeaponBagId);
		}
		var dispWpName = card.EquippedWeaponBagId <= 0 ? "初期装備" : current.Weapon.name;
		var dispLvStr = card.EquippedWeaponBagId <= 0 ? "-" : current.Level.ToString();

		// カードアイコン.
		var rootObj = this.GetScript<RectTransform>("UnitIcon").gameObject;
		var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", rootObj);
		go.GetOrAddComponent<ListItem_UnitIcon>().Init(m_card);
		// スキル
		this.GetScript<RectTransform>("CurrentWeaponSkill").gameObject.SetActive(current.IsHaveSkill);
		if(current.IsHaveSkill){
			var skill = current.Parameter.ActionSkillList[0] ?? current.Parameter.PassiveSkillList[0];
			this.GetScript<TextMeshProUGUI>("CurrentWeaponSkill/txtp_SkillName").text = skill.Skill.display_name;
			this.GetScript<TextMeshProUGUI>("CurrentWeaponSkill/txtp_SkillLv").text = skill.Level.ToString();
			// TODO : スキルアイコン.
		}
		this.GetScript<RectTransform>("EquipWeaponSkill").gameObject.SetActive(m_weapon.IsHaveSkill);
		if (m_weapon.IsHaveSkill) {
			var skill = m_weapon.Parameter.ActionSkillList[0] ?? m_weapon.Parameter.PassiveSkillList[0];
			this.GetScript<TextMeshProUGUI>("EquipWeaponSkill/txtp_SkillName").text = skill.Skill.display_name;
			this.GetScript<TextMeshProUGUI>("EquipWeaponSkill/txtp_SkillLv").text = skill.Level.ToString();
            // TODO : スキルアイコン.
        }
        // 武器アイコン
		var prefab = Resources.Load("_Common/View/ListItem_WeaponIcon") as GameObject;
		var sg = GameObject.Instantiate(prefab, this.GetScript<RectTransform>("CurrentWeaponInfo/WeaponIcon"), false);
		sg.GetOrAddComponent<ListItem_WeaponIcon>().Init(current, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly, false, true);
		sg = GameObject.Instantiate(prefab, this.GetScript<RectTransform>("EquipWeaponInfo/WeaponIcon"), false);
		sg.GetOrAddComponent<ListItem_WeaponIcon>().Init(m_weapon, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly, false, true);
		// パラメータ.
		this.GetScript<TextMeshProUGUI>("txtp_CurrentWeaponName").text = dispWpName;
		this.GetScript<TextMeshProUGUI>("CurrentWeaponHP/txtp_HP").text = current.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("CurrentWeaponATK/txtp_ATK").text = current.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("CurrentWeaponSPD/txtp_SPD").text = current.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("CurrentWeaponDEF/txtp_DEF").text = current.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("CurrentWeaponLv/txtp_WeaponLv").text = dispLvStr;
		this.GetScript<TextMeshProUGUI>("txtp_EquipWeaponName").text = m_weapon.Weapon.name;
		this.GetScript<TextMeshProUGUI>("EquipWeaponHP/txtp_HP").text = m_weapon.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("EquipWeaponATK/txtp_ATK").text = m_weapon.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("EquipWeaponSPD/txtp_SPD").text = m_weapon.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("EquipWeaponDEF/txtp_DEF").text = m_weapon.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("EquipWeaponLv/txtp_WeaponLv").text = m_weapon.Level.ToString();
        StatusDifferenceCheck(current);

        // ボタン.
        this.GetScript<CustomButton>("bt_Close").onClick.AddListener(DidTapCancel);
		this.GetScript<CustomButton>("Equip/bt_Common").onClick.AddListener(DidTapEquip);
		this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(DidTapCancel);
        SetBackButton();
    }

    private void StatusDifferenceCheck(WeaponData current)
    {
        var diff = m_weapon.Parameter.Hp - current.Parameter.Hp;
        if (diff > 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponHP/txtp_UpHP").text = "(+" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponHP/txtp_UpHP").gameObject.SetActive(true);
        } else if (diff < 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponHP/txtp_DownHP").text = "(-" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponHP/txtp_DownHP").gameObject.SetActive(true);
        }

        diff = m_weapon.Parameter.Attack - current.Parameter.Attack;
        if (diff > 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponATK/txtp_UpATK").text = "(+" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponATK/txtp_UpATK").gameObject.SetActive(true);
        } else if (diff < 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponATK/txtp_DownATK").text = "(-" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponATK/txtp_DownATK").gameObject.SetActive(true);
        }

        diff = m_weapon.Parameter.Agility - current.Parameter.Agility;
        if (diff > 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponSPD/txtp_UpSPD").text = "(+" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponSPD/txtp_UpSPD").gameObject.SetActive(true);
        } else if (diff < 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponSPD/txtp_DownSPD").text = "(-" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponSPD/txtp_DownSPD").gameObject.SetActive(true);
        }

        diff = m_weapon.Parameter.Defense - current.Parameter.Defense;
        if (diff > 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponDEF/txtp_UpDEF").text = "(+" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponDEF/txtp_UpDEF").gameObject.SetActive(true);
        } else if (diff < 0) {
            this.GetScript<TextMeshProUGUI>("EquipWeaponDEF/txtp_DownDEF").text = "(-" + Mathf.Abs(diff).ToString() + ")";
            this.GetScript<TextMeshProUGUI>("EquipWeaponDEF/txtp_DownDEF").gameObject.SetActive(true);
        }
    }

    protected override void DidBackButton ()
    {
        DidTapCancel();
    }

	#region Button.

	// ボタン : 装備実施.
	void DidTapEquip()
	{
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.CardsSetWeapon(new EquippedWeapon[] { new EquippedWeapon{CardId = m_card.CardId, WeaponBagId=m_weapon.BagId}}, (bSuccess, res) => {
			res.AffectedCardDataList.CacheSet();
			res.AffectedWeaponDataList.CacheSet();
			m_card = CardData.CacheGet(m_card.CardId);
			m_weapon = WeaponData.CacheGet(m_weapon.BagId);         
            this.Dispose();
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
			if (m_didTapEquip != null) {
                m_didTapEquip(m_card);
            }
		});      
	}

	// ボタン : キャンセル.
    void DidTapCancel()
	{
		this.Dispose();
	}

    #endregion.

    private WeaponData m_weapon;
	private CardData m_card;   
	private Action<CardData> m_didTapEquip;
}
                       