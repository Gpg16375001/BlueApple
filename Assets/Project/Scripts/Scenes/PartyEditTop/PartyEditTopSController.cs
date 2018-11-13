using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class PartyEditTopSController : ScreenControllerBase {

    public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("PartyEdit/Screen_PartyEditTop", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_PartyEditTop>();
        c.Init ();
    }
}
