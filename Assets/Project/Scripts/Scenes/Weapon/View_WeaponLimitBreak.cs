using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using SmileLab.UI;
using TMPro;


/// <summary>
/// View : 武器限界突破View.
/// </summary>
public class View_WeaponLimitBreak : ViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_WeaponLimitBreak Create(WeaponData weapon, Action<WeaponData> didClose)
    {
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponLimitBreak");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponLimitBreak>();
        c.InitInternal(weapon, didClose);
        return c;
    }
	private void InitInternal(WeaponData weapon, Action<WeaponData> didClose)
	{
		m_weapon = weapon;
		m_didClose = didClose;
  
		// リスト設定.
        m_listView = this.GetComponent<WeaponListView>();
		m_listView.DidUpdateList += CallbackUpdateList;
		m_listView.DidCallbackUpateListItem += CallbackDidUpdateListItem;
		m_listView.DidTapIconEvent += DidTapWeaponMaterial;
		m_listView.DidLongTapIconEvent += DidLongTapWeaponMaterial;
		m_listView.Init(removeTarget: null, 
		                sortType: WeaponListView.SortType.LimitBreak,
		                invisibleWeapons: WeaponData.CacheGetAll().Where(w => w.BagId == m_weapon.BagId || w.WeaponId != m_weapon.WeaponId).ToArray());

		// 初回情報表示.
		this.UpdateLimitBreakInfo();
		this.UpdateSelectedMaterialList();

		// ボタン.
		this.SetCanvasCustomButtonMsg("Deselect/bt_TopLineGray", DidTapSelectAllClear, false);
		this.SetCanvasCustomButtonMsg("DoLimitBreak/bt_Common", DidTapLimitBreak, false);
        for (var i = 0; i < 9; ++i) {
            var btnName = string.Format("bt_SelectMaterialBase{0}", i+1);
            var btIdx = i+0;
            this.GetScript<CustomButton>(btnName).onClick.AddListener(() => DidTapSelectClear(btIdx));
        }
	}
    // コールバック : リスト更新.
    void CallbackUpdateList()
    {
		var nowGrade = Math.Min(m_weapon.LimitBreakGrade+m_materialList.Count, 9);
        var cnt = m_listView.WeaponIconList.Count;
        var bExistChooseMaterial = m_materialList.Count > 0;
        this.GetScript<RectTransform>("NoItem").gameObject.SetActive(cnt <= 0);
        this.GetScript<CustomButton>("Deselect/bt_TopLineGray").interactable = bExistChooseMaterial;
        this.GetScript<CustomButton>("DoLimitBreak/bt_Common").interactable = bExistChooseMaterial;
        foreach (var item in m_listView.WeaponIconList) {
            item.IsEnable = nowGrade < 9 && item.WeaponData.LimitBreakGrade < 1 && m_materialList.Count < 9 && !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped || m_materialList.Any(r => r.BagId == item.WeaponData.BagId);
            item.IsNoLimitBreak = item.WeaponData.LimitBreakGrade < 1;
            item.IsSelected = m_materialList.Exists(m => m.BagId == item.WeaponData.BagId);
            item.SetInteractable(true);
        }
        UpdateSelectedMaterialList();
    }
    // コールバック : ListView側のリストアイテム更新時に呼ばれる.
    private void CallbackDidUpdateListItem(int index, GameObject updateObj)
    {
        var lists = updateObj.GetComponentsInChildren<ListItem_WeaponIcon>(true);
        if (lists == null || lists.Length <= 0) {
            return; // 未初期化.コンポーネントがくっついていない.
        }
        var item = lists[0];

        var isUpdate = false;
        foreach (var x in m_listView.WeaponIconList) {
            if (x.WeaponData.IsLocked && m_materialList.Any()) {
                m_materialList.RemoveAll(r => r.BagId == x.WeaponData.BagId && x.WeaponData.IsLocked);
                isUpdate = true;
            }
        }

        // 選択中素材の更新.
        var nowGrade = Math.Min(m_weapon.LimitBreakGrade + m_materialList.Count, 9);
        if (m_materialList.Count > 0) {
            item.IsEnable = nowGrade < 9 && item.WeaponData.LimitBreakGrade < 1 && m_materialList.Count < 9 && !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped || m_materialList.Any(r => r.BagId == item.WeaponData.BagId);
            item.IsSelected = m_materialList.Exists(m => m.BagId == item.WeaponData.BagId);
            item.SetInteractable(true);
        }

        if (isUpdate) {
            UpdateLimitBreakInfo();
            UpdateSelectedMaterialList();
        }
    }

	// 上限突破情報表示の更新.
	private void UpdateLimitBreakInfo()
	{
		// 武器アイコン
		var rootObj = this.GetScript<RectTransform>("SelectWeapon/WeaponIcon").gameObject;
		var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", rootObj);
		go.GetOrAddComponent<ListItem_WeaponIcon>().Init(m_weapon, ListItem_WeaponIcon.DispStatusType.IconOnly);

		// ラベル
		var bExistChooseMaterial = m_materialList.Count > 0;
		var totalCost = m_materialList.Sum(m => m.CostUsedMaterial);
		this.GetScript<TextMeshProUGUI>("txtp_WeaponName").text = m_weapon.Weapon.name;
		this.GetScript<TextMeshProUGUI>("txtp_GetEnhance").text = m_materialList.Sum(m => m.ExpGainedMaterial).ToString();
		this.GetScript<TextMeshProUGUI>("txtp_Coin").text = totalCost.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_SelectNum").text = m_materialList.Count.ToString();
		this.GetScript<Image>("img_IconUpArrow").gameObject.SetActive(bExistChooseMaterial);
		this.GetScript<CustomButton>("Deselect/bt_TopLineGray").interactable = bExistChooseMaterial;
        this.GetScript<CustomButton>("DoLimitBreak/bt_Common").interactable = bExistChooseMaterial;

		// リミットブレイクアイコン.     
		var nowGrade = Math.Min(m_weapon.LimitBreakGrade+m_materialList.Count, 9);
        for (int i = 1; i <= 9; ++i) {
            var starOn = this.GetScript<Transform>(string.Format("SelectWeaponSymbol{0}/LimitBreakIconOn", i));
			var starAdd = this.GetScript<Transform>(string.Format("SelectWeaponSymbol{0}/LimitBreakIconAdd", i));
			var starOff = this.GetScript<Transform>(string.Format("SelectWeaponSymbol{0}/LimitBreakIconOff", i));         
            starOn.gameObject.SetActive(nowGrade >= i);
			starAdd.gameObject.SetActive(m_weapon.LimitBreakGrade < i && nowGrade >= i);
			starOff.gameObject.SetActive(nowGrade < i);
        }
        
        foreach (var item in m_listView.WeaponIconList) {
            item.IsEnable = nowGrade < 9 && item.WeaponData.LimitBreakGrade < 1 && m_materialList.Count < 9 && !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped || m_materialList.Any(r => r.BagId == item.WeaponData.BagId);
            item.IsNoLimitBreak = item.WeaponData.LimitBreakGrade < 1;
            item.IsSelected = m_materialList.Exists(m => m.BagId == item.WeaponData.BagId);
            item.SetInteractable(true);
        }

	}
	// 選択中のマテリアルリスト更新.
    private void UpdateSelectedMaterialList()
    {
        for (var i = 0; i < 9; ++i) {
			var iconRootName = string.Format("bt_SelectMaterialBase{0}/WeaponIcon", i+1);
            var rootObj = this.GetScript<RectTransform>(iconRootName).gameObject;
            if (m_materialList.Count <= i) {
                rootObj.DestroyChildren();
                continue;
            }
            var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", rootObj);
            go.GetOrAddComponent<ListItem_WeaponIcon>().Init(m_materialList[i], ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);
        }
    }

	/// <summary>
    /// 破棄.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        if (m_didClose != null) {
            m_didClose(m_weapon);
        }
    }

	#region ButtionDelegate.

    // ボタン : 上限突破.
    void DidTapLimitBreak()
    {
        // クレドの不足
		var cost = m_materialList.Sum(m => m.CostUsedMaterial);
		if(AwsModule.UserData.UserData.GoldCount < cost) {
			PopupManager.OpenPopupOK("限界突破に必要なクレドが足りません。");
			return;
		}

        // 確認ポップアップ      
        if (m_materialList.Any(x => x.Weapon.rarity.rarity >= 3 || x.LimitBreakGrade >= 1)) {
            View_SaleAlertPop.CreateRarityAleart (m_weapon,
                (didCall) => {
                    LimitBreak();
                    didCall();
                },
                (didCall) => {
                    didCall();
                }
            );
        } else {
            LimitBreak();
        }
    }

    void LimitBreak()
	{
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        View_GlobalMenu.IsVisible = false;
        View_PlayerMenu.IsVisible = false;

        SendAPI.WeaponsLimitBreakWeapon(m_weapon.BagId, m_materialList.Select(m => m.BagId).ToArray(), (bSuccess, res) =>{

            View_FadePanel.SharedInstance.IsLightLoading = false;
            if (!bSuccess || res == null){
                View_GlobalMenu.IsVisible = true;
                View_PlayerMenu.IsVisible = true;
			    LockInputManager.SharedInstance.IsLock = false;
                return;
			}

            // 演出表示.
            View_WeaponLimitBreakMovie.Create(() => {
                LockInputManager.SharedInstance.IsLock = false;
                View_WeaponLimitBreakResult.Create(m_weapon, res.WeaponData, () => {
                    res.WeaponData.CacheSet();
                    m_weapon = res.WeaponData;
                    View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
                    AwsModule.UserData.UserData = res.UserData;
                    UpdateLimitBreakInfo();
                    View_GlobalMenu.IsVisible = true;
                    View_PlayerMenu.IsVisible = true;
                    if (m_weapon.LimitBreakGrade >= 9){
                        this.Dispose();
                    }
                });
            });
        });

        // 選択表示の差分更新.
        m_materialList.CacheDelete();
        m_materialList.Clear();
        m_listView.UpdateList();
        this.UpdateSelectedMaterialList();
    }

    // ボタン : 各武器素材タップ.
    void DidTapWeaponMaterial(WeaponData weapon)
	{
		var item = m_listView.WeaponIconList.Find(w => w.WeaponData.BagId == weapon.BagId);
        // すでに選択追加済み素材の場合は選択解除する.
        if (item.IsSelected) {
            item.IsSelected = false;
            m_materialList.Remove(item.WeaponData);
            this.UpdateLimitBreakInfo();
		    this.UpdateSelectedMaterialList();
            return;
        }
		// 素材は9まで.
        if (m_materialList.Count >= 9) {
            return;
        }
        // 追加.
        item.IsSelected = true;
		m_materialList.Add(weapon);
		this.UpdateLimitBreakInfo();
		this.UpdateSelectedMaterialList();
	}

	// ボタン : 素材リストアイコン長押し.
	void DidLongTapWeaponMaterial(WeaponData weapon)
	{
        // 通常.
        WeaponSContoller.CreateWeaponDetailPop(weapon, false, this.m_listView.UpdateList);
    }

	// ボタン : 単体素材解除.
    void DidTapSelectClear(int index)
    {
        if (m_materialList.Count <= index) {
            return;
        }
        m_materialList.RemoveAt(index);
		this.UpdateLimitBreakInfo();
		this.UpdateSelectedMaterialList();
    }   

    // ボタン : 選択全解除.
    void DidTapSelectAllClear()
    {
        m_materialList.Clear();
		this.UpdateLimitBreakInfo();
		this.UpdateSelectedMaterialList();
        m_listView.WeaponIconList
                  .Select(l => l.IsSelected = false)
                  .ToList();
    }

    #endregion

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private WeaponData m_weapon;
	private Action<WeaponData> m_didClose;
	private WeaponListView m_listView;
	private List<WeaponData> m_materialList = new List<WeaponData>();
}
