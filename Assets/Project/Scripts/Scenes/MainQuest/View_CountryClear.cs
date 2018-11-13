using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// View : 国クリア演出View.
/// </summary>
public class View_CountryClear : ViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_CountryClear Create(Belonging belonging, Action didEnd)
    {
		var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_CountryClear");
		var c = go.GetOrAddComponent<View_CountryClear>();
		c.InitInternal(belonging, didEnd);
        return c;
    }
	private void InitInternal(Belonging belonging, Action didEnd)
    {
		AwsModule.ProgressData.UpdateSeenCountryClearEffectList(belonging.Enum);
		this.GetScript<TextMeshProUGUI>("txtp_Country").text = belonging.name;
		this.GetScript<uGUISprite>("EmblemIcon").ChangeSprite(((int)belonging.Enum).ToString());
        this.StartCoroutine(this.CoPlayOpenChapter(didEnd));
    }
    private IEnumerator CoPlayOpenChapter(Action didEnd)
    {
		LockInputManager.SharedInstance.IsLock = true;
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play();
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didEnd != null) {
            didEnd();
        }
        this.Dispose();
		LockInputManager.SharedInstance.IsLock = false;
    }

    void Awake()
    {
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }
}