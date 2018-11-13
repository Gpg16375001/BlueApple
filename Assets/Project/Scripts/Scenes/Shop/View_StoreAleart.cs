using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : ショップアイテムが一つも取得できなかった時専用のアラートポップ.
/// </summary>
public class View_StoreAleart : ViewBase
{

    /// <summary>
    /// 生成.
    /// </summary>
    public static void Create(Action didClose)
	{
		var go = GameObjectEx.LoadAndCreateObject("Shop/View_StoreAlert");
		var c = go.GetOrAddComponent<View_StoreAleart>();
		c.InitInternal(didClose);
	}
    private void InitInternal(Action didClose)
	{
		m_didClose = didClose;      
		this.SetCanvasCustomButtonMsg("R/bt_Common", DidTapOK);
	}

    void DidTapOK()
	{
		this.StartCoroutine(this.CoPlayClose());
	}
	IEnumerator CoPlayClose()
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
		if (m_didClose != null) {
			m_didClose();
        }
        this.Dispose();
    }

    void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.FadeCamera);
	}

	private Action m_didClose;
}
