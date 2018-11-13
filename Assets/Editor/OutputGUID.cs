using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

public static class OutputGUID {
    [MenuItem ("Tools/SmileLab/Output All Asset GUID")]
    public static void ExecOutputGUID()
    {
        Exec ();
    }

    private static void Exec() {
        StringBuilder builder = new StringBuilder ();
        foreach (var path in AssetDatabase.GetAllAssetPaths()) {
            var guid = AssetDatabase.AssetPathToGUID (path);

            builder.AppendFormat ("{0}: {1}", path, guid);
            builder.AppendLine ();
        }

        System.IO.File.WriteAllText (System.IO.Path.Combine (Application.dataPath, "guid.txt"), builder.ToString ());
    }
}
