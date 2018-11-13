using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class Screen_UnitList : UnitListBase
{


    public void Init(CardData forcus)
    {
        // 
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;

        base.Init (forcus == null ? 0 : forcus.CardId);

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    protected override bool GetDispOrganizing (CardData card)
    {
        return base.GetDispOrganizing (card);
    }

    protected override Action<CardData> GetDidLondTapIcon ()
    {
        return null;
    }

    protected override void DidTapIcon(CardData card)
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToUnitDetails(card, 
                () => {
                    ScreenChanger.SharedInstance.GoToUnitList(card);
                }
            );
        });
    }

    protected override void DidLongTapIcon (CardData card)
    {
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    void DidTapBack()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, 
            () => {
                View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black,
                    () => {
                        ScreenChanger.SharedInstance.GoToMyPage ();
                    }
                );
            }
        );
    }
}
