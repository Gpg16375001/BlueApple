using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class PartyDetailsSController : ScreenControllerBase
{
    public bool IsBattleInit;
    public bool IsPvP;
    public Action BackScene;

    public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("PartyDetails/Screen_PartyDetails", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_PartyDetails>();
        c.Init(BackScene, IsBattleInit, IsPvP);
    }
}
