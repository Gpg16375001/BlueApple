using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class ForceSelected : MonoBehaviour {
    private Sprite m_NormalSprite;
    private Selectable m_Selectable;

    void Awake()
    {
        m_Selectable = GetComponent<Selectable> ();
        if (m_Selectable != null && m_Selectable.transition == Selectable.Transition.SpriteSwap) {
            m_NormalSprite = m_Selectable.image.sprite;
        }
    }

    public void SetForceSelected()
    {
        if (m_Selectable != null) {
            if (m_Selectable.transition == Selectable.Transition.SpriteSwap) {
                m_Selectable.image.sprite = m_Selectable.spriteState.highlightedSprite;
            } else if (m_Selectable.transition == Selectable.Transition.ColorTint) {
                m_Selectable.image.color = m_Selectable.colors.highlightedColor;
            }
        }
    }

    public void Reset()
    {
        if (m_Selectable != null) {
            if (m_Selectable.transition == Selectable.Transition.SpriteSwap) {
                m_Selectable.image.sprite = m_NormalSprite;
            } else if (m_Selectable.transition == Selectable.Transition.ColorTint) {
                m_Selectable.image.color = m_Selectable.colors.normalColor;
            }
        }
    }
}
