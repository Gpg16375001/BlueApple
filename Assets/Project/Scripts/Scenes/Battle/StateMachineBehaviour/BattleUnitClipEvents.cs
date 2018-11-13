using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitClipEvents : MonoBehaviour {

    BattleUnitMotionModule rootModule;

    public void Init(BattleUnitMotionModule root)
    {
        rootModule = root;
    }

    public void StepSt()
    {
        if(rootModule != null)
            rootModule.StepSt ();
    }

    public void StepEn()
    {
        if(rootModule != null)
            rootModule.StepEn ();
    }

    public void CamFocus_to_Enemy()
    {
        if (rootModule != null) {
            rootModule.AttackCameraTargetSet ();
            //rootModule.CallAction ();
        }
    }

    public void CamFocus_to_Chara()
    {
        if (rootModule != null) {
            rootModule.AttackCameraMyselfSet ();
        }
    }

    public void CamFocus_to_Default()
    {
        if (rootModule != null) {
            rootModule.AttackCameraReset ();
        }
    }

    // 攻撃を振るエフェクト
    public void AttackSt(int num)
	{
		Debug.Log("AttackSt : "+num);
        if (rootModule != null) {
            rootModule.PlayAttackSt (num);
        }
	}

    // 攻撃時エフェクト
    public void ActionEff()
    {
        Debug.Log ("AttackEff");
        if (rootModule != null) {
            rootModule.PlayAttackEffect ();
            rootModule.CallAction ();
        }
    }

    // ダメージエフェクト
    public void DamageEff()
    {
        Debug.Log ("DamageEff");
        if (rootModule != null) {
            rootModule.PlayDamageEffect ();
        }
    }

    // ダメージ表示
    public void DamageNumEff()
    {
        Debug.Log ("DamageNumEff");
        if (rootModule != null) {
            rootModule.PlayDamageNumber ();
        }
    }

    // 回復表示
    public void HealEff()
    {
        Debug.Log ("HealEff");
        if (rootModule != null) {
            rootModule.PlayHealEffect ();
        }
    }

    // 射撃エフェクト
    public void MuzzleFlash()
    {
        Debug.Log ("MuzzleFlash");
        if (rootModule != null) {
            rootModule.PlayMuzzleFlash ();
        }
    }

    // SE再生
    public void PlaySE(string key)
    {
        Debug.Log ("PlaySE: key = " + key);
        if (rootModule != null) {
            if (key == "attack") {
                rootModule.PlayAttackSe ();
            }
        }
    }
}
