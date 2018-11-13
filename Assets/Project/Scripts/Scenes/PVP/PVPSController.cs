using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class PVPSController : ScreenControllerBase {
    public bool IsPlayerSelect;

    private ReceivePvpGetOpponentList m_ApiResponse;
    public override void Init (System.Action<bool> didConnectEnd)
    {
        SendAPI.PvpGetOpponentList ((result, response) => {
            if(!result) {
                didConnectEnd (false);
                return;
            }
            m_ApiResponse = response;
            // ユニットリソースのロードをします。
            didConnectEnd (true);
        });
    }

    public override void CreateBootScreen ()
    {
        // 
        var go = GameObjectEx.LoadAndCreateObject("PVP/Screen_PVP", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_PVP>();

        if (m_ApiResponse.ResultCode == 1106) {
            // PVPチーム編成に飛ばす
            PopupManager.OpenPopupSystemOK ("PvPチームが編成されていません。編成を行ってください。", () => {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
                    ScreenChanger.SharedInstance.GoToPVPPartyEdit(true);
                });
            });
            return;
        }
        if (m_ApiResponse.ResultCode > 0) {
            return;
        }
        c.Init(m_ApiResponse, IsPlayerSelect);
    }
}