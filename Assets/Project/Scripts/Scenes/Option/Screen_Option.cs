using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// Screen : 設定画面.
/// </summary>
public class Screen_Option : ViewBase
{
    /// <summary>
    /// TODO : 初期化.
    /// </summary>
    public void Init(OptionBootMenu bootMode)
    {
        // Screenに連なるViewの操作をする必要があるので専用コントローラをアタッチ.
        this.gameObject.GetOrAddComponent<OptionViewController>().ChangeView(bootMode);

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;

        // フェードを開ける.
        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }
}
