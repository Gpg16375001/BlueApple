using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// View : 章解放演出View.
/// </summary>
public class View_ChapterOpen : ViewBase
{   
    /// <summary>
    /// 生成.
    /// </summary>
	public static View_ChapterOpen Create(MainQuestChapterInfo chapterInfo, Action didEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_ChapterOpen");
		var c = go.GetOrAddComponent<View_ChapterOpen>();
		c.InitInternal(chapterInfo, didEnd);
		return c;
	}
	private void InitInternal(MainQuestChapterInfo chapterInfo, Action didEnd)
	{
		AwsModule.ProgressData.UpdateSeenChapterReleaseEffectList(chapterInfo);
		this.GetScript<TextMeshProUGUI>("txtp_ChapterTitle").text = chapterInfo.chapter_name;
		this.GetScript<TextMeshProUGUI>("txtp_ChapterNum").text = chapterInfo.chapter.ToString();
		this.StartCoroutine(this.CoPlayOpenChapter(didEnd));
	}
	private IEnumerator CoPlayOpenChapter(Action didEnd)
	{
		LockInputManager.SharedInstance.IsLock = true;
		var anim = this.GetScript<Animation>("AnimParts");
		anim.Play();
		do{
			yield return null;
		}while (anim.isPlaying);
		if(didEnd != null){
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