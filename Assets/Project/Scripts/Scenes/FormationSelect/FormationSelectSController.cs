using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class FormationSelectSController : ScreenControllerBase
{
    public bool IsPvp;
    public Action BackScene;

    public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("FormationSelect/Screen_FormationSelect", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_FormationSelect>();
        c.Init(IsPvp, BackScene);
    }
}
