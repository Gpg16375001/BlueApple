using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class PlayerSupportEditSController : ScreenControllerBase
{
    public bool DispGlobalMenu { get; set; }
    public string PrevSceneName { get; set; }

	public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("PlayerSupportEdit/Screen_PlayerSupportEdit", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        go.GetOrAddComponent<Screen_PlayerSupportEdit> ().Init (DispGlobalMenu, string.IsNullOrEmpty(PrevSceneName) ? ScreenChanger.SharedInstance.PrevSceneName : PrevSceneName);
    }
}
