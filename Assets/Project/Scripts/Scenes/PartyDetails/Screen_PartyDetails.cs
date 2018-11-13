using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class Screen_PartyDetails : ViewBase
{
    public void Init(Action BackScene, bool isBattleInit, bool isPvP)
    {
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_GlobalMenu.DidTapButton += GlobalMenuDidTapButton;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackButton;

        m_BackScene = BackScene;

        EditParty = isPvP ? AwsModule.PartyData.PvPTeam : AwsModule.PartyData.CurrentTeam;
        int count = EditParty.Count;

        m_ListItems = new ListItem_PartyDetailsUnit[count + 1];
        for (int i = 1; i <= count + 1; ++i) {
            m_ListItems[i - 1] = this.GetScript<ListItem_PartyDetailsUnit> (string.Format ("ListItem_PartyDetailsUnit{0}", i));
        }

        for (int i = 1; i <= count; ++i) {
            m_ListItems [i - 1].Init (EditParty [i]);
        }
        if (isBattleInit && !isPvP) {
            m_ListItems [5].Init (AwsModule.BattleData.SupportCard.ConvertCardData(), true);
        } else {
            // 6番目は必要ないので消す。
            m_ListItems [5].gameObject.SetActive(false);
        }
        m_NowMode = ListItem_PartyDetailsUnit.DisplayMode.Status;

        // ボタン設定
        this.SetCanvasCustomButtonMsg("SwitchStatus/bt_Navi", DidTapSwitchStatus);

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    void GlobalMenuDidTapButton(System.Action exec)
    {
        if (EditParty.IsEmpty) {
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

    #region Button Delegate
    void DidTapSwitchStatus()
    {
        if (m_NowMode == ListItem_PartyDetailsUnit.DisplayMode.Status) {
            m_NowMode = ListItem_PartyDetailsUnit.DisplayMode.Skill;
        } else if (m_NowMode == ListItem_PartyDetailsUnit.DisplayMode.Skill) {
            m_NowMode = ListItem_PartyDetailsUnit.DisplayMode.Status;
        }
        System.Array.ForEach (m_ListItems, x => x.SetDisplayMode (m_NowMode));
    }

    void DidTapBackButton()
    {
        if (m_BackScene != null) {
            m_BackScene ();
        } else {
            View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToPartyEdit ());
        }
    }
    #endregion

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    /// <summary>
    /// カード情報表示スクリプトリスト
    /// </summary>
    private ListItem_PartyDetailsUnit[] m_ListItems;
    /// <summary>
    /// 現在表示している情報のモード
    /// </summary>
    private ListItem_PartyDetailsUnit.DisplayMode m_NowMode;

    private Action m_BackScene;

    private Party EditParty;
}
