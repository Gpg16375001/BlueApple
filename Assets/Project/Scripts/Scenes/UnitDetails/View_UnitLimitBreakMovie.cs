using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

public class View_UnitLimitBreakMovie : ViewBase {
    public static void Create(CardData card, int limitBreakCount, Action didEnd)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_UnitLimitBreakMovie");
        var c = go.GetOrAddComponent<View_UnitLimitBreakMovie>();
        c.InitInternal(card, limitBreakCount, didEnd);
    }

    void InitInternal(CardData card, int limitBreakCount, Action didEnd)
    {
        
        var loader = new UnitResourceLoader (card);
        loader.LoadFlagReset ();
        loader.IsLoadLive2DModel = true;
        loader.IsLoadVoiceFile = true;

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        View_GlobalMenu.IsVisible = false;
        View_PlayerMenu.IsVisible = false;
        SetCanvasCustomButtonMsg ("bg", DidTap);

        loader.LoadResource (
            (resource) => {
                gameObject.SetActive (true);
                GetScript<TextMeshProUGUI>("txtp_Charaname").SetText(card.Card.nickname);
                var live2d = GameObject.Instantiate(resource.Live2DModel);
                var rootCanvas = GetScript<Canvas>("CharacterAnchor");
                live2d.transform.SetParent(rootCanvas.transform);
                live2d.transform.localScale = Vector3.one;
                live2d.transform.localPosition = Vector3.zero;

                var renderCntl = live2d.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>();
                renderCntl.SortingLayer = rootCanvas.sortingLayerName;
                renderCntl.SortingOrder = rootCanvas.sortingOrder;

                for (int i = 1; i <= 4; ++i) {
                    GetScript<RectTransform> (string.Format ("{0}/Get", i)).gameObject.SetActive (limitBreakCount == i);
                    GetScript<RectTransform> (string.Format ("{0}/Off", i)).gameObject.SetActive (limitBreakCount < i);
                    GetScript<RectTransform> (string.Format ("{0}/On", i)).gameObject.SetActive (limitBreakCount > i);
                }
                View_FadePanel.SharedInstance.IsLightLoading = false;
                m_VoicePlayer = live2d.GetOrAddComponent<Live2dVoicePlayer>();
                this.StartCoroutine(this.CoWaitEndAnimation(didEnd, m_VoicePlayer, card.Card.voice_sheet_name));
            }
        );
        gameObject.SetActive (false);
    }

    public override void Dispose ()
    {
        if (m_VoicePlayer != null) {
            m_VoicePlayer.Stop ();
        }
        base.Dispose ();
    }
    void DidTap()
    {
        m_Touch = true;
    }

    IEnumerator CoWaitEndAnimation(Action didEnd, Live2dVoicePlayer voicePlayer, string sheetName)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("UnitLimitBreakMovieStart");

        yield return new WaitUntil(() => !anim.isPlaying);
        LockInputManager.SharedInstance.IsLock = false;

        m_Touch = false;

        voicePlayer.Play(sheetName, SoundVoiceCueEnum.panel_release);

        yield return new WaitUntil(() => m_Touch);

        LockInputManager.SharedInstance.IsLock = true;
        anim.Play ("UnitLimitBreakMovieExit");
        yield return new WaitUntil(() => !anim.isPlaying);

        if(didEnd != null){
            didEnd();
        }
        LockInputManager.SharedInstance.IsLock = false;

        View_GlobalMenu.IsVisible = true;
        View_PlayerMenu.IsVisible = true;
        this.Dispose();
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    Live2dVoicePlayer m_VoicePlayer;
    bool m_Touch;
}
