using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using UniRx;


/// <summary>
/// キャラ進化ムービーのモーション操作クラス.
/// </summary>
public class UnitEvolutionMovieMotionModule : ViewBase
{
	/// <summary>
    /// アニメーター.
    /// </summary>
    public Animator Animator { get; private set; }

	/// <summary>
    /// 初期化.
    /// </summary>
    public void Init(Action didEnd)
    {
		m_didEnd = didEnd;

        var animList = this.gameObject.GetComponentsInChildren<Animator>();
        if (animList == null || animList.Length <= 0) {
			Debug.LogError("[UnitEvolutionMovieMotionModule] Init Error!! : Animator is null or empty or not active all.");
        }
        // Animatorが複数ついていることは無い想定.
        Animator = animList[0];
        m_stateObserver = Animator.GetBehaviour<StateMachineObservables>();
        if (m_stateObserver == null) {
			Debug.LogError("[UnitEvolutionMovieMotionModule] Init Error!! : " + Animator.gameObject.name + "にStateMachineObservablesがアタッチされていない.");
        }
    }

    /// <summary>
    /// ステート遷移の監視を開始.
    /// </summary>
    public void RegistStateChangeStream()
    {
        // 既に購読中のストリームがあれば無視する.
        if (m_bReadingStream) {
            return;
        }
        m_bReadingStream = true;

        // TODO : 購読開始.とりあえず開始と終了.
        m_stateObserver.OnStateEnterObservable
                       .TakeUntilDestroy(this)     // 自身が破棄された場合停止.
                       .Subscribe(CallbackStateEnter);

        m_stateObserver.OnStateExitObservable
                       .TakeUntilDestroy(this)
                       .Subscribe(CallbackStateExit);

    }
    private bool m_bReadingStream = false;

	#region Subscribe enter receivers.

    // コールバック : ステート遷移(Enter).
    void CallbackStateEnter(AnimatorStateInfo stateInfo)
    {
    }

    #endregion

    #region Subscribe exit receivers.

    // コールバック : ステート遷移(Exit).
    void CallbackStateExit(AnimatorStateInfo stateInfo)
    {
        // 演出終了.
        if (stateInfo.IsName("Base Layer.out")) {
            Debug.Log("演出終了");
			if(m_didEnd != null){
				m_didEnd();
			}
        }
    }

    #endregion

	private StateMachineObservables m_stateObserver;
	private Action m_didEnd;
}
