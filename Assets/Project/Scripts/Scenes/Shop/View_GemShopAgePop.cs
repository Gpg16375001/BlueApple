using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class View_GemShopAgePop : PopupViewBase {
    private static View_GemShopAgePop instance;
    public static View_GemShopAgePop Create()
    {
        if(instance != null){
            instance.Dispose();
        }
        var go = GameObjectEx.LoadAndCreateObject("Shop/View_GemShopAgePop");
        instance = go.GetOrAddComponent<View_GemShopAgePop>();
        instance.InitInternal();
        return instance;
    }

    private void InitInternal()
    {
        // 年齢確認前の情報表示
        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("Next/bt_Common", DidTapNext);
        SetBackButton ();

        PlayOpenCloseAnimation (true);
    }

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

    void DidTapClose()
    {
        if (IsClosed) {
            return;
        }

        // 閉じる
        PlayOpenCloseAnimation (false, () => {
            Dispose();
        });
    }

    void DidTapNext()
    {
        if (IsClosed) {
            return;
        }

        // 自分を閉じて年齢入力画面を開く
        PlayOpenCloseAnimation (false, () => {
            View_GemShopAgeIuputPop.Create();
            Dispose();
        });
    }
}
