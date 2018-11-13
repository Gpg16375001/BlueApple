using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : 売却時のアラート系ポップアップ.
/// </summary>
public class View_SaleAlertPop : PopupViewBase
{

	/// <summary>
	/// 武器単体売却.
	/// </summary>
	public static View_SaleAlertPop CreateSingleSale(WeaponData weapon, Action<int> didSale)
	{
		var c = Instanciate();
		c.InitInternal(ViewMode.SingleSale, weapon, didSale);
		return c;
	}

    /// <summary>
    /// 武器レアリティアラート.
    /// </summary>
	public static View_SaleAlertPop CreateRarityAleart(WeaponData weapon, Action didSale)
    {
		var c = Instanciate();
        c.InitInternal(ViewMode.RarityAleart, weapon, (price) => {
            if (didSale != null) {
                didSale();
            }
        });
        return c;
    }

    /// <summary>
    /// 武器レアリティアラート.
    /// </summary>
	public static View_SaleAlertPop CreateRarityAleart(WeaponData weapon, Action<Action> didOk, Action<Action> didCancel)
    {
		var c = Instanciate();
        c.InitInternal(ViewMode.RarityAleart, weapon, didOk, didCancel);
        return c;
    }

    /// <summary>
    /// マギカイト単体売却.
    /// </summary>
    public static View_SaleAlertPop CreateSingleSale(MagikiteData magikite, Action<int> didSale)
    {
        var c = Instanciate();
        c.InitInternal(ViewMode.SingleSale, magikite, didSale);
        return c;
    }

    /// <summary>
    /// マギカイトレアリティアラート.
    /// </summary>
    public static View_SaleAlertPop CreateRarityAleart(MagikiteData magikite, Action didSale)
    {
        var c = Instanciate();
        c.InitInternal(ViewMode.RarityAleart, magikite, (price) => {
            if(didSale != null) {
                didSale();
            }
        });
        return c;
    }

