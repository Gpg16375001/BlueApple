using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

using System.Linq;
using TMPro;

using SmileLab;
using SmileLab.UI;

public class Screen_Event : ViewBase {

	// Use this for initialization
    public void Init(SpriteAtlas topAtlas, SpriteAtlas bannerAtlas)
    {
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        var eventInfos = MasterDataTable.event_info.EnableEventInfo;

        // バナー一覧の生成
        SetEventBanner(eventInfos, bannerAtlas);

        // イベントTOP画像の設定
        SetEventTop(eventInfos.First(), topAtlas);

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    void SetEventBanner(EventInfo[] infos, SpriteAtlas bannerAtlas)
    {
        var eventBannerScroll = GetScript<ScrollRect> ("ScrollAreaEvent");

        int loopCount = infos.Length;
        for (int i = 0; i < loopCount; ++i) {
            var info = infos [i];
            var go = GameObjectEx.LoadAndCreateObject ("Event/ListItem_EventBanner", eventBannerScroll.content.gameObject);
            go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
            go.GetOrAddComponent<ListItem_EventBanner> ().Init(info, bannerAtlas);
        }
    }

    void SetEventTop(EventInfo info, SpriteAtlas topAtlas)
    {
        //m_TopInfo = info;
        if (info == null) {
            GetScript<RectTransform> ("Main").gameObject.SetActive(false);
            return;
        }

        if (topAtlas != null) {
            GetScript<Image> ("img_BnrLargeMain").overrideSprite = topAtlas.GetSprite (info.id.ToString());
        }

        if (info.event_type == EventTypeEnum.Main) {
            GetScript<RectTransform> ("Shop").gameObject.SetActive(true);
            SetCanvasCustomButtonMsg ("Shop/bt_CommonS02", DidTapShop);
        } else {
            GetScript<RectTransform> ("Shop").gameObject.SetActive(false);
        }
    }

    void DidTapShop()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToShop();
            LockInputManager.SharedInstance.IsLock = false;
        }); 
    }

    void DidTapBack()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }


    //private EventInfo m_TopInfo;
}
