using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class UnitListSController : ScreenControllerBase {
    public override void Init (System.Action<bool> didConnectEnd)
    {
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm009, true);
		didConnectEnd(true);
    }

    public override void CreateBootScreen ()
    {
        // 
        var go = GameObjectEx.LoadAndCreateObject("UnitList/Screen_UnitList", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_UnitList>();
        c.Init(null);
    }
}
