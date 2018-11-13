﻿using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEditor.AnimatedValues;

namespace SmileLab.UI {
    [CustomEditor(typeof(ImageStateTransition))]
    public class ImageStateTransitionEditor : Editor
    {
        SerializedProperty m_TargetGraphicProperty;
        SerializedProperty m_TransitionProperty;
        SerializedProperty m_ColorBlockProperty;
        SerializedProperty m_SpriteStateProperty;
        SerializedProperty m_AnimTriggerProperty;

        AnimBool m_ShowColorTint       = new AnimBool();
        AnimBool m_ShowSpriteTrasition = new AnimBool();
        AnimBool m_ShowAnimTransition  = new AnimBool();

        protected virtual void OnEnable()
        {
            m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
            m_TransitionProperty    = serializedObject.FindProperty("m_Transition");
            m_ColorBlockProperty    = serializedObject.FindProperty("m_Colors");
            m_SpriteStateProperty   = serializedObject.FindProperty("m_SpriteState");
            m_AnimTriggerProperty   = serializedObject.FindProperty("m_AnimationTriggers");

            var trans = GetTransition(m_TransitionProperty);
            m_ShowColorTint.value       = (trans == Selectable.Transition.ColorTint);
            m_ShowSpriteTrasition.value = (trans == Selectable.Transition.SpriteSwap);
            m_ShowAnimTransition.value  = (trans == Selectable.Transition.Animation);

            m_ShowColorTint.valueChanged.AddListener(Repaint);
            m_ShowSpriteTrasition.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update();

            var trans = GetTransition(m_TransitionProperty);

            var graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
            if (graphic == null)
                graphic = (target as ImageStateTransition).GetComponent<Graphic>();

            m_ShowColorTint.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Button.Transition.ColorTint);
            m_ShowSpriteTrasition.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Button.Transition.SpriteSwap);
            m_ShowAnimTransition.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Button.Transition.Animation);

            EditorGUILayout.PropertyField(m_TransitionProperty);

            ++EditorGUI.indentLevel;
            {
                if (trans == Selectable.Transition.ColorTint || trans == Selectable.Transition.SpriteSwap)
                {
                    EditorGUILayout.PropertyField(m_TargetGraphicProperty);
                }

                switch (trans)
                {
                case Selectable.Transition.ColorTint:
                    if (graphic == null)
                        EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
                    break;

                case Selectable.Transition.SpriteSwap:
                    if (graphic as Image == null)
                        EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
                    break;
                }

                if (EditorGUILayout.BeginFadeGroup(m_ShowColorTint.faded))
                {
                    EditorGUILayout.PropertyField(m_ColorBlockProperty);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSpriteTrasition.faded))
                {
                    EditorGUILayout.PropertyField(m_SpriteStateProperty);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowAnimTransition.faded))
                {
                    EditorGUILayout.PropertyField(m_AnimTriggerProperty);
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;

            serializedObject.ApplyModifiedProperties();
        }

        static Selectable.Transition GetTransition(SerializedProperty transition)
        {
            return (Selectable.Transition)transition.enumValueIndex;
        }

    }
}
