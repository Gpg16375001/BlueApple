using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : 武器まとめて売却時の確認画面.
/// </summary>
public class View_SaleBundlePop : PopupViewBase
{

    private static View_SaleBundlePop Instanciate()
    {
        var go = GameObjectEx.LoadAndCreateObject("Sale/View_SaleBundlePop");
        return go.GetOrAddComponent<View_SaleBundlePop>();
    }

	/// <summary>
    /// 生成.
    /// </summary>
	public static View_SaleBundlePop Create(List<WeaponData> list)
    {
        var c = Instanciate ();
        c.InitInternal(list);
        return c;
    }
	private void InitInternal(List<WeaponData> list)
	{
		m_weaponList = list;

		// リスト.
		var grid = this.GetScript<GridLayoutGroup>("MaterialGrid");
		grid.gameObject.DestroyChildren();
		foreach(var d in m_weaponList){
			var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", grid.gameObject);
			var c = go.GetOrAddComponent<ListItem_WeaponIcon>();
			c.Init(d, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly, false, true);

            // 限界突破 または レアリティが3 以上は目立たせる
            if (d.LimitBreakGrade >= 1 || d.Rarity >= 3) {
                var eff = GameObjectEx.LoadAndCreateObject("Sale/eff_SaleBundleIcon", go);
                eff.gameObject.GetOrAddComponent<ListItem_SaleBundleEffect>().Init();
                if (d.LimitBreakGrade >= 1) {
                    c.GetScript<Image>("img_NoLimitBreak").gameObject.SetActive(true);
                }
            }
        }

		// ラベル.
		var current = AwsModule.UserData.UserData.GoldCount;
		var sum = m_weaponList.Sum(w => w.Price);
		var bAlaert = m_weaponList.Exists(w => w.Rarity >= 3 || w.LimitBreakGrade > 0);
        this.GetScript<RectTransform>("AlertMagiRoot").gameObject.SetActive(false);
        this.GetScript<RectTransform>("AlertRoot").gameObject.SetActive(bAlaert);
        this.GetScript<RectTransform>("NotesRoot").gameObject.SetActive(!bAlaert);
		this.GetScript<TextMeshProUGUI>("TotalCoin/txtp_Coin").text = current.ToString("#,0"); 
		this.GetScript<TextMeshProUGUI>("ChangeTotalCoin/txtp_Coin").text = (current+sum).ToString("#,0"); 

		// ボタン.
		this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(DidTapClose);
		this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapWeaponOK);
        SetBackButton ();
	}

    /// <summary>
    /// 生成.
    /// </summary>
    public static View_SaleBundlePop Create(List<MagikiteData> list, Action<string, int> didSale)
    {
        var c = Instanciate ();
        c.InitInternal(list, didSale);
        return c;
    }
    private void InitInternal(List<MagikiteData> list, Action<string, int> didSale)
    {
        m_didSale = didSale;
        m_magikiteList = list;

        // リスト.
        var grid = this.GetScript<GridLayoutGroup>("MaterialGrid");
        grid.gameObject.DestroyChildren();
        foreach(var d in m_magikiteList){
            var go = GameObjectEx.LoadAndCreateObject("UnitDetails/ListItem_MagiIcon", grid.gameObject);
            var c = go.GetOrAddComponent<ListItem_MagiIcon>();
            c.UpdateItem(d);
        }

        // ラベル.
        var current = AwsModule.UserData.UserData.GoldCount;
        var sum = m_magikiteList.Sum(w => w.Price);
        var bAlaert = m_magikiteList.Exists(w => w.Magikite.rarity >= 3);
        this.GetScript<RectTransform>("AlertRoot").gameObject.SetActive(false);
        this.GetScript<RectTransform>("AlertMagiRoot").gameObject.SetActive(bAlaert);
        this.GetScript<RectTransform>("NotesRoot").gameObject.SetActive(!bAlaert);
        this.GetScript<TextMeshProUGUI>("TotalCoin/txtp_Coin").text = current.ToString("#,0"); 
        this.GetScript<TextMeshProUGUI>("ChangeTotalCoin/txtp_Coin").text = (current+sum).ToString("#,0"); 

        // ボタン.
        this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(DidTapClose);
        this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapMagikiteOK);
        SetBackButton ();
    }

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

	#region ButtonDelegate.

	// ボタン : 閉じる.
    void DidTapClose()
    {
        if (m_viewSaleOKPop != null) {
            return;
        }

        if (IsClosed) {
            return;
        }
        this.PlayOpenCloseAnimation(false, Dispose);
    }

    // ボタン：OK
    void DidTapWeaponOK()
	{
        if (m_viewSaleOKPop != null) {
            return;
        }

        if (IsClosed) {
            return;
        }
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.WeaponsSellWeapon(m_weaponList.Select(w => w.BagId).ToArray(), (bSuccess, res) => {
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
            if (!bSuccess || res == null){
                return;
			}
			View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
			AwsModule.UserData.UserData = res.UserData;

            m_viewSaleOKPop = WeaponSContoller.CreateCompleteWeaponSalePop("選択したアイテム", m_weaponList.Sum(w => w.Price));
			m_weaponList.CacheDelete(); // 売却処理後の差分更新としてキャッシュ削除.
            this.PlayOpenCloseAnimation(false, Dispose);
		});
	}

    // ボタン：OK
    void DidTapMagikiteOK()
    {
        if (IsClosed) {
            return;
        }
        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;
        SendAPI.MagikitesSellMagikite(m_magikiteList.Select(w => w.BagId).ToArray(), (bSuccess, res) => {
            View_FadePanel.SharedInstance.IsLightLoading = false;
            LockInputManager.SharedInstance.IsLock = false;
            if(!bSuccess || res == null){
                return;
            }
            var price = res.UserData.GoldCount - AwsModule.UserData.UserData.GoldCount;    
            AwsModule.UserData.UserData = res.UserData;         

            m_magikiteList.CacheDelete(); // 売却処理後の差分更新としてキャッシュ削除.
            this.PlayOpenCloseAnimation(false, Dispose);

            if(m_didSale != null) {
                m_didSale("選択したアイテム", price);
            }
        });
    }

    #endregion

	private List<WeaponData> m_weaponList;
    private List<MagikiteData> m_magikiteList;
    private Action<string, int> m_didSale;
    private View_SaleOKPop m_viewSaleOKPop;
}