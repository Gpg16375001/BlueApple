using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace SmileLab.UI {
    [AddComponentMenu("SmileLab/UI/CustomButton", 1000)]
    [ExecuteInEditMode]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class CustomButton : Selectable, ISubmitHandler, IPointerClickHandler, IPointerExitHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent {}

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        [SerializeField]
        private bool m_PlayClickSe = false;

        [SerializeField]
        private SoundClipName m_OnClickSe;

        [FormerlySerializedAs("onLongPress")]
        [SerializeField]
        private ButtonClickedEvent m_OnLongPress = new ButtonClickedEvent();

        [FormerlySerializedAs("onRelease")]
        [SerializeField]
        private ButtonClickedEvent m_OnRelease = new ButtonClickedEvent();

        [SerializeField]
        private bool m_PlayLongPressSe = false;

        [SerializeField]
        private SoundClipName m_OnLongPressSe;

        [SerializeField]
        private GameObject[] m_TransitionObjects;
        private IStateTransition[] m_StateTransitions;

        public bool m_EnableLongPress;

        public float m_LongPressThredshold = 1.0f;

        private bool m_IsForceHighlight = false;
        public bool ForceHighlight {
            get {
                return m_IsForceHighlight;
            }
            set {
                if (value != m_IsForceHighlight) {
                    m_IsForceHighlight = value;
                    if (m_IsForceHighlight) {
                        DoStateTransition (SelectionState.Highlighted, false);
                    } else {
                        DoStateTransition (currentSelectionState, false);
                    }
                }
            }
        }

        protected CustomButton()
        {
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        public void SetClickSe(SoundClipName soundClip)
        {
            m_PlayClickSe = true;
            m_OnClickSe = soundClip;
        }

        public ButtonClickedEvent onLongPress
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
        }

        public void SetLongPressSe(SoundClipName soundClip)
        {
            m_PlayLongPressSe = true;
            m_OnLongPressSe = soundClip;
        }

        public ButtonClickedEvent onRelease
        {
            get { return m_OnRelease; }
            set { m_OnRelease = value; }
        }

        protected override void Awake ()
        {
            base.Awake ();
            // Editorモードの時にnullで来ることがあるため
            if (m_TransitionObjects != null) {
                m_StateTransitions = m_TransitionObjects.Where(x => x != null)
                    .Select (x => x.GetComponent<IStateTransition> ()).Where (x => x != null).ToArray ();
            }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable() || m_IsCallLongPress)
                return;

            // 多重入力を制御する。
            if (LockInputManager.SharedInstance.IsDuplicationInputCheck) {
                return;
            }

            if (m_PlayClickSe) {
                SoundManager.SharedInstance.PlaySE (m_OnClickSe);
            }
            m_OnClick.Invoke ();
            LockInputManager.SharedInstance.IsDuplicationInputCheck = true;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            if (!IsActive() || !IsInteractable() || m_IsCallLongPress)
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        private TransitionState TransitionStateFromSelectionState(SelectionState state)
        {
            switch (state) {
            case SelectionState.Normal:
                return TransitionState.Normal;
            case SelectionState.Highlighted:
                return TransitionState.Highlighted;
            case SelectionState.Pressed:
                return TransitionState.Pressed;
            case SelectionState.Disabled:
                return TransitionState.Disabled;
            }
            return TransitionState.Normal;
        }

        protected override void DoStateTransition (SelectionState state, bool instant)
        {
            // 強制的にハイライト状態にする。
            if (m_IsForceHighlight) {
                state = SelectionState.Highlighted;
                instant = true;
            }
            if (m_StateTransitions != null && m_StateTransitions.Length > 0) {
                var transitionState = TransitionStateFromSelectionState (state);
                // 追加で登録されているオブジェクトも繊維処理
                Array.ForEach (m_StateTransitions, x => x.DoStateTransition (transitionState, instant));
            }
            base.DoStateTransition (state, instant);
        }

        public override void OnPointerDown (PointerEventData eventData)
        {
            base.OnPointerDown (eventData);

            if (!IsActive () || !IsInteractable ()) {
                return;
            }

            m_IsPressing = true;
            m_IsCallLongPress = false;

            if (m_EnableLongPress) {
                StartCoroutine ("CheckWaitLongPress");
            }
        }
        IEnumerator CheckWaitLongPress()
        {
            if (!m_EnableLongPress)
                yield break;

            m_PressSec = 0f;
            while (m_IsPressing) {
                m_PressSec += Time.unscaledDeltaTime;
                if (!m_IsCallLongPress && m_PressSec >= m_LongPressThredshold) {
                    m_IsCallLongPress = true;
                    if (m_PlayLongPressSe) {
                        SoundManager.SharedInstance.PlaySE (m_OnLongPressSe);
                    }
                    m_OnLongPress.Invoke ();
                    yield break;
                }
                yield return null;
            }
        }

        public override void OnPointerUp (PointerEventData eventData)
        {
            base.OnPointerUp (eventData);
            Release ();
        }

        public override void OnPointerExit (PointerEventData eventData)
        {
            base.OnPointerExit (eventData);
            Release ();

        }

        public override void OnDeselect (BaseEventData eventData)
        {
            base.OnDeselect (eventData);

            Release ();
        }

        private void Release()
        {
            StopCoroutine ("CheckWaitLongPress");

            // Releaseコールバック
            m_OnRelease.Invoke ();

            m_IsPressing = false;
            m_PressSec = 0.0f;

            if (currentSelectionState != SelectionState.Normal) {
                DoStateTransition (SelectionState.Normal, false);
            }
        }

        private bool m_IsCallLongPress = false;
        private bool m_IsPressing = false;
        private float m_PressSec = 0.0f;
    }
}
