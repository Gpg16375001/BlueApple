using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class PresentBoxSController : ScreenControllerBase
{
    public const int ONE_REQUEST_COUNT = 100;
    public override void Init (System.Action<bool> didConnectEnd)
    {
        #if true
        // 50件ずつデータを取得する。
        SendPresentboxGetReceivableList(didConnectEnd);
        #else
        presentDataList = new PresentData[10];
        receivablePresentCount = 10;
        didConnectEnd(true);
        #endif
    }

    public override void CreateBootScreen ()
    {
        var go = GameObjectEx.LoadAndCreateObject("PresentBox/Screen_PresentBox", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_PresentBox>();
        c.Init(receivableCount, presentDataList, historyCount, historyDataList);
    }

    private void SendPresentboxGetReceivableList(System.Action<bool> didConnectEnd) {
        SendAPI.PresentboxGetReceivableList (0, ONE_REQUEST_COUNT, 0, (success, response) => {
            if(success) {
                presentDataList = response.PresentDataList;
                receivableCount = response.ReceivablePresentCount;
            } else {
                // なにがしかエラー出たらマイページに戻す
                ScreenChanger.SharedInstance.GoToMyPage();
            }
            SendPresentboxGetReceivedList(didConnectEnd);
        });
    }

    private void SendPresentboxGetReceivedList(System.Action<bool> didConnectEnd) {
        SendAPI.PresentboxGetReceivedList (0, ONE_REQUEST_COUNT, (success, response) => {
            if(success) {
                historyDataList = response.PresentDataList;
                historyCount = response.ReceivedPresentCount;
            } else {
                // なにがしかエラー出たらマイページに戻す
                ScreenChanger.SharedInstance.GoToMyPage();
            }
            didConnectEnd(success);
        });
    }

    private PresentData[] presentDataList;
    private PresentData[] historyDataList;
    private int receivableCount;
    private int historyCount;
}
