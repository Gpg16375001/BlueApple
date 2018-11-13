using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// View : オープニングムービー.
/// </summary>
public class View_OpMovie : ViewBase
{
	/// <summary>
    /// 生成.
    /// </summary>
	public static View_OpMovie Create(string url, Action didEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("Tutorial/View_OpMovie");
		var c = go.GetOrAddComponent<View_OpMovie>();
		c.InitInternal(url, didEnd);
		return c;
	}
	private void InitInternal(string url, Action didEnd)
	{
		m_url = url;
		m_didEnd = didEnd;      
		this.StartCoroutine("PlayMovie");
		this.SetCanvasButtonMsg("SkipButton", DidTapSkip);
	}

	// 再生開始
	IEnumerator PlayMovie()
	{
		var player = this.GetScript<VideoPlayer>("Movie");
		player.source = VideoSource.Url;
		player.url = m_url;
		player.Play();
		do {
			yield return null;
		} while (!player.isPlaying);
		while (player.isPlaying){
			yield return null;
		}
		if (m_didEnd != null) {
            m_didEnd();
        }
		this.Dispose();
	}   

	// ボタン : スキップボタン押下.
    void DidTapSkip()
	{
		this.StopCoroutine("PlayMovie");
        if (m_didEnd != null) {
            m_didEnd();
        }
        this.Dispose();
	}

	void Awake()
    {
		var player = this.GetScript<VideoPlayer>("Movie");
		player.targetCamera = CameraHelper.SharedInstance.FadeCamera;      

		var canvas = this.GetScript<Canvas>("UIRoot");
		var scaler = canvas.gameObject.GetOrAddComponent<CanvasScaler>();
        if (SystemInfo.deviceModel.Contains("iPad")) {
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0;
        }
    }

	private string m_url;
	private Action m_didEnd;
}
