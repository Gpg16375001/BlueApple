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
/// View : 武器まとめて売却画面.
/// </summary>
[RequireComponent(typeof(WeaponListView))]
public class View_WeaponSaleList : ViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_WeaponSaleList Create(Action didClose)
    {
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponSaleList");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_WeaponSaleList>();
		c.InitInternal(didClose);
        return c;
    }
    private void InitInternal(Action didClose)
	{
        m_didClose = didClose;

        // リスト設定.
        m_listView = this.GetComponent<WeaponListView>();
		m_listView.DidTapIconEvent += DidTapIcon;
		m_listView.DidLongTapIconEvent += DidLongTapIcon;
		m_listView.DidUpdateList += CallbackUpdateList;
		m_listView.DidCallbackUpateListItem += DidCallbackUpdateListItem;
		m_listView.Init(null, m_listView.sortType);

        this.UpdateSaleInfo();

		// ラベル.
        this.GetScript<TextMeshProUGUI>("txtp_WeaponLimit").text = string.Format("/ {0}", AwsModule.UserData.UserData.WeaponBagCapacity);
        this.GetScript<TextMeshProUGUI>("txtp_WeaponTotal").text = WeaponData.CacheGetAll().Count.ToString();

		// ボタン.
		this.GetScript<CustomButton>("Deselect/bt_TopLineGray").onClick.AddListener(DidTapAllDeselecte);
		this.GetScript<CustomButton>("Sale/bt_Common").onClick.AddListener(DidTapSale);
        this.SetCanvasCustomButtonMsg("bt_Ascentd", DidTapOrder);
        this.SetCanvasCustomButtonMsg("bt_Descend", DidTapOrder);
        this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.isDescending);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.isDescending);
    }

    // コールバック : リスト更新.
    void CallbackUpdateList()
    {
        // 表示数更新.
        var cnt = WeaponData.CacheGetAll().Count;
        this.GetScript<TextMeshProUGUI>("txtp_WeaponTotal").text = cnt.ToString();
        this.GetScript<RectTransform>("NoItem").gameObject.SetActive(cnt <= 0);

        // 選択中武器の更新.
        foreach (var item in m_listView.WeaponIconList) {
            if (m_saleDataList.Exists(m => m.BagId == item.WeaponData.BagId)) {
                m_saleDataList.Remove(m_saleDataList.Find(m => m.BagId == item.WeaponData.BagId));
                item.IsSelected = true;
                m_saleDataList.Add(item.WeaponData);
            }
        }

        UpdateSaleInfo();
    }

	#region ButtonDelegate

    // 並び替え.
	private void DidTapOrder()
    {
        m_listView.isDescending = !m_listView.isDescending;
        this.GetScript<Image>("bt_Ascentd").gameObject.SetActive(!m_listView.isDescending);
        this.GetScript<Image>("bt_Descend").gameObject.SetActive(m_listView.isDescending);
        m_listView.UpdateList();
    }

    // アイコンタップ.
	void DidTapIcon(WeaponData weapon)
	{      
		var item = m_listView.WeaponIconList.Find(w => w.WeaponData.BagId == weapon.BagId);
		if(item.IsSelected){
			item.IsSelected = false;
			m_saleDataList.Remove(item.WeaponData);
			this.UpdateSaleInfo();
			return;
		}
		if(item.WeaponData.IsEquipped){
			return; // 装備中の武器は売却できない.
		}
		if (m_saleDataList.Count >= 10) {
            return; // 10個以上選択できない.
        }

		item.IsSelected = true;
		m_saleDataList.Add(item.WeaponData);
		this.UpdateSaleInfo();
	}

	// ボタン : 素材リストアイコン長押し.
	void DidLongTapIcon(WeaponData weapon)
	{
        // 通常.
        WeaponSContoller.CreateWeaponDetailPop(weapon, false, () => {
            this.m_listView.UpdateList();
        });
    }

    // 全選択解除.
    void DidTapAllDeselecte()
	{
		m_listView.WeaponIconList.Select(w => w.IsSelected= false).ToList();
		m_saleDataList.Clear();
		this.UpdateSaleInfo();
	}

    // 売却.
    void DidTapSale()
	{
		WeaponSContoller.CraeteConfirmWeaponSaleListPop(m_saleDataList);
	}

    #endregion

	/// <summary>
    /// 破棄.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        if (m_didClose != null) {
            m_didClose();
        }
    }

    // 売却情報更新.
    private void UpdateSaleInfo()
	{
        var bExistChooseMaterial = m_saleDataList.Count > 0;
        var current = AwsModule.UserData.UserData.GoldCount;
		var sum = m_saleDataList.Sum(item => item.Price);
		this.GetScript<TextMeshProUGUI>("txtp_SelectNum").text = m_saleDataList.Count.ToString();   // 選択数(10max)
	    this.GetScript<TextMeshProUGUI>("txtp_GetCoin").text = sum.ToString("#,0");             // 取得できるコイン.
		this.GetScript<TextMeshProUGUI>("txtp_Coin").text = (current+sum).ToString("#,0");      // 売却後の所持コイン.      
        this.GetScript<CustomButton>("Sale/bt_Common").interactable = bExistChooseMaterial;
		this.GetScript<CustomButton>("Deselect/bt_TopLineGray").interactable = bExistChooseMaterial;
        foreach (var item in m_listView.WeaponIconList) {
            item.SetInteractable(true);
            m_saleDataList.RemoveAll(r => r.BagId == item.WeaponData.BagId && item.WeaponData.IsLocked);
        }
    }

    // コールバック : ListView側のリストアイテム更新時に呼ばれる.
    private void DidCallbackUpdateListItem(int index, GameObject updateObj)
	{
		var lists = updateObj.GetComponentsInChildren<ListItem_WeaponIcon>(true);
        if (lists == null || lists.Length <= 0) {
            return; // 未初期化.コンポーネントがくっついていない.
        }
        var item = lists[0];

        foreach (var x in m_listView.WeaponIconList) {
            if (x.WeaponData.IsLocked && m_saleDataList.Any()) {
                m_saleDataList.RemoveAll(r => r.BagId == x.WeaponData.BagId && x.WeaponData.IsLocked);
            }
        }

		// 選択中武器の更新.
		if (item.IsEnable && m_saleDataList.Count > 0) {
			item.IsSelected = m_saleDataList.Exists(m => m.BagId == item.WeaponData.BagId);
		}
		// 装備中の武器は売却できない仕様.
		item.IsEnable = !item.WeaponData.IsLocked && !item.WeaponData.IsEquipped;
	}

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    private Action m_didClose;
    private WeaponListView m_listView;
	private List<WeaponData> m_saleDataList = new List<WeaponData>();
}
