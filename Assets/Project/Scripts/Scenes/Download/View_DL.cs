using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;

public class View_DL : ViewBase {
    private static View_DL Instanse;

    public static View_DL CreateIfMissing()
    {
        if (Instanse != null) {
            return Instanse;
        }

        var go = GameObjectEx.LoadAndCreateObject ("Download/View_DL");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        Instanse = go.GetOrAddComponent<View_DL> ();
        Instanse.Init ();
        return Instanse;
    }

    public static void DisposeProc()
    {
        Instanse.Dispose ();
        Instanse = null;
    }

    private Image gaugeImage;
    private uGUIPageScrollRect pageScroll;
    private TextMeshProUGUI progressText;
    private TextMeshProUGUI dataSize;
    private void Init()
    {
        gaugeImage = GetScript<Image> ("img_DLGauge");
        progressText = GetScript<TextMeshProUGUI> ("txtp_Progress");
        dataSize = GetScript<TextMeshProUGUI> ("txtp_DataSize");
        var infiniteGridLayoutGroup = GetScript<InfiniteGridLayoutGroup> ("Content");
        infiniteGridLayoutGroup.Initialize (true);

        pageScroll = GetScript<uGUIPageScrollRect> ("ScrollBg");
        pageScroll.SetInfinit (true, 7);
        pageScroll.RotationInterval = 10.0f;


        SetCanvasCustomButtonMsg("bt_ArrowPage_2", DidTapRight);
        SetCanvasCustomButtonMsg("bt_ArrowPage_1", DidTapLeft);

        SetProgress (0.0f, 0, 0);
    }

    public void SetProgress(float progress, int now, int max)
    {
        //Debug.Log (progress);
        if (max <= 0) {
            dataSize.SetText (string.Empty);
        } else {
            dataSize.SetTextFormat ("({0}/{1})", now, max);
        }
        progressText.SetTextFormat ("{0:F0}", progress * 100);
        gaugeImage.rectTransform.localScale = new Vector3(progress, 1.0f, 1.0f);
    }

    public void DidTapRight()
    {
        pageScroll.Paging (-1);
    }

    public void DidTapLeft()
    {
        pageScroll.Paging (1);
    }

    void Awake()
    {
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }
}
