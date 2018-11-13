using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : タイトル表示時の注意書き.
/// </summary>
public class View_TitleNotes : ViewBase
{   
    /// <summary>
    /// 表示開始.
    /// </summary>
    public static void DispStart(Action didEnd)
	{
		View_FadePanel.SharedInstance.FadeOut(View_FadePanel.FadeColor.Black, null, true);
		var go = GameObjectEx.LoadAndCreateObject("Title/View_TitleNotes");
		var c = go.GetOrAddComponent<View_TitleNotes>();
		c.InitInternal(didEnd);
	}
	private void InitInternal(Action didEnd)
	{
		this.StartCoroutine(this.CoPlayStart(didEnd));
	}

	private IEnumerator CoPlayStart(Action didEnd)
	{
		yield return new WaitForSeconds(0.5f);

		var bFading = true;
		View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, () => bFading = false);

		while(bFading){
			yield return null;
		}
		// 適当な秒数表示.
		yield return new WaitForSeconds(3f);

		View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => {
			if (didEnd != null) {
                didEnd();
            }
			this.Dispose();
		});      
	}

	private void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}
}
