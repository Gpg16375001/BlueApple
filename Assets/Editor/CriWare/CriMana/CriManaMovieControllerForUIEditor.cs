/****************************************************************************
 *
 * Copyright (c) 2015 CRI Middleware Co., Ltd.
 *
 ****************************************************************************/

using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CriManaMovieControllerForUI))]
public class CriManaMovieControllerForUIEditor : Editor
{
	private CriManaMovieControllerForUI source = null;
	
	private void OnEnable()
	{
		source = (CriManaMovieControllerForUI)base.target;
	}
	
	public override void OnInspectorGUI()
	{
		if (this.source == null) {
			return;
		}

		Undo.RecordObject(target, null);

		GUI.changed = false;
		{
			EditorGUILayout.PrefixLabel("Startup Parameters");
			++EditorGUI.indentLevel;
			{
				EditorGUI.BeginChangeCheck();
				string moviePath = EditorGUILayout.TextField(new GUIContent("Movie Path", "The path to the movie file."), source.moviePath);
				if (EditorGUI.EndChangeCheck()) {
					source.moviePath = moviePath;
				}
				source.playOnStart = EditorGUILayout.Toggle(new GUIContent("Play On Start", "Immediatly play movie after Start of the component."), source.playOnStart);
				EditorGUI.BeginChangeCheck();
				bool loop = EditorGUILayout.Toggle(new GUIContent("Loop", "Movie is played in continuous loop."), source.loop);
				if (EditorGUI.EndChangeCheck()) {
					source.loop = loop;
				}
			}
			--EditorGUI.indentLevel;
			++EditorGUI.indentLevel;
			{
				EditorGUILayout.PrefixLabel("Render Parameters");
				EditorGUI.BeginChangeCheck();
				bool additiveMode = EditorGUILayout.Toggle(new GUIContent("Additive Mode", "Movie is rendered in additive blend mode."), source.additiveMode);
				if (EditorGUI.EndChangeCheck()) {
					source.additiveMode = additiveMode;
				}
				source.material = (Material)EditorGUILayout.ObjectField(new GUIContent("Material",
					"The material to render movie.\n" +
					"If 'none' use an internal default material."), source.material, typeof(Material), true);
				source.renderMode = (CriManaMovieMaterial.RenderMode)EditorGUILayout.EnumPopup(new GUIContent("Render Mode",
				  "- Always: Render movie at each frame.\n" +
				  "- OnVisibility: Render movie only when owner GameObject is visible or UI.Graphic Target is active. Optimization when movie is not visible on screen.\n" +
				  "- Never: Never render movie to the material. You need to call 'RenderMovie()' to control rendering."), source.renderMode);

				EditorGUILayout.PrefixLabel("Renderer Control");
				++EditorGUI.indentLevel;
				{
					source.target = (UnityEngine.UI.Graphic)EditorGUILayout.ObjectField("Target", source.target, typeof(UnityEngine.UI.Graphic), true);
					source.useOriginalMaterial = EditorGUILayout.Toggle("Use Original Material", source.useOriginalMaterial);
				}
				--EditorGUI.indentLevel;
			}
			--EditorGUI.indentLevel;
		}
		if (GUI.changed) {
			EditorUtility.SetDirty(this.source);
		}
	}
}
