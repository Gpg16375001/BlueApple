using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using SmileLab;

/// <summary>
/// 起動スクリプト.シーンの起点.
/// </summary>
public class Boot : ViewBase
{
    IEnumerator Start () 
    {
        // 画面向き設定.横向き両対応にする.
        Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        // FPSを30固定にする。
        Application.targetFrameRate = 30;
        // 何らかの強制終了などでTimeScaleがおかしくなっていた場合ここで調整.
        Time.timeScale = 1f;

		var bConfirmedNotes = true;
		View_TitleNotes.DispStart(() => bConfirmedNotes = false);

		while(!AppCore.IsInit){
			yield return null;
		}      

        // チュートリアル中であればブート段階で必要アセットのDLを行う.
        if (AwsModule.ProgressData.TutorialStageNum >= 0) {
            TutorialResourceDownloader.StartDownload();
        }

		while(bConfirmedNotes){
            yield return null;
        }

		ScreenChanger.SharedInstance.GoToTitle();
	}
}
