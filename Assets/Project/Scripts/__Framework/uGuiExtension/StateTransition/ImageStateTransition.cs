using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

namespace SmileLab.UI {
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class ImageStateTransition : UIBehaviour, IStateTransition {
        // Type of the transition that occurs when the button state changes.
        [FormerlySerializedAs("transition")]
        [SerializeField]
        private Selectable.Transition m_Transition = Selectable.Transition.ColorTint;

        // Colors used for a color tint-based transition.
        [FormerlySerializedAs("colors")]
        [SerializeField]
        private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

        [FormerlySerializedAs("spriteState")]
        [SerializeField]
        private SpriteState m_SpriteState;

        [FormerlySerializedAs("animationTriggers")]
        [SerializeField]
        private AnimationTriggers m_AnimationTriggers = new AnimationTriggers();

        [FormerlySerializedAs("highlightGraphic")]
        [FormerlySerializedAs("m_HighlightGraphic")]
        [SerializeField]
        private Graphic m_TargetGraphic;


        public Selectable.Transition transition {
            get {
                return m_Transition;
            } 
            set {
                if (!m_Transition.Equals(value)) {
                    m_Transition = value;
                    OnSetProperty ();
                }
            }
        }

        public ColorBlock colors {
            get {
                return m_Colors;
            } 
            set {
                if (!m_Colors.Equals(value)) {
                    m_Colors = value;
                    OnSetProperty ();
                }
            }
        }

        public SpriteState spriteState 
        {
            get { 
                return m_SpriteState;
            }
            set {
                if (!m_SpriteState.Equals(value)) {
                    m_SpriteState = value;
                    OnSetProperty ();
                }
            }
        }

        public AnimationTriggers animationTriggers 
        {
            get { 
                return m_AnimationTriggers;
            }
            set {
                if (!((m_AnimationTriggers == null && value == null) || (m_AnimationTriggers != null && m_AnimationTriggers.Equals(value)))) {
                    m_AnimationTriggers = value;
                    OnSetProperty ();
                }
            }
        }

        public Graphic targetGraphic 
        {
            get { 
                return targetGraphic;
            }
            set {
                if (!((targetGraphic == null && value == null) || (targetGraphic != null && targetGraphic.Equals(value)))) {
                    targetGraphic = value;
                    OnSetProperty ();
                }
            }
        }

        public Image image
        {
            get { return m_TargetGraphic as Image; }
            set { m_TargetGraphic = value; }
        }

        // Get the animator
        public Animator animator
        {
            get { return GetComponent<Animator>(); }
        }

        protected override void Awake ()
        {
            base.Awake ();
            m_TargetGraphic = GetComponent<Graphic> ();
        }

        public void DoStateTransition(TransitionState state, bool instant)
        {
            Color tintColor;
            Sprite transitionSprite;
            string triggerName;

            switch (state)
            {
            case TransitionState.Normal:
                tintColor = m_Colors.normalColor;
                transitionSprite = null;
                triggerName = m_AnimationTriggers.normalTrigger;
                break;
            case TransitionState.Highlighted:
                tintColor = m_Colors.highlightedColor;
                transitionSprite = m_SpriteState.highlightedSprite;
                triggerName = m_AnimationTriggers.highlightedTrigger;
                break;
            case TransitionState.Pressed:
                tintColor = m_Colors.pressedColor;
                transitionSprite = m_SpriteState.pressedSprite;
                triggerName = m_AnimationTriggers.pressedTrigger;
                break;
            case TransitionState.Disabled:
                tintColor = m_Colors.disabledColor;
                transitionSprite = m_SpriteState.disabledSprite;
                triggerName = m_AnimationTriggers.disabledTrigger;
                break;
            default:
                tintColor = Color.black;
                transitionSprite = null;
                triggerName = string.Empty;
                break;
            }

            if (gameObject.activeInHierarchy)
            {
                switch (m_Transition)
                {
                case Selectable.Transition.ColorTint:
                    StartColorTween(tintColor * m_Colors.colorMultiplier, instant);
                    break;
                case Selectable.Transition.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
                case Selectable.Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
                }
            }
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (m_TargetGraphic == null)
                return;

            m_TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : m_Colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
            if (transition != Selectable.Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(m_AnimationTriggers.normalTrigger);
            animator.ResetTrigger(m_AnimationTriggers.pressedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
        }


        private void OnSetProperty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                InternalEvaluateAndTransitionToSelectionState(true);
            else
#endif
                InternalEvaluateAndTransitionToSelectionState(false);
        }

        private void InternalEvaluateAndTransitionToSelectionState(bool instant)
        {
            var transitionState = TransitionState.Normal;
            DoStateTransition(transitionState, instant);
        }
    }
}