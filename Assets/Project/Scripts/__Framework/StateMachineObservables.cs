using UnityEngine;
using System.Collections;
using UniRx;


namespace SmileLab
{

    /*
     * 使い方：
     * 
     * ex. 
     * // StateMachineObservalbesを取得.
     * animator = GetComponent <Animator>();
     * stateMachineObservables = animator.GetBehaviour<StateMachineObservalbes>(); 
     * 
     * // 開始したアニメーションのshortNameHashを表示する.
     * stateMachineObservables
     *     .OnStateEnterObservable
     *     .Subscribe(stateInfo => Debug.Log(stateInfo.shortNameHash));
    */

    // TODO : UniRx標準のObservableStateMachineTriggerでいいんじゃ無いかという気がしてる.
    /// <summary>
    /// AnmatorのState遷移を監視する.State個々に設定せずLayer単位で設定すること.
    /// Unity5より実装されたStateMachineBehaviourを利用したUniRxによる通知ロジック.
    /// </summary>
    public class StateMachineObservables : StateMachineBehaviour
    {
        #region OnStateEnter

        /// <summary>新しいステートに移り変わった時に実行.</summary>
        public IObservable<AnimatorStateInfo> OnStateEnterObservable { get { return onStateEnterSubject.AsObservable(); } }
        private Subject<AnimatorStateInfo> onStateEnterSubject = new Subject<AnimatorStateInfo>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateEnterSubject.OnNext(stateInfo);
        }

        #endregion

        #region OnStateExit

        /// <summary>ステートが次のステートに移り変わる直前に実行.</summary>
        public IObservable<AnimatorStateInfo> OnStateExitObservable { get { return onStateExitSubject.AsObservable(); } }
        private Subject<AnimatorStateInfo> onStateExitSubject = new Subject<AnimatorStateInfo>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateExitSubject.OnNext(stateInfo);
        }

        #endregion

        #region OnStateMachineEnter

        /// <summary>スクリプトが貼り付けられたステートマシンに遷移してきた時に実行.</summary>
        public IObservable<int> OnStateMachineEnterObservable { get { return onStateMachineEnterSubject.AsObservable(); } }
        private Subject<int> onStateMachineEnterSubject = new Subject<int>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            onStateMachineEnterSubject.OnNext(stateMachinePathHash);
        }

        #endregion

        #region OnStateMachineExit

        /// <summary>スクリプトが貼り付けられたステートマシンから出て行く時に実行.</summary>
        public IObservable<int> OnStateMachineExitObservable { get { return onStateMachineExitrSubject.AsObservable(); } }
        private Subject<int> onStateMachineExitrSubject = new Subject<int>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            onStateMachineExitrSubject.OnNext(stateMachinePathHash);
        }

        #endregion

        #region OnStateMove

        /// <summary>MonoBehaviour.OnAnimatorMoveの直後に実行される.</summary>
        public IObservable<AnimatorStateInfo> OnStateMoveObservable { get { return onStateMoveSubject.AsObservable(); } }
        private Subject<AnimatorStateInfo> onStateMoveSubject = new Subject<AnimatorStateInfo>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateMoveSubject.OnNext(stateInfo);
        }

        #endregion

        #region OnStateMove

        /// <summary>最初と最後のフレームを除く、各フレーム単位で実行.</summary>
        public IObservable<AnimatorStateInfo> OnStateUpdateObservable { get { return onStateUpdateSubject.AsObservable(); } }
        private Subject<AnimatorStateInfo> onStateUpdateSubject = new Subject<AnimatorStateInfo>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateUpdateSubject.OnNext(stateInfo);
        }

        #endregion


        #region OnStateIK

        /// <summary>MonoBehaviour.OnAnimatorIKの直後に実行される.</summary>
        public IObservable<AnimatorStateInfo> OnStateIKObservable { get { return onStateIKSubject.AsObservable(); } }
        private Subject<AnimatorStateInfo> onStateIKSubject = new Subject<AnimatorStateInfo>();

        /// <summary>StateMachineBehaviour側で呼び出されるコールバック.publicだが直接参照しないように注意.</summary>
        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onStateIKSubject.OnNext(stateInfo);
        }

        #endregion
    }

}