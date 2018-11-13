using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class SelectUnitSController : ScreenControllerBase
{
    /// <summary>
    /// バトル準備か否か.
    /// </summary>
    public bool IsBattleInit { get; set; }

    public bool IsGlobalMenu { get; set; }

    public bool IsDispOrganizing { get; set; }

    public bool IsPvP { get; set; }

    public bool IsDispRemove { get; set; }

    public int RemoveTargetCardID;

    public int FocusTargetCardID;

    public ElementEnum? DispOnlyElement;

    public System.Action<CardData> DidSelect;

    public System.Action DidBack;

    public override void CreateBootScreen()
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitList/Screen_UnitList", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_SelectUnit>();
        c.Init(IsBattleInit, IsDispOrganizing, IsPvP, IsDispRemove, RemoveTargetCardID, FocusTargetCardID, DispOnlyElement, IsGlobalMenu, DidSelect, DidBack);
    }
}
