using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


public class View_MainQuest_Debug : ViewBase
{   
	/// <summary>
    /// 演出確認用.
    /// </summary>
	public static void CreateEffectCheck(Action didClose)
	{
		var go = new GameObject("debug_for_effect_check");
		var c = go.GetOrAddComponent<View_MainQuest_Debug>();
		c.InitInternalEffectCheck(didClose);
	}
	private void InitInternalEffectCheck(Action didClose)
	{
		this.LoadEffectCheckAdvProject(() => {
			UtageModule.SharedInstance.SetActiveCore(true);
			UtageModule.SharedInstance.StartScenario("__UtageCustomCommandTest", () => {
				UtageModule.SharedInstance.SetActiveCore(false);
				this.Dispose();
				didClose();
            }, false);
		});      
	}   
	public void LoadEffectCheckAdvProject(Action didLoad)
	{
		UtageModule.SharedInstance.LoadUseChapter("__UtageCustomCommandTest", () => {
            Debug.Log("[TutorialSController] Utage Load Success.");
			didLoad();
		});
	}

	/// <summary>
    /// 演出確認用.
    /// </summary>
    public static void CreateFujiReview(Action didClose)
    {
        var go = new GameObject("fuji_review");
        var c = go.GetOrAddComponent<View_MainQuest_Debug>();
		c.InitInternalFujiReview(didClose);
    }
	private void InitInternalFujiReview(Action didClose)
    {
		this.LoadFujiReviewAdvProject(() => {
            UtageModule.SharedInstance.SetActiveCore(true);
			UtageModule.SharedInstance.StartScenario("__FujiForReview", () => {
                UtageModule.SharedInstance.SetActiveCore(false);
                this.Dispose();
                didClose();
            }, false);
        });      
    }
	public void LoadFujiReviewAdvProject(Action didLoad)
    {
		UtageModule.SharedInstance.LoadUseChapter("__FujiForReview", didLoad);
    }
}