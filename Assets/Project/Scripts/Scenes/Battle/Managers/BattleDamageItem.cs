using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class BattleDamageItem : ViewBase {
    private bool m_IsCritical;
    private bool m_IsWeek;
    private bool m_IsResist;
    private bool m_IsHit;

    public void Init(int number, bool isHit, bool isCritical, bool week, bool resist, float waitTime = 0.0f, System.Action playCallback=null)
    {
        var damageText = GetScript<TextMeshPro> ("Num");
        if (isHit) {
            damageText.text = number.ToString ();
            GetScript<Animation> ("Miss").gameObject.SetActive(false);
        } else {
            damageText.gameObject.SetActive (false);
            GetScript<Animation> ("Miss").gameObject.SetActive(true);
        }

        m_IsCritical = isCritical;
        m_IsWeek = week;
        m_IsResist = resist;
        m_IsHit = isHit;

        StartCoroutine (Play (waitTime, playCallback));
    }

    private IEnumerator Play(float waitTime, System.Action playCallback)
    {
        if (waitTime > 0.0f) {
            yield return new WaitForSeconds (waitTime);
        }

        if (m_IsHit) {
            if (m_IsCritical) {
                GetScript<Animation> ("Num").Play ("BattleDamageNumCriticalPop");
            } else {
                GetScript<Animation> ("Num").Play ("BattleDamageNumPop");
            }
        } else {
            GetScript<Animation> ("Miss").Play ();
        }
        if (m_IsWeek) {
            var weekAnime = GetScript<Animation> ("Week");
            weekAnime.gameObject.SetActive (true);
            weekAnime.Play ();
            GetScript<Transform> ("Resist").gameObject.SetActive (false);
        } else if (m_IsResist) {
            var resistAnime = GetScript<Animation> ("Resist");
            resistAnime.gameObject.SetActive (true);
            resistAnime.Play ();
            GetScript<Transform> ("Week").gameObject.SetActive (false);
        } else {
            GetScript<Transform> ("Week").gameObject.SetActive (false);
            GetScript<Transform> ("Resist").gameObject.SetActive (false);
        }

        if (playCallback != null) {
            playCallback ();
        }
    }
}
