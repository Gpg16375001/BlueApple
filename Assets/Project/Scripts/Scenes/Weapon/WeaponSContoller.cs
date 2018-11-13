using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ScreenController : 武器画面.
/// </summary>
public class WeaponSContoller : ScreenControllerBase
{
    /// <summary>前のシーンに戻るコールバック.</summary>
    public Action BackSceneProc { get; set; }
    /// <summary>装備者.</summary>
    public CardData EquipedCard { get; set; }


    /// <summary>初期化.</summary>
    public override void Init(Action<bool> didConnectEnd)
    {
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm009, true);
        View_PlayerMenu.DidTapBackButton += CallbackDidTapBack;
        RequestWeaponList(didConnectEnd);
    }

    // 通信リクエスト : 武器一覧.
    private static void RequestWeaponList(Action<bool> didConnectEnd)
    {
        SendAPI.WeaponsGetWeaponList((bSuccess, res) => {
            if (!bSuccess || res == null) {
                Debug.LogError("[WeaponSContoller] Init Error!! : Request error.");
                didConnectEnd(false);
                return;
            }
            res.WeaponDataList.CacheSet();
            didConnectEnd(true);
        });
    }

    /// <summary>起動画面作成.</summary>
    public override void CreateBootScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("Weapon/Screen_WeaponList", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        rootScreen = go.GetOrAddComponent<Screen_WeaponList>();
        rootScreen.Init(EquipedCard, BackSceneProc);
    }

    #region Create View.

    /// <summary>武器詳細画面ポップアップ作成.</summary>
    public static View_WeaponDetailsPop CreateWeaponDetailPop(WeaponData weapon, Action<Action> didProc = null)
    {
        if (weapon == null) {
            return null;
        }
        var view = View_WeaponDetailsPop.Create(weapon, rootScreen.UpdateList, didProc);
        viewStack.Push(view);
        return view;

    }

    /// <summary>武器詳細画面ポップアップ作成.</summary>
    public static View_WeaponDetailsPop CreateWeaponDetailPop(WeaponData weapon, bool isSceq, Action didUpdate, Action<Action> didProc = null)
    {
        if (weapon == null) {
            return null;
        }
        var view = View_WeaponDetailsPop.Create(weapon, didUpdate, didProc, isSceq);
        viewStack.Push(view);
        return view;
    }

    /// <summary>武器装備用ユニット一覧画面作成.</summary>
	public static View_WeaponEquipUnitList CreateWeaponEquipUnitListView(WeaponData weapon, Action<CardData> didSelect, Action<Action> didProc = null)
    {
        if (weapon == null) {
            return null;
        }
        var view = View_WeaponEquipUnitList.Create(weapon, didSelect, didProc);
        viewStack.Push(view);
        return view;
    }

    /// <summary>武器装備最終確認ポップ.</summary>
	public static View_WeaponEquip CreateConfirmWeaponEquipedPop(WeaponData weapon, CardData card, Action<CardData> didEquip)
    {
        if (weapon == null) {
            return null;
        }
        var view = View_WeaponEquip.Create(weapon, card, didEquip);
        viewStack.Push(view);
        return view;
    }

    /// <summary>武器強化画面作成.</summary>
	public static View_WeaponEnhance CreateWeaponEnhanceView(WeaponData weapon, Action<WeaponData> didClose)
    {
        if (weapon == null) {
            return null;
        }
        var view = View_WeaponEnhance.Create(weapon, (data) => {
            rootScreen.gameObject.SetActive(true);
            didClose(data);
        });
        rootScreen.gameObject.SetActive(false);
        viewStack.Push(view);
        return view;
    }

    /// <summary>武器上限突破画面作成.</summary>
	public static View_WeaponLimitBreak CreateWeaponLimitBreakView(WeaponData weapon, Action<WeaponData> didClose)
    {
        if (weapon == null) {
            return null;
        }
        var view = View_WeaponLimitBreak.Create(weapon, didClose);
        viewStack.Push(view);
        return view;
    }

    /// <summary>武器売却画面作成.</summary>
    public static View_SaleAlertPop CreateWeaponSaleView(WeaponData weapon)
    {
        if (weapon == null) {
            return null;
        }
        View_SaleAlertPop view = null;
        if (weapon.Rarity >= 3 || weapon.LimitBreakGrade > 0) {
            view = View_SaleAlertPop.CreateRarityAleart(weapon, () => View_SaleAlertPop.CreateSingleSale(weapon, (price) => { }));
        } else {
            view = View_SaleAlertPop.CreateSingleSale(weapon, (price) => { });
        }
        viewStack.Push(view);
        return view;
    }

    /// <summary>武器まとめて売却リスト画面作成.</summary>
    public static View_WeaponSaleList CreateWeaponSaleListView()
	{
        var view = View_WeaponSaleList.Create(() => {
            rootScreen.gameObject.SetActive(true);
            rootScreen.UpdateList();
        });
        rootScreen.gameObject.SetActive(false);
        viewStack.Push(view);
        return view;
	}

    /// <summary>武器まとめて売却確認ポップ.</summary>
	public static View_SaleBundlePop CraeteConfirmWeaponSaleListPop(List<WeaponData> list)
	{
		var view = View_SaleBundlePop.Create(list);
		viewStack.Push(view);
        return view;
	}

    /// <summary>武器売却完了ポップ.</summary>
    public static View_SaleOKPop CreateCompleteWeaponSalePop(string name, int price)
	{
		var view = View_SaleOKPop.Create(name, price, RefleshViewList);
        return view;
	}

    /// <summary>フィルター.</summary>
	public static View_WeaponFilterPop CreateFilterPop(Action<WeaponFilterSetting.Data> didClose)
	{
		var view = View_WeaponFilterPop.Create(didClose);
        viewStack.Push(view);
		return view;
	}

	#endregion

    // 上に重なるViewのリフレッシュ.
	private static void RefleshViewList()
	{
		// ルートの画面に戻るがちらつくので一旦暗転させてとのこと.
		View_FadePanel.SharedInstance.FadeOut(View_FadePanel.FadeColor.Black, () => {
			UniRx.MainThreadDispatcher.StartCoroutine(ClearAndWaitRefleshView(() => View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black)));
        });
	}
    static IEnumerator ClearAndWaitRefleshView(Action didClear)
	{
		foreach (var v in viewStack) {
            if (v == null || v.IsDestroyed) {
                continue;
            }
            v.Dispose();
        }

		var bEnd = true;
		do {
			bEnd = true;
			foreach (var v in viewStack) {
				bEnd &= v.IsDestroyed;
			}
			yield return null;
		}while(!bEnd);

		viewStack.Clear();
		rootScreen.UpdateList();
		didClear();
	}

	// コールバック : ヘッダーより戻る.
	void CallbackDidTapBack()
	{
        if (m_isTapBack){
            return;
        }

		if(viewStack.Count > 0){
			var view = viewStack.Pop();
			if (view.IsDestroyed) {
                CallbackDidTapBack();
                return; // 自身で破棄している.
            }
			if(!view.gameObject.activeSelf){
				view.Dispose();
				CallbackDidTapBack();
                return; // 自身で破棄している.
			}         
			view.Dispose();         
			return;
		}
        m_isTapBack = true;
        if (BackSceneProc != null){
			BackSceneProc();
		}else{
			View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
		}
	}

	private static Screen_WeaponList rootScreen;
	private static Stack<ViewBase> viewStack = new Stack<ViewBase>();
    private bool m_isTapBack;
}
