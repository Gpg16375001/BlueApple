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
/// View : 武器強化画面.
/// </summary>
public class View_WeaponEnhance : ViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_WeaponEnhance Create(WeaponData weapon, Action<WeaponData> didClose)
    {
        var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponEnhance");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponEnhance>();
        c.InitInternal(weapon, didClose);
        return c;
    }
    private void InitInternal(WeaponData weapon, Action<WeaponData> didClose)
    {
        m_weapon = weapon;
        m_didClose = didClose;

        // リスト設定.
        m_listView = this.GetComponent<WeaponListView>();
        m_listView.DidUpdateList += CallbackDidUpdateList;
        m_listView.DidCallbackUpateListItem += CallbackDidUpdateListItem;
        m_listView.DidTapIconEvent += DidTapMaterialIcon;
        m_listView.DidLongTapIconEvent += DidLongTapMaterialIcon;
        m_listView.Init(removeTarget: null,
                        sortType: WeaponListView.SortType.Rarity,  // TODO: 今後ソートタイプを保存しときたくなるかもしれない？ > m_listView.CurrentSortType
                        invisibleWeapons: WeaponData.CacheGetAll().Where(w => w.BagId == m_weapon.BagId || w.WeaponId == m_weapon.WeaponId).ToArray());

        // 初回情報表示
        this.UpdateReinforceInfo();
        this.UpdateSelectedMaterialList();

        // ボタン.
        this.GetScript<CustomButton>("Enhance/bt_Common").onClick.AddListener(DidTapReinforce);
        this.GetScript<CustomButton>("Deselect/bt_TopLineGray").onClick.AddListener(DidTapSelectAllClear);
        this.SetCanvasCustomButtonMsg("bt_Ascentd", DidTapOrder);
        this.SetCanvasCustomButtonMsg("bt_Descend", DidTapOrder);
        this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(true);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(false);
        for (var i = 0; i < 10; ++i) {
            var btnName = string.Format("bt_SelectMaterialBase{0}", i + 1);
            var btIdx = i + 0;
            this.GetScript<CustomButton>(btnName).onClick.AddListener(() => DidTapSelectClear(btIdx));
        }
    }
    // コールバック : ListView側のリスト全体更新時に呼ばれる.
    private void CallbackDidUpdateList()
    {
        var bExistChooseMaterial = m_materialList.Count > 0;
        this.GetScript<CustomButton>("Deselect/bt_TopLineGray").interactable = bExistChooseMaterial;
        this.GetScript<CustomButton>("Enhance/bt_Common").interactable = bExistChooseMaterial;

        // 選択中武器の更新.
        var enhancePoint = m_materialList.Sum(m => m.ExpGainedMaterial);
        var exp = m_weapon.Exp + enhancePoint;
        var lv = Math.Min(MasterDataTable.weapon_level.GetLevel(m_weapon.Weapon.level_table_id, exp), m_weapon.CurrentLimitBreakMaxLevel);
        foreach (var item in m_listView.WeaponIconList) {
            if (m_materialList.Exists(m => m.BagId == item.WeaponData.BagId)) {
                m_materialList.Remove(m_materialList.Find(m => m.BagId == item.WeaponData.BagId));
                m_materialList.Add(item.WeaponData);
            }
            item.IsEnable = lv < m_weapon.CurrentLimitBreakMaxLevel && m_materialList.Count < 10 && !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped || m_materialList.Any(r => r.BagId == item.WeaponData.BagId);
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

        // 素材として選択中にロックされたとき.
        var isUpdate = false;
        foreach (var x in m_listView.WeaponIconList) {
            if (x.WeaponData.IsLocked && m_materialList.Any(r => r.BagId == x.WeaponData.BagId)) {
                m_materialList.Remove(m_materialList.First(r => r.BagId == x.WeaponData.BagId));
                isUpdate = true;
            }
        }

        // 選択中素材の更新.
        var enhancePoint = m_materialList.Sum(m => m.ExpGainedMaterial);
        var exp = m_weapon.Exp + enhancePoint;
        var lv = Math.Min(MasterDataTable.weapon_level.GetLevel(m_weapon.Weapon.level_table_id, exp), m_weapon.CurrentLimitBreakMaxLevel);
        if (m_materialList.Count > 0) {
            item.IsEnable = lv < m_weapon.CurrentLimitBreakMaxLevel && m_materialList.Count < 10 && !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped || m_materialList.Any(r => r.BagId == item.WeaponData.BagId);
            item.IsSelected = m_materialList.Exists(m => m.BagId == item.WeaponData.BagId);
            item.SetInteractable(true);
        }

        if (isUpdate) {
            UpdateReinforceInfo();
            UpdateSelectedMaterialList();
        }
    }

    // 強化情報表示の更新.
    private void UpdateReinforceInfo()
    {
        // 武器アイコン
        var rootObj = this.GetScript<RectTransform>("SelectWeapon/WeaponIcon").gameObject;
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", rootObj);
        go.GetOrAddComponent<ListItem_WeaponIcon>().Init(m_weapon, ListItem_WeaponIcon.DispStatusType.IconOnly);

        // -- ラベル類 --
        var bExistChooseMaterial = m_materialList.Count > 0;
		var totalCost = m_materialList.Sum(m => m.CostUsedMaterial);
        var enhancePoint = m_materialList.Sum(m => m.ExpGainedMaterial);
        this.GetScript<TextMeshProUGUI>("txtp_UnitLv").text = m_weapon.Level.ToString();
        this.GetScript<Image>("img_ChangeArrow").gameObject.SetActive(bExistChooseMaterial);
        this.GetScript<TextMeshProUGUI>("txtp_AfterUnitLv").gameObject.SetActive(bExistChooseMaterial);
        this.GetScript<TextMeshProUGUI>("txtp_LimitLv").text = m_weapon.CurrentLimitBreakMaxLevel.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_WeaponName").text = m_weapon.Weapon.name;
        this.GetScript<TextMeshProUGUI>("txtp_GetEnhance").text = enhancePoint.ToString();
        this.GetScript<Image>("img_IconUpArrow").gameObject.SetActive(bExistChooseMaterial);
        this.GetScript<TextMeshProUGUI>("txtp_Coin").text = totalCost.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_SelectNum").text = m_materialList.Count.ToString();
        this.GetScript<CustomButton>("Deselect/bt_TopLineGray").interactable = bExistChooseMaterial;
        this.GetScript<CustomButton>("Enhance/bt_Common").interactable = bExistChooseMaterial;

        // 強化に応じたレベルアップ情報.
        var exp = m_weapon.Exp + enhancePoint;
        var lv = Math.Min(MasterDataTable.weapon_level.GetLevel(m_weapon.Weapon.level_table_id, exp), m_weapon.CurrentLimitBreakMaxLevel);
        var currentLvExp = lv < m_weapon.CurrentLimitBreakMaxLevel ? MasterDataTable.weapon_level.GetCurrentLevelExp(m_weapon.Weapon.level_table_id, lv, exp) : 0;
        var nextLvExp = lv < m_weapon.CurrentLimitBreakMaxLevel ? MasterDataTable.weapon_level.GetNextLevelExp(m_weapon.Weapon.level_table_id, lv, exp) : 0;
        this.GetScript<TextMeshProUGUI>("txtp_AfterUnitLv").text = lv.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_LvPoint").text = currentLvExp.ToString();
        this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").text = nextLvExp.ToString();
        this.GetScript<Image>("EXP_Gauge").fillAmount = (float)currentLvExp / (float)nextLvExp;

        // 最大レベルで素材選択できないように.
        foreach (var item in m_listView.WeaponIconList) {
            item.IsEnable = lv < m_weapon.CurrentLimitBreakMaxLevel && m_materialList.Count < 10 && !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped || m_materialList.Any(r => r.BagId == item.WeaponData.BagId);
            item.IsSelected = m_materialList.Exists(m => m.BagId == item.WeaponData.BagId);
            item.SetInteractable(true);
        }
    }
    // 選択中のマテリアルリスト更新.
    private void UpdateSelectedMaterialList()
    {
        for (var i = 0; i < 10; ++i) {
            var iconRootName = string.Format("bt_SelectMaterialBase{0}/WeaponIcon", i + 1);
            var rootObj = this.GetScript<RectTransform>(iconRootName).gameObject;
            if (m_materialList.Count <= i) {
                rootObj.DestroyChildren();
                continue;
            }
            var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", rootObj);
            go.GetOrAddComponent<ListItem_WeaponIcon>().Init(m_materialList[i], ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);
        }

        // 選択中武器の更新.
        foreach (var item in m_listView.WeaponIconList) {
            if (m_materialList.Exists(m => m.BagId == item.WeaponData.BagId)) {
                m_materialList.Remove(m_materialList.Find(m => m.BagId == item.WeaponData.BagId));
                item.IsSelected = true;
                m_materialList.Add(item.WeaponData);
            }
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

    #region ButtonDelegate.

    // 並び替え.
    private void DidTapOrder()
    {
        m_listView.Descending = !m_listView.Descending;
        this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.Descending);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.Descending);
        m_listView.UpdateList();
    }

    // ボタン : 強化する.
    void DidTapReinforce()
    {
        // クレドの不足
		var cost = m_materialList.Sum(m => m.CostUsedMaterial);
		if(AwsModule.UserData.UserData.GoldCount < cost) {
			PopupManager.OpenPopupOK("強化に必要なクレドが足りません。");
			return;
		}

        // 確認ポップアップ      
        if (m_materialList.Any(x => x.Weapon.rarity.rarity >= 3 || x.LimitBreakGrade >= 1)) {
            View_SaleAlertPop.CreateRarityAleart (m_weapon,
                (didCall) => {
                    Reinforce();
                    didCall();
                },
                (didCall) => {
                    didCall();
                }
            );
        } else {
            Reinforce();
        }
    }
    void Reinforce()
    {
        LockInputManager.SharedInstance.IsLock = true;
		View_FadePanel.SharedInstance.IsLightLoading = true;
        View_GlobalMenu.IsVisible = false;
        View_PlayerMenu.IsVisible = false;

        // 通信.
        SendAPI.WeaponsReinforceWeapon(m_weapon.BagId, m_materialList.Select(m => m.BagId).ToArray(), (bSuccess, res) => {

            View_FadePanel.SharedInstance.IsLightLoading = false;
            if (!bSuccess || res == null) {
                View_GlobalMenu.IsVisible = true;
                View_PlayerMenu.IsVisible = true;
			    LockInputManager.SharedInstance.IsLock = false;
                return;
            }
			// 成功演出.
            View_EnhanceCaption.CreateWeapon(res.ReinforcementDegreeId, () => {
                // カウントアップ演出.
                m_routineCountUp = this.StartCoroutine(PlayCountUpAnimation(res.WeaponData));
                res.WeaponData.CacheSet();
                View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
                AwsModule.UserData.UserData = res.UserData;
            });
		});

		// 表示の差分更新.
		m_materialList.CacheDelete();
        m_materialList.Clear();
		m_listView.UpdateList();
		this.UpdateSelectedMaterialList();
	}
	// カウントアップ演出開始
	private IEnumerator PlayCountUpAnimation(WeaponData targetData)
	{      
        // 演出終了後の共通情報更新処理定義.
		Action didUpdateInfo = () => { 
			LockInputManager.SharedInstance.IsLock = false;

            // レベルアップ時はリザルトを表示.
            if (m_weapon.Level < targetData.Level) {
                m_viewEnhanceResult = View_WeaponEnhanceResult.Create(m_weapon, targetData, () => {
                    View_GlobalMenu.IsVisible = true;
                    View_PlayerMenu.IsVisible = true;
                    if (targetData.Level >= targetData.CurrentLimitBreakMaxLevel){
                        this.Dispose();
                    }
                });
                this.GetScript<RectTransform>("SelectWeapon/WeaponIcon").GetComponentInChildren<ListItem_WeaponIcon>().Init(targetData, ListItem_WeaponIcon.DispStatusType.IconOnly);
            } else {
                View_GlobalMenu.IsVisible = true;
                View_PlayerMenu.IsVisible = true;
            }
            m_weapon = targetData;
            this.UpdateReinforceInfo();
		};

		// スキップ待機.
        this.StartCoroutine(WaitSkipTap(() => {
            if (m_routineCountUp != null) {
                this.StopCoroutine(m_routineCountUp);
            }
			didUpdateInfo();
        }));

		var exp = m_weapon.Exp;
		var nextExp = (float)m_weapon.NextLevelExp;
		var currentExp = (float)m_weapon.CurrentLevelExp;
		var level = m_weapon.Level;
		var addVal = Mathf.Ceil(nextExp * Time.unscaledDeltaTime);
		while (exp < targetData.Exp) { 
			var per = (float)currentExp / (float)nextExp;
            this.GetScript<Image>("EXP_WhitePanel_1").fillAmount = per;
			currentExp += addVal;
            exp += (int)addVal;
            this.GetScript<TextMeshProUGUI>("txtp_LvPoint").text = currentExp.ToString();
            // レベルアップ.
            if (currentExp >= nextExp) {
                this.GetScript<TextMeshProUGUI>("txtp_UnitLv").text = (++level).ToString();
				nextExp = MasterDataTable.weapon_level.GetNextLevelExp(m_weapon.Weapon.level_table_id, level, exp);
                currentExp = 0;
				addVal = Mathf.Ceil(nextExp * Time.unscaledDeltaTime);
                this.GetScript<TextMeshProUGUI>("txtp_NextLvPoint").text = nextExp.ToString();
				this.GetScript<Animation>("Lv").Play();
            }
			yield return null;
		}

		didUpdateInfo();
	}
	private IEnumerator WaitSkipTap(Action didSkip)
    {
        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                didSkip();
                break;
            }
            yield return null;
        }
        if (m_viewEnhanceResult != null) {
            yield return null;
            while (true) {
                if (Input.GetMouseButtonDown(0)) {
                    m_viewEnhanceResult.Dispose();
                    break;
                }
                yield return null;
            }
            do {
                yield return null;
            } while (m_viewEnhanceResult != null && !m_viewEnhanceResult.IsDestroyed);
            m_viewEnhanceResult = null;
        }
    }

	// ボタン : 素材リストアイコンタップ.
	void DidTapMaterialIcon(WeaponData data)
	{
		var item = m_listView.WeaponIconList.Find(w => w.WeaponData.BagId == data.BagId);
		// すでに選択追加済み素材の場合は選択解除する.
		if(item.IsSelected){
			item.IsSelected = false;
			m_materialList.Remove(item.WeaponData);
			this.UpdateReinforceInfo();  
			this.UpdateSelectedMaterialList();
			return;
		}
		// 選択上限は10個.
        if (m_materialList.Count >= 10) {
            return;
        }
        // 追加.
		item.IsSelected = true;
		m_materialList.Add(data);
        this.UpdateReinforceInfo();
		this.UpdateSelectedMaterialList();
	}

	// ボタン : 素材リストアイコン長押し.
	void DidLongTapMaterialIcon(WeaponData data)
	{
        // 通常.
        WeaponSContoller.CreateWeaponDetailPop(data, false, this.m_listView.UpdateList);
    }

    // ボタン : 単体素材解除.
    void DidTapSelectClear(int index)
	{
		if(m_materialList.Count <= index){
            return;
        }
		m_materialList.RemoveAt(index);
        this.UpdateReinforceInfo();  
		this.UpdateSelectedMaterialList();
	}

	// ボタン : 選択全解除.
    void DidTapSelectAllClear()
    {
        m_materialList.Clear();
        this.UpdateReinforceInfo();
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

	private Coroutine m_routineCountUp;
	private View_WeaponEnhanceResult m_viewEnhanceResult;
}
