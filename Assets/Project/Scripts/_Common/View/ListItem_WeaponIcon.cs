using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

/// <summary>
/// 武器アイコン表示クラス
/// </summary>
public class ListItem_WeaponIcon : ViewBase
{
	/// <summary>表示している武器情報.</summary>
	public WeaponData WeaponData { get; private set; }
    
    /// <summary>選択中？</summary>
	public bool IsSelected 
	{ 
		get { 
			return this.GetScript<Image>("img_IconSelectFrame").gameObject.activeSelf;
		} 
		set {
			this.GetScript<Image>("img_IconSelectFrame").gameObject.SetActive(value);
		}
	}

    /// <summary>無効状態？</summary>
    public bool IsEnable
	{
		get {
			return this.GetScript<Image>("img_Grayout").gameObject.activeSelf;
		}
		set {
			this.GetScript<Image>("img_Grayout").gameObject.SetActive(!value);
			this.IsEnableButton = value;
		}
	}

    /// <summary>限界突破済み表示？</summary>
    public bool IsNoLimitBreak
	{
		get {
			return this.GetScript<Image>("img_NoLimitBreak").gameObject.activeSelf;
		}
		set {
			this.GetScript<Image>("img_NoLimitBreak").gameObject.SetActive(!value);
        }
    }

	public enum DispStatusType
    {
        Default,      
        HP,
        ATK,
        DEF,
        SPD,
        LimitBreak,
        Price,
        RarityAndElementOnly,
        IconOnly,
    }

    public void Awake()
    {
        var iconImg = this.GetScript<Image>("IconWeaponDummy");
        m_Blank = iconImg.sprite;
    }

