using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class Screen_PartyChange : ViewBase
{
    public void Init(bool isBattleInit)
    {
        m_IsBattleInit = isBattleInit;

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_GlobalMenu.DidTapButton += GlobalMenuDidTapButton;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackButton;


        var rootGo = this.GetScript<Transform> ("PartyGrid").gameObject;
        for (int i = 0; i < AwsPartyData.PartyMax; ++i) {
            var go = GameObjectEx.LoadAndCreateObject ("PartyChange/ListItem_PartyChange", rootGo);
            var script = go.GetOrAddComponent<ListItem_PartyChange> ();
            script.Init (i, DidTapListItem);
        }

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    void DidTapListItem(int index)
    {
        // カレント設定を変更して前の画面に戻る。
        AwsModule.PartyData.CurrentTeamIndex = index;
        if (m_IsBattleInit) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToQuestPreparation());
        } else {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPartyEdit());
        }
    }

    void DidTapBackButton()
    {
        if (m_IsBattleInit) {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToQuestPreparation());
        } else {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPartyEdit());
        }
    }

    void GlobalMenuDidTapButton(System.Action exec)
    {
        if (AwsModule.PartyData.CurrentTeam.IsEmpty) {
            PopupManager.OpenPopupYN("ユニットが一体も設定されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    exec ();
                },
                () => {
                }
            );
            return;
        }

        // 変更が保存されていない場合
        if (AwsModule.PartyData.IsModify) {
            PopupManager.OpenPopupYN("変更が保存されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    exec ();
                },
                () => {
                }
            );
            return;
        }

        exec ();
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    private bool m_IsBattleInit;
}
