using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ScreenController : アイテムショップ
/// </summary>
public class ShopSController : ScreenControllerBase
{
	public override void Dispose()
	{
        if (m_screenShop != null) {
            m_screenShop.Dispose ();
        }
		base.Dispose();
	}

	public override void Init(Action<bool> didConnectEnd)
	{
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm010, true);
		SendAPI.ShopGetProductList((bSuccess, res) => {
			if(!bSuccess || res == null){
				didConnectEnd(false);
				return;
			}
			AwsModule.UserData.UserData = res.UserData;
			m_productList = new ShopProductData[res.ShopProductDataList.Length];
			res.ShopProductDataList.CopyTo(m_productList, 0);
			didConnectEnd(true);
		});
	}

	public override void CreateBootScreen()
	{
		var go = GameObjectEx.LoadAndCreateObject("Shop/Screen_Shop", this.gameObject);
		m_screenShop = go.GetOrAddComponent<Screen_Shop>();
		m_screenShop.Init(m_productList);
	}

	private ShopProductData[] m_productList;
	private Screen_Shop m_screenShop;
}
