using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_PVPMain : ViewBase {
    public void Init(PvpUserData userData, Action didTapPlayerSelect, Action OpenBpRecovery)
    {
        m_ContestId = userData.ContestId;
        m_OpenBpRecovery = OpenBpRecovery;
        m_DidTapPlayerSelect = didTapPlayerSelect;

        var mainCard = AwsModule.UserData.MainCard;
        // お気に入りユニットのLive2Dロード
        var characterAnchor = this.GetScript<RectTransform>("CharacterAnchor").gameObject;
        characterAnchor.DestroyChildren ();
        var loader = new UnitResourceLoader(mainCard);
        loader.IsLoadSpineModel = false;
        loader.IsLoadTimeLine = false;
        loader.IsLoadLive2DModel = true;
        loader.LoadResource (resouce => {
            var live2dGo = Instantiate(resouce.Live2DModel) as GameObject;
            live2dGo.transform.SetParent(characterAnchor.transform);
            live2dGo.transform.localScale = Vector3.one;
            live2dGo.transform.localPosition = Vector3.zero;
        });

        // infoの設定
        // お気に入りユニットアイコンの設定
        var unitIconAnchor = this.GetScript<RectTransform> ("UnitIcon").gameObject;
        var iconGo = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", unitIconAnchor);
        iconGo.GetOrAddComponent<ListItem_UnitIcon> ().Init (mainCard);

        var bqGridGo = GetScript<RectTransform> ("BP/BPGrid").gameObject;
        m_ViewBPGrid = bqGridGo.GetOrAddComponent<View_BPGrid> ();
        m_ViewBPGrid.Init ();

        SetInfo(userData);

        // ランキングの状態を見てIntractive切り替え
        // TODO: サーバー情報を見て切り替えするようにする。
        GetScript<CustomButton>("PVPRanking/bt_CommonS01").interactable = false;

        // ボタン設定
        SetCanvasCustomButtonMsg ("PVPSearch/bt_Common", DidTapPVPSearch);
        SetCanvasCustomButtonMsg ("PVPRanking/bt_CommonS01", DidTapPVPRanking, false);
        SetCanvasCustomButtonMsg ("PVPReward/bt_CommonS01", DidTapPVPReward);
        SetCanvasCustomButtonMsg ("Shop/bt_CommonS01", DidTapShop);
        SetCanvasCustomButtonMsg ("BP/bt_Charge", DidTapBPCharge);
    }

    public void LoadBG(Action didLoaded)
    {
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            didLoaded();
        });
    }

    public void SetInfo(PvpUserData userData)
    {
        GetScript<TextMeshProUGUI> ("txtp_WinNum").SetText (userData.WinCount);
        GetScript<TextMeshProUGUI> ("txtp_LoseNum").SetText (userData.LoseCount);
        GetScript<TextMeshProUGUI> ("txtp_PVPMedal").SetText (AwsModule.UserData.UserData.PvpMedalCount);
        GetScript<TextMeshProUGUI> ("txtp_PVPWinPoint").SetText (userData.WinningPoint);

        /*
        // 仕様削除　次のClass周りの表示設定
        var txtpNextClassPoint = GetScript<TextMeshProUGUI> ("txtp_NextClassPoint");
        var txtpPVPPoint = GetScript<TextMeshProUGUI> ("txtp_PVPPoint");
        var imgClassMaxGo = GetScript<RectTransform> ("img_ClassMax").gameObject;
        var imgClassBar = GetScript<Image> ("img_PVPPointBar");
        var classPointMax = false;

        GetScript<TextMeshProUGUI> ("txtp_PVPClass").SetText ("");
        if(classPointMax) {
            txtpNextClassPoint.gameObject.SetActive (false);
            imgClassMaxGo.SetActive (true);
            imgClassBar.fillAmount = 1.0f;
        } else {
            imgClassMaxGo.SetActive (false);
            txtpNextClassPoint.gameObject.SetActive (true);
            txtpNextClassPoint.SetText (0);
            txtpPVPPoint.SetText (0);
            imgClassBar.fillAmount = 0.0f;
        }
        */

        // BPセット
        m_ViewBPGrid.UpdateBP (userData.BattlePoint);
    }

    public void UpdatePvpUser(PvpUserData userData)
    {
        m_ViewBPGrid.UpdateBP (userData.BattlePoint);
    }

    void DidTapPVPSearch()
    {
        if (m_DidTapPlayerSelect != null) {
            m_DidTapPlayerSelect ();
        }
    }

    void DidTapPVPRanking()
    {
        //PopupManager.OpenPopupOK ("ランキングは未実装です。");
    }

    void DidTapPVPReward()
    {
        View_PVPAchievementList.Create (m_ContestId);
    }

    void DidTapShop()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                ScreenChanger.SharedInstance.GoToShop();
            }
        );
    }

    void DidTapBPCharge()
    {
        if (m_OpenBpRecovery != null) {
            m_OpenBpRecovery ();
        }
    }

    Action m_DidTapPlayerSelect;
    View_BPGrid m_ViewBPGrid;

    int m_ContestId;
    Action m_OpenBpRecovery;
}
