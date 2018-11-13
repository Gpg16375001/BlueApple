using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// View : クレドショップのOKポップ.
/// </summary>
public class View_CoinShopOk : ViewBase
{   
    /// <summary>
    /// 生成.
    /// </summary>
	public static void Create(ShopList item, Action didTrade)
	{
		var go = GameObjectEx.LoadAndCreateObject("Shop/View_CoinShopOKPop");
		var c = go.GetOrAddComponent<View_CoinShopOk>();
		c.InitInternal(item, didTrade);
	}
	private void InitInternal(ShopList item, Action didTrade)
	{
		m_shopItem = item;
		m_didTrade = didTrade;

		// ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_SelectCoinItem").text = item.product.count_1.ToString("#,0");
		this.GetScript<TextMeshProUGUI>("txtp_Price").text = item.use_item_count.ToString("#,0");

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("Cancel/bt_CommonS01", DidTapClose);
		this.SetCanvasCustomButtonMsg("Exchange/bt_CommonS02", DidTapTrade);
	}


	#region ButtoneDelegate.

	// ボタン : 閉じる.
	void DidTapClose()
	{
		this.StartCoroutine(this.CoPlayClose());
	}

	// ボタン : 購入する.
    void DidTapTrade()
	{
        LockInputManager.SharedInstance.IsLock = true;
		View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.ShopPurchaseProduct(m_shopItem.id, 1, (bSuccess, res) => { 
            LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
			if(!bSuccess || res == null){
				return;
			}
			AwsModule.UserData.UserData = res.UserData;
            View_PlayerMenu.CreateIfMissing().UpdateView(res.UserData);
			PopupManager.OpenPopupOK("購入しました。", m_didTrade);
			this.StartCoroutine(this.CoPlayClose());  // 確認ポップを閉じてから閉じると後ろのViewのボタンを閉じる間際に押せるので並列して閉じておく.
		});      
	}

    #endregion

	IEnumerator CoPlayClose(Action didEnd = null)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didEnd != null) {
            didEnd();
        }
		this.Dispose();
    }

	void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	private ShopList m_shopItem;
	private Action m_didTrade;
}
