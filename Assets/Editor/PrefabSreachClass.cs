using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEditor;

public class PrefabSreachClass : EditorWindow
{

    private string m_SreachClassName;
    private Dictionary<GameObject, List<Component>> m_Prefabs = new Dictionary<GameObject, List<Component>>();
    private Dictionary<GameObject, List<bool>> m_FolderOpend = new Dictionary<GameObject, List<bool>>();
    private Type m_SreachClass;
    private Vector2 m_ScrollPos;
    private List<FieldInfo> m_SerializeFieldList = new List<FieldInfo>();
    private Dictionary<FieldInfo, bool> m_DisplayField = new Dictionary<FieldInfo, bool>();

    [MenuItem ("Window/SmileLab/SreachPrefabClass")]
    public static void ShowWindow ()
    {
        var wnd = GetWindow<PrefabSreachClass> ();

        wnd.minSize = new Vector2 (400, 300);
        wnd.Show ();
    }

    public void OnGUI ()
    {
        GUILayout.BeginHorizontal ();
        GUILayout.FlexibleSpace ();
        GUILayout.Label ("Prefab Sreach Class");
        GUILayout.FlexibleSpace ();
        GUILayout.EndHorizontal ();

        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label ("Search Class Name");
        m_SreachClassName = EditorGUILayout.TextArea (m_SreachClassName);
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.Space ();

        if (!string.IsNullOrEmpty (m_SreachClassName)) {
            if (GUILayout.Button ("Class Find")) {
                if (m_SreachClass == null || m_SreachClass.Name != m_SreachClassName) {
                    m_SreachClass = GetTypeByClassName (m_SreachClassName);
                    m_SerializeFieldList = GetSerializeFieldList (m_SreachClass);
                    m_DisplayField.Clear ();
                    for (int i = 0; i < m_SerializeFieldList.Count; ++i) {
                        m_DisplayField.Add (m_SerializeFieldList [i], true);
                    }
                }
            }
            if (m_SreachClass != null) {
                if (GUILayout.Button ("Sreach")) {
                    SreachClass (m_SreachClass);
                }
            } else {
                m_Prefabs.Clear ();
                EditorGUILayout.HelpBox ("Not Found Class", MessageType.Error);
            }
        } else {
            m_Prefabs.Clear ();
        }

        if (m_Prefabs != null && m_Prefabs.Count > 0) {
            m_ScrollPos = EditorGUILayout.BeginScrollView (m_ScrollPos);
            foreach (var kv in m_Prefabs) {
                EditorGUILayout.ObjectField ("Prefab", kv.Key, typeof(GameObject), false);


                EditorGUI.indentLevel++;
                for (int i = 0; i < kv.Value.Count; ++i) {
                    m_FolderOpend[kv.Key][i] = EditorGUILayout.Foldout (m_FolderOpend[kv.Key][i], kv.Value[i].name);
                    if (m_FolderOpend [kv.Key] [i]) {
                        //EditorGUILayout.LabelField (kv.Value[i].name);
                        SerializedObject prefabSo = new SerializedObject (kv.Value [i]);

                        EditorGUI.indentLevel++;
                        var itr = prefabSo.GetIterator ();
                        while(itr.NextVisible(true))  {
                            // 上のプロパティを文字列で指定する.
                            EditorGUILayout.PropertyField (itr, true);
                        }
                        prefabSo.ApplyModifiedProperties ();
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.Space ();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space ();
            }
            EditorGUILayout.EndScrollView ();
        }
    }

    private void SreachClass(Type t)
    {
        m_Prefabs.Clear ();
        m_FolderOpend.Clear ();
        foreach (var path in SearchFile(System.IO.Path.Combine(Application.dataPath, "Resources"), "*.prefab")) {
            var assetPath = path.Remove (0, Application.dataPath.Length + 1);
            assetPath = System.IO.Path.Combine ("Assets", assetPath);
            SreachClassCore (assetPath, t);
        }
    }

    private void SreachClassCore(string path, Type t)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject> (path);
        var classes = prefab.GetComponentsInChildren (t, true);
        if (classes.Count () > 0) {
            m_Prefabs.Add (prefab, classes.ToList ());
            m_FolderOpend.Add (prefab, new List<bool> (classes.Count ()));
            for (int i = 0; i < classes.Count (); ++i) {
                m_FolderOpend [prefab].Add (false);
            }
        }
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

    private List<FieldInfo> GetSerializeFieldList(Type t)
    {
        List<FieldInfo> ret = new List<FieldInfo> ();
        if (t != null) {
            ret.AddRange (t.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where (x => !x.IsNotSerialized));
            if (t.BaseType != null) {
                ret.AddRange (GetSerializeFieldList (t.BaseType));
            }
        }
        return ret;
    }

    public Type GetTypeByClassName( string className )
    {
        string assemblyName = string.Empty;
        var splitNames = className.Split ('.');
        if(splitNames.Length > 1) {
            className = splitNames[splitNames.Length - 1];
            System.Text.StringBuilder assemblyNameBuilder = new System.Text.StringBuilder ();
            for (int i = 0; i < splitNames.Length - 1; ++i) {
                if (i != 0) {
                    assemblyNameBuilder.Append (".");
                }
                assemblyNameBuilder.Append (splitNames [i]);
            }
            assemblyName = assemblyNameBuilder.ToString ();
        }
        foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() ) {
            if (!string.IsNullOrEmpty (assemblyName) && assembly.GetName().Name != assemblyName) {
                continue;
            }
            foreach (Module module in assembly.GetModules()) {
                foreach( Type type in module.GetTypes() ) {
                    if( type.Name == className ) {
                        return type;
                    }
                }
            }

            foreach (Module module in assembly.GetLoadedModules()) {
                foreach( Type type in module.GetTypes() ) {
                    if( type.Name == className ) {
                        return type;
                    }
                }
            }

            foreach( Type type in assembly.GetTypes() ) {
                if( type.Name == className ) {
                    return type;
                }
            }

            foreach (Type type in assembly.GetExportedTypes()) {
                if( type.Name == className ) {
                    return type;
                }
            }
        }
        return null;
    }
}
