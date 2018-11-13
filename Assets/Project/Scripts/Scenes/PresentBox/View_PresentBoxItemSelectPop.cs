using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_PresentBoxItemSelectPop : ViewBase
{
    public static View_PresentBoxItemSelectPop Create()
    {
        var go = GameObjectEx.LoadAndCreateObject("PresentBox/View_PresentBoxItemSelectPop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var c = go.GetOrAddComponent<View_PresentBoxItemSelectPop>();
        c.InitInternal();
        return c;
    }

    // TODO: 本当は交換対象候補データをもらわないといけない
    private void InitInternal()
    {
        InfiniteGridLayoutGroup gridLayout = GetScript<InfiniteGridLayoutGroup> ("SelectItemGrid");

        SetCanvasButtonMsg ("Get/bt_CommonHighlightColor", DidTapGet);
        SetCanvasButtonMsg ("Cancel/bt_Common", DidTapCancel);
        SetCanvasButtonMsg ("bt_Close", DidTapCancel);

        var prefab = Resources.Load ("PresentBox/ListItem_SelectItem") as GameObject;
        gridLayout.OnUpdateItemEvent.AddListener (UpdateItem);
        gridLayout.Initialize (prefab, 20, 43, false);
    }

    private void UpdateItem(int index, GameObject obj)
    {
        ListItem_SelectItem item = obj.GetComponent<ListItem_SelectItem> ();
        item.Init (index);
    }

    void DidTapGet()
    {
        this.Dispose ();
    }

    void DidTapCancel()
    {
        this.Dispose ();
    }
}
