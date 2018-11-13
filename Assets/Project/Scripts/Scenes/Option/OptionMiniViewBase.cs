using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;


/// <summary>
/// 設定画面の右部詳細画面のMiniView基底クラス.
/// </summary>
public class OptionMiniViewBase : ViewBase
{

    /// <summary>
    /// 非同期なRemoveComponent.
    /// </summary>
	public void AsyncDetach(Action didDetach = null)
    {
        WillDetachProc(() => {
            Destroy(this);
            if(didDetach != null){
                didDetach();
            }
        });
    }

    // Detachの直前にやりたい処理.
    protected virtual void WillDetachProc(Action didProcEnd)
    {
        if(didProcEnd != null){
            didProcEnd();
        }
    }
}
