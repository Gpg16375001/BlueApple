using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RendererSortingLayer))]
public class RendererSortingLayerEditor : Editor {

    public override void OnInspectorGUI()
    {
        MonoBehaviour gameObject = target as MonoBehaviour;
        Renderer renderer = gameObject.GetComponent<Renderer> ();
        if (renderer == null) {
            return;
        }

        var sortingLayerNames = GetSortingLayerNames ();
        var sortingLayerIds = GetSortingLayerUniqueIDs ();
        renderer.sortingLayerID = EditorGUILayout.IntPopup("Sorting Layer", renderer.sortingLayerID, sortingLayerNames, sortingLayerIds);
        renderer.sortingOrder = EditorGUILayout.IntField(new GUIContent("Order in Layer", "描画プライオリティ"), renderer.sortingOrder);
    }

    /// <summary>
    /// ソートレイヤー名取得
    /// </summary>
    public static string[] GetSortingLayerNames() {
        System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
        System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, null);
    }

    /// <summary>
    /// ソートレイヤーID取得
    /// </summary>
    public static int[] GetSortingLayerUniqueIDs() {
        System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
        System.Reflection.PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, null);
    }
}
