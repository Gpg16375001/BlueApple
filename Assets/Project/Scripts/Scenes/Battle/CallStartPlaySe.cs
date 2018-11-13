using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class CallStartPlaySe : MonoBehaviour {

    public string clipName;
    public float delayTime;
   
	// Use this for initialization
    IEnumerator Start () {
        yield return new WaitForSeconds (delayTime);

        SoundManager.SharedInstance.PlaySE (clipName);
	}
}
