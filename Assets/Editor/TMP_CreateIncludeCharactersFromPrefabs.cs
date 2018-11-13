using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using TMPro;

public static class TMP_CreateIncludeCharactersFromPrefabs {
    [MenuItem ("Tools/TextMeshPro/Create Include Text From Prefabs")]
    public static void ExcludeEOF()
    {
        Exec ();
    }

    private static void Exec()
    {
        List<string> stringList = new List<string> ();
        foreach (var path in SearchFile(System.IO.Path.Combine(Application.dataPath, "Resources"), "*.prefab")) {
            var assetPath = path.Remove (0, Application.dataPath.Length + 1);
            assetPath = System.IO.Path.Combine ("Assets", assetPath);
            stringList.Add(ExecCore (assetPath));
        }

        System.IO.File.WriteAllText (
            System.IO.Path.Combine (Application.dataPath, "Font/IncludeCharacters/FromPrefab.txt"),
            string.Join ("", stringList.ToArray ())
        );
        AssetDatabase.Refresh ();
    }

    private static string ExecCore(string path)
    {
        Debug.Log (path);
        List<string> stringList = new List<string> ();
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject> (path);
        var texts = prefab.GetComponentsInChildren<TextMeshProUGUI> (true);

        foreach (var text in texts) {
            stringList.Add(text.text);
        }
        return string.Join("", stringList.ToArray());
    }

    private static string[] SearchFile(string rootPath, string pattern)
    {
        List<string> ret = new List<string> ();
        ret.AddRange (System.IO.Directory.GetFiles (rootPath, pattern));
        foreach (var dirctory in System.IO.Directory.GetDirectories(rootPath)) {
            ret.AddRange (SearchFile (dirctory, pattern));
        }
        return ret.ToArray ();
    }
}
