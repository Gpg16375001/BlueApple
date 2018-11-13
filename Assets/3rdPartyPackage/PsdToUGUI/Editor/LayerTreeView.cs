using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotoshopFile;
using UnityEditor;
using System.Linq;

public class TreeItem {
    public Layer Layer;
    List<TreeItem> Children;
    TreeItem Parent;
    bool Opened;
    bool RootObject;
    string ObjectName;

    public TreeItem(Layer layer, bool root = false)
    {
        Layer = layer;
        ObjectName = layer == null ? string.Empty : PSDEditorWindow.GetObjectName (layer);
        Children = new List<TreeItem> ();
        Opened = true;

        RootObject = root;
        Parent = null;
    }

    static int indentLevel = 0;
    const float indentSpace = 15.0f;
    public void Show()
    {
        if (RootObject) {
            foreach (var child in Children) {
                child.Show ();
            }
        } else {
            if (Children.Count > 0) {
                var prevIsOutput = Layer.IsOutput;
                EditorGUILayout.BeginHorizontal ();
                GUILayout.Space (indentSpace * indentLevel);
                Layer.IsOutput = EditorGUILayout.ToggleLeft ("", Layer.IsOutput, GUILayout.Width(12));
                Opened = EditorGUILayout.Foldout (Opened, ObjectName);
                EditorGUILayout.EndHorizontal ();

                if (prevIsOutput != Layer.IsOutput) {
                    Children.ForEach (x => x.SetOutput (Layer.IsOutput, true));

                    if (Layer.IsOutput && Parent != null) {
                        Parent.SetOutput (Layer.IsOutput);
                    }
                }
            } else {
                EditorGUILayout.BeginHorizontal ();
                GUILayout.Space (indentSpace * indentLevel);
                Layer.IsOutput = EditorGUILayout.ToggleLeft (ObjectName, Layer.IsOutput);
                if (Layer.IsOutput && Parent != null) {
                    Parent.SetOutput (Layer.IsOutput);
                }
                EditorGUILayout.EndHorizontal ();
            }

            indentLevel++;
            if (Opened) {
                foreach (var child in Children) {
                    child.Show ();
                }
            }
            indentLevel--;
        }
    }

    public void AddChild(TreeItem item)
    {
        Children.Add (item);
        item.SetParent (this);
    }

    public void Clear()
    {
        Children.Clear ();
    }

    private void SetParent(TreeItem parent)
    {
        Parent = parent;
    }

    private void SetOutput(bool isOutput, bool includeChild=false)
    {
        if (Layer == null) {
            return;
        }
        Layer.IsOutput = isOutput;

        if (Layer.IsOutput && Parent != null) {
            Parent.SetOutput (isOutput);
        }

        if (includeChild) {
            Children.ForEach (x => x.SetOutput (isOutput, includeChild));
        }
    }
}

public class LayerTreeView {
    List<Layer> Layers;
    TreeItem Root;

    public LayerTreeView()
    {
        Root = new TreeItem (null, true);
    }

    public void BuildView(IEnumerable<Layer> layers)
    {
        Layers = new List<Layer>(layers);
        Layers.Reverse ();
        List<TreeItem> itemList = new List<TreeItem> ();

       
        Root.Clear ();
        foreach (var layer in Layers) {
            var objectNames = layer.ObjectPath.Split ('/');
            var depth = objectNames.Length - 1;
            var item = new TreeItem(layer);
            if (depth <= 0) {
                Root.AddChild (item);
            } else {
                var parent = itemList.FirstOrDefault (x => PSDEditorWindow.IsParent(x.Layer, layer));
                if (parent != null) {
                    parent.AddChild (item);
                }
            }
            itemList.Add (item);
        }
    }

    public void ShowGUI()
    {
        Root.Show ();
    }
}
