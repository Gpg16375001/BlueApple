using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : 武器上限突破演出.
/// </summary>
public class View_WeaponLimitBreakMovie : ViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static void Create(Action didEnd = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponLimitBreakMovie");
		var c = go.GetOrAddComponent<View_WeaponLimitBreakMovie>();
		c.WaitEndAnimation(didEnd);
	}
	private void WaitEndAnimation(Action didEnd)
	{
		LockInputManager.SharedInstance.IsLock = true;
		this.StartCoroutine(this.CoWaitEndAnimation(didEnd));
	}
	IEnumerator CoWaitEndAnimation(Action didEnd)
	{
		SoundManager.SharedInstance.PlaySE(SoundClipName.se009);
        var anim = this.GetScript<Animation>("AnimParts");
		anim.Play();
		do {
			yield return null;
		} while (anim.isPlaying);
		if(didEnd != null){
			didEnd();
		}
		this.Dispose();
		LockInputManager.SharedInstance.IsLock = false;
	}

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }
}
