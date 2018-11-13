using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : 武器詳細画面.
/// </summary>
public class View_WeaponDetailsPop : PopupViewBase
{   
	/// <summary>
	/// 生成.
	/// </summary>
	public static View_WeaponDetailsPop Create(WeaponData data, Action didUpdateInfo, Action<Action> didProc = null, bool isSceq = true)
	{
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponDetailsPop");
        var c = go.GetOrAddComponent<View_WeaponDetailsPop>();
		c.InitInternal(data, didUpdateInfo, didProc, isSceq);
		return c;
	}
	private void InitInternal(WeaponData data, Action didUpdateInfo, Action<Action> didProc, bool isSceq = true)
	{
		m_didUpdateInfo = didUpdateInfo;      

		View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;

        m_weaponResourceLoader = new WeaponResourceLoader(data.Weapon);

        // 初回情報更新.
        this.UpdateInfo(data);

		// ボタン.
		this.GetScript<CustomButton>("bt_LockOff").gameObject.SetActive(!data.IsLocked);
		this.GetScript<CustomButton>("bt_LockOn").gameObject.SetActive(data.IsLocked);
		this.GetScript<CustomButton>("bt_Close").onClick.AddListener(DidTapClose);
		this.GetScript<CustomButton>("bt_LockOn").onClick.AddListener(DidTapLock);
		this.GetScript<CustomButton>("bt_LockOff").onClick.AddListener(DidTapLock);
		this.GetScript<CustomButton>("EquipChange/bt_Sub").onClick.AddListener(DidTapEquip);
		this.GetScript<CustomButton>("Enhance/bt_Common").onClick.AddListener(DidTapEnhance);
		this.GetScript<CustomButton>("LimitBreak/bt_Common").onClick.AddListener(DidTapLimitBreak);
		this.GetScript<CustomButton>("Sale/bt_Common").onClick.AddListener(DidTapSale);
        SetBackButton ();

        if (!isSceq) {
            this.GetScript<RectTransform>("Equip").gameObject.SetActive(false);
            this.GetScript<RectTransform>("Button").gameObject.SetActive(false);
            this.GetComponent<Canvas>().sortingOrder = 400;
        }

        if (didProc != null) {
            StartCoroutine(CoDidProc(didProc));
        }
    }

    /// <summary>
    /// 破棄メソッド.
    /// </summary>
	public override void Dispose()
    {
        m_weaponResourceLoader.Dispose();
        base.Dispose ();
    }

    // 情報更新処理.
	private void UpdateInfo(WeaponData data)
	{
		m_data = data;
		m_equipCard = data.IsEquipped ? CardData.CacheGet(data.CardId) : null;

		this.GetScript<TextMeshProUGUI>("txtp_WeaponName").text = m_data.Weapon.name;
        this.GetScript<TextMeshProUGUI>("txtp_WeaponLv").text = m_data.Level.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_WeaponLvLimit").SetTextFormat("/{0}", m_data.CurrentLimitBreakMaxLevel.ToString());
		this.GetScript<TextMeshProUGUI>("txtp_LvPoint").SetTextFormat("{0} / {1}", m_data.CurrentLevelExp, m_data.NextLevelExp);
		this.GetScript<Image>("LvGauge/img_CommonGauge").fillAmount = m_data.CurrentLevelProgress;

        // 立ち絵
        m_weaponResourceLoader.LoadResource(source => this.GetScript<Image>("WeaponIcon").sprite = source.PortraitImage);
        // ステータス.
        this.GetScript<TextMeshProUGUI>("txtp_HP").text = m_data.Parameter.Hp.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_ATK").text = m_data.Parameter.Attack.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_SPD").text = m_data.Parameter.Agility.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_DEF").text = m_data.Parameter.Defense.ToString();
        // スキル
        this.GetScript<RectTransform>("SkillStatus").gameObject.SetActive(m_data.IsHaveSkill);
        if (m_data.IsHaveSkill) {
            var skill = m_data.Parameter.ActionSkillList[0] ?? m_data.Parameter.PassiveSkillList[0];
            this.GetScript<TextMeshProUGUI>("txtp_SkillName").text = skill.Skill.display_name;
            this.GetScript<TextMeshProUGUI>("txtp_SkillLv").text = skill.Level.ToString();
			this.GetScript<TextMeshProUGUI>("txtp_SkillNotes").text = skill.Skill.flavor;
        }
        // レアリティ
        for (int i = 1; i <= 5; ++i) {
            var starObj = this.GetScript<Transform>(string.Format("StarGrid/Star{0}", i));
			starObj.gameObject.SetActive(m_data.Rarity >= i);
        }
        // 限界突破
        var nowGrade = m_data.LimitBreakGrade;
        for (int i = 1; i <= 9; ++i) {
            var starOn = this.GetScript<Transform>(string.Format("SelectWeaponSymbol{0}/LimitBreakIconOn", i));
            var starOff = this.GetScript<Transform>(string.Format("SelectWeaponSymbol{0}/LimitBreakIconOff", i));         
            starOn.gameObject.SetActive(nowGrade >= i);
            starOff.gameObject.SetActive(nowGrade < i);
        }
		// アイコン
		var equipCardRoot = this.GetScript<Transform>("UnitIconRoot").gameObject;
		equipCardRoot.DestroyChildren();
		if(m_equipCard != null){
			this.GetScript<TextMeshProUGUI>("txtp_NoWeapon").gameObject.SetActive(false);
			var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", equipCardRoot);
			go.GetOrAddComponent<ListItem_UnitIcon>().Init(m_equipCard);
		} else {
			this.GetScript<TextMeshProUGUI>("txtp_NoWeapon").gameObject.SetActive(true);
		}
		// 詳細
		this.GetScript<TextMeshProUGUI>("txtp_Profile").text = m_data.Weapon.details;

