using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TMPro;

using System.Linq;

/// <summary>
/// TextMeshPro用のキャラクター一覧ファイルを作成する。
/// </summary>
public class IncludeCharactersPostprocess : AssetPostprocessor 
{
    static readonly string DefaultFontAssetPath = "Assets/Resources/Font/FOT-MATISSEPRON-DB SDF.asset";

    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetsPath)
    {
        var changeAssets = importedAssets.Concat (deletedAssets).Concat (movedAssets).Concat (movedFromAssetsPath);
        bool isChangeIncludeCaracterData = changeAssets.Any (x => x.StartsWith ("Assets/Font/IncludeCharacters/") && x.EndsWith(".txt"));
        if (isChangeIncludeCaracterData) {
            // 統合データの更新
            CombineTextData();
        }
    }

    private static void CombineTextData()
    {
        Debug.Log ("Start CombineTextData");
        var textFiles = SearchFile (System.IO.Path.Combine (Application.dataPath, "Font/IncludeCharacters/"), "*.txt");
        HashSet<char> data = new HashSet<char> ();
        foreach (var textFile in textFiles) {
            var textData = System.IO.File.ReadAllText (textFile);
            foreach(var c in textData.ToCharArray ()) {
                data.Add (c);
            }
        }

        var includeTextFile = System.IO.Path.Combine (Application.dataPath, "Font/IncludeCharacters.txt");
        string originalText = string.Empty;
        if (System.IO.File.Exists (includeTextFile)) {
            originalText = System.IO.File.ReadAllText (includeTextFile);
        }
        var dataArray = data.ToArray ();
        System.Array.Sort (dataArray);
        var newText = new string (dataArray);
        if (newText != originalText) {
            System.IO.File.WriteAllText (
                includeTextFile,
                newText
            );
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset> (DefaultFontAssetPath);
            string notIncluded = TMP_AssetCheckCharacterWindow.CheckNotIncludeCharacters (font , newText);
            var notIncludedTextFile = System.IO.Path.Combine (Application.dataPath, "Font/NotIncludedCharacters.txt");
            if (!string.IsNullOrEmpty (notIncluded)) {
                Debug.LogWarning (string.Format ("下記の文字が{0}のアセットに含まれていません\n{1}", font.name, notIncluded));
                System.IO.File.WriteAllText (notIncludedTextFile, notIncluded);
            } else {
                System.IO.File.WriteAllText (notIncludedTextFile, string.Empty);
            }

            AssetDatabase.Refresh ();
        }
        Debug.Log ("End CombineTextData");
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
