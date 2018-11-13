using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class BattleHealItem : ViewBase {

    public void Init(int number, float waitTime = 0.0f, System.Action playCallback=null)
    {
        GetScript<TextMeshPro> ("Num").text = number.ToString();
        StartCoroutine (Play (waitTime, playCallback));
    }

    private IEnumerator Play(float waitTime, System.Action playCallback)
    {
        if (waitTime > 0.0f) {
            yield return new WaitForSeconds (waitTime);
        }

        GetScript<Animation> ("Num").Play ();

        if (playCallback != null) {
            playCallback ();
        }
    }
}