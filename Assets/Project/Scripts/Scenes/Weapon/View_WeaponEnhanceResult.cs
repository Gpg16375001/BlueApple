﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using SmileLab;
using SmileLab.Net.API;
using TMPro;
using Live2D.Cubism.Rendering;
using UniRx;
using UniRx.Triggers;


/// <summary>
/// View : 武器強化結果画面.
/// </summary>
public class View_WeaponEnhanceResult : ViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_WeaponEnhanceResult Create(WeaponData current, WeaponData result, Action didClose = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponEnhanceResult");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponEnhanceResult>();
		c.InitInternal(current, result, didClose);
		return c;
	}
	private void InitInternal(WeaponData current, WeaponData result, Action didClose)
	{
        m_didClose = didClose;
		m_currentData = current;
		m_resultData = result;

        // ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_WeaponName").text = m_currentData.Weapon.name;
  
		this.GetScript<TextMeshProUGUI>("BeforeWeaponHP/txtp_HP").text = m_currentData.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponATK/txtp_ATK").text = m_currentData.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponDEF/txtp_DEF").text = m_currentData.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponSPD/txtp_SPD").text = m_currentData.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("BeforeWeaponLv/txtp_Lv").text = m_currentData.Level.ToString();

		this.GetScript<TextMeshProUGUI>("AfterWeaponHP/txtp_HP").text = m_resultData.Parameter.Hp.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponATK/txtp_ATK").text = m_resultData.Parameter.Attack.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponDEF/txtp_DEF").text = m_resultData.Parameter.Defense.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponSPD/txtp_SPD").text = m_resultData.Parameter.Agility.ToString();
		this.GetScript<TextMeshProUGUI>("AfterWeaponLv/txtp_Lv").text = m_resultData.Level.ToString();

        // ボタン.
        var trigger = this.gameObject.GetOrAddComponent<ObservableEventTrigger>();
        trigger.OnPointerDownAsObservable()
               .Subscribe(pointer => this.Dispose());
    }

    /// <summary>
    /// 破棄メソッド.
    /// </summary>
    public override void Dispose()
    {
        this.StartCoroutine(CoPlayClose());
    }
    IEnumerator CoPlayClose()
    {
		SoundManager.SharedInstance.PlaySE(SoundClipName.se005);
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		if(m_didClose != null){
			m_didClose();
		}
        base.Dispose();
    }

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private WeaponData m_currentData;
	private WeaponData m_resultData;
    private Action m_didClose;
}
