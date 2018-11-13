using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// timeScaleに依存しないアニメーション再生を行うための拡張
/// </summary>
[RequireComponent(typeof(Animation))]
public class IgnoreTimeScaleAnimation : MonoBehaviour
{
    [SerializeField]
    Animation m_animation;

    // 現在再生しているClip名を保存しておく
    HashSet<string> plaingClipNames = new HashSet<string>();
    // 前フレームの速度を保存しておく
    float prevSpeed;

    public bool isPlaying {
        get {
            return m_animation != null && m_animation.isPlaying;
        }
    }

    public AnimationState this[string name] {
        get {
            if (m_animation == null) {
                return null;
            }
            return m_animation [name];
        }
    }

    /// <summary>
    /// 指定clipの再生をする
    /// </summary>
    /// <param name="clip">Clip名</param>
    public void Play(string clip) {
        if (m_animation == null) {
            return;
        }
        plaingClipNames.Add (clip);
        m_animation [clip].speed = 1.0f / Time.timeScale;
        m_animation.Play (clip);

        prevSpeed = 1.0f / Time.timeScale;
    }

    void Awake()
    {
        // アニメーションが設定されていないときはGetComponentする
        if (m_animation == null) {
            m_animation = GetComponent<Animation> ();
            // アニメーションが取れないかつ設定されていない場合は無効化する
            if (m_animation == null) {
                enabled = false;
            }
        }
        prevSpeed = 1.0f / Time.timeScale;
    }

    void Start()
	{
        // playAutomaticallyなら自動再生のアニメーションに適用.
        if (m_animation.playAutomatically && m_animation.clip != null) {
            if (!plaingClipNames.Contains (m_animation.clip.name)) {
                plaingClipNames.Add (m_animation.clip.name);
            }
            m_animation [m_animation.clip.name].speed = 1.0f / Time.timeScale;
            prevSpeed = 1.0f / Time.timeScale;
        }
	}

	/// <summary>
	/// deltaTimeが再生中に変わった場合に反映する
	/// </summary>
	void LateUpdate()
    {
        float speed = 1.0f / Time.timeScale;
        if (!Mathf.Approximately(prevSpeed, speed)) {
            foreach (var name in plaingClipNames.ToList()) {
                if (m_animation [name].enabled) {
                    m_animation [name].speed = speed;
                } else {
                    plaingClipNames.Remove (name);
                }
            }
        }
        prevSpeed = speed;
    }
}
