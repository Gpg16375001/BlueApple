using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// ScreenController : アイテムリスト
/// </summary>
public class ItemListSController : ScreenControllerBase 
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public override void Init(System.Action<bool> didConnectEnd)
    {
        if (didConnectEnd != null) {
            didConnectEnd (true);
        }
    }
        
    /// <summary>
    /// 起動画面の生成.
    /// </summary>
    public override void CreateBootScreen()
    {
        // BGM再生テスト.
        SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm009, true);

        var go = GameObjectEx.LoadAndCreateObject("ItemList/Screen_ItemList", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_ItemList> ();
        c.Init();

        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black, null);
    }
}
