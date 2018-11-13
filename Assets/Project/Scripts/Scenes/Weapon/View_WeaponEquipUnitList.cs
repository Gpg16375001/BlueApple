using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : 武器装備可能ユニットの一覧.
/// </summary>
public class View_WeaponEquipUnitList : UnitListBase
{
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_WeaponEquipUnitList Create(WeaponData weapon, Action<CardData> DidSelect, Action<Action> didProc = null)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitList/Screen_UnitList");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponEquipUnitList>();
        c.InitInternal(weapon, DidSelect, didProc);
        return c;
    }
    private void InitInternal(WeaponData weapon, Action<CardData> DidSelect, Action<Action> didProc)
    {
        m_weapon = weapon;
        m_DidSelect = DidSelect;

        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;

        base.Init (0, true, true);

        if (didProc != null) {
            StartCoroutine(CoDidProc(didProc));
        }
    }

    protected override List<CardData> GetCardList ()
    {
        return CardData.CacheGetAll().FindAll(c => c.CanEquipped(m_weapon));
    }

    protected override bool GetDispRemove (CardData card)
    {
        return m_weapon.BagId == card.EquippedWeaponBagId;
    }

    #region Button.

    // アイコンタップ時の処理
    protected override void DidTapIcon(CardData card)
    {
		// 外す場合の挙動.
		if(m_weapon.BagId == card.EquippedWeaponBagId){
            LockInputManager.SharedInstance.IsLock = true;
			View_FadePanel.SharedInstance.IsLightLoading = true;
			SendAPI.CardsSetWeapon(new EquippedWeapon[] { new EquippedWeapon{CardId = card.CardId, WeaponBagId = 0} }, (bSuccess, res) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                res.AffectedCardDataList.CacheSet();
				res.AffectedWeaponDataList.CacheSet();
				m_weapon.IsEquipped = false;
				this.Dispose();
				if(m_DidSelect != null){
					m_DidSelect(null);
				}
			});
			return;
		}
        WeaponSContoller.CreateConfirmWeaponEquipedPop(m_weapon, card, CallbackDecideEquip);
    }
    // コールバック：確認画面より武器装備確定時.
    void CallbackDecideEquip(CardData card)
    {
        if (m_DidSelect != null) {
            m_DidSelect(card);
        }
        this.Dispose();
    }

    // アイコン長押し.
    protected override void DidLongTapIcon (CardData card)
    {
        if (card == null) {
            return;
        }
        // 武器一覧へ.
        View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => {         
			ScreenChanger.SharedInstance.GoToUnitDetails(card, () => {
				// 戻って来る際は現在設定でここに.
				View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => {
                    ScreenChanger.SharedInstance.GoToWeapon(null, true, m_DidBack, () => {
                        WeaponSContoller.CreateWeaponDetailPop(m_weapon, proc => {
                            WeaponSContoller.CreateWeaponEquipUnitListView(m_weapon, data => proc(), didProc => proc());
                        });
                    });
                });
			}, true);
		});      
    }
        
        
    #endregion

    protected override void SortAndFilter ()
    {
        base.SortAndFilter ();

        // はずす表示に関して
        int index = m_SortFilterCardDataList.FindIndex(x => x.EquippedWeaponBagId == m_weapon.BagId);
        if (index >= 0) {
            var removeCard = m_SortFilterCardDataList[index];
            m_SortFilterCardDataList.RemoveAt(index);
            m_SortFilterCardDataList.Insert(0, removeCard);
        }
    }

    IEnumerator CoDidProc(Action<Action> didProc)
    {
        yield return null;
        didProc(() => { });
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    private WeaponData m_weapon;
    private Action<CardData> m_DidSelect;
    private Action m_DidBack;

    private bool m_IsDispRemove;
    private int m_RemoveTargetCardID;
}
