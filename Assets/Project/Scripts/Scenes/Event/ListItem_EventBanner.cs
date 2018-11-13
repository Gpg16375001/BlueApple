using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using SmileLab;

public class ListItem_EventBanner : ViewBase {

    public void Init(EventInfo info, SpriteAtlas bannerAtlas)
    {
        m_Info = info;
        if (m_Info == null) {
            gameObject.SetActive (false);
            return;
        }

        if (bannerAtlas != null) {
            GetScript<Image> ("bt_Banner").sprite = bannerAtlas.GetSprite (info.id.ToString ());
        }
        SetCanvasCustomButtonMsg ("bt_Banner", DidTapBanner);
    }

    void DidTapBanner()
    {
        switch (m_Info.event_type) {
        case EventTypeEnum.Enhance:
            // 強化イベントへ
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToDailyQuest(1);
                LockInputManager.SharedInstance.IsLock = false;
            });
            break;
        case EventTypeEnum.Evolution:
            // 進化イベントへ
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                ScreenChanger.SharedInstance.GoToDailyQuest(2);
                LockInputManager.SharedInstance.IsLock = false;
            });
            break;
        case EventTypeEnum.Main:
            // メインイベントへ
            break;
        case EventTypeEnum.PvP:
            // PvPへ
            LockInputManager.SharedInstance.IsLock = true;
            if(AwsModule.PartyData.PvPTeam.IsEmpty) {
                // PVPチーム編成に飛ばす
                LockInputManager.SharedInstance.IsLock = false;
                PopupManager.OpenPopupOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                    LockInputManager.SharedInstance.IsLock = true;
                    View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                        ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                        LockInputManager.SharedInstance.IsLock = false;
                    });
                });
            } else {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    ScreenChanger.SharedInstance.GoToPVP();
                    LockInputManager.SharedInstance.IsLock = false;
                });
            }
            break;
        }
    }

    EventInfo m_Info;
}
