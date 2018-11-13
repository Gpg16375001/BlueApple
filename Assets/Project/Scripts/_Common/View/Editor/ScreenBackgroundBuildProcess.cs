using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;

public class ScreenBackgroundBuildProcess : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild (UnityEditor.BuildTarget target, string path)
    {
        var objects = Resources.LoadAll<GameObject> ("");
        bool isChanged = false;
        foreach (var obj in objects) {
            var scripts = obj.GetComponentsInChildren<ScreenBackground> (true);
            if (scripts.Length > 0) {
                Debug.Log (obj.name);
                foreach (var script in scripts) {
                    script.BuildProcess ();
                }
                EditorUtility.SetDirty (obj);

                isChanged = true;
            }
        }

        if (isChanged) {
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }
    }
}
