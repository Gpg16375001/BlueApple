using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorDefineSetting : ScriptableObject
{
    [SerializeField]
    List<KeyValuePair<string, Color>> Colors = new List<KeyValuePair<string, Color>> ();

    [SerializeField]
    List<KeyValuePair<string, ColorBlock>> ColorBlocks = new List<KeyValuePair<string, ColorBlock>> ();

    public Color GetColor(string key)
    {
        int index = Colors.FindIndex (x => x.Key == key);
        if (index < 0) {
            return default(Color);
        }
        return Colors [index].Value;
    }

    public ColorBlock GetColorBlock(string key)
    {
        int index = ColorBlocks.FindIndex (x => x.Key == key);
        if (index < 0) {
            return default(ColorBlock);
        }
        return ColorBlocks [index].Value;
    }
}
