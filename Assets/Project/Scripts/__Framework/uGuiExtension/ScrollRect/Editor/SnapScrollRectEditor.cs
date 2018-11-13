using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SnapScrollRect))]
public class SnapScrollRectEditor : ScrollRectEditor {
    SerializedProperty m_SnappedEvent;
    SerializedProperty m_ItemPositionChangeEvent;
    SerializedProperty m_Smoothness;
    SerializedProperty m_ScrollWeight;
    SerializedProperty m_BeginSnapMomentVerocity;


    protected override void OnEnable ()
    {
        base.OnEnable ();
        m_Smoothness                = serializedObject.FindProperty("smoothness");
        m_ScrollWeight              = serializedObject.FindProperty("scrollWeight");
        m_SnappedEvent              = serializedObject.FindProperty("SnappedEvent");
        m_ItemPositionChangeEvent   = serializedObject.FindProperty("ItemPositionChangeEvent");
        m_BeginSnapMomentVerocity   = serializedObject.FindProperty("BeginSnapMomentVerocity");
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();

        EditorGUILayout.PropertyField(m_Smoothness);

        EditorGUILayout.PropertyField(m_ScrollWeight);

        EditorGUILayout.PropertyField (m_BeginSnapMomentVerocity);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_SnappedEvent);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_ItemPositionChangeEvent);

        serializedObject.ApplyModifiedProperties();
    }
}
