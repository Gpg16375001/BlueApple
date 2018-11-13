using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

using TMPro;

namespace SmileLab.UI {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPStateTransition : MonoBehaviour, IStateTransition {
        [FormerlySerializedAs("colors")]
        [SerializeField]
        private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

        private TextMeshProUGUI m_Target;

        void Awake()
        {
            m_Target = GetComponent<TextMeshProUGUI> ();
        }

        public void DoStateTransition(TransitionState state, bool instant)
        {
            Color tintColor;
            switch (state)
            {
            case TransitionState.Normal:
                tintColor = m_Colors.normalColor;
                break;
            case TransitionState.Highlighted:
                tintColor = m_Colors.highlightedColor;
                break;
            case TransitionState.Pressed:
                tintColor = m_Colors.pressedColor;
                break;
            case TransitionState.Disabled:
                tintColor = m_Colors.disabledColor;
                break;
            default:
                tintColor = Color.black;
                break;
            }

            if (gameObject.activeInHierarchy)
            {
                StartColorTween(tintColor * m_Colors.colorMultiplier, instant);
            }
        }

        private void StartColorTween(Color color, bool instant)
        {
            if (m_Target == null)
                return;
            m_Target.CrossFadeColor(color, instant ? 0f : m_Colors.fadeDuration, true, true);
        }
    }
}
