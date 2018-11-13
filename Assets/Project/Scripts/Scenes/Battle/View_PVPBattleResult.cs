using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_PVPBattleResult : ViewBase {
    public bool IsVisible 
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    public static View_PVPBattleResult Create(ReceivePvpFinishBattle response, bool isWin)
    {
        var go = GameObjectEx.LoadAndCreateObject("PVP/View_PVPResult");
		go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var instance = go.GetOrAddComponent<View_PVPBattleResult>();
        instance.InitInternal(response, isWin);
        return instance;
    }

    private void InitInternal(ReceivePvpFinishBattle response, bool isWin)
    {
        AwsModule.UserData.UserData = response.UserData;

        // MainCardLive2D作成
        if (isWin) {
            // 勝ったら自分のカード
            LoadLive2D (AwsModule.UserData.MainCard);
        } else {
            // 負けたら相手のカード
            LoadLive2D (AwsModule.BattleData.PVPBattleEntryData.OpponentPvpTeamData.MainCardData);
        }

        // 結果追加
        GetScript<TextMeshProUGUI>("BasePoint/txtp_Point").SetText(response.WinningPoint - response.ConsecutiveWinsBonusWinningPoint);
        GetScript<TextMeshProUGUI>("Bonus/txtp_Point").SetText(response.ConsecutiveWinsBonusWinningPoint);
        GetScript<TextMeshProUGUI>("txtp_TotalPoint").SetText(response.WinningPoint);

        GetScript<TextMeshProUGUI>("txtp_PVPMedal").SetText(response.PvpMedal);

        DispResult (isWin);

        SetCanvasCustomButtonMsg ("OK/bt_Common", DidTapOK);
    }

    private void DispResult(bool isWin)
    {
        GetScript<Transform> ("eff_PVPWin").gameObject.SetActive (isWin);
        GetScript<Transform> ("eff_PVPLose").gameObject.SetActive (!isWin);
    }

    UnitResourceLoader loader;
    private void LoadLive2D(CardData card)
    {
        var characterAnchorCanvas = GetScript<Canvas> ("CharacterAnchor");
        loader = new UnitResourceLoader (card);
        loader.LoadFlagReset ();
        loader.IsLoadLive2DModel = true;
        loader.LoadResource ((resouce) => {
            var live2dGo = Instantiate(resouce.Live2DModel) as GameObject;
            live2dGo.transform.SetParent(characterAnchorCanvas.transform);
            live2dGo.transform.localScale = Vector3.one;
            live2dGo.transform.localPosition = Vector3.zero;

            var renderCntl = live2dGo.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>();
            renderCntl.SortingLayer = characterAnchorCanvas.sortingLayerName;
            renderCntl.SortingOrder = characterAnchorCanvas.sortingOrder;
        });
    }

    void DidTapOK()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black,
            () => {
                AwsModule.BattleData.GameOverProc();

                LockInputManager.SharedInstance.IsLock = false;
                ScreenChanger.SharedInstance.GoToPVP();
            }
        );
    }

    public override void Dispose ()
    {
        if (loader != null) {
            loader.Dispose ();
        }
        loader = null;
        base.Dispose ();
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }
}
