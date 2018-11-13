using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class PartyChangeSController : ScreenControllerBase
{
    public bool IsBattleInit;

    public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("PartyChange/Screen_PartyChange", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_PartyChange>();
        c.Init(IsBattleInit);
    }
}
