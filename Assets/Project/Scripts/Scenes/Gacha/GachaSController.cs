using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net;
using SmileLab.Net.API;
using System;


/// <summary>
/// ScreenController : ガチャ.
/// </summary>
public class GachaSController : ScreenControllerBase
{
	/// <summary>
    /// 通信してからその情報を元に画面を構成する.
    /// </summary>
	public override void Init(Action<bool> didConnectEnd)
	{
		SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm010, true);
		
		SendAPI.GachaGetProductList((bSuccess, res) => {
			if (!bSuccess || res == null) {
				Debug.LogError("[GachaSController] Init Error!! : Request error.");
				didConnectEnd(false);
				return;
			}
			AwsModule.UserData.UserData = res.UserData;
			m_data = new GachaClientUseData(res);
			View_GlobalMenu.Setup( m_data );
			didConnectEnd(true);
		});
	}

	/// <summary>
	/// 起動画面生成.
	/// </summary>
	public override void CreateBootScreen()
	{
		var go = GameObjectEx.LoadAndCreateObject("Gacha/Screen_Gacha", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_Gacha>();
		c.Init(m_data);
	}
 
	private GachaClientUseData m_data;
}
