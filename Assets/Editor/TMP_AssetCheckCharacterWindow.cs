using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TMP_AssetCheckCharacterWindow : EditorWindow
{
    [MenuItem ("Window/TextMeshPro/Check Characters")]
    public static void ShowWindow ()
    {
        var wnd = GetWindow<TMP_AssetCheckCharacterWindow> ();

        wnd.minSize = new Vector2 (400, 300);
        wnd.Show ();
    }


    private TMP_FontAsset FontForTmp;
    private TextAsset IncludeAsset;

    private bool isChecked;
    private string NotIncludedCharacters;

    public void OnGUI ()
    {
        GUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        GUILayout.Label ("Check Characters");
        GUILayout.FlexibleSpace ();
        GUILayout.EndHorizontal ();

        bool changed = false;
        EditorGUI.BeginChangeCheck ();
        FontForTmp = (TMP_FontAsset)EditorGUILayout.ObjectField ("TextMeshPro Font", FontForTmp, typeof(TMP_FontAsset), true);
        IncludeAsset = (TextAsset)EditorGUILayout.ObjectField ("Check Characters", IncludeAsset, typeof(TextAsset), true);
        changed = EditorGUI.EndChangeCheck ();
        if (changed) {
            isChecked = false;
        }
        EditorGUILayout.Space ();

        if (FontForTmp != null && IncludeAsset != null) {
            if (GUILayout.Button ("Check!!!")) {
                isChecked = true;
                NotIncludedCharacters = CheckNotIncludeCharacters (FontForTmp, IncludeAsset.text);
            }

            if (isChecked) {
                if (string.IsNullOrEmpty (NotIncludedCharacters)) {
                    EditorGUILayout.HelpBox ("All Included TextMeshPro Asset", MessageType.Info);
                } else {
                    GUILayout.Label ("Not Included Characters");

                    EditorGUILayout.Space ();

                    GUILayout.Label (NotIncludedCharacters);

                    EditorGUILayout.Space ();

                    if (GUILayout.Button ("Save Not Include Characters")) {
                        var path = EditorUtility.SaveFilePanel( "", "", "", "" );
                        if (!string.IsNullOrEmpty (path)) {
                            System.IO.File.WriteAllText (path, NotIncludedCharacters);
                        }
                    }
                }
            }
        } else {
            EditorGUILayout.HelpBox ("Choose the TextMeshPro font and Check TextData", MessageType.Info);
        }

        EditorGUILayout.Space ();
    }

    public static string CheckNotIncludeCharacters(TMP_FontAsset font, string includeText)
    {
        if (font == null || includeText == null) {
            return null;
        }

        List<char> notIncludeCharacters = new List<char> ();
        foreach (var c in includeText.ToCharArray()) {
            if (!font.characterDictionary.ContainsKey((int)c)) {
                notIncludeCharacters.Add (c);
            }
        }

        return new string (notIncludeCharacters.ToArray());
    }
}