using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

using TMPro;
using UnityEngine.UI;


/// <summary>
/// Screen : 武器画面.
/// </summary>
[RequireComponent(typeof(WeaponListView))]
public class Screen_WeaponList : ViewBase
{   
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(CardData equipCard = null, Action backSceneCallback = null)
	{
		m_equipCard = equipCard;
        m_didBackScene = () => {
		    if(m_viewWeaponEquip != null){
                m_viewWeaponEquip.Dispose();
			    return;
		    }
		    if(m_viewWeaponDetailsPop != null){
                m_viewWeaponDetailsPop.Dispose();
			    return;
		    }
		    if(m_viewWeaponSaleList != null){
                m_viewWeaponSaleList.Dispose();
			    return;
		    }
            backSceneCallback();
        };

		View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
		View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;

        var weaponSortData = AwsModule.LocalData.WeaponSortData;

        // リスト設定.
        m_listView = this.GetComponent<WeaponListView>();
		m_listView.DidUpdateList += CallbackUpdateList;
		m_listView.DidTapIconEvent += DidTapIcon;
		m_listView.DidLongTapIconEvent += DidLongTapIcon;
		m_listView.Init(removeTarget: m_equipCard != null ? WeaponData.CacheGet(m_equipCard.EquippedWeaponBagId): null, 
		                afterSortType: (WeaponListView.SortType)weaponSortData.SortType,
		                invisibleWeapons: m_equipCard != null ? WeaponData.CacheGetAll().FindAll(w => !w.CanEquipped(m_equipCard)).ToArray(): new WeaponData[0]);
        m_listView.UpdateWeaponSortData();
        m_listView.UpdateFilterData();
        m_listView.UpdateList();

		// ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_WeaponLimit").text = string.Format("/ {0}", AwsModule.UserData.UserData.WeaponBagCapacity);
		this.GetScript<TextMeshProUGUI>("txtp_WeaponTotal").text = WeaponData.CacheGetAll().Count.ToString();

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_Ascentd", DidTapOrder);
		this.SetCanvasCustomButtonMsg("bt_Descend", DidTapOrder);
		this.GetScript<CustomButton>("Sale/bt_Common").onClick.AddListener(DidTapSale);
		this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.isDescending);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.isDescending);

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
	}

	/// <summary>
	/// リスト更新.キャッシュの取り直しから行う.
	/// </summary>
	public void UpdateList()
	{
        this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.isDescending);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.isDescending);
        m_listView.UpdateSortDropDownCaption();
        m_listView.UpdateFilterData();
        m_listView.UpdateList();      
	}

	// コールバック : リスト更新.
    void CallbackUpdateList()
	{
		// 表示数更新.
		var cnt = WeaponData.CacheGetAll().Count;
		this.GetScript<TextMeshProUGUI>("txtp_WeaponTotal").text = cnt.ToString();
		this.GetScript<RectTransform>("NoItem").gameObject.SetActive(cnt <= 0);
	}

    #region ButtonDelegate.

    // 並び替え.
	private void DidTapOrder()
    {
        m_listView.isDescending = !m_listView.isDescending;
        this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.isDescending);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.isDescending);
        m_listView.UpdateList();
    }

    // 売却
    void DidTapSale()
	{
        if(IsOpenViews()){
            return;
        }

        m_listView.DisposeFilterPop();
        m_viewWeaponSaleList = WeaponSContoller.CreateWeaponSaleListView();
	}

	// アイコンタップ時の処理
	void DidTapIcon(WeaponData weapon)
    {
        if(IsOpenViews()){
            return;
        }

		// 装備者の設定がある場合ははずすか直接装備.
		if(m_equipCard != null){
			// はずす.
			if(m_equipCard.EquippedWeaponBagId == weapon.BagId){
				this.RequestRemoveEquip(() => {
					// 外す場合は原則そのままシーンバック.
					if (m_didBackScene != null) {
                        m_didBackScene();
                    } else {
                        m_listView.UpdateList();
                    }
				});
			}
			// 直接装備.
			else{
                m_viewWeaponEquip = WeaponSContoller.CreateConfirmWeaponEquipedPop(weapon, m_equipCard, card => {
                    m_viewWeaponEquip.Dispose();
                    m_viewWeaponEquip = null;

                    if (m_didBackScene != null){
						m_didBackScene(); 
					}else{
						m_listView.UpdateList();
					}
				});
			}
			return;
		}

        // 通常.
        m_viewWeaponDetailsPop = WeaponSContoller.CreateWeaponDetailPop(weapon, true, () => {
            m_listView.UpdateWeaponSortData();
            this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.isDescending);
            this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.isDescending);
            this.m_listView.UpdateList();
        });
    }

	// アイコン長押し時の処理
	void DidLongTapIcon(WeaponData weapon)
    {
        if(IsOpenViews()){
            return;
        }

		// 装備者の設定がある場合ははずすか直接装備.
		if(m_equipCard != null){
            m_viewWeaponDetailsPop = WeaponSContoller.CreateWeaponDetailPop(weapon, false, this.m_listView.UpdateList);
        }
    }

    #endregion   

	// 通信リクエスト : 装備解除.
    private void RequestRemoveEquip(Action didEnd)
	{
		if(m_equipCard == null){
			if(didEnd != null){
				didEnd();
			}
			return;
		}
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.CardsSetWeapon(new EquippedWeapon[] { new EquippedWeapon{CardId = m_equipCard.CardId, WeaponBagId=0}}, (bSuccess, res) => {
            res.AffectedCardDataList.CacheSet();
            res.AffectedWeaponDataList.CacheSet();
			if (didEnd != null) {
                didEnd();
            }
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
        });      
	}

    /// <summary>
    /// なにがしかのViewが開いている
    /// </summary>
    /// <returns><c>true</c> if this instance is open views; otherwise, <c>false</c>.</returns>
    bool IsOpenViews()
    {
        return m_viewWeaponEquip != null || m_viewWeaponDetailsPop != null || m_viewWeaponSaleList != null;
    }

	void Awake()
	{
		var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
	}

	private CardData m_equipCard;   // 装備対象者.ルート時点でこれを設定している場合繊維が異なる.
	private Action m_didBackScene;
	private WeaponListView m_listView;
    private View_WeaponEquip m_viewWeaponEquip;
    private View_WeaponDetailsPop m_viewWeaponDetailsPop;
    private View_WeaponSaleList m_viewWeaponSaleList;
}
