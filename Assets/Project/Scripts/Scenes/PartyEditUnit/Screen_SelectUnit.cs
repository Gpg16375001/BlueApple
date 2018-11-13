using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class Screen_SelectUnit : UnitListBase
{
    public void Init(bool isBattleInit, bool isDispOrganizing, bool isPvP, bool isDispRemove, int removeTargetCardID, int focusCardID, ElementEnum? dispOnlyElement, bool isGlobalMenu, Action<CardData> DidSelect, Action DidBack)
    {
        m_RemoveTargetCardID = removeTargetCardID;
        m_IsDispRemove = isDispRemove;
        m_IsBattleInit = isBattleInit;
        m_DidSelect = DidSelect;
        m_DidBack = DidBack;
        m_IsDispOrganizing = isDispOrganizing;
        m_IsGlobalMenu = isGlobalMenu;
        m_IsPvP = isPvP;

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        base.Init (0, true, true, dispOnlyElement);

        View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black);
    }

    protected override bool GetDispRemove (CardData card)
    {
        return m_IsDispRemove && m_RemoveTargetCardID == card.CardId;
    }

    protected override bool GetDispOrganizing (CardData card)
    {
        var party = AwsModule.PartyData.CurrentTeam;
        if (m_IsPvP) {
            party = AwsModule.PartyData.PvPTeam;
        }
        return m_IsDispOrganizing && party.GetPosition (card) > 0 && m_RemoveTargetCardID != card.CardId;
    }

    // アイコンタップ時の処理
    protected override void DidTapIcon (CardData card)
    {
        if (m_DidSelect != null) {
            m_DidSelect (card);
        }

        if (m_DidBack != null) {
            m_DidBack ();
        }
    }

    protected override void DidLongTapIcon (CardData card)
    {
        if (card == null) {
            return;
        }

        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => 
            ScreenChanger.SharedInstance.GoToUnitDetails (card, 
                () => View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (
                    View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToSelectUnit (m_IsBattleInit, m_IsDispOrganizing,
                        m_IsPvP, m_IsDispRemove, m_RemoveTargetCardID, card.CardId, m_DispOnlyElement, m_IsGlobalMenu, m_DidSelect, m_DidBack)
                ),
                false
            )
        );
    }

    protected override void SortAndFilter ()
    {
        base.SortAndFilter ();

        // はずす表示に関して
        if (m_IsDispRemove) {
            int index = m_SortFilterCardDataList.FindIndex (x => x.CardId == m_RemoveTargetCardID);
            if(index >= 0) {
                var removeCard = m_SortFilterCardDataList [index];
                m_SortFilterCardDataList.RemoveAt (index);
                m_SortFilterCardDataList.Insert(0, removeCard);
            }
        }
    }

    void DidTapBack()
    {
        if (m_DidBack != null) {
            m_DidBack ();
        }
    }


    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    private bool m_IsBattleInit;

    private bool m_IsDispRemove;
    private int m_RemoveTargetCardID;

    private Action<CardData> m_DidSelect;
    private Action m_DidBack;

    // 編成中表示のフラグ
    private bool m_IsDispOrganizing;
    private bool m_IsPvP;

    //
    private bool m_IsGlobalMenu;

}
