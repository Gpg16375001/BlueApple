using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// View : 通貨ショップ.
/// </summary>
public class View_CoinShop : PopupViewBase
{
	public static event Action DidBuy;

    /// <summary>
    /// 生成.
    /// </summary>
	public static View_CoinShop Create()
	{
		var go = GameObjectEx.LoadAndCreateObject("Shop/View_CoinShop");
		var c = go.GetOrAddComponent<View_CoinShop>();
		c.InitInternal();
		return c;
	}
    private void InitInternal()
	{
		this.UpdateList();     

		// ボタン.
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
        SetBackButton ();
    }

	public override void Dispose()
	{
		DidBuy = null;
		base.Dispose();
	}

    protected override void DidBackButton ()
    {
        DidTapClose ();
    }

	// リスト更新.
	private void UpdateList()
	{
        LockInputManager.SharedInstance.IsLock = true;
		this.RequestShopProductList(array => {
            m_productList = new ShopProductData[array.Length];
            array.CopyTo(m_productList, 0);
            this.CreateList();
            LockInputManager.SharedInstance.IsLock = false;
        });
		this.GetScript<TextMeshProUGUI>("txtp_Wallet").text = AwsModule.UserData.UserData.GemCount.ToString("#,0"); 

		if(DidBuy != null){
			DidBuy();
		}
	}
	// 通信リクエスト : ショップ商品リスト.
	private void RequestShopProductList(Action<ShopProductData[]> didEnd)
	{
		View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.ShopGetProductList((bSuccess, res) => {
			if(!bSuccess || res== null){
				View_FadePanel.SharedInstance.IsLightLoading = false;
				return;
			}
			AwsModule.UserData.UserData = res.UserData;
			didEnd(res.ShopProductDataList);
			View_FadePanel.SharedInstance.IsLightLoading = false;
		});
	}

    // リスト作成.
    private void CreateList()
	{
		m_list = MasterDataTable.shop_list.DataList.FindAll(s => s.shop_category.name == "クレドを購入");
		var grid = this.GetScript<GridLayoutGroup>("CoinShopItemGrid");
        grid.gameObject.DestroyChildren();
		foreach(var item in m_list){
			var go = GameObjectEx.LoadAndCreateObject("Shop/ListItem_CoinShopItem", grid.gameObject);
			var c = go.GetOrAddComponent<ListItem_CoinShopItem>();
			c.Init(item, Array.Find(m_productList, p => p.ShopProductId == item.id), UpdateList);
		}
	}

	#region ButtonDelegate.

    // ボタン：閉じる.
    void DidTapClose()
	{
        if (IsClosed) {
            return;
        }
        PlayOpenCloseAnimation (false, Dispose);
	}

    #endregion
 
	private List<ShopList> m_list;
	private ShopProductData[] m_productList;
}
