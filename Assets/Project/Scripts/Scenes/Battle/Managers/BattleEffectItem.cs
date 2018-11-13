using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffectItem : MonoBehaviour {
    public bool IsNext {
        get {
            return m_IsNext || m_MayDestory;
        }
    }

    public bool MayDestory
    {
        get {
            return m_MayDestory;
        }
    }

    void Awake()
    {
        // アニメーションの終了タイミングを取得する。
        animators = GetComponentsInChildren<Animator> (true);
        animations = GetComponentsInChildren<Animation> (true);
        particles = GetComponentsInChildren<ParticleSystem> (true);
    }

    public void SetRendererSortSetting(string sortingLayerName)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer> (true)) {
            renderer.sortingLayerName = sortingLayerName;
        }
    }

    /// <summary>
    /// アニメーションタイムラインから任意のタイミングで次へ遷移させるためのイベント
    /// </summary>
    public void GoNext()
    {
        m_IsNext = true;
    }

    /// <summary>
    /// Sets the next callback.
    /// </summary>
    /// <param name="callback">Callback.</param>
    public void SetNextCallback(Action<GameObject> callback)
    {
        m_NextCallback = callback;
    }

    /// <summary>
    /// Sets the destory callback.
    /// </summary>
    /// <param name="callback">Callback.</param>
    public void SetDestoryCallback(Action<GameObject> callback)
    {
        m_DestoryCallback = callback;
    }

    /// <summary>
    /// アニメーションなどの状態をみて状態を更新する。
    /// </summary>
    public void UpdateAnimationState()
    {
        // 再生待機
        if (m_WaitPlay) {
            return;
        }

        if (m_MayDestory) {
            return;
        }

        if (!m_MayDestory && this.gameObject == null) {
            m_MayDestory = true;
            return;
        }

        m_MayDestory = 
            (animators.Length == 0 ||  Array.TrueForAll (animators, (animator) => animator.GetCurrentAnimatorStateInfo (0).IsName ("Exit"))) &&
            (animations.Length == 0 || Array.TrueForAll (animations, (animation) => !animation.isPlaying)) && 
            (particles.Length == 0 || Array.TrueForAll (particles, (particle) => !particle.isEmitting || !particle.emission.enabled));
    }

    /// <summary>
    /// Calls the next callback.
    /// </summary>
    public void CallNextCallback()
    {
        if (!m_CalledNextCallback && IsNext && m_NextCallback != null) {
            m_CalledNextCallback = true;
            m_NextCallback (this.gameObject);
        }
    }

    /// <summary>
    /// Calls the destory callback.
    /// </summary>
    public void CallDestoryCallback()
    {
        if (MayDestory && m_DestoryCallback != null) {
            m_DestoryCallback (this.gameObject);
        }
    }

    public void WaitPlay()
    {
        m_WaitPlay = true;
    }

    public void SetWait(float delay)
    {
        m_WaitPlay = true;
        m_DelayTime = delay;

        Array.ForEach (animators, x => x.enabled = false);
        Array.ForEach (animations, x => x.enabled = false);
        Array.ForEach (particles, x => x.Pause());

        StartCoroutine (PlayWait ());
    }

    IEnumerator PlayWait()
    {
        yield return new WaitForSeconds (m_DelayTime);
        m_WaitPlay = false;
        Array.ForEach (animators, x => x.enabled = true);
        Array.ForEach (animations, x => x.enabled = true);
        Array.ForEach (particles, x => x.Play());
    }

    public void Play()
    {
        m_WaitPlay = false;
    }

    private float m_DelayTime;
    private bool m_WaitPlay;
    private bool m_IsNext;
    private bool m_MayDestory;
    private Animator[] animators;
    private Animation[] animations;
    private ParticleSystem[] particles;

    private bool m_CalledNextCallback = false;
    private Action<GameObject> m_NextCallback = null;
    private Action<GameObject> m_DestoryCallback = null;
}
