using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;

public class EffectClipEvent : MonoBehaviour {

    public void BattleCameraShakeStart(float time)
    {
        CameraHelper.SharedInstance.ShakeBattleCamera (time:time);
    }

    public void BattleCameraShakeStop()
    {
        CameraHelper.SharedInstance.ShakeStopBattleCamera ();
    }

    public void StartSE(string clipName)
    {
        SoundManager.SharedInstance.PlaySE (clipName);
    }
}
