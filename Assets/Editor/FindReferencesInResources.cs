using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Reflection;

public static class FindReferencesInResources {
    [MenuItem("Assets/Find References In Resources")]
    public static void FindReferencesInResourcesExec()
    {
        var findAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var allResources = Resources.LoadAll ("");
        var findAssets = new List<int> ();
        foreach (var obj in allResources) {
            var objPath = AssetDatabase.GetAssetPath (obj);
            var dependencies = AssetDatabase.GetDependencies (objPath);
            bool find = System.Array.FindIndex (dependencies, x => x.Equals (findAssetPath)) >= 0;
            if (find) {
                findAssets.Add (obj.GetInstanceID());
            }
        }

        if (findAssets.Count > 0) {
            Selection.instanceIDs = findAssets.ToArray ();
            ShowObjectsInList (Selection.instanceIDs);
            EditorApplication.RepaintProjectWindow ();
        }
    }

    private static void ShowObjectsInList(int[] instanceIDs)
    {
        var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var asm = Assembly.Load("UnityEditor");
        var projectwindowtype = asm.GetType("UnityEditor.ProjectBrowser");
        var projectwindow = EditorWindow.GetWindow(projectwindowtype, false, "Project", false);

        var ShowObjectsInListFunc = projectwindowtype.GetMethod("ShowObjectsInList", flag);
        ShowObjectsInListFunc.Invoke (projectwindow, new object[] {instanceIDs});
    }
}
