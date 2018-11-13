using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : 汎用強化演出用View.
/// </summary>
public class View_EnhanceCaption : ViewBase
{   
    /// <summary>
    /// 生成.引数は1=成功、2=大成功、3=超成功.
    /// </summary>
	public static View_EnhanceCaption CreateUnit(int degreeId, Action didEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_EnhanceCaption");
		var c = go.GetOrAddComponent<View_EnhanceCaption>();
		c.InitInternal(degreeId, didEnd, ViewMode.Unit);
		return c;
	}
	/// <summary>
    /// 生成.引数は1=成功、2=大成功、3=超成功.
    /// </summary>
    public static View_EnhanceCaption CreateWeapon(int degreeId, Action didEnd)
    {
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/View_EnhanceCaption");
        var c = go.GetOrAddComponent<View_EnhanceCaption>();
		c.InitInternal(degreeId, didEnd, ViewMode.Weapon);
        return c;
    }

	private void InitInternal(int degreeId, Action didEnd, ViewMode viewMode)
	{
		m_didEnd = didEnd;
		m_viewMode = viewMode;

		// degreeIdに応じて再生アニメーションを変える.
		this.StartCoroutine(PlayEffect(degreeId));
	}   
    // 演出再生.
	IEnumerator PlayEffect(int degreeId)
	{
		var animTypeName = "";
		if(m_viewMode == ViewMode.Unit){
			animTypeName = "chara";
		}else if(m_viewMode == ViewMode.Weapon){
			animTypeName = "weapon";
		}

		SoundManager.SharedInstance.PlaySE(SoundClipName.se009);
		var animName = string.Format("EnhanceCaption_{0}{1}", animTypeName, degreeId.ToString("d2"));
		var anim = this.GetScript<Animation>("AnimParts");
		anim.Play(animName);      
		do {
			yield return null;
		} while (anim.isPlaying);      
		if(m_didEnd != null){
			m_didEnd();
		}
		this.Dispose();
	}

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

	private ViewMode m_viewMode;
	// enum : 表示モード.
    private enum ViewMode
	{
		Unit,
		Weapon,
	}

	private Action m_didEnd;
}
