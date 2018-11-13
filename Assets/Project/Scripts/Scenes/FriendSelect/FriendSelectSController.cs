using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using System;

public class FriendSelectSController : ScreenControllerBase
{
	public override void Init(Action<bool> didConnectEnd)
	{
        AwsModule.BattleData.SetStage(AwsModule.ProgressData.CurrentQuest.BattleStageID);
		// 情報表示用にマスターデータだけ先んじてロードしておく.
		AwsModule.BattleData.LoadBattleMasterData(bSuccess => { 
			if(!bSuccess){
				didConnectEnd(false);
				return;
			}
			didConnectEnd(true);
		}, true);
	}

	public override void CreateBootScreen ()
    {
        // BGM再生テスト.
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm012, true);

        var go = GameObjectEx.LoadAndCreateObject("FriendSelect/Screen_FriendSelect", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_FriendSelect>();
        c.Init();
    }
}