	public void Init(WeaponData weapon, DispStatusType statusType = DispStatusType.Default, bool isDispRemove = false, bool isDispEquip = false, bool isDispLimitBreak = false, Action<WeaponData> DidTap = null, Action<WeaponData> DidLongPress = null)
	{
		if (WeaponData != null) { 
			IconLoader.RemoveLoadedEvent(ItemTypeEnum.weapon, WeaponData.WeaponId, SetIcon);
            // アイコンが切り替わるので古いものの表示を削除
			var iconImg = this.GetScript<Image>("IconWeaponDummy");
            iconImg.sprite = m_Blank;
		}
		WeaponData = weapon;
        m_DidTap = DidTap;
        m_DidLongPress = DidLongPress;

		var iconBg = this.GetScript<Image>("ListIconBg");
		iconBg.sprite = IconLoader.LoadWeaponListIconBg(WeaponData.Rarity);
        var iconFrame = this.GetScript<Image>("ListIconFrame");
		iconFrame.sprite = IconLoader.LoadWeaponListIconFrame(WeaponData.Rarity);

		var footerRoot = this.GetScript<RectTransform>("FooterInfo");      
		var txtpUnitStatus = this.GetScript<TextMeshProUGUI>("txtp_UnitStatus");
		var statusRoot = this.GetScript<Transform>("Status");
        var levelRoot = this.GetScript<Transform>("Lv");
        var starGrid = this.GetScript<Transform>("StarGrid");
		var symbolGrid = this.GetScript<Transform>("SymbolGrid");
        var elementIcon = GetScript<Image>("ElementIcon");
		if(WeaponData == null){
			txtpUnitStatus.gameObject.SetActive(false);
            elementIcon.gameObject.SetActive(false);
            starGrid.gameObject.SetActive(false);
            levelRoot.gameObject.SetActive(false);
            return;
		}

		IconLoader.LoadWeaponIcon(WeaponData.WeaponId, SetIcon);

		footerRoot.gameObject.SetActive(statusType != DispStatusType.RarityAndElementOnly);

		// level表示            
		levelRoot.gameObject.SetActive(statusType == DispStatusType.Default);
		this.GetScript<TextMeshProUGUI>("txtp_Lv").SetText(weapon.Level);
		this.GetScript<TextMeshProUGUI>("txtp_LvMaxNum").SetTextFormat("/{0}", weapon.CurrentLimitBreakMaxLevel);

		// レアリティの設定
        for (int i = 1; i <= 5; ++i) {
            var starObj = this.GetScript<Transform>(string.Format("StarGrid/Star{0}", i));
			starObj.gameObject.SetActive(weapon.Rarity >= i);
        }
		// 限界突破値の設定      
		var nowGrade = weapon.LimitBreakGrade;
		for (int i = 1; i <= 9; ++i) { 
			var starOn = this.GetScript<Transform>(string.Format("Symbol{0}/LimitBreakIconOn", i));
			var starOff = this.GetScript<Transform>(string.Format("Symbol{0}/LimitBreakIconOff", i));
			starOn.gameObject.SetActive(nowGrade >= i);
			starOff.gameObject.SetActive(nowGrade < i);
		}

		// ステータス表示切り替え
		starGrid.gameObject.SetActive(true);
        switch (statusType) {
			case DispStatusType.RarityAndElementOnly:
            case DispStatusType.Default:
				symbolGrid.gameObject.SetActive(false);
				statusRoot.gameObject.SetActive(false);
				txtpUnitStatus.gameObject.SetActive(false);
				iconBg.gameObject.SetActive(true);
                break;
            case DispStatusType.HP:
				symbolGrid.gameObject.SetActive(false);
				statusRoot.gameObject.SetActive(true);       
                txtpUnitStatus.gameObject.SetActive(true);
				iconBg.gameObject.SetActive(true);
				txtpUnitStatus.SetText(weapon.Parameter.Hp);
                break;
            case DispStatusType.ATK:
				symbolGrid.gameObject.SetActive(false);
				statusRoot.gameObject.SetActive(true);
                txtpUnitStatus.gameObject.SetActive(true);
				iconBg.gameObject.SetActive(true);
				txtpUnitStatus.SetText(weapon.Parameter.Attack);
                break;
            case DispStatusType.DEF:
				symbolGrid.gameObject.SetActive(false);
				statusRoot.gameObject.SetActive(true);
                txtpUnitStatus.gameObject.SetActive(true);
				iconBg.gameObject.SetActive(true);
				txtpUnitStatus.SetText(weapon.Parameter.Defense);
                break;
            case DispStatusType.SPD:
				symbolGrid.gameObject.SetActive(false);
				statusRoot.gameObject.SetActive(true);
                txtpUnitStatus.gameObject.SetActive(true);
				iconBg.gameObject.SetActive(true);
				txtpUnitStatus.SetText(weapon.Parameter.Agility);
                break;
			case DispStatusType.LimitBreak:
				symbolGrid.gameObject.SetActive(true);
				statusRoot.gameObject.SetActive(true);
				txtpUnitStatus.gameObject.SetActive(false);
				iconBg.gameObject.SetActive(true);
				break;
			case DispStatusType.Price:
				symbolGrid.gameObject.SetActive(false);
				statusRoot.gameObject.SetActive(true);
                GetScript<Image>("img_ListIconCr").preserveAspect = true;
				txtpUnitStatus.gameObject.SetActive(true);
				iconBg.gameObject.SetActive(true);
                txtpUnitStatus.enableAutoSizing = true;
                txtpUnitStatus.fontSizeMin = 10f;
                txtpUnitStatus.fontSizeMax = 21f;
				txtpUnitStatus.SetText(weapon.Price.ToString("#,0"));
				break;
			case DispStatusType.IconOnly:
				symbolGrid.gameObject.SetActive(false);
                statusRoot.gameObject.SetActive(false);
                txtpUnitStatus.gameObject.SetActive(false);
                footerRoot.gameObject.SetActive(false);
				break;
        }
		this.SetImageIcon(statusType);

		// TODO : 属性アイコンどうする！
		elementIcon.gameObject.SetActive(false);
		//elementIcon.gameObject.SetActive(true);
		//elementIcon.sprite = IconLoader.LoadElementIcon(WeaponData);
        
		// ロック中表示
        this.GetScript<Transform>("img_Grayout").gameObject.SetActive(false);
		// ロック中表示
        this.GetScript<Transform>("img_WeaponLock").gameObject.SetActive(WeaponData.IsLocked && statusType != DispStatusType.RarityAndElementOnly);
		// 外すボタンの表示
		this.GetScript<Transform>("img_UnitRemove").gameObject.SetActive(isDispRemove && statusType != DispStatusType.RarityAndElementOnly);
		// 装備中表示
		this.GetScript<Transform>("img_WeaponEquip").gameObject.SetActive(isDispEquip &&
		                                                                  !this.GetScript<Transform>("img_UnitRemove").gameObject.activeSelf && 
		                                                                  WeaponData.IsEquipped && 
		                                                                  statusType != DispStatusType.RarityAndElementOnly);
		// 最大限界突破済みかどうか.
		// TODO : 素材武器なのかどうか表示
		this.GetScript<Transform>("img_WeaponMaterial").gameObject.SetActive(false);

		var iconButton = GetScript<CustomButton>("IconWeaponDummy");
        if (m_DidTap == null && DidLongPress == null) {
            // コールバックが設定されない場合はボタンとして機能しないようにする。
            iconButton.targetGraphic.raycastTarget = false;
            iconButton.enabled = false;
        } else {
            iconButton.targetGraphic.raycastTarget = true;
            iconButton.enabled = true;
            iconButton.onClick.RemoveListener(DidTapIcon);
            iconButton.onClick.AddListener(DidTapIcon);
            if (DidLongPress == null) {
                iconButton.m_EnableLongPress = false;
            } else {
                iconButton.m_EnableLongPress = true;
                iconButton.onLongPress.RemoveListener(DidLongPressIcon);
                iconButton.onLongPress.AddListener(DidLongPressIcon);
            }
        }
	}
	private void SetImageIcon(DispStatusType statusType)
	{
		this.GetScript<Image>("img_ListIconATK").gameObject.SetActive(statusType == DispStatusType.ATK);
		this.GetScript<Image>("img_ListIconDEF").gameObject.SetActive(statusType == DispStatusType.DEF);
		this.GetScript<Image>("img_ListIconSPD").gameObject.SetActive(statusType == DispStatusType.SPD);
		this.GetScript<Image>("img_ListIconHP").gameObject.SetActive(statusType == DispStatusType.HP);
		this.GetScript<Image>("img_ListIconCr").gameObject.SetActive(statusType == DispStatusType.Price);

	}
     
	public void SetInteractable(bool value)
    {
		foreach (var b in this.gameObject.GetComponentsInChildren<CustomButton>()) {
            b.interactable = value;
        }
    }
   
	public void SetIcon(IconLoadSetting data, Sprite icon)
    {
        // 更新状態によっては内部データが変わっている可能性があるので判定が必要
		if (WeaponData.WeaponId == data.id) {
			var iconImg = this.GetScript<Image>("IconWeaponDummy");
            iconImg.sprite = icon;
        }
    }

	private void DidTapIcon()
    {
        if (m_DidTap != null && !this.IsEnable) {
			m_DidTap(WeaponData);
        }
    }

    private void DidLongPressIcon()
    {
        if (m_DidLongPress != null) {
			m_DidLongPress(WeaponData);
        }
    }

	void OnDestroy()
    {
		if (WeaponData != null) {
			IconLoader.RemoveLoadedEvent(ItemTypeEnum.weapon, WeaponData.WeaponId, SetIcon);
        }
    }
    
	private Action<WeaponData> m_DidTap;
	private Action<WeaponData> m_DidLongPress;

	private Sprite m_Blank;
}