        this.GetScript<CustomButton>("Sale/bt_Common").interactable = m_equipCard == null &&    // 装備してる武器は売却できないように。
                                                                            !m_data.IsLocked;   // ロック中の武器を売却できないように。
        this.GetScript<CustomButton>("Enhance/bt_Common").interactable = m_data.Level < m_data.CurrentLimitBreakMaxLevel;
        this.GetScript<CustomButton>("LimitBreak/bt_Common").interactable = m_data.LimitBreakGrade < 9;
	}
	void SetIcon(IconLoadSetting data, Sprite icon)
    {
        // 更新状態によっては内部データが変わっている可能性があるので判定が必要
        if (m_equipCard.CardId == data.id) {
            var iconImg = this.GetScript<Image>("EquipUnit");
            iconImg.sprite = icon;
        }
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

	#region ButtonDelegate.
 
	// ボタン : 閉じる.
    void DidTapClose()
	{
        if(IsOpenViews()){
            return;
        }

        if (IsClosed) {
            return;
        }
        this.PlayOpenCloseAnimation(false, Dispose);
	}

	// ボタン : 保護.
    void DidTapLock()
	{
        if(IsOpenViews()){
            return;
        }

        if (IsClosed) {
            return;
        }

        LockInputManager.SharedInstance.IsLock = true;
		View_FadePanel.SharedInstance.IsLightLoading = true;
		if(m_data.IsLocked){
			// アンロック.
			SendAPI.WeaponsUnlockWeapon(m_data.BagId, (bSuccess, res) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if (!bSuccess || res == null) {
                    return;
                }
				res.WeaponData.CacheSet();            
				this.GetScript<CustomButton>("bt_LockOff").gameObject.SetActive(!res.WeaponData.IsLocked);
				this.GetScript<CustomButton>("bt_LockOn").gameObject.SetActive(res.WeaponData.IsLocked);            
				m_data = res.WeaponData;
			    this.UpdateInfo(m_data);
                if (m_didUpdateInfo != null){
					m_didUpdateInfo();
				}
            });
		}else{
			// ロック.
			SendAPI.WeaponsLockWeapon(m_data.BagId, (bSuccess, res) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if (!bSuccess || res == null) {
                    return;
                }
				res.WeaponData.CacheSet();
				this.GetScript<CustomButton>("bt_LockOff").gameObject.SetActive(!res.WeaponData.IsLocked);
                this.GetScript<CustomButton>("bt_LockOn").gameObject.SetActive(res.WeaponData.IsLocked);            
				m_data = res.WeaponData;
			    this.UpdateInfo(m_data);
                if (m_didUpdateInfo != null) {
                    m_didUpdateInfo();
                }
            });
		}

	}

	// ボタン : 装備.
    void DidTapEquip()
	{
        if(IsOpenViews()){
            return;
        }

        if (IsClosed) {
            return;
        }

        m_viewWeaponEquipUnitList = WeaponSContoller.CreateWeaponEquipUnitListView(m_data, card => {
			this.UpdateInfo(WeaponData.CacheGet(m_data.BagId));
            if (m_didUpdateInfo != null) {
                m_didUpdateInfo();
            }
		});
	}   

	// ボタン : 強化.
    void DidTapEnhance()
	{
        if(IsOpenViews()){
            return;
        }

        if (IsClosed) {
            return;
        }

        m_viewWeaponEnhance = WeaponSContoller.CreateWeaponEnhanceView(m_data, weapon => {
			this.UpdateInfo(weapon);
            if (m_didUpdateInfo != null) {
                m_didUpdateInfo();
            }
		});
	}

	// ボタン : 上限突破.
    void DidTapLimitBreak()
	{
        if(IsOpenViews()){
            return;
        }

        if (IsClosed) {
            return;
        }

        m_viewWeaponLimitBreak = WeaponSContoller.CreateWeaponLimitBreakView(m_data, weapon => {
            this.UpdateInfo(weapon);
            if (m_didUpdateInfo != null) {
                m_didUpdateInfo();
            }
        });
	}

	// ボタン : 売却.
    void DidTapSale()
    {
        if(IsOpenViews()){
            return;
        }

        if (IsClosed) {
            return;
        }

        m_viewSaleAlertPop = WeaponSContoller.CreateWeaponSaleView(m_data);
    }

    #endregion

    IEnumerator CoDidProc(Action<Action> didProc)
    {
        yield return null;
        didProc(() => this.UpdateInfo(WeaponData.CacheGet(m_data.BagId)));
        if (m_didUpdateInfo != null) {
            m_didUpdateInfo();
        }
    }

    /// <summary>
    /// なにがしかのViewが開いている
    /// </summary>
    /// <returns><c>true</c> if this instance is open views; otherwise, <c>false</c>.</returns>
    bool IsOpenViews()
    {
        return m_viewWeaponEnhance != null || m_viewWeaponEquipUnitList != null || m_viewWeaponLimitBreak != null || m_viewSaleAlertPop != null;
    }

	private WeaponData m_data;
	private Action m_didUpdateInfo;
	private CardData m_equipCard;
    private View_WeaponEnhance m_viewWeaponEnhance;
    private View_WeaponEquipUnitList m_viewWeaponEquipUnitList;
    private View_WeaponLimitBreak m_viewWeaponLimitBreak;
    private View_SaleAlertPop m_viewSaleAlertPop;
    private WeaponResourceLoader m_weaponResourceLoader;
}
