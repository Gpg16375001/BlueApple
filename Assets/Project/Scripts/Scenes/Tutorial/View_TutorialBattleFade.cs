using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : チュートリアル中のバトル案内用フェード.上に矢印とか出る.
/// </summary>
public class View_TutorialBattleFade : ViewBase
{   
	/// enum : 表示モード.
	public enum ViewMode 
	{
		None,
		Attack, 
		TimeLine, 
		Target,
		CharacterSkill,
		SPSkill,
	}

    
    /// <summary>
    /// なければ生成.
    /// </summary>
	public static void CreateIfMissing(ViewMode boot)
	{
		if(instance == null){
			var go = GameObjectEx.LoadAndCreateObject("Tutorial/View_TutorialBattle");
            instance = go.GetOrAddComponent<View_TutorialBattleFade>();
		}      
		instance.Initinternal(boot);
	}
	private void Initinternal(ViewMode boot)
	{
		ChangeView(boot);
	}

    /// <summary>
    /// 表示切り替え.
    /// </summary>
	public void ChangeView(ViewMode mode)
	{
		var enums = Enum.GetValues(typeof(ViewMode)) as ViewMode[];
		foreach(var e in enums){
			if(e == ViewMode.None){
				continue;
			}
			this.GetScript<Transform>(e.ToString()).gameObject.SetActive(e == mode);
		}
	}
    
    void Awake()
	{
		this.GetScript<RectTransform>("Root").gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	private static View_TutorialBattleFade instance;
}
