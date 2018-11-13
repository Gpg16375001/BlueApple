using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class BattleDropItemEffect : MonoBehaviour {

    public static void Create(Vector3 start, Vector3 end)
    {
        var go = GameObjectEx.LoadAndCreateObject ("Battle/BattleEffect/eff_UIDropItem0101", BattleProgressManager.Shared.BattleUI.gameObject);
        go.GetOrAddComponent<BattleDropItemEffect> ().InitInternal (start, end);
    }

    private void InitInternal(Vector3 start, Vector3 end)
    {
        this.transform.position = start;

        m_EndPos = end;
        m_Animation = gameObject.GetComponent<Animation> ();

        StartCoroutine (CoPlay ());
    }

    private IEnumerator CoPlay()
    {
        m_Animation.Play ("born");

        yield return new WaitUntil (() => !m_Animation.isPlaying);

        m_Moving = true;
        m_Animation.Play ("move");
        iTween.MoveTo (gameObject, new Hashtable () {
            {"position", m_EndPos},
            {"time", 0.5f},
            {"easetype", iTween.EaseType.linear},
            {"oncompletetarget", gameObject},
            {"oncomplete", "MoveEnd"}
        });

        yield return new WaitUntil(() => !m_Moving);

        BattleProgressManager.Shared.BattleUI.PlayGetItemEffect ();
        m_Animation.Play ("vanish");

        yield return new WaitUntil (() => !m_Animation.isPlaying);


        gameObject.SetActive (false);
        Destroy (gameObject);
    }

    private void MoveEnd()
    {
        m_Moving = false;
    }

    private bool m_Moving;
    private Animation m_Animation;
    private Vector3 m_EndPos;
}
