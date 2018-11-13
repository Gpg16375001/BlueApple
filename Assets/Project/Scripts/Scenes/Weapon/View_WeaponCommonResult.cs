using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;
using TMPro;
using UniRx;
using UniRx.Triggers;


/// <summary>
/// View : 武器強化、現凸共通結果画面.
/// </summary>
public class View_WeaponCommonResult : ViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_WeaponCommonResult Create(WeaponData before, WeaponData after, Action didClose = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponCommonResult");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponCommonResult>();
		c.InitInternal(before, after, didClose);
		return c;
	}
	private void InitInternal(WeaponData before, WeaponData after, Action didClose)
	{
		m_didClose = didClose;

		this.GetScript<TextMeshProUGUI>("txtp_WeaponName").text = before.Weapon.name;

		// 前
		this.GetScript<TextMeshProUGUI>("BeforeWeaponHP/txtp_HP").text = before.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponATK/txtp_ATK").text = before.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponDEF/txtp_DEF").text = before.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponSPD/txtp_SPD").text = before.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponLv/txtp_Lv").text = before.Level.ToString();      
		var haveActionSkill = before.Parameter.ActionSkillList != null && before.Parameter.ActionSkillList.Length > 0;
		this.GetScript<RectTransform>("BeforeWeaponSkill").gameObject.SetActive(haveActionSkill);
		if(haveActionSkill){
			var skill = before.Parameter.ActionSkillList[0];
			this.GetScript<TextMeshProUGUI>("BeforeWeaponSkill/txtp_SkillName").text = skill.Skill.display_name;
			this.GetScript<TextMeshProUGUI>("BeforeWeaponSkill/txtp_SkillLv").text = skill.Level.ToString();
		}
		// 後
		this.GetScript<TextMeshProUGUI>("AfterWeaponHP/txtp_HP").text = after.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponATK/txtp_ATK").text = after.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponDEF/txtp_DEF").text = after.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponSPD/txtp_SPD").text = after.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponLv/txtp_Lv").text = after.Level.ToString();
		haveActionSkill = after.Parameter.ActionSkillList != null && after.Parameter.ActionSkillList.Length > 0;
        this.GetScript<RectTransform>("AfterWeaponSkill").gameObject.SetActive(haveActionSkill);
        if (haveActionSkill) {
			var skill = after.Parameter.ActionSkillList[0];
			this.GetScript<TextMeshProUGUI>("AfterWeaponSkill/txtp_SkillName").text = skill.Skill.display_name;
			this.GetScript<TextMeshProUGUI>("AfterWeaponSkill/txtp_SkillLv").text = skill.Level.ToString();
        }

		// ボタン.
        var trigger = this.gameObject.GetOrAddComponent<ObservableEventTrigger>();
        trigger.OnPointerDownAsObservable()
		       .Subscribe(pointer => this.StartCoroutine(CoPlayClose()));
	}
    
	IEnumerator CoPlayClose()
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		if(m_didClose != null){
			m_didClose();
		}
        this.Dispose();
    }

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private Action m_didClose;
}
