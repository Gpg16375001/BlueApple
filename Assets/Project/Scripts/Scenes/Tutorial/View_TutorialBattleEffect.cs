using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_TutorialBattleEffect : ViewBase {

    /// <summary>
    /// 表示中？
    /// </summary>
    public bool IsVisible 
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    public bool IsEndOfPlaybackHide = true;

    public static View_TutorialBattleEffect Create(string prefabs, int sortingOrder = 60)
    {
        var go = GameObjectEx.LoadAndCreateObject(prefabs);
        /*
        var canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        canvas.sortingOrder = sortingOrder;
        var scaler = go.GetOrAddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1136f, 640f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
        go.GetOrAddComponent<GraphicRaycaster>();
*/
        var instance = go.GetOrAddComponent<View_TutorialBattleEffect>();
        instance.InitInternal ();
        return instance;
    }

    protected virtual void InitInternal()
    {
        m_anim = GetComponentsInChildren<Animation> (true).FirstOrDefault();
    }

    public bool isPlaying {
        get {
            return m_anim != null && m_anim.isPlaying;
        }
    }

    public void Play(System.Action didAnim=null)
    {
        enabled = true;
        IsVisible = true;

        m_didAnim = didAnim;
        if (m_anim != null) {
            m_anim [m_anim.clip.name].normalizedTime = 0.0f;
            m_anim.Play ();
        } else if(m_didAnim != null) {
            m_didAnim ();
        }
    }

    public void Play(string name, System.Action didAnim=null)
    {
        enabled = true;
        IsVisible = true;

        m_didAnim = didAnim;
        if (m_anim != null) {
            m_anim [name].normalizedTime = 0.0f;
            m_anim.Play (name);
        } else if(m_didAnim != null) {
            m_didAnim ();
        }
    }

    void Update()
    {
        if (m_anim != null && !m_anim.isPlaying) {
            if (m_didAnim != null) {
                m_didAnim ();
            }
            if (IsEndOfPlaybackHide) {
                IsVisible = false;
            }
            // 複数回didAnimが通らないように
            enabled = false;
        }
    }

    Animation m_anim;
    System.Action m_didAnim;
}