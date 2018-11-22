using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class View_EventScenarioEffect : ViewBase {
    public void StartEffect(Action didEnd)
    {
        gameObject.AttachUguiRootComponent (UtageModule.SharedInstance.GetCamera ("SpriteCamera"));
        gameObject.SetLayerRecursively (LayerMask.NameToLayer("Utage"));
        m_anim = this.GetScript<Animation> ("AnimParts");
        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black, () => {
            this.StartCoroutine (WaitPlayingAnim (didEnd));
        });
    }
    // アニメーション再生、待機.
    IEnumerator WaitPlayingAnim(Action didEnd)
    {
        if(m_anim == null){
            didEnd();
            yield break;
        }

        LockInputManager.SharedInstance.IsLock = true;

        yield return null;  // 念のため1フレ待つ.
        m_anim.gameObject.SetActive(true);
        while(m_anim.isPlaying){
            yield return null;
        }
        didEnd();

        LockInputManager.SharedInstance.IsLock = false;
        Dispose ();
    }

    Animation m_anim;
}
