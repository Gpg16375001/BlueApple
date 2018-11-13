using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using SmileLab;


/// <summary>
/// ガチャムービーのモーション操作クラス.
/// </summary>
public class GachaMovieMotionModule : ViewBase
{
	/// <summary>
    /// アニメーター.
    /// </summary>
	public Animator Animator { get; private set; }

    /// <summary>
    /// 終了してる？
    /// </summary>
	public bool IsEnd { get; private set; }

    /// <summary>
    /// イベント：予兆演出に入るたび呼ばれる.
    /// </summary>
	public event Action DidEnterStateIn;
	/// <summary>
    /// イベント：予兆演出に入るたび呼ばれる.
    /// </summary>
    public event Action DidExitServiceCheck;


    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init()
	{
		var animList = this.gameObject.GetComponentsInChildren<Animator>();
        if(animList == null || animList.Length <= 0){
			Debug.LogError("[GachaMovieMotionModule] Init Error!! : Animator is null or empty or not active all.");
		}
        // Animatorが複数ついていることは無い想定.
        Animator = animList[0];
        m_stateObserver = Animator.GetBehaviour<StateMachineObservables>();
        if(m_stateObserver == null){
			Debug.LogError("[GachaMovieMotionModule] Init Error!! : "+Animator.gameObject.name+"にStateMachineObservablesがアタッチされていない.");
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
		// --- SE再生 ---
		SoundManager.SharedInstance.StopSE();
		// 中央のマギカイト(青いクリスタル)に力が貯まり、キャラが出るまでの音.
		if (stateInfo.IsTag("presage") || stateInfo.IsName("Base Layer.presage02_service_r4")) {
			SoundManager.SharedInstance.PlaySE(SoundClipName.se146);
		}

		if (stateInfo.IsTag("in") || stateInfo.IsName("Base Layer.card_service_r4_in")) { 
			// キャラ名が表示される時の音.
            SoundManager.SharedInstance.PlaySE(SoundClipName.se147);
			// 出現中~キャラが表示しきるまでの音.
			SoundManager.SharedInstance.PlaySE(SoundClipName.se148);
		}
		// 以下レアリティ演出SE.  
		if (stateInfo.IsName("Base Layer.card_r3_in")) {
			SoundManager.SharedInstance.PlaySE(SoundClipName.se149);
		} else if (stateInfo.IsName("Base Layer.card_r4_in") || stateInfo.IsName("Base Layer.card_service_r4_in")) {
            SoundManager.SharedInstance.PlaySE(SoundClipName.se149);
            SoundManager.SharedInstance.PlaySE(SoundClipName.se150);
		} else if (stateInfo.IsName("Base Layer.card_r5_in")) {
            SoundManager.SharedInstance.PlaySE(SoundClipName.se149);
            SoundManager.SharedInstance.PlaySE(SoundClipName.se150);
        }
		
		// 演出スタート.
		if (stateInfo.IsTag("in")) {
            Debug.Log("予兆演出開始");
			if(DidEnterStateIn != null){
				DidEnterStateIn();
			}
		}      
	}

	#endregion

	#region Subscribe exit receivers.

	// コールバック : ステート遷移(Exit).
    void CallbackStateExit(AnimatorStateInfo stateInfo)
    {
		if (stateInfo.IsName("Base Layer.ServiceCheck")){
			Debug.Log("星４確定チェック終了.");
			if (DidExitServiceCheck != null) {
                DidExitServiceCheck();
            }
		}
		if(stateInfo.IsName("Base Layer.intro_in_service_r4")){
			SoundManager.SharedInstance.PlaySE(SoundClipName.se147);
            SoundManager.SharedInstance.PlaySE(SoundClipName.se148);
		}
		// 演出終了.
		if (stateInfo.IsName("Base Layer.coda_out")) {
			Debug.Log("演出終了");
			IsEnd = true;
		}
    }

    #endregion

	private StateMachineObservables m_stateObserver;
}