	private static View_SaleAlertPop Instanciate()
	{
		var go = GameObjectEx.LoadAndCreateObject("Sale/View_SaleAlertPop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        return go.GetOrAddComponent<View_SaleAlertPop>();
	}
 
    // 内部初期化.
	private void InitInternal(ViewMode mode, WeaponData weapon, Action<int> didSale)
	{
        m_didRarityOk = didSale;
        m_currentMode = mode;
        m_ItemType = ItemTypeEnum.weapon;
		m_weapon = weapon;
        m_magikite = null;

        this.GetScript<TextMeshProUGUI> ("txtp_SaleSingleItemName").SetText (m_weapon.Weapon.name);
		this.GetScript<RectTransform>("RarityAlert").gameObject.SetActive(m_currentMode == ViewMode.RarityAleart);
		this.GetScript<RectTransform>("SaleSingleAlert").gameObject.SetActive(m_currentMode == ViewMode.SingleSale);

		if(m_currentMode == ViewMode.RarityAleart){
            this.GetScript<CustomButton>("OK/bt_Common").SetClickSe(SoundClipName.se002);
            this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapOK);
            this.GetScript<TextMeshProUGUI> ("txtp_ItemType").SetText(MasterDataTable.item_type[(int)ItemTypeEnum.weapon].display_name);
		}else if(m_currentMode == ViewMode.SingleSale){
			this.GetScript<TextMeshProUGUI>("txtp_Price").text = m_weapon.Price.ToString("#,0");
			this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapOK);
		}      
		this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(DidTapCancel);
        SetBackButton ();
	}
 
    // 内部初期化.
	private void InitInternal(ViewMode mode, WeaponData weapon, Action<Action> didOk, Action<Action> didCancel)
	{
		m_currentMode = mode;
        m_ItemType = ItemTypeEnum.weapon;
		m_weapon = weapon;

		if(m_currentMode != ViewMode.RarityAleart){
            return;
		}    

        this.GetScript<CustomButton>("OK/bt_Common").SetClickSe(SoundClipName.se002);
        this.GetScript<TextMeshProUGUI> ("txtp_SaleSingleItemName").SetText (m_weapon.Weapon.name);
		this.GetScript<RectTransform>("RarityAlert").gameObject.SetActive(m_currentMode == ViewMode.RarityAleart);
		this.GetScript<RectTransform>("SaleSingleAlert").gameObject.SetActive(m_currentMode == ViewMode.SingleSale);
        this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(() => { didOk(Close); });
        this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(() => { didCancel(Close); });
        SetBackButton ();
	}

    // 内部初期化.
    private void InitInternal(ViewMode mode, MagikiteData magikite, Action<int> didSale)
    {
        m_didRarityOk = didSale;
        m_didSaleMagikite = didSale;
        m_currentMode = mode;
        m_ItemType = ItemTypeEnum.magikite;
        m_magikite = magikite;
        m_weapon = null;

        this.GetScript<TextMeshProUGUI> ("txtp_SaleSingleItemName").SetText (m_magikite.Magikite.name);
        this.GetScript<RectTransform>("RarityAlert").gameObject.SetActive(m_currentMode == ViewMode.RarityAleart);
        this.GetScript<RectTransform>("SaleSingleAlert").gameObject.SetActive(m_currentMode == ViewMode.SingleSale);

        if(m_currentMode == ViewMode.RarityAleart){
            this.GetScript<RectTransform> ("txtp_Alert1").gameObject.SetActive (false);
            this.GetScript<TextMeshProUGUI> ("txtp_ItemType").SetText(MasterDataTable.item_type[(int)ItemTypeEnum.magikite].display_name);
            this.GetScript<CustomButton>("OK/bt_Common").SetClickSe(SoundClipName.se002);
            this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapOK);
        } else if(m_currentMode == ViewMode.SingleSale){
            this.GetScript<TextMeshProUGUI>("txtp_Price").text = m_magikite.Price.ToString("#,0");
            this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapOK);
        }
        this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(DidTapCancel);
        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

	#region ButtonDelegate

	// ボタン : OK
    void DidTapOK()
	{
        if (IsClosed) {
            return;
        }
 
        // TODO : モードに応じた行動.
        if (m_currentMode == ViewMode.SingleSale){
            if (m_ItemType == ItemTypeEnum.weapon) {
                this.RequestWeaponSale ();
            } else if(m_ItemType == ItemTypeEnum.magikite) {
                this.RequestMagikiteSale ();
            }
        } else if(m_currentMode == ViewMode.RarityAleart) {
            if (m_didRarityOk != null) {
                m_didRarityOk (0);
            }
            this.Close();
        } else {
            this.Close();
		}
    }

    // ボタン：キャンセル
    void DidTapCancel()
	{
        if (IsClosed) {
            return;
        }
        this.Close();
    }

    void Close()
    {
        PlayOpenCloseAnimation (false, () => {
            if(m_didClose != null) {
                m_didClose();
            }
            Dispose();
        });
    }

    #endregion

    // 通信リクエスト : 武器売却.
    private void RequestWeaponSale()
	{
		m_didClose = () => WeaponSContoller.CreateCompleteWeaponSalePop(m_weapon.Weapon.name, m_weapon.Price);

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.WeaponsSellWeapon(new long[] { m_weapon.BagId }, (bSuccess, res) => { 
            LockInputManager.SharedInstance.IsLock = false;
			View_FadePanel.SharedInstance.IsLightLoading = false;
            if (!bSuccess || res == null){
				return;
			}
			AwsModule.UserData.UserData = res.UserData;
			View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);

			m_weapon.CacheDelete();
            Close();
		});
	}

    // 通信リクエスト : マギカイト売却.
    private void RequestMagikiteSale()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.MagikitesSellMagikite(new long[] { m_magikite.BagId }, (bSuccess, res) => {
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
            if(!bSuccess || res == null){
                return;
            }
            var price = res.UserData.GoldCount - AwsModule.UserData.UserData.GoldCount;
            AwsModule.UserData.UserData = res.UserData;

            m_magikite.CacheDelete();
            Close();

            m_didSaleMagikite (price);
        });
    }

	// enum : 表示モード
	private enum ViewMode
	{
        RarityAleart,
		SingleSale,
	}

	private ViewMode m_currentMode;
    private ItemTypeEnum m_ItemType;

	private WeaponData m_weapon;

    private MagikiteData m_magikite;

	private Action m_didClose;
    private Action<int> m_didSaleMagikite;
    private Action<int> m_didRarityOk;

    private bool m_isTap;
}
