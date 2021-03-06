﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SmileLab 
{
    /// <summary>
    /// 入力操作のロック管理
    /// </summary>
    public class LockInputManager : ViewBase
    {
        // uGUI用のイベントシステム.
        [SerializeField]
        private GameObject eventSystemObj;

    	/// <summary>
    	/// 共通インスタンス
    	/// </summary>
    	public static LockInputManager SharedInstance { get; private set; }

    	/// <summary>
    	/// UnlockBlock制御中かどうか
    	/// </summary>
    	public bool IsInUnlockBlock {
    		get {
    			return m_bInUnlockBlock;
    		}
    	}
    	/// <summary>
    	/// 入力制御中かどうか
    	/// </summary>
    	public bool IsLock {
    		get{
    			if(m_bInUnlockBlock){
    				Debug.LogError("Error!! : LockInputManager is unlocking now.");
    				return true;
    			}
    			return m_countLock.IsLock;
    		}
    		set{
    			if(m_bInUnlockBlock){
    				m_unlockBlockDelayList.Add(value);// ブロック中の入力はすべて記録し、ブロック解除時にすべて反映させる。
    			}
    			else{
    				m_countLock.IsLock = value;

    			}
    		}
    	}
    	
        bool m_IsDuplicationInputCheck;
        public bool IsDuplicationInputCheck {
            get {
                return m_IsDuplicationInputCheck;
            }
            set {
                if (!m_IsDuplicationInputCheck && value) {
                    StartCoroutine ("WaitDuplicationCheck");
                } else {
                    StopCoroutine ("WaitDuplicationCheck");
                }
                m_IsDuplicationInputCheck = value;
            }
        }

        public float IsDuplicationInputControlTime = 0.2f;
        private WaitForSecondsRealtime WiatDuplicationTime;
        IEnumerator WaitDuplicationCheck()
        {
            yield return WiatDuplicationTime;
            m_IsDuplicationInputCheck = false;
        }

    	/// <summary>
    	/// 強制的に入力ブロックを解除する.
    	/// </summary>
    	public void ForceUnlockInput()
    	{
    		m_countLock.Reset();
    	}
    	
    	/// <summary>
    	/// 一時的に入力ブロックを解除する.
    	///     解除したいエリアを UnlockBlockBegin() と UnlockBlockEnd() で囲むべし. 囲んだエリアで LockInput 関係をいじるのは禁止！
    	/// </summary>
    	public void UnlockBlockBegin()
    	{
    		// ネストはダメ
    		if(m_bInUnlockBlock){
    			Debug.LogError("[UnlockBlockBegin] Error!! UnlockBlockBegin : LockInputManager already begin unlock.");
    			return;
    		}
    		m_bInUnlockBlock = true;
    		m_bPrevBlockInput = this.BlockInput;
    		this.BlockInput = false;

    		if(m_unlockBlockDelayList.Count > 0){
    			Debug.LogError("[UnlockBlockBegin] Error!! UnlockBlockBegin : already lock.");
    			return;
    		}
    		m_unlockBlockDelayList.Clear();
    	}
    	/// <summary>
    	/// 一時的に入力ブロックを解除してたのを元の戻す.
    	///     解除したいエリアを UnlockBlockBegin() と UnlockBlockEnd() で囲むべし. 囲んだエリアで LockInput 関係をいじるのは禁止！
    	/// </summary>
    	public void UnlockBlockEnd()
    	{
    		this.BlockInput = m_bPrevBlockInput;
    		m_bInUnlockBlock = false;
    		
    		foreach(bool b in m_unlockBlockDelayList){
    			m_countLock.IsLock = b;
    		}
    		m_unlockBlockDelayList.Clear();
    	}
    	
    #region internal proc.
    	// UI入力ブロック
    	private bool BlockInput
    	{
    		get{
                return eventSystemObj != null ? eventSystemObj.activeSelf : false;
    		}
    		set{
                if(eventSystemObj != null) eventSystemObj.SetActive(!value);
    		}
    	}
    	
    	void Awake()
    	{
            if(SharedInstance != null) {
                SharedInstance.Dispose();
            }
            SharedInstance = this;
    		
    		m_countLock = new CountingLock(delegate(bool bLock) {
    			this.BlockInput = bLock;
    		});
    		m_unlockBlockDelayList = new List<bool>();
            WiatDuplicationTime = new WaitForSecondsRealtime (IsDuplicationInputControlTime);
    	}
    #endregion
    	
    	private CountingLock      m_countLock;
    	private bool              m_bInUnlockBlock = false;
    	private bool              m_bPrevBlockInput;
    	private List<bool>        m_unlockBlockDelayList;
    }
}
