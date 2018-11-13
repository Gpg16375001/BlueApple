using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// ScreenController : マイページ
/// </summary>
public class OptionSController : ScreenControllerBase
{
	public override void Dispose()
	{
		View_PlayerMenu.IsEnableButtons = true;
		View_GlobalMenu.IsEnableButtons = true;
		base.Dispose();
	}

	/// <summary>
	/// 起動モード.
	/// </summary>
	public OptionBootMenu BootMenu { get; set; }

	public override void Init(Action<bool> didConnectEnd)
	{
		SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm009, true);
		base.Init(didConnectEnd);
	}

	/// <summary>
	/// 起動画面生成.
	/// </summary>
	public override void CreateBootScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("Option/Screen_Option", this.gameObject);
        var c = go.GetOrAddComponent<Screen_Option>();
        c.Init(BootMenu);
    }
}