using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(uGUIPageScrollRect), true)]
	[CanEditMultipleObjects]

	public class uGUIPageScrollRectEditor : ScrollRectEditor
	{
		public override void OnInspectorGUI()
		{
			uGUIPageScrollRect obj = target as uGUIPageScrollRect;

			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
				EditorGUILayout.PropertyField( serializedObject.FindProperty("currentPageIndex"), new GUIContent("Current Page Index") );
				EditorGUILayout.IntField( "Display Transform Index", obj.DispTransformIndex );
				EditorGUILayout.PropertyField( serializedObject.FindProperty("maxPage"), new GUIContent("MaxPage") );
				EditorGUILayout.PropertyField( serializedObject.FindProperty("isInfinite"), new GUIContent("IsInfinite") );
			EditorGUILayout.EndVertical();
		}
	}
}
