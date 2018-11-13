using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_BattleEffect : ViewBase {

    /// <summary>
    /// 表示中？
    /// </summary>
    public bool IsVisible 
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    public bool IsEndOfPlaybackHide = true;

    public static View_BattleEffect Create(string prefabs, int sortingOrder = 60)
    {
        var go = GameObjectEx.LoadAndCreateObject(prefabs);
        var instance = go.GetOrAddComponent<View_BattleEffect>();
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

    public void Play(System.Action playCallback = null)
    {
        BattleScheduler.Instance.AddSchedule (CoPlay (m_anim.clip.name, playCallback));
    }

    public void Play(string name, System.Action playCallback = null)
    {
        BattleScheduler.Instance.AddSchedule (CoPlay (name, playCallback));
    }

    private IEnumerator CoPlay(string name, System.Action playCallback)
    {
        enabled = true;
        IsVisible = true;

        if (playCallback != null) {
            playCallback ();
        }
        if (m_anim != null) {
            m_anim [name].normalizedTime = 0.0f;
            m_anim.Play (name);

            yield return new WaitUntil(() => !isPlaying);

            if (IsEndOfPlaybackHide) {
                IsVisible = false;
            }
        }
    }

    Animation m_anim;
}
